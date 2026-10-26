using EffectEvents;
using EffectHelpers;
using EventHelpers;
using Meta;
using UnityEngine;

public class KillStatController : MonoBehaviour, IEffectTriggerListener
{
	private MetaManager metaManager;

	private EffectController effectController;

	private PlayerWeaponController weaponController;

	private PlayerStats stats;

	private PlayerGUILink GUILink;

	private AttributeModifier moneyReceived;

	private AttributeModifier experienceReceived;

	private AttributeModifier killstreakPenalty;

	private Meta.PerkData perkData;

	private float lastKillStreakMultiplier = 1f;

	private float killStreakPenaltyCooldownCounter;

	private bool blockKillstreak;

	public bool BlockKillStreak
	{
		get
		{
			return blockKillstreak;
		}
		set
		{
			blockKillstreak = value;
		}
	}

	private void Awake()
	{
		GameObject gameObject = base.transform.root.gameObject;
		metaManager = MetaManager.Instance;
		effectController = GetComponent<EffectController>();
		weaponController = GetComponent<PlayerWeaponController>();
		GUILink = gameObject.GetComponent<PlayerGUILink>();
	}

	private void Start()
	{
		if (!GlobalHelpers.RegisterEffectEventListener(base.gameObject, this))
		{
			Debug.LogError(string.Format("KillStatController:Start() -> Didn't find 'EffectEventDelegator' in {0}", base.transform.root.name));
		}
		GameObject gameObject = base.transform.root.gameObject;
		Soldier component = gameObject.GetComponent<Soldier>();
		if (component != null)
		{
			stats = component.Stats;
			if (metaManager != null)
			{
				perkData = metaManager.CreateOrGetPerkData(component.Perk);
			}
		}
		moneyReceived = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.MoneyReceived, gameObject);
		experienceReceived = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.ExperienceReceived, gameObject);
		killstreakPenalty = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.KillstreakPenalty, gameObject);
	}

	private void Update()
	{
		if (killStreakPenaltyCooldownCounter > 0f)
		{
			killStreakPenaltyCooldownCounter = Mathf.Max(killStreakPenaltyCooldownCounter - Time.deltaTime, 0f);
		}
	}

	public void OnEffectTrigger(TriggerBase e)
	{
		if (stats == null)
		{
			return;
		}
		if (e.Id == TriggerId.DamageDealt)
		{
			DamageDealt damageDealt = e as DamageDealt;
			stats.AddDamageDealt(damageDealt.Amount);
			LifetimeStatsHandler.AddDamageDone(damageDealt.Amount);
		}
		else if (e.Id == TriggerId.HealingDealt)
		{
			HealingDealt healingDealt = e as HealingDealt;
			stats.AddHealingDealt(healingDealt.Amount);
		}
		else if (e.Id == TriggerId.BeingHit)
		{
			if (killStreakPenaltyCooldownCounter == 0f)
			{
				KillStreakPenalty();
				killStreakPenaltyCooldownCounter = 0.5f;
			}
		}
		else if (e.Id == TriggerId.Kill)
		{
			Kill kill = e as Kill;
			Zed zed = ZedByObject(kill.Victim);
			if (zed != null)
			{
				OnKilledZed(zed);
			}
			LifetimeStatsHandler.AddKill();
		}
		else if (e.Id == TriggerId.BeingKilled)
		{
			BeingKilled beingKilled = (BeingKilled)e;
			string text = ((!(beingKilled.Attacker == base.gameObject)) ? beingKilled.Attacker.name : "Suicide");
			string text2 = string.Empty;
			switch (GameState.Instance.GameSettings.Difficulty)
			{
			case DifficultySetting.Beginner:
				text2 = Localizer.GetString("gui_beginner");
				break;
			case DifficultySetting.Normal:
				text2 = Localizer.GetString("gui_normal");
				break;
			case DifficultySetting.Hard:
				text2 = Localizer.GetString("gui_hard");
				break;
			case DifficultySetting.Hell:
				text2 = Localizer.GetString("gui_hell");
				break;
			}
			GA.API.Design.NewEvent("Cause of Death:" + text2 + ":" + text + ":" + beingKilled.SystemEffect.name, base.gameObject.transform.position);
		}
		else if (e.Id == TriggerId.Death)
		{
			stats.AddDeath();
			LifetimeStatsHandler.AddDeath();
		}
	}

	private void OnKilledZed(Zed zed)
	{
		if (zed != null)
		{
			ChanceToDropWeapon(zed);
			stats.AddKill();
			ProcessKillStreak();
			LifetimeStatsHandler.AddZedKill(zed);
			AddMoney(zed.Bounty);
			int experience = zed.Experience;
			PerkProgression(experience);
			WeaponProgression(experience * 2);
		}
	}

	private void AddMoney(int money)
	{
		float multiplier = moneyReceived.Multiplier;
		int num = stats.AddMoney(money, multiplier * lastKillStreakMultiplier);
		if (num > 0 && GUILink != null)
		{
			LifetimeStatsHandler.AddCashEarned(num);
			money = (int)((float)money * multiplier);
			GUILink.OnMoneyGained(money, num, lastKillStreakMultiplier);
		}
	}

	private void ChanceToDropWeapon(Zed zed)
	{
		if (!(zed != null))
		{
			return;
		}
		WeaponDropManager instance = WeaponDropManager.Instance;
		if (instance != null)
		{
			string weaponName = string.Empty;
			if (instance.DropWeapon(zed, out weaponName))
			{
				UnlockWeapon(weaponName);
			}
		}
	}

	private void PerkProgression(int experience)
	{
		if (metaManager != null && experienceReceived != null)
		{
			metaManager.PerkAddExperience(perkData, (int)((float)experience * experienceReceived.Multiplier));
		}
	}

	private bool UnlockWeapon(string weaponName)
	{
		if (metaManager != null && !string.IsNullOrEmpty(weaponName))
		{
			return metaManager.UnlockWeapon(weaponName);
		}
		return false;
	}

	private void WeaponProgression(int experience)
	{
		if (metaManager != null && weaponController != null && weaponController.ActiveWeapon != null)
		{
			string text = weaponController.ActiveWeapon.Name;
			if (!string.IsNullOrEmpty(text))
			{
				metaManager.WeaponAddExperience(text, experience);
				SetWeaponExp(text);
			}
		}
	}

	public void SetWeaponExp(string weaponName)
	{
		WeaponData weaponData = metaManager.CreateOrGetWeaponData(weaponName);
		int weaponMaxLevel = MetaManager.Instance.WeaponMaxLevel;
		int num = Mathf.Min(weaponData.Level, weaponMaxLevel);
		int num2 = Mathf.Min(num + 1, weaponMaxLevel);
		int num3 = 0;
		int num4 = 0;
		if (num == weaponMaxLevel && num2 == weaponMaxLevel)
		{
			num3 = metaManager.WeaponExperienceByLevel(num2) - metaManager.WeaponExperienceByLevel(num - 1);
			num4 = num3;
		}
		else
		{
			num3 = metaManager.WeaponExperienceByLevel(num2) - metaManager.WeaponExperienceByLevel(num);
			num4 = weaponData.Experience - metaManager.WeaponExperienceByLevel(weaponData.Level);
		}
		GUILink.SetWeaponExperience(num4, num3, num);
	}

	private Zed ZedByObject(GameObject go)
	{
		if (go != null)
		{
			return go.GetComponent<Zed>();
		}
		return null;
	}

	private void ProcessKillStreak()
	{
		if (stats == null)
		{
			return;
		}
		if (!blockKillstreak)
		{
			int killStreak = stats.KillStreak;
			float moneyMultiplier = 1f;
			if (killStreak >= 10 && killStreak < 20)
			{
				moneyMultiplier = 1.25f;
			}
			else if (killStreak >= 20 && killStreak < 30)
			{
				moneyMultiplier = 1.5f;
			}
			else if (killStreak >= 30)
			{
				moneyMultiplier = 2f;
			}
			lastKillStreakMultiplier = moneyMultiplier;
			if (GUILink != null)
			{
				GUILink.SetKillStreak(killStreak, moneyMultiplier);
			}
			switch (killStreak)
			{
			case 10:
				SpeechManager.PlayGlobalSpeech(SpeechManager.GlobalSpeechLine.KillStreakShort);
				break;
			case 30:
				SpeechManager.PlayGlobalSpeech(SpeechManager.GlobalSpeechLine.KillStreakLong);
				break;
			case 100:
				SpeechManager.PlayGlobalSpeech(SpeechManager.GlobalSpeechLine.KillStreakExtreme);
				break;
			}
		}
		else
		{
			stats.ResetKillStreak();
		}
	}

	private void KillStreakPenalty()
	{
		if (stats == null)
		{
			return;
		}
		bool protectionBuff = false;
		if (effectController != null)
		{
			protectionBuff = effectController.HasEffect("SuperResistanceBonusEffect");
		}
		if (stats.KillStreakPenalty(protectionBuff, killstreakPenalty.Multiplier))
		{
			if (GUILink != null)
			{
				GUILink.ResetKillStreak();
			}
			lastKillStreakMultiplier = 1f;
			ProcessKillStreak();
		}
	}

	private void OnResetKillStreak()
	{
		if (stats != null)
		{
			stats.ResetKillStreak();
		}
		if (GUILink != null)
		{
			GUILink.ResetKillStreak();
		}
		lastKillStreakMultiplier = 1f;
	}
}
