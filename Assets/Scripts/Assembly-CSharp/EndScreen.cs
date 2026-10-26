using System.Collections.Generic;
using Acheivements;
using Calamity.Input;
using Meta;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
	public enum Stage
	{
		SpawnBanner = 0,
		SpawnLevelMedal = 1,
		SpawnLargeAchievements = 2,
		SpawnSmallAchievements = 3,
		SpawnPlayerStats = 4,
		ChangePage = 5,
		ChangePageSlow = 6,
		ChangeToLastPage = 7,
		None = 8
	}

	public enum AchievementType
	{
		None = 0,
		Unlock = 1,
		WeaponLevelUp = 2,
		PerkLevelUp = 3,
		MasterDoc = 4,
		SpeedRunner = 5,
		ZedSlayer = 6
	}

	public class EndScreenAchievementData
	{
		public AchievementType Type;

		public AcheivementStatus Status;

		public GameObject Object;

		public string Name;

		public int Level;

		public Perk Perk = Perk.None;

		public bool IsUnlock;
	}

	public class EndScreenLargeAchievementData
	{
		public AchievementType Type;

		public AcheivementStatus Status;

		public GameObject Object;

		public string Name;

		public int Level;

		public Queue<EndScreenAchievementLarge.ProgressLevel> Levels = new Queue<EndScreenAchievementLarge.ProgressLevel>();

		public Perk Perk = Perk.None;

		public bool IsUnlock;
	}

	public EndScreenInputInfo InputInfo;

	public TweenPosition ScrollTween;

	public TweenPosition ButtonsTween;

	public UISprite LeftArrow;

	public UISprite RightArrow;

	public UILabel LabelDifficulty;

	public UILabel LabelGameLength;

	public UILabel LabelMedal;

	public UILabel LabelTitle;

	public TweenScale MedalLabelScale;

	public List<GameObject> Pages;

	public List<EndScreenPlayerStats> PlayerStats;

	public List<GuiButton> Buttons;

	public List<PlayerGUI> playerGuiList = new List<PlayerGUI>();

	public GameObject MedalPrefab;

	public GameObject AchievementPrefab;

	public GameObject AchievementLargePrefab;

	public GameObject VictoryPrefab;

	public GameObject EliminatedPrefab;

	public TweenTable AchievementTable;

	public float ArrowHighlightDuration = 0.2f;

	private Stage stage;

	private GameObject Banner;

	private int scrollIndex;

	private Queue<EndScreenLargeAchievementData> largeAchievementDisplayQueue = new Queue<EndScreenLargeAchievementData>();

	private GameObject currentLargeAchievement;

	private List<EndScreenAchievementData> achievementList = new List<EndScreenAchievementData>();

	private List<GameObject> AchievementElements = new List<GameObject>();

	private string spawnMedalImage = string.Empty;

	private string spawnMedalText = string.Empty;

	private string endWaveText = string.Empty;

	private int counterCompleteIndex;

	private int activePlayers;

	private int callIndex;

	private int buttonIndex;

	private float arrowTickCount;

	private bool handleInput = true;

	private bool handleAxisInput = true;

	private bool achievementSpawned;

	private bool largeAchievementSpawned;

	private bool canHighlightArrows = true;

	private bool statsSpawned;

	private bool displayBumperInfo;

	private bool everythingDisplayed;

	private bool skipLargeAchievement;

	private bool skipSmallAchievements;

	private bool skipSmallAchievementsFast;

	private bool hadAchievements;

	private GuiInGameManager manager;

	private GameSettings settings;

	public GuiInGameManager Manager
	{
		get
		{
			return manager;
		}
	}

	private void Start()
	{
		HideAchievementArrows();
		ProcessAchievements();
		Buttons[buttonIndex].OnSelect();
		MetaManager.Instance.SaveAllData();
		ClearProgressionLogs();
		for (int i = 0; i < Buttons.Count; i++)
		{
			Buttons[i].SetOnReleaseCallback(OnMenuButtonClicked);
			Buttons[i].CustomButtonData = i;
		}
	}

	private void OnMenuButtonClicked(ButtonActions sender)
	{
		int num = (int)sender.CustomData;
		HandleMenuSelection(num);
	}

	private void ClearProgressionLogs()
	{
		ProgressionLogs instance = ProgressionLogs.Instance;
		if (instance != null)
		{
			instance.Reset();
		}
	}

	private void Update()
	{
		HandleInput();
		if (!canHighlightArrows)
		{
			OnArrowResetTick();
		}
	}

	public void SetManagerReference(GuiInGameManager manager)
	{
		this.manager = manager;
	}

	public void SetPlayerGuiReferences(PlayerGUI[] playerGuis)
	{
		activePlayers = PlayerManager.Instance.NumberOfPlayers;
		for (int i = 0; i < 4; i++)
		{
			PlayerStats[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < playerGuis.Length; j++)
		{
			if (playerGuis[j] != null)
			{
				playerGuiList.Add(playerGuis[j]);
				PlayerStats[playerGuis[j].GetID()].gameObject.SetActive(true);
			}
		}
		SetPlayersName();
	}

	private string GetWeaponImageByName(string name)
	{
		string result = string.Empty;
		GameObject gameObject = WeaponFactory.Instance.WeaponPrefabByName(name);
		if (gameObject != null)
		{
			result = gameObject.GetComponent<Weapon>().Image;
		}
		else
		{
			Debug.Log("<EndScreen> WeaponFactory is missing weapon by name: " + name);
		}
		return result;
	}

	private Perk GetWeaponPerkByName(string name)
	{
		Perk result = Perk.None;
		GameObject gameObject = WeaponFactory.Instance.WeaponPrefabByName(name);
		if (gameObject != null)
		{
			result = gameObject.GetComponent<Weapon>().SoldierPerk;
		}
		return result;
	}

	private string GetPerkLocalizerKey(Perk perk)
	{
		string result = string.Empty;
		switch (perk)
		{
		case Perk.Berserker:
			result = "gui_berserker";
			break;
		case Perk.Commando:
			result = "gui_commando";
			break;
		case Perk.Demolition:
			result = "gui_demolition";
			break;
		case Perk.Firebug:
			result = "gui_firebug";
			break;
		case Perk.Medic:
			result = "gui_medic";
			break;
		case Perk.Sharpshooter:
			result = "gui_sharpshooter";
			break;
		case Perk.SupportSpecialist:
			result = "gui_support";
			break;
		case Perk.None:
			result = string.Empty;
			break;
		}
		return result;
	}

	private void HandleInput()
	{
		if (!handleInput)
		{
			return;
		}
		bool @bool = InputManager.GetBool(InputAction.MenuForward);
		bool bool2 = InputManager.GetBool(InputAction.MenuBackward);
		int @int = InputManager.GetInt(InputAction.MenuHorizontal, InputAction.MenuHorizontalAlternate);
		int int2 = InputManager.GetInt(InputAction.MenuVertical, InputAction.MenuVerticalAlternate);
		int num = InputManager.GetInt(InputAction.MenuBumperLeft) * -1 + InputManager.GetInt(InputAction.MenuBumperRight);
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch && !@bool)
		{
			@bool = InputManager.GetBool(InputAction.SkipSplashScreen);
		}
		if (achievementSpawned && statsSpawned && scrollIndex > 0 && hadAchievements)
		{
			TweenPages(@int);
		}
		if (hadAchievements && scrollIndex == 1 && achievementSpawned)
		{
			if (num != 0)
			{
				manager.PlaySound(GuiInGameManager.Sound.ShopList);
			}
			AchievementTable.HandleHorizontalInput(num);
		}
		if (everythingDisplayed)
		{
			if (int2 != 0)
			{
				manager.PlaySound(GuiInGameManager.Sound.ShopList);
			}
			HandleHorizontalAxisInput(int2);
		}
		if (bool2)
		{
			if (!everythingDisplayed)
			{
				switch (stage)
				{
				case Stage.SpawnLargeAchievements:
					if (!largeAchievementSpawned)
					{
						if (currentLargeAchievement != null)
						{
							Object.Destroy(currentLargeAchievement);
						}
						HandleNextPhase(Stage.ChangePage);
						manager.PlaySound(GuiInGameManager.Sound.Forward);
					}
					break;
				case Stage.SpawnSmallAchievements:
					if (!achievementSpawned)
					{
						skipSmallAchievements = true;
						skipSmallAchievementsFast = true;
						manager.PlaySound(GuiInGameManager.Sound.Forward);
					}
					break;
				case Stage.SpawnPlayerStats:
					if (!statsSpawned)
					{
						SetAllStatsInstantly();
						manager.PlaySound(GuiInGameManager.Sound.Forward);
					}
					break;
				}
			}
			else if (scrollIndex == 1)
			{
				HandleNextPhase(Stage.ChangePage);
				manager.PlaySound(GuiInGameManager.Sound.Forward);
			}
			else if (scrollIndex == 2)
			{
				TweenPages(-1);
				manager.PlaySound(GuiInGameManager.Sound.Forward);
			}
		}
		if (!@bool)
		{
			return;
		}
		if (!everythingDisplayed)
		{
			switch (stage)
			{
			case Stage.SpawnLargeAchievements:
				if (!largeAchievementSpawned && !skipLargeAchievement)
				{
					skipLargeAchievement = true;
					if (currentLargeAchievement != null)
					{
						DestroyActiveLargeAchievement();
					}
					manager.PlaySound(GuiInGameManager.Sound.Forward);
				}
				break;
			case Stage.SpawnSmallAchievements:
				if (!achievementSpawned)
				{
					skipSmallAchievements = true;
					manager.PlaySound(GuiInGameManager.Sound.Forward);
				}
				break;
			case Stage.SpawnPlayerStats:
				if (!statsSpawned)
				{
					SetAllStatsInstantly();
					manager.PlaySound(GuiInGameManager.Sound.Forward);
				}
				break;
			}
		}
		else
		{
			handleInput = false;
			handleAxisInput = false;
			if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
			{
				HandleMenuSelection(buttonIndex);
			}
		}
	}

	private void HandleMenuSelection(int buttonIndex)
	{
		switch (buttonIndex)
		{
		case 0:
			manager.RetryLevel();
			break;
		case 1:
			manager.ReturnToMainMenu();
			break;
		default:
			manager.ReturnToMainMenu(true);
			break;
		}
		manager.PlaySound(GuiInGameManager.Sound.Forward);
	}

	public void HandleHorizontalAxisInput(int axis)
	{
		if (axis == 0 || !handleAxisInput)
		{
			return;
		}
		axis *= -1;
		Buttons[buttonIndex].OnDeselect();
		manager.PlaySound(GuiInGameManager.Sound.ShopList);
		if (axis < 0)
		{
			if (buttonIndex == 0)
			{
				buttonIndex = Buttons.Count - 1;
			}
			else
			{
				buttonIndex--;
			}
		}
		else if (axis > 0)
		{
			if (buttonIndex == Buttons.Count - 1)
			{
				buttonIndex = 0;
			}
			else
			{
				buttonIndex++;
			}
		}
		Buttons[buttonIndex].OnSelect();
	}

	public void TweenNextPage()
	{
		TweenPages(1);
	}

	public void TweenPages(int axis)
	{
		if (axis == 0)
		{
			return;
		}
		bool flag = false;
		int num = scrollIndex;
		if (axis < 0)
		{
			if (scrollIndex > 1)
			{
				scrollIndex--;
				flag = true;
			}
		}
		else if (axis > 0 && scrollIndex < 2)
		{
			scrollIndex++;
			flag = true;
		}
		if (flag)
		{
			manager.PlaySound(GuiInGameManager.Sound.MenuButton);
			handleInput = false;
			InputInfo.HideAll();
			float num2 = 0.2f;
			float timePast = ScrollTween.TimePast;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			if (timePast == 1f || timePast == 0f)
			{
				zero.x = scrollIndex * -1280;
				zero2.x = ScrollTween.transform.localPosition.x;
				Tween(zero, zero2, num2);
			}
			else
			{
				num2 = ((num <= scrollIndex) ? (num2 * timePast) : (num2 * timePast));
				zero.x = scrollIndex * -1280;
				zero2.x = ScrollTween.transform.localPosition.x;
				Tween(zero, zero2, num2);
			}
		}
	}

	public void OnTableTweenComplete()
	{
	}

	public void Tween(Vector3 to, Vector3 from, float duration)
	{
		if (ScrollTween == null)
		{
			ScrollTween = base.gameObject.GetComponent<TweenPosition>();
		}
		if (ScrollTween != null)
		{
			ScrollTween.Reset2();
			ScrollTween.to = to;
			ScrollTween.from = from;
			ScrollTween.duration = duration;
			ScrollTween.enabled = true;
			ScrollTween.Play(true);
		}
		else
		{
			Debug.Log("<EndScreenPageBase> - Could not find a TweenPosition component attached to this object.");
		}
	}

	public void Tween(Vector3 to, Vector3 from, float duration, float delay)
	{
		if (ScrollTween == null)
		{
			ScrollTween = base.gameObject.GetComponent<TweenPosition>();
		}
		if (ScrollTween != null)
		{
			ScrollTween.delay = delay;
			Tween(to, from, duration);
		}
		else
		{
			Debug.Log("<EndScreenPageBase> - Could not find a TweenPosition component attached to this object.");
		}
	}

	public void Tween(Vector3 to, Vector3 from, float duration, float delay, GameObject callbackReciever, string callback)
	{
		if (ScrollTween == null)
		{
			ScrollTween = base.gameObject.GetComponent<TweenPosition>();
		}
		if (ScrollTween != null)
		{
			ScrollTween.delay = delay;
			ScrollTween.eventReceiver = callbackReciever;
			ScrollTween.callWhenFinished = callback;
			Tween(to, from, duration);
		}
		else
		{
			Debug.Log("<EndScreenPageBase> - Could not find a TweenPosition component attached to this object.");
		}
	}

	private List<PerkLevelUpLog> SortByPerk(List<PerkLevelUpLog> list)
	{
		List<Perk> list2 = new List<Perk>();
		List<PerkLevelUpLog> list3 = new List<PerkLevelUpLog>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			if (list2.Contains(list[i].perk))
			{
				continue;
			}
			list2.Add(list[i].perk);
			for (int j = 0; j < list.Count; j++)
			{
				if (list[i].perk == list[j].perk)
				{
					list3.Add(list[j]);
				}
			}
		}
		return list3;
	}

	private List<WeaponLevelUpLog> SortWeaponsByLevel(List<WeaponLevelUpLog> list)
	{
		List<string> list2 = new List<string>();
		List<WeaponLevelUpLog> list3 = new List<WeaponLevelUpLog>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			if (list2.Contains(list[i].weaponName))
			{
				continue;
			}
			list2.Add(list[i].weaponName);
			for (int j = 0; j < list.Count; j++)
			{
				if (list[i].weaponName == list[j].weaponName)
				{
					list3.Add(list[j]);
				}
			}
		}
		return list3;
	}

	private void ProcessAchievements()
	{
		if (ProgressionLogs.Instance == null)
		{
			return;
		}
		List<PerkLevelUpLog> perkLevelUps = ProgressionLogs.Instance.GetPerkLevelUps();
		List<WeaponUnlockLog> weaponUnlocks = ProgressionLogs.Instance.GetWeaponUnlocks();
		List<WeaponLevelUpLog> weaponLevelUps = ProgressionLogs.Instance.GetWeaponLevelUps();
		List<AcheivementBase> progressionLogs = AcheivementManager.Instance.GetProgressionLogs();
		for (int i = 0; i < progressionLogs.Count; i++)
		{
			AchievementType type = AchievementType.None;
			AcheivementBase acheivementBase = progressionLogs[i];
			if (acheivementBase.Status != 0)
			{
				switch (acheivementBase.ID)
				{
				case AchievementId.MasterDoc:
					type = AchievementType.MasterDoc;
					break;
				case AchievementId.SpeedRunner:
					type = AchievementType.SpeedRunner;
					break;
				case AchievementId.ZedSlayer:
					type = AchievementType.ZedSlayer;
					break;
				}
				AddAchievement(Perk.None, 0, type, acheivementBase.Status);
			}
		}
		perkLevelUps = SortByPerk(perkLevelUps);
		weaponLevelUps = SortWeaponsByLevel(weaponLevelUps);
		for (int j = 0; j < weaponUnlocks.Count; j++)
		{
			AddAchievement(GetWeaponPerkByName(weaponUnlocks[j].weaponName), weaponUnlocks[j].weaponName, true, AchievementType.Unlock);
			for (int k = 0; k < weaponLevelUps.Count; k++)
			{
				if (weaponUnlocks[j].weaponName == weaponLevelUps[k].weaponName)
				{
					AddAchievement(GetWeaponPerkByName(weaponLevelUps[k].weaponName), weaponLevelUps[k].weaponName, weaponLevelUps[k].level, AchievementType.WeaponLevelUp);
					weaponLevelUps.RemoveAt(k);
					k--;
				}
			}
		}
		for (int l = 0; l < weaponLevelUps.Count; l++)
		{
			AddAchievement(GetWeaponPerkByName(weaponLevelUps[l].weaponName), weaponLevelUps[l].weaponName, weaponLevelUps[l].level, AchievementType.WeaponLevelUp);
		}
		for (int m = 0; m < perkLevelUps.Count; m++)
		{
			AddAchievement(perkLevelUps[m].perk, perkLevelUps[m].level, AchievementType.PerkLevelUp);
		}
		foreach (EndScreenAchievementData achievement in achievementList)
		{
			QueLargeAchievement(achievement);
		}
	}

	private bool HasAchievementElement(string name)
	{
		for (int i = 0; i < AchievementElements.Count; i++)
		{
			EndScreenAchievementSmall component = AchievementElements[i].GetComponent<EndScreenAchievementSmall>();
			if (component != null && component.Type == AchievementType.WeaponLevelUp && component.Name == name)
			{
				return true;
			}
		}
		return false;
	}

	private bool HasAchievementElement(Perk perk)
	{
		for (int i = 0; i < AchievementElements.Count; i++)
		{
			EndScreenAchievementSmall component = AchievementElements[i].GetComponent<EndScreenAchievementSmall>();
			if (component != null && component.Type == AchievementType.PerkLevelUp && component.Perk == perk)
			{
				return true;
			}
		}
		return false;
	}

	private EndScreenAchievementData GetAchievementDataByName(string name)
	{
		for (int i = 0; i < achievementList.Count; i++)
		{
			if (achievementList[i] != null && achievementList[i].Name == name)
			{
				return achievementList[i];
			}
		}
		return null;
	}

	private EndScreenAchievementData GetAchievementDataByPerk(Perk perk)
	{
		for (int i = 0; i < achievementList.Count; i++)
		{
			if (achievementList[i] != null && achievementList[i].Perk == perk)
			{
				return achievementList[i];
			}
		}
		return null;
	}

	private EndScreenAchievementSmall GetAchievementElement(string name)
	{
		for (int i = 0; i < AchievementElements.Count; i++)
		{
			EndScreenAchievementSmall component = AchievementElements[i].GetComponent<EndScreenAchievementSmall>();
			if (component != null && component.Type == AchievementType.WeaponLevelUp && component.Name == name)
			{
				return component;
			}
		}
		return null;
	}

	private EndScreenAchievementSmall GetAchievementElement(Perk perk)
	{
		for (int i = 0; i < AchievementElements.Count; i++)
		{
			EndScreenAchievementSmall component = AchievementElements[i].GetComponent<EndScreenAchievementSmall>();
			if (component != null && component.Type == AchievementType.PerkLevelUp && component.Perk == perk)
			{
				return component;
			}
		}
		return null;
	}

	private void AddAchievement(Perk perk, string name, int level, AchievementType type)
	{
		EndScreenAchievementData endScreenAchievementData = new EndScreenAchievementData();
		endScreenAchievementData.Perk = perk;
		endScreenAchievementData.Type = type;
		endScreenAchievementData.Level = level;
		endScreenAchievementData.Name = name;
		achievementList.Add(endScreenAchievementData);
	}

	private void AddAchievement(Perk perk, string name, bool isUnlock, AchievementType type)
	{
		EndScreenAchievementData endScreenAchievementData = new EndScreenAchievementData();
		endScreenAchievementData.Perk = perk;
		endScreenAchievementData.Type = type;
		endScreenAchievementData.Name = name;
		endScreenAchievementData.IsUnlock = isUnlock;
		achievementList.Add(endScreenAchievementData);
	}

	private void AddAchievement(Perk perk, int level, AchievementType type)
	{
		EndScreenAchievementData endScreenAchievementData = new EndScreenAchievementData();
		endScreenAchievementData.Type = type;
		endScreenAchievementData.Level = level;
		endScreenAchievementData.Perk = perk;
		achievementList.Add(endScreenAchievementData);
	}

	private void AddAchievement(Perk perk, int level, AchievementType type, AcheivementStatus status)
	{
		EndScreenAchievementData endScreenAchievementData = new EndScreenAchievementData();
		endScreenAchievementData.Type = type;
		endScreenAchievementData.Status = status;
		endScreenAchievementData.Level = level;
		endScreenAchievementData.Perk = perk;
		achievementList.Add(endScreenAchievementData);
	}

	private bool DisplayAchievements()
	{
		if (achievementList.Count > 0 && achievementList[0] != null)
		{
			DisplayAchievement(achievementList[0]);
			return true;
		}
		return false;
	}

	private void DisplayAchievement(EndScreenAchievementData achievement)
	{
		bool flag = false;
		if (achievement.Type == AchievementType.PerkLevelUp)
		{
			flag = HasAchievementElement(achievement.Perk);
		}
		else if (achievement.Type == AchievementType.WeaponLevelUp)
		{
			flag = HasAchievementElement(GetWeaponImageByName(achievement.Name));
		}
		if (!flag)
		{
			manager.PlaySound(GuiInGameManager.Sound.AchievementEarned);
			MetaManager instance = MetaManager.Instance;
			if (skipSmallAchievements)
			{
				AchievementElements.Add(AchievementTable.AddInstantly(ref AchievementPrefab));
			}
			else
			{
				AchievementElements.Add(AchievementTable.Add(ref AchievementPrefab));
			}
			EndScreenAchievementSmall component = AchievementElements[AchievementElements.Count - 1].GetComponent<EndScreenAchievementSmall>();
			if (component != null)
			{
				if (achievement.Type == AchievementType.WeaponLevelUp || achievement.Type == AchievementType.Unlock)
				{
					component.SetStartValues(achievement.Type, achievement.Perk, GetPerkLocalizerKey(achievement.Perk), GetWeaponImageByName(achievement.Name), achievement.Status, instance.WeaponLevel(achievement.Name));
				}
				else if (achievement.Type == AchievementType.PerkLevelUp)
				{
					component.SetStartValues(achievement.Type, achievement.Perk, GetPerkLocalizerKey(achievement.Perk), GetPerkLocalizerKey(achievement.Perk), achievement.Status, instance.PerkLevel(achievement.Perk));
				}
				else
				{
					component.SetStartValues(achievement.Type, achievement.Perk, GetPerkLocalizerKey(achievement.Perk), achievement.Name, achievement.Status, 0);
				}
			}
		}
		achievementList.Remove(achievement);
		if (achievementList.Count > 0)
		{
			if (skipSmallAchievements)
			{
				DisplayAchievements();
			}
			else if (!flag)
			{
				Invoke("DisplayAchievements", 0.15f);
			}
			else
			{
				DisplayAchievements();
			}
			return;
		}
		if (AchievementElements.Count > 17)
		{
			displayBumperInfo = true;
		}
		achievementSpawned = true;
		if (!skipSmallAchievementsFast)
		{
			HandleNextPhase(Stage.ChangePageSlow);
			return;
		}
		skipSmallAchievementsFast = false;
		HandleNextPhase(Stage.ChangePage);
	}

	private bool HasLargeQueueAchievement(string name)
	{
		foreach (EndScreenLargeAchievementData item in largeAchievementDisplayQueue)
		{
			if (item != null && item.Name == name)
			{
				return true;
			}
		}
		return false;
	}

	private bool HasLargeQueueAchievement(Perk perk)
	{
		foreach (EndScreenLargeAchievementData item in largeAchievementDisplayQueue)
		{
			if (item != null && item.Perk == perk)
			{
				return true;
			}
		}
		return false;
	}

	private EndScreenLargeAchievementData GetLargeQueueAchievement(Perk perk)
	{
		foreach (EndScreenLargeAchievementData item in largeAchievementDisplayQueue)
		{
			if (item != null && item.Perk == perk)
			{
				return item;
			}
		}
		return null;
	}

	private EndScreenLargeAchievementData GetLargeQueueAchievement(string name)
	{
		foreach (EndScreenLargeAchievementData item in largeAchievementDisplayQueue)
		{
			if (item != null && item.Name == name)
			{
				return item;
			}
		}
		return null;
	}

	private void QueLargeAchievement(EndScreenAchievementData achievement)
	{
		MetaManager instance = MetaManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		EndScreenLargeAchievementData endScreenLargeAchievementData = new EndScreenLargeAchievementData();
		EndScreenAchievementLarge.ProgressLevel progressLevel = new EndScreenAchievementLarge.ProgressLevel();
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		int level = achievement.Level;
		bool flag4 = false;
		int num = 0;
		if (achievement.Type == AchievementType.Unlock)
		{
			flag2 = true;
			endScreenLargeAchievementData.IsUnlock = true;
		}
		else if (achievement.Type == AchievementType.PerkLevelUp)
		{
			flag3 = true;
			if (!HasLargeQueueAchievement(achievement.Perk))
			{
				flag2 = true;
			}
			else
			{
				flag = true;
			}
		}
		else if (achievement.Type == AchievementType.WeaponLevelUp)
		{
			flag3 = true;
			if (!HasLargeQueueAchievement(achievement.Name))
			{
				flag2 = true;
			}
		}
		else
		{
			flag2 = true;
		}
		progressLevel.Level = achievement.Level;
		endScreenLargeAchievementData.Status = achievement.Status;
		endScreenLargeAchievementData.Level = achievement.Level;
		endScreenLargeAchievementData.Name = achievement.Name;
		endScreenLargeAchievementData.Perk = achievement.Perk;
		endScreenLargeAchievementData.Type = achievement.Type;
		endScreenLargeAchievementData.Object = achievement.Object;
		MetaManager instance2 = MetaManager.Instance;
		if (instance2 != null)
		{
			switch (achievement.Type)
			{
			case AchievementType.WeaponLevelUp:
			{
				int num2 = instance2.WeaponLevel(achievement.Name);
				if (level == num2)
				{
					flag4 = true;
				}
				num = instance2.WeaponMaxLevel;
				float num3;
				float num4;
				float num5;
				if (level < num)
				{
					num3 = instance2.WeaponExperienceByLevel(level + 1);
					num4 = instance2.WeaponTotalExperience(achievement.Name);
					num5 = instance2.WeaponExperienceByLevel(level);
				}
				else
				{
					num3 = instance2.WeaponExperienceByLevel(num);
					num4 = instance2.WeaponTotalExperience(achievement.Name);
					num5 = instance2.WeaponExperienceByLevel(num - 1);
				}
				float xpFractionTo = ((!flag4) ? 1f : ((num4 - num5) / (num3 - num5)));
				progressLevel.XpFractionFrom = 0f;
				progressLevel.XpFractionTo = xpFractionTo;
				progressLevel.XpFrom = num5;
				progressLevel.XpTo = num3;
				break;
			}
			case AchievementType.PerkLevelUp:
			{
				int num2 = instance2.PerkLevel(achievement.Perk);
				if (level == num2)
				{
					flag4 = true;
				}
				num = instance2.PerkMaxLevel;
				float num3;
				float num4;
				float num5;
				if (level < num)
				{
					num3 = instance2.PerkExperienceByLevel(level + 1);
					num4 = instance2.PerkTotalExperience(achievement.Perk);
					num5 = instance2.PerkExperienceByLevel(level);
				}
				else
				{
					num3 = instance2.PerkExperienceByLevel(num);
					num4 = instance2.PerkTotalExperience(achievement.Perk);
					num5 = instance2.PerkExperienceByLevel(num - 1);
				}
				float xpFractionTo = ((!flag4) ? 1f : ((num4 - num5) / (num3 - num5)));
				progressLevel.XpFractionFrom = 0f;
				progressLevel.XpFractionTo = xpFractionTo;
				progressLevel.XpFrom = num5;
				progressLevel.XpTo = num3;
				break;
			}
			}
		}
		if (flag2)
		{
			if (flag3)
			{
				endScreenLargeAchievementData.Levels.Enqueue(progressLevel);
			}
			largeAchievementDisplayQueue.Enqueue(endScreenLargeAchievementData);
		}
		else if (flag3)
		{
			if (flag)
			{
				GetLargeQueueAchievement(achievement.Perk).Levels.Enqueue(progressLevel);
			}
			else
			{
				GetLargeQueueAchievement(achievement.Name).Levels.Enqueue(progressLevel);
			}
		}
	}

	private bool DispalyLargeAchievements()
	{
		if (largeAchievementDisplayQueue.Count > 0)
		{
			skipLargeAchievement = false;
			EndScreenLargeAchievementData endScreenLargeAchievementData = largeAchievementDisplayQueue.Dequeue();
			currentLargeAchievement = NGUITools.AddChild(Pages[0], AchievementLargePrefab);
			EndScreenAchievementLarge component = currentLargeAchievement.GetComponent<EndScreenAchievementLarge>();
			if (endScreenLargeAchievementData.Type == AchievementType.WeaponLevelUp || endScreenLargeAchievementData.Type == AchievementType.Unlock)
			{
				component.SetStartValues(endScreenLargeAchievementData.Type, endScreenLargeAchievementData.Levels, endScreenLargeAchievementData.Perk, GetPerkLocalizerKey(endScreenLargeAchievementData.Perk), GetWeaponImageByName(endScreenLargeAchievementData.Name), endScreenLargeAchievementData.Status);
			}
			else if (endScreenLargeAchievementData.Type == AchievementType.PerkLevelUp)
			{
				component.SetStartValues(endScreenLargeAchievementData.Type, endScreenLargeAchievementData.Levels, endScreenLargeAchievementData.Perk, GetPerkLocalizerKey(endScreenLargeAchievementData.Perk), GetPerkLocalizerKey(endScreenLargeAchievementData.Perk), endScreenLargeAchievementData.Status);
			}
			else
			{
				component.SetStartValues(endScreenLargeAchievementData.Type, endScreenLargeAchievementData.Levels, endScreenLargeAchievementData.Perk, GetPerkLocalizerKey(endScreenLargeAchievementData.Perk), endScreenLargeAchievementData.Name, endScreenLargeAchievementData.Status);
			}
			component.SetOnCompleteCallback(this);
			manager.PlaySound(GuiInGameManager.Sound.HudUnlock);
			return true;
		}
		return false;
	}

	public void DestroyActiveLargeAchievement()
	{
		if (!(currentLargeAchievement != null))
		{
			return;
		}
		Object.Destroy(currentLargeAchievement);
		bool flag = false;
		flag = largeAchievementDisplayQueue.Count > 0;
		if (!largeAchievementSpawned)
		{
			if (flag)
			{
				if (skipLargeAchievement)
				{
					DispalyLargeAchievements();
				}
				else
				{
					Invoke("DispalyLargeAchievements", 0.3f);
				}
			}
			else
			{
				largeAchievementSpawned = true;
				HandleNextPhase(Stage.ChangePage);
			}
		}
		else
		{
			HandleNextPhase(Stage.ChangePage);
		}
	}

	private void SpawnMedal()
	{
		GameObject gameObject = NGUITools.AddChild(base.gameObject, MedalPrefab);
		gameObject.transform.position = Vector3.zero;
		gameObject.GetComponent<EndScreenMedal>().SetEndData(new Vector3(0f, -800f, 0f), new Vector3(0.5f, 0.5f, 0.5f), this, "OnMedalSpawnComplete", spawnMedalImage, spawnMedalText);
		manager.PlaySound(GuiInGameManager.Sound.MedalEarned);
	}

	public void OnMedalSpawnComplete()
	{
		LabelMedal.text = spawnMedalText;
		MedalLabelScale.Reset();
		MedalLabelScale.Play(true);
		HandleNextPhase(Stage.SpawnLargeAchievements);
	}

	private void InitializeLevelMedalSettings()
	{
		if (settings != null)
		{
			if (settings.Difficulty == DifficultySetting.Beginner)
			{
				spawnMedalImage = "beginner_medal";
			}
			if (settings.Difficulty == DifficultySetting.Normal)
			{
				spawnMedalImage = "bronze_medal";
			}
			if (settings.Difficulty == DifficultySetting.Hard)
			{
				spawnMedalImage = "silver_medal";
			}
			if (settings.Difficulty == DifficultySetting.Hell)
			{
				spawnMedalImage = "gold_medal";
			}
			spawnMedalText = Localizer.GetString(settings.GuiLevelName) + '\n' + settings.Difficulty.ToString() + " " + Localizer.GetString("gui_difficulty") + '\n' + Localizer.GetString("gui_completed");
			HandleNextPhase(Stage.SpawnLevelMedal);
		}
	}

	private void StartStatsCounter()
	{
		callIndex = 1;
		SetPlayersKills();
	}

	private void SetAllStatsInstantly()
	{
		SetPlayersKills();
		SetPlayersDamage();
		SetPlayersKills();
		SetPlayersKillStreak();
		for (int i = 0; i < playerGuiList.Count; i++)
		{
			PlayerStats[playerGuiList[i].GetID()].SetStatsInstantly();
		}
		statsSpawned = true;
		WhenAllDone();
	}

	private void SetPlayersName()
	{
		counterCompleteIndex = 0;
		for (int i = 0; i < playerGuiList.Count; i++)
		{
			PlayerStats[playerGuiList[i].GetID()].SetName(Localizer.GetString("gui_player") + " " + (playerGuiList[i].GetID() + 1));
		}
	}

	private void SetPlayersKills()
	{
		counterCompleteIndex = 0;
		int num = 0;
		List<int> list = new List<int>();
		for (int i = 0; i < playerGuiList.Count; i++)
		{
			if (playerGuiList[i] != null)
			{
				list.Add(playerGuiList[i].Stats.Kills);
				if (playerGuiList[i].Stats.Kills > num)
				{
					num = playerGuiList[i].Stats.Kills;
				}
			}
		}
		bool flag = false;
		for (int j = 0; j < playerGuiList.Count; j++)
		{
			if (playerGuiList[j] != null)
			{
				flag = ((activePlayers > 1 && playerGuiList[j].Stats.Kills == num) ? true : false);
				PlayerStats[playerGuiList[j].GetID()].SetKills(playerGuiList[j].Stats.Kills, flag);
			}
		}
	}

	private void SetPlayersDeaths()
	{
		counterCompleteIndex = 0;
		int num = -1;
		for (int i = 0; i < playerGuiList.Count; i++)
		{
			if (playerGuiList[i] != null)
			{
				if (playerGuiList[i].Stats.Deaths < num)
				{
					num = playerGuiList[i].Stats.Deaths;
				}
				else if (num == -1)
				{
					num = playerGuiList[i].Stats.Deaths;
				}
			}
		}
		bool flag = false;
		for (int j = 0; j < playerGuiList.Count; j++)
		{
			if (playerGuiList[j] != null)
			{
				flag = ((activePlayers > 1 && playerGuiList[j].Stats.Deaths == num) ? true : false);
				PlayerStats[playerGuiList[j].GetID()].SetDeaths(playerGuiList[j].Stats.Deaths, flag);
			}
		}
	}

	private void SetPlayersDamage()
	{
		counterCompleteIndex = 0;
		int num = 0;
		for (int i = 0; i < playerGuiList.Count; i++)
		{
			if (activePlayers > 1 && playerGuiList[i] != null && playerGuiList[i].Stats.DamageDealt > num)
			{
				num = playerGuiList[i].Stats.DamageDealt;
			}
		}
		bool flag = false;
		for (int j = 0; j < playerGuiList.Count; j++)
		{
			if (playerGuiList[j] != null)
			{
				flag = ((activePlayers > 1 && playerGuiList[j].Stats.DamageDealt == num) ? true : false);
				PlayerStats[playerGuiList[j].GetID()].SetDamage(playerGuiList[j].Stats.DamageDealt, flag);
			}
		}
	}

	public void SetPlayersKillStreak()
	{
		counterCompleteIndex = 0;
		int num = 0;
		for (int i = 0; i < playerGuiList.Count; i++)
		{
			if (playerGuiList[i] != null && playerGuiList[i].Stats.LongestKillStreak > num)
			{
				num = playerGuiList[i].Stats.LongestKillStreak;
			}
		}
		bool flag = false;
		for (int j = 0; j < playerGuiList.Count; j++)
		{
			if (playerGuiList[j] != null)
			{
				flag = ((activePlayers > 1 && playerGuiList[j].Stats.LongestKillStreak == num) ? true : false);
				PlayerStats[playerGuiList[j].GetID()].SetKillStreak(playerGuiList[j].Stats.LongestKillStreak, flag);
			}
		}
	}

	public void OnCounterComplete()
	{
		counterCompleteIndex++;
		if (counterCompleteIndex == activePlayers)
		{
			if (callIndex == 0)
			{
				SetPlayersKills();
			}
			else if (callIndex == 1)
			{
				SetPlayersKillStreak();
			}
			else if (callIndex == 2)
			{
				SetPlayersDeaths();
			}
			else if (callIndex == 3)
			{
				SetPlayersDamage();
			}
			else if (callIndex == 4)
			{
				statsSpawned = true;
				WhenAllDone();
			}
			manager.PlaySound(GuiInGameManager.Sound.ShopPurchaseOther);
			callIndex++;
		}
	}

	private void OnLeftPress()
	{
		StartArrowHighlight();
		LeftArrow.spriteName = "arrow_left_black_outline";
	}

	private void OnRightPress()
	{
		StartArrowHighlight();
		RightArrow.spriteName = "arrow_right_black_outline";
	}

	public void OnButtonRelease()
	{
		LeftArrow.spriteName = "arrow_left_white_outline";
		RightArrow.spriteName = "arrow_right_white_outline";
	}

	public void OnArrowPressed(int axis)
	{
		switch (axis)
		{
		case -1:
			OnLeftPress();
			break;
		case 1:
			OnRightPress();
			break;
		}
	}

	private void HideAchievementArrows()
	{
		LeftArrow.enabled = false;
		RightArrow.enabled = false;
	}

	private void ShowAchievementArrows()
	{
		LeftArrow.enabled = true;
		RightArrow.enabled = true;
	}

	private void ResetArrowHighLight()
	{
		arrowTickCount = 0f;
		canHighlightArrows = true;
		OnButtonRelease();
	}

	private void StartArrowHighlight()
	{
		OnButtonRelease();
		arrowTickCount = 0f;
		canHighlightArrows = false;
	}

	private void OnArrowResetTick()
	{
		arrowTickCount += Time.deltaTime;
		if (arrowTickCount >= ArrowHighlightDuration)
		{
			ResetArrowHighLight();
		}
	}

	private bool SpawnVictoryBanner()
	{
		if (VictoryPrefab != null)
		{
			Banner = NGUITools.AddChild(base.gameObject, VictoryPrefab);
			SetBannerGameInfo();
			return true;
		}
		return false;
	}

	private bool SpawnEliminatedBanner()
	{
		if (EliminatedPrefab != null)
		{
			Banner = NGUITools.AddChild(base.gameObject, EliminatedPrefab);
			SetBannerGameInfo();
			return true;
		}
		return false;
	}

	public void SetBannerGameInfo()
	{
		settings = GameState.Instance.GameSettings;
		string text = string.Empty;
		string text2 = string.Empty;
		if (settings != null)
		{
			if (settings.Difficulty == DifficultySetting.Beginner)
			{
				text2 = Localizer.GetString("gui_beginner");
			}
			else if (settings.Difficulty == DifficultySetting.Normal)
			{
				text2 = Localizer.GetString("gui_normal");
			}
			else if (settings.Difficulty == DifficultySetting.Hard)
			{
				text2 = Localizer.GetString("gui_hard");
			}
			else if (settings.Difficulty == DifficultySetting.Hell)
			{
				text2 = Localizer.GetString("gui_hell");
			}
			if (settings.GameLength == GameLengthSetting.Short)
			{
				text = Localizer.GetString("gui_short");
			}
			else if (settings.GameLength == GameLengthSetting.Normal)
			{
				text = Localizer.GetString("gui_normal");
			}
			else if (settings.GameLength == GameLengthSetting.Long)
			{
				text = Localizer.GetString("gui_long");
			}
			EndScreenBannerVictory component = Banner.GetComponent<EndScreenBannerVictory>();
			component.LabelDifficultyTitle.text = Localizer.GetString("gui_difficulty") + ": " + text2;
			component.LabelLengthTitle.text = Localizer.GetString("gui_game_length") + ": " + text;
			if (component.LabelWave != null)
			{
				component.LabelWave.text = endWaveText;
			}
		}
	}

	public void SetEndResult(bool isVictorious, int wave, int maxWaves)
	{
		settings = GameState.Instance.GameSettings;
		if (isVictorious)
		{
			SpawnVictoryBanner();
			if (MetaManager.Instance.IsLevelCompletedForFirstTime(settings.LevelName, settings.Difficulty))
			{
				Invoke("InitializeLevelMedalSettings", 2.7f);
			}
			else
			{
				Invoke("DelayedLargeAchievementSpawn", 3.7f);
			}
		}
		else
		{
			if (wave == maxWaves)
			{
				endWaveText = Localizer.GetString("gui_end_screen_wave") + " " + Localizer.GetString("gui_end_screen_wave_boss");
			}
			else
			{
				endWaveText = Localizer.GetString("gui_end_screen_wave") + " " + wave;
			}
			SpawnEliminatedBanner();
			Invoke("DelayedLargeAchievementSpawn", 3.7f);
		}
		Buttons[buttonIndex].OnSelect();
		Buttons[0].gameObject.SetActive(false);
		Buttons[1].gameObject.SetActive(false);
		Buttons[2].gameObject.SetActive(false);
	}

	private void DelayedLargeAchievementSpawn()
	{
		HandleNextPhase(Stage.SpawnLargeAchievements);
	}

	private void HandleNextPhase(Stage newStage)
	{
		switch (newStage)
		{
		case Stage.SpawnLevelMedal:
			Invoke("SpawnMedal", 1.5f);
			break;
		case Stage.SpawnLargeAchievements:
			if (!DispalyLargeAchievements())
			{
				HandleNextPhase(Stage.ChangePage);
				break;
			}
			hadAchievements = true;
			UpdateInputInfo();
			InputInfo.TweenIn();
			break;
		case Stage.SpawnSmallAchievements:
			if (!DisplayAchievements())
			{
				HandleNextPhase(Stage.ChangePage);
			}
			break;
		case Stage.SpawnPlayerStats:
			StartStatsCounter();
			if (!InputInfo.IsVisible)
			{
				UpdateInputInfo();
				InputInfo.TweenIn();
			}
			break;
		case Stage.ChangePageSlow:
			Invoke("TweenNextPage", 2f);
			break;
		case Stage.ChangePage:
			TweenNextPage();
			break;
		case Stage.ChangeToLastPage:
			TweenPages(-1);
			break;
		}
		stage = newStage;
	}

	public void OnScrollComplete()
	{
		if (!everythingDisplayed)
		{
			switch (scrollIndex)
			{
			case 1:
				if (!achievementSpawned)
				{
					HandleNextPhase(Stage.SpawnSmallAchievements);
				}
				break;
			case 2:
				if (!statsSpawned)
				{
					HandleNextPhase(Stage.SpawnPlayerStats);
				}
				break;
			}
		}
		UpdateInputInfo();
		handleInput = true;
	}

	public void UpdateInputInfo()
	{
		InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Bumper, EndScreenInputInfo.Row.Top, string.Empty, false);
		switch (scrollIndex)
		{
		case 0:
			InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Forward, EndScreenInputInfo.Row.Top, Localizer.GetString("gui_end_screen_input_skip"), true);
			InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Backward, EndScreenInputInfo.Row.Bottom, Localizer.GetString("gui_end_screen_input_next"), true);
			break;
		case 1:
			if (achievementSpawned)
			{
				if (displayBumperInfo)
				{
					InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Bumper, EndScreenInputInfo.Row.Top, Localizer.GetString("gui_end_screen_input_bumper"), true);
				}
				InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Forward, EndScreenInputInfo.Row.Top, Localizer.GetString("gui_end_screen_input_select"), true);
				InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Backward, EndScreenInputInfo.Row.Bottom, Localizer.GetString("gui_end_screen_input_stats"), true);
			}
			else
			{
				InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Forward, EndScreenInputInfo.Row.Top, Localizer.GetString("gui_end_screen_input_skip"), true);
				InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Backward, EndScreenInputInfo.Row.Bottom, Localizer.GetString("gui_end_screen_input_next"), true);
			}
			break;
		case 2:
			if (statsSpawned)
			{
				if (hadAchievements)
				{
					InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Forward, EndScreenInputInfo.Row.Top, Localizer.GetString("gui_end_screen_input_select"), true);
					InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Backward, EndScreenInputInfo.Row.Bottom, Localizer.GetString("gui_end_screen_input_achievements"), true);
				}
				else
				{
					InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Backward, EndScreenInputInfo.Row.Top, string.Empty, false);
					InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Forward, EndScreenInputInfo.Row.Bottom, Localizer.GetString("gui_end_screen_input_select"), true);
				}
			}
			else
			{
				InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Forward, EndScreenInputInfo.Row.Top, Localizer.GetString("gui_end_screen_input_skip"), true);
				InputInfo.SetButtonInput(EndScreenInputInfo.InputType.Backward, EndScreenInputInfo.Row.Bottom, Localizer.GetString("gui_end_screen_input_skip"), true);
			}
			break;
		}
	}

	private void WhenAllDone()
	{
		handleInput = false;
		InputInfo.HideAll();
		ButtonsTween.callWhenFinished = "OnButtonTweenComplete";
		ButtonsTween.eventReceiver = base.gameObject;
		ButtonsTween.enabled = true;
		ButtonsTween.Play(true);
		everythingDisplayed = true;
		statsSpawned = true;
		Buttons[0].gameObject.SetActive(true);
		Buttons[1].gameObject.SetActive(true);
		Buttons[2].gameObject.SetActive(true);
	}

	public void OnButtonTweenComplete()
	{
		handleInput = true;
		UpdateInputInfo();
	}
}
