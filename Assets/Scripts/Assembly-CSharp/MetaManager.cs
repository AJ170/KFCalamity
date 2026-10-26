using System.Collections.Generic;
using Acheivements;
using GameEvents;
using Meta;
using UnityEngine;

public class MetaManager : MonoBehaviour
{
	private const int LIFETIME_DATA_SAVE_INTERVAL = 10;

	private const int WEAPON_DATA_SAVE_INTERVAL = 15;

	public bool AutoSaveStats = true;

	private static MetaManager instance;

	private FileManager fileManager;

	private OuyaPurchaseManager ouyaPurchaseManager;

	private LevelDatabase perkLevelDatabase = new PerkLevelDatabase();

	private LevelDatabase weaponLevelDatabase = new WeaponLevelDatabase();

	private List<Meta.PerkData> perks = new List<Meta.PerkData>();

	private List<WeaponData> weapons = new List<WeaponData>();

	private List<LevelData> levels = new List<LevelData>();

	private LifetimeStatsData lifetimeStats;

	private float lifeTimeSaveDataTimer;

	private float weaponSaveDataTimer;

	private bool autoSaveActive;

	private PerkBonuses perkBonuses;

	private WeaponBonuses weaponBonuses;

	private ProgressionLogs progressionLogs;

	public static MetaManager Instance
	{
		get
		{
			return instance;
		}
	}

	public int PerkMaxLevel
	{
		get
		{
			return perkLevelDatabase.MaxLevel;
		}
	}

	public int WeaponMaxLevel
	{
		get
		{
			return weaponLevelDatabase.MaxLevel;
		}
	}

	private void Awake()
	{
		instance = this;
		perkBonuses = new PerkBonuses(this);
		weaponBonuses = new WeaponBonuses(this);
		progressionLogs = new ProgressionLogs();
	}

	private void Start()
	{
		fileManager = FileManager.Instance;
		ouyaPurchaseManager = OuyaPurchaseManager.Instance;
		LoadAllData();
	}

	public void StartAutoSaveStats()
	{
		lifeTimeSaveDataTimer = 10f;
		weaponSaveDataTimer = 15f;
		autoSaveActive = true;
	}

	public void StopAutoSaveStats()
	{
		lifeTimeSaveDataTimer = 0f;
		weaponSaveDataTimer = 0f;
		autoSaveActive = false;
	}

	private void UpdateAutoSave()
	{
		if (AutoSaveStats && autoSaveActive)
		{
			lifeTimeSaveDataTimer -= Time.deltaTime;
			weaponSaveDataTimer -= Time.deltaTime;
			if (lifeTimeSaveDataTimer <= 0f)
			{
				SaveData(MetaDataType.LifetimeStats);
				lifeTimeSaveDataTimer = 10f;
			}
			if (weaponSaveDataTimer <= 0f)
			{
				SaveData(MetaDataType.Weapon);
				weaponSaveDataTimer = 15f;
			}
		}
	}

	private void Update()
	{
		UpdateAutoSave();
	}

	private void OnDestroy()
	{
		if (perkBonuses != null)
		{
			perkBonuses.Shutdown();
		}
		if (weaponBonuses != null)
		{
			weaponBonuses.Shutdown();
		}
		if (progressionLogs != null)
		{
			progressionLogs.Shutdown();
		}
		SaveAllData();
		instance = null;
	}

	private void LoadAllData()
	{
		LoadData(MetaDataType.Perk);
		LoadData(MetaDataType.Weapon);
		LoadData(MetaDataType.Level);
		LoadData(MetaDataType.LifetimeStats);
	}

	private void LoadData(MetaDataType type)
	{
		if (type == MetaDataType.None)
		{
			return;
		}
		string path = PathByDataType(type);
		List<string> filesByDirectory = fileManager.GetFilesByDirectory(path);
		foreach (string item in filesByDirectory)
		{
			List<string> list = fileManager.Load(item);
			if (list != null)
			{
				MetaData data = CreateDataByDataType(type);
				OnDataLoaded(data, list);
			}
		}
	}

