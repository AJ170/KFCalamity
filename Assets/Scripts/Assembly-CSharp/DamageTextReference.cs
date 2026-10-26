using UnityEngine;

public class DamageTextReference : MonoBehaviour
{
	public enum settingType
	{
		None = 0,
		Player = 1,
		Zed = 2
	}

	public settingType PremadeSettings;

	public DamageTextSetting Settings;

	private DamageTextSetting premadeSettingPlayer;

	private DamageTextSetting premadeSettingZed;

	public Vector3 Offset = Vector3.zero;

	public Health HealthComponent;

	private DamageText damageText;

	private bool inLineOfSight;

	private bool isStealthed;

	private LineOfSightTarget lineOfSightTarget;

	private StealthController stealthController;

	private void Awake()
	{
		lineOfSightTarget = GetComponent<LineOfSightTarget>();
		stealthController = GetComponent<StealthController>();
		if (stealthController != null)
		{
			stealthController.RegisterListener(OnStealthUpdate);
		}
		if (lineOfSightTarget != null)
		{
			lineOfSightTarget.RegisterGainVisibilityListener(OnGainVisibility);
			lineOfSightTarget.RegisterLostVisibilityListener(OnLostVisibility);
		}
	}

	private void Start()
	{
		InitializeSettings();
		if (GuiInGameManager.Instance != null)
		{
			damageText = GuiInGameManager.Instance.AddDamageText(base.gameObject, HealthComponent, Settings);
			damageText.SetOffset(Offset);
		}
		if (lineOfSightTarget != null)
		{
			InLineOfSight(lineOfSightTarget.Visible);
		}
		if (stealthController != null)
		{
			StealthController.StealthState stealthState = stealthController.GetStealthState();
			OnStealthUpdate(stealthState);
		}
	}

	private void InitializeSettings()
	{
		premadeSettingPlayer = new DamageTextSetting();
		premadeSettingPlayer.WeakColor = new Color(0.7f, 0.698f, 0.56f);
		premadeSettingPlayer.NormalColor = new Color(0.847f, 0.823f, 0.215f);
		premadeSettingPlayer.EffectiveColor = new Color(0.98f, 0.717f, 0.086f);
		premadeSettingPlayer.HealColor = new Color(0.325f, 0.8549f, 0.2549f);
		premadeSettingPlayer.WeakDamageThreshold = 0.9f;
		premadeSettingPlayer.EffectiveDamageThreshold = 1.1f;
		premadeSettingPlayer.IgnoreLineOfSight = true;
		premadeSettingZed = new DamageTextSetting();
		premadeSettingZed.WeakColor = new Color(0.651f, 0.5647f, 0.5294f);
		premadeSettingZed.NormalColor = new Color(0.9215f, 0.3686f, 0.2f);
		premadeSettingZed.EffectiveColor = new Color(1f, 0.098f, 0.098f);
		premadeSettingZed.HealColor = new Color(0.325f, 0.8549f, 0.2549f);
		premadeSettingZed.WeakDamageThreshold = 0.9f;
		premadeSettingZed.EffectiveDamageThreshold = 1.1f;
		premadeSettingZed.IgnoreLineOfSight = false;
		if (PremadeSettings == settingType.Player)
		{
			Settings = premadeSettingPlayer;
		}
		else if (PremadeSettings == settingType.Zed)
		{
			Settings = premadeSettingZed;
		}
	}

	private void OnDestroy()
	{
		if (lineOfSightTarget != null)
		{
			lineOfSightTarget.UnregisterGainVisibilityListener(OnGainVisibility);
			lineOfSightTarget.UnregisterLostVisibilityListener(OnLostVisibility);
		}
		if (stealthController != null)
		{
			stealthController.UnregisterListener(OnStealthUpdate);
		}
		if (damageText != null)
		{
			damageText.TriggerDestroy();
		}
	}

	public void OnStealthUpdate(StealthController.StealthState state)
	{
		switch (state)
		{
		case StealthController.StealthState.Detected:
			SetStealth(true);
			break;
		case StealthController.StealthState.Stealthed:
			SetStealth(false);
			break;
		case StealthController.StealthState.Visible:
			SetStealth(false);
			break;
		}
	}

	private void OnGainVisibility()
	{
		InLineOfSight(true);
	}

	private void OnLostVisibility()
	{
		InLineOfSight(false);
	}

	public void InLineOfSight(bool inSight)
	{
		inLineOfSight = inSight;
		UpdateDamageTextVisibility();
	}

	public void SetStealth(bool stealthed)
	{
		isStealthed = stealthed;
		UpdateDamageTextVisibility();
	}

	public void UpdateDamageTextVisibility()
	{
		if (damageText != null)
		{
			damageText.InLineOfSight = inLineOfSight && !isStealthed;
		}
	}
}