	private void OnDataLoaded(MetaData data, List<string> lines)
	{
		data.Deserialize(lines);
		data.HasNewData = false;
	}

	public void SaveAllData()
	{
		SaveData(MetaDataType.Perk);
		SaveData(MetaDataType.Weapon);
		SaveData(MetaDataType.Level);
		SaveData(MetaDataType.LifetimeStats);
	}

	private void SaveData(MetaDataType type)
	{
		if (type == MetaDataType.None)
		{
			return;
		}
		string text = PathByDataType(type);
		List<MetaData> list = DataByDataType(type);
		foreach (MetaData item in list)
		{
			if (item.CanSave() && fileManager.Save(text + item.FileName(), item.Serialize()))
			{
				OnDataSaved(item);
			}
		}
	}

	private void OnDataSaved(MetaData data)
	{
		data.HasNewData = false;
	}

	private void AddPerkData(Meta.PerkData data)
	{
		perks.Add(data);
	}

	private Meta.PerkData CreatePerkData(Perk perk)
	{
		Meta.PerkData perkData = new Meta.PerkData((int)perk);
		AddPerkData(perkData);
		return perkData;
	}

	public Meta.PerkData CreateOrGetPerkData(Perk perk)
	{
		Meta.PerkData perkData = GetPerkData(perk);
		if (perkData == null)
		{
			perkData = CreatePerkData(perk);
		}
		return perkData;
	}

	private Meta.PerkData GetPerkData(int perk)
	{
		if (perk != -1)
		{
			foreach (Meta.PerkData perk2 in perks)
			{
				if (perk2.Perk == perk)
				{
					return perk2;
				}
			}
		}
		return null;
	}

	public Meta.PerkData GetPerkData(Perk perk)
	{
		return GetPerkData((int)perk);
	}

	public void PerkAddExperience(Meta.PerkData data, int experience)
	{
		if (data == null || !ShouldProgress(data))
		{
			return;
		}
		int totalExperience = data.AddExperience(experience);
		int level = data.Level;
		if (level == PerkMaxLevel)
		{
			return;
		}
		int levelByExperience = perkLevelDatabase.GetLevelByExperience(totalExperience);
		if (level != levelByExperience)
		{
			for (int i = level; i < levelByExperience; i++)
			{
				data.LevelUp();
				OnPerkLevelUp(data);
			}
		}
	}

	private void OnPerkLevelUp(Meta.PerkData data)
	{
		EventSystem eventSystem = EventSystem.Instance;
		if (eventSystem != null)
		{
			eventSystem.Trigger(new PerkLevelUp((Perk)data.Perk, data.Level));
		}
		progressionLogs.PerkLevelUp(data);
	}

	public int PerkLevel(Perk perk)
	{
		Meta.PerkData perkData = GetPerkData(perk);
		if (perkData != null)
		{
			return perkData.Level;
		}
		return 1;
	}

	public int PerkTotalExperience(Perk perk)
	{
		Meta.PerkData perkData = GetPerkData(perk);
		if (perkData != null)
		{
			return perkData.Experience;
		}
		return 0;
	}

	public int PerkExperienceByLevel(int level)
	{
		return perkLevelDatabase.GetLevelExperience(level);
	}

	private void AddWeaponData(WeaponData data)
	{
		weapons.Add(data);
	}

	private WeaponData CreateWeaponData(string weaponName)
	{
		WeaponData weaponData = new WeaponData(weaponName);
		AddWeaponData(weaponData);
		return weaponData;
	}

	public WeaponData CreateOrGetWeaponData(string weaponName)
	{
		WeaponData weaponData = GetWeaponData(weaponName);
		if (weaponData == null)
		{
			weaponData = CreateWeaponData(weaponName);
		}
		return weaponData;
	}

	private WeaponData GetWeaponData(string weaponName)
	{
		foreach (WeaponData weapon in weapons)
		{
			if (weapon.Name == weaponName)
			{
				return weapon;
			}
		}
		return null;
	}

	public bool UnlockWeapon(string weaponName)
	{
		if (!IsWeaponUnlocked(weaponName))
		{
			CreateWeaponData(weaponName);
			OnWeaponUnlocked(GetWeaponData(weaponName));
			return true;
		}
		return false;
	}

	public void WeaponAddExperience(string weaponName, int experience)
	{
		WeaponData weaponData = CreateOrGetWeaponData(weaponName);
		if (weaponData == null || !ShouldProgress(weaponData))
		{
			return;
		}
		int totalExperience = weaponData.AddExperience(experience);
		int level = weaponData.Level;
		if (level == WeaponMaxLevel)
		{
			return;
		}
		int levelByExperience = weaponLevelDatabase.GetLevelByExperience(totalExperience);
		if (level != levelByExperience)
		{
			for (int i = level; i < levelByExperience; i++)
			{
				weaponData.LevelUp();
				OnWeaponLevelUp(weaponData);
			}
		}
	}

	private void OnWeaponUnlocked(WeaponData data)
	{
		WeaponFactory weaponFactory = WeaponFactory.Instance;
		EventSystem eventSystem = EventSystem.Instance;
		if (weaponFactory != null && eventSystem != null)
		{
			Weapon weapon = weaponFactory.WeaponTemplate(data.Name);
			if (weapon != null && weapon.ShopDisplayMode != Weapon.ShopDisplayModes.AlwaysUnlocked)
			{
				eventSystem.Trigger(new WeaponUnlocked(weapon));
				progressionLogs.WeaponUnlock(data);
			}
		}
	}

	private void OnWeaponLevelUp(WeaponData data)
	{
		WeaponFactory weaponFactory = WeaponFactory.Instance;
		EventSystem eventSystem = EventSystem.Instance;
		if (weaponFactory != null && eventSystem != null)
		{
			Weapon weapon = weaponFactory.WeaponTemplate(data.Name);
			if (weapon != null)
			{
				eventSystem.Trigger(new WeaponLevelUp(weapon, data.Level));
			}
		}
		progressionLogs.WeaponLevelUp(data);
	}

	public bool IsWeaponUnlocked(string weaponName)
	{
		return GetWeaponData(weaponName) != null;
	}

	public int WeaponLevel(string weaponName)
	{
		WeaponData weaponData = GetWeaponData(weaponName);
		if (weaponData != null)
		{
			return weaponData.Level;
		}
		return 1;
	}

	public int WeaponTotalExperience(string weaponName)
	{
		WeaponData weaponData = GetWeaponData(weaponName);
		if (weaponData != null)
		{
			return weaponData.Experience;
		}
		return 0;
	}

	public int WeaponExperienceByLevel(int level)
	{
		return weaponLevelDatabase.GetLevelExperience(level);
	}

	private void AddLevelData(LevelData data)
	{
		levels.Add(data);
	}

	private LevelData CreateLevelData(string levelName)
	{
		LevelData levelData = new LevelData(levelName);
		AddLevelData(levelData);
		return levelData;
	}

	private LevelData CreateOrGetLevelData(string levelName)
	{
		LevelData levelData = GetLevelData(levelName);
		if (levelData == null)
		{
			levelData = CreateLevelData(levelName);
		}
		return levelData;
	}

	private LevelData GetLevelData(string levelName)
	{
		foreach (LevelData level in levels)
		{
			if (level.Name == levelName)
			{
				return level;
			}
		}
		return null;
	}

	public void LevelCompleted(string levelName, DifficultySetting difficulty)
	{
		LevelData levelData = CreateOrGetLevelData(levelName);
		if (!IsLevelCompleted(levelName, difficulty))
		{
			levelData.LevelCompleted(difficulty);
			OnLevelCompleted(levelData, difficulty);
		}
	}

	private void OnLevelCompleted(LevelData data, DifficultySetting difficulty)
	{
		progressionLogs.LevelCompleted(data, difficulty);
	}

	public bool IsLevelCompleted(string levelName, DifficultySetting difficulty)
	{
		LevelData levelData = GetLevelData(levelName);
		if (levelData != null)
		{
			bool result = false;
			switch (difficulty)
			{
			case DifficultySetting.Beginner:
				result = levelData.BeginnerCompleted;
				break;
			case DifficultySetting.Normal:
				result = levelData.NormalCompleted;
				break;
			case DifficultySetting.Hard:
				result = levelData.HardCompleted;
				break;
			case DifficultySetting.Hell:
				result = levelData.HellCompleted;
				break;
			}
			return result;
		}
		return false;
	}

	public bool IsLevelCompletedForFirstTime(string levelName, DifficultySetting difficulty)
	{
		if (IsLevelCompleted(levelName, difficulty))
		{
			LevelCompletedLog? levelCompleted = progressionLogs.GetLevelCompleted();
			if (levelCompleted.HasValue)
			{
				LevelCompletedLog value = levelCompleted.Value;
				return value.levelName == levelName && value.difficulty == difficulty;
			}
		}
		return false;
	}

	public void UpdateAchievement(string levelName, AchievementId stat, AcheivementStatus value)
	{
		LevelData levelData = CreateOrGetLevelData(levelName);
		if (!IsStatAchieved(levelName, stat, value))
		{
			levelData.UpdateAchievement(stat, value);
		}
	}

	public bool IsStatAchieved(string levelName, AchievementId stat, AcheivementStatus value)
	{
		LevelData levelData = GetLevelData(levelName);
		if (levelData != null)
		{
			bool result = false;
			switch (value)
			{
			case AcheivementStatus.Bronze:
				switch (stat)
				{
				case AchievementId.SpeedRunner:
					result = levelData.SpeedBronzeAchieved;
					break;
				case AchievementId.ZedSlayer:
					result = levelData.ZedBronzeAchieved;
					break;
				case AchievementId.MasterDoc:
					result = levelData.DocBronzeAchieved;
					break;
				}
				break;
			case AcheivementStatus.Silver:
				switch (stat)
				{
				case AchievementId.SpeedRunner:
					result = levelData.SpeedSilverAchieved;
					break;
				case AchievementId.ZedSlayer:
					result = levelData.ZedSilverAchieved;
					break;
				case AchievementId.MasterDoc:
					result = levelData.DocSilverAchieved;
					break;
				}
				break;
			case AcheivementStatus.Gold:
				switch (stat)
				{
				case AchievementId.SpeedRunner:
					result = levelData.SpeedGoldAchieved;
					break;
				case AchievementId.ZedSlayer:
					result = levelData.ZedGoldAchieved;
					break;
				case AchievementId.MasterDoc:
					result = levelData.DocGoldAchieved;
					break;
				}
				break;
			}
			return result;
		}
		return false;
	}

	private LifetimeStatsData CreateLifetimeStatsData()
	{
		lifetimeStats = new LifetimeStatsData();
		return lifetimeStats;
	}

	private LifetimeStatsData CreateOrGetLevelData()
	{
		LifetimeStatsData lifetimeStatsData = lifetimeStats;
		if (lifetimeStatsData == null)
		{
			lifetimeStatsData = CreateLifetimeStatsData();
		}
		return lifetimeStatsData;
	}

	public void UpdateStat(LifetimeStat stat, uint amount)
	{
		LifetimeStatsData lifetimeStatsData = CreateOrGetLevelData();
		if (lifetimeStatsData != null && stat != 0)
		{
			lifetimeStatsData.UpdateStat(stat, amount);
		}
	}

	public void UpdateStat(LifetimeStat stat, int amount)
	{
		if (amount > 0)
		{
			UpdateStat(stat, (uint)amount);
		}
	}

	public void UpdateStat(LifetimeStat stat)
	{
		UpdateStat(stat, 1);
	}

	public uint LifetimeStat(LifetimeStat stat)
	{
		if (lifetimeStats != null)
		{
			uint result = 0u;
			switch (stat)
			{
			case Meta.LifetimeStat.Kills:
				result = lifetimeStats.Kills;
				break;
			case Meta.LifetimeStat.Deaths:
				result = lifetimeStats.Deaths;
				break;
			case Meta.LifetimeStat.CashEarned:
				result = lifetimeStats.CashEarned;
				break;
			case Meta.LifetimeStat.SyringesUsed:
				result = lifetimeStats.SyringesUsed;
				break;
			case Meta.LifetimeStat.DamageDone:
				result = lifetimeStats.DamageDone;
				break;
			case Meta.LifetimeStat.EasyGamesCompleted:
				result = lifetimeStats.EasyGamesCompleted;
				break;
			case Meta.LifetimeStat.NormalGamesCompleted:
				result = lifetimeStats.NormalGamesCompleted;
				break;
			case Meta.LifetimeStat.HardGamesCompleted:
				result = lifetimeStats.HardGamesCompleted;
				break;
			case Meta.LifetimeStat.HellGamesCompleted:
				result = lifetimeStats.HellGamesCompleted;
				break;
			case Meta.LifetimeStat.ClotsKilled:
				result = lifetimeStats.ClotsKilled;
				break;
			case Meta.LifetimeStat.GorefastsKilled:
				result = lifetimeStats.GorefastsKilled;
				break;
			case Meta.LifetimeStat.CrawlersKilled:
				result = lifetimeStats.CrawlersKilled;
				break;
			case Meta.LifetimeStat.StalkersKilled:
				result = lifetimeStats.StalkersKilled;
				break;
			case Meta.LifetimeStat.SirensKilled:
				result = lifetimeStats.SirensKilled;
				break;
			case Meta.LifetimeStat.BloatsKilled:
				result = lifetimeStats.BloatsKilled;
				break;
			case Meta.LifetimeStat.HusksKilled:
				result = lifetimeStats.HusksKilled;
				break;
			case Meta.LifetimeStat.ScrakesKilled:
				result = lifetimeStats.ScrakesKilled;
				break;
			case Meta.LifetimeStat.FleshpoundsKilled:
				result = lifetimeStats.FleshpoundsKilled;
				break;
			case Meta.LifetimeStat.PatriarchsKilled:
				result = lifetimeStats.PatriarchsKilled;
				break;
			}
			return result;
		}
		return 0u;
	}

	private List<MetaData> DataByDataType(MetaDataType type)
	{
		List<MetaData> list = new List<MetaData>();
		switch (type)
		{
		case MetaDataType.Perk:
			foreach (Meta.PerkData perk in perks)
			{
				list.Add(perk);
			}
			break;
		case MetaDataType.Weapon:
			foreach (WeaponData weapon in weapons)
			{
				list.Add(weapon);
			}
			break;
		case MetaDataType.Level:
			foreach (LevelData level in levels)
			{
				list.Add(level);
			}
			break;
		case MetaDataType.LifetimeStats:
			if (lifetimeStats != null)
			{
				list.Add(lifetimeStats);
			}
			break;
		}
		return list;
	}

	private MetaData CreateDataByDataType(MetaDataType type)
	{
		MetaData result = null;
		switch (type)
		{
		case MetaDataType.Perk:
			result = CreatePerkData(Perk.None);
			break;
		case MetaDataType.Weapon:
			result = CreateWeaponData(string.Empty);
			break;
		case MetaDataType.Level:
			result = CreateLevelData(string.Empty);
			break;
		case MetaDataType.LifetimeStats:
			result = CreateLifetimeStatsData();
			break;
		}
		return result;
	}

	public string PathByDataType(MetaDataType type)
	{
		string result = string.Empty;
		switch (type)
		{
		case MetaDataType.Perk:
			result = fileManager.PerkPath;
			break;
		case MetaDataType.Weapon:
			result = fileManager.WeaponPath;
			break;
		case MetaDataType.Level:
			result = fileManager.LevelPath;
			break;
		case MetaDataType.LifetimeStats:
			result = fileManager.StatsPath;
			break;
		}
		return result;
	}

	private bool ShouldProgress(MetaData data)
	{
		return true;
	}
}
