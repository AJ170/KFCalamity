using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHud : PlayerHudBase
{
	public HudCashScroll ScrollingCash;

	public TweenPosition PositionTween;

	public AbilityButtonNormal AbilityButton;

	public AbilityButtonNormal UtilityButton1;

	public AbilityButtonNormal UtilityButton2;

	public AbilityButtonNormal UtilityButton3;

	public TweenAlpha BackgroundTweenAlpha;

	public TweenAlpha WeaponFlashAlphaTween;

	public TweenScale WeaponFlashScaleTween;

	public UILabel LabelAmmoCurrent;

	public UILabel LabelAmmoSpare;

	public UILabel LabelAmmoSlash;

	public UILabel LabelSecondaryAmmo;

	public UILabel LabelSecondaryAmmoSpare;

	public UILabel LabelSecondarySlash;

	public UILabel LabelCash;

	public UILabel LabelDeath;

	public UILabel LabelWeaponLevel;

	public UISprite SpriteBackground;

	public UISprite SpriteWeaponBackground;

	public UISprite SpriteAbilityBackground;

	public UISprite SpriteWeaponIcon;

	public UISprite SpriteWeaponSlot1;

	public UISprite SpriteWeaponSlot2;

	public UISprite SpriteWeaponSlot3;

	public UISprite SpriteWeaponSlot4;

	public UISprite SecondaryAmmoLogo;

	public UISprite MainAmmoLogo;

	public UISprite WeaponXPBar;

	public UISprite WeaponXPBarBG;

	public KillStreak killStreakCounter;

	public TweenAlpha Alpha;

	public ControlOverlay[] ControlOverlays;

	public float HealthFeedbackLifeTime = 0.3f;

	private bool isHealthUpdating;

	private float healthTweenDuration;

	private UISprite[] backgrounds;

	private int playerNumber;

	private List<GameObject> colliderList = new List<GameObject>();

	private Health healthScript;

	private CallbackItem<Health, int, int, float> callBackRecieverDamage;

	private HudMoneyHandler moneyHandler = new HudMoneyHandler();

	private TweenPosition[] positionTweens;

	private static bool isCreated;

	public static bool IsCreated
	{
		get
		{
			return isCreated;
		}
	}

	public override KillStreak GetKillStreakCounter()
	{
		return killStreakCounter;
	}

	private ControlOverlay FindOverlayByName(string overlayName)
	{
		if (ControlOverlays == null)
		{
			return null;
		}
		return (ControlOverlays == null) ? null : Array.Find(ControlOverlays, (ControlOverlay a) => !(a == null) && a.Name == overlayName);
	}

	public override AbilityButtonNormal GetAbilityButton(AbilitySlot slot)
	{
		switch (slot)
		{
		case AbilitySlot.Grenade:
			return UtilityButton2;
		case AbilitySlot.SyringeSelf:
			return UtilityButton1;
		case AbilitySlot.SyringeTarget:
			return UtilityButton1;
		case AbilitySlot.Perk:
			return AbilityButton;
		case AbilitySlot.Dodge:
			return UtilityButton3;
		default:
			return null;
		}
	}

	protected override void OnSyncingGui(PlayerHudBase otherHud)
	{
		inventoryWeapons[InventoryWeaponSlot.Melee].SlotSprite = SpriteWeaponSlot1;
		inventoryWeapons[InventoryWeaponSlot.Pistol].SlotSprite = SpriteWeaponSlot2;
		inventoryWeapons[InventoryWeaponSlot.Primary1].SlotSprite = SpriteWeaponSlot3;
		inventoryWeapons[InventoryWeaponSlot.Primary2].SlotSprite = SpriteWeaponSlot4;
		killStreakCounter.gameObject.SetActive(true);
		killStreakCounter.SyncGui(otherHud.GetKillStreakCounter());
	}

	private void Awake()
	{
	}

	public override void Start()
	{
		moneyHandler.MoneyLabel = LabelCash;
		moneyHandler.MoneyScroll = ScrollingCash;
		moneyHandler.PlayerHud = this;
		ScrollingCash.MoneyHandler = moneyHandler;
		if (equippedSlot == InventoryWeaponSlot.None)
		{
			SpriteWeaponIcon.gameObject.SetActive(false);
		}
		SecondaryAmmoLogo.gameObject.SetActive(false);
		SetupWeaponSlot(InventoryWeaponSlot.Melee, SpriteWeaponSlot1);
		SetupWeaponSlot(InventoryWeaponSlot.Pistol, SpriteWeaponSlot2);
		SetupWeaponSlot(InventoryWeaponSlot.Primary1, SpriteWeaponSlot3);
		SetupWeaponSlot(InventoryWeaponSlot.Primary2, SpriteWeaponSlot4);
		LabelDeath.text = Localizer.GetString("gui_dead");
		LabelDeath.enabled = false;
		positionTweens = GetComponentsInChildren<TweenPosition>();
		isCreated = true;
	}

	private new void OnDestroy()
	{
		isCreated = false;
	}

	public override void SetWeaponExperience(int exp, int maxExp, int weaponLvl)
	{
		float num = 0.25f;
		float num2 = 0.72f;
		float num3 = (float)exp / (float)maxExp;
		float num4 = (num2 - num) * num3;
		LabelWeaponLevel.enabled = true;
		WeaponXPBar.enabled = true;
		WeaponXPBarBG.enabled = true;
		LabelWeaponLevel.text = Localizer.GetString("gui_weapon_level") + " " + weaponLvl;
		WeaponXPBar.fillAmount = num + num4;
	}

	protected override void OnWeaponSlotStateChanged(WeaponSlotState slotState, InventoryWeaponSlot slot, InventoryWeapon weapon)
	{
		switch (slotState)
		{
		case WeaponSlotState.Passive:
			weapon.SlotSprite.spriteName = "weapon_slot_passive";
			break;
		case WeaponSlotState.Selected:
			weapon.SlotSprite.spriteName = "weapon_slot_selected";
			break;
		case WeaponSlotState.Empty:
			weapon.SlotSprite.spriteName = "weapon_slot_empty";
			break;
		case WeaponSlotState.None:
			weapon.SlotSprite.spriteName = "weapon_slot_passive";
			break;
		default:
			weapon.SlotSprite.spriteName = "weapon_slot_passive";
			break;
		}
	}

	protected override void OnNoWeapon(InventoryWeaponSlot slot, InventoryWeapon weapon)
	{
		weapon.SlotSprite.gameObject.SetActive(false);
	}

	public override void RegisterHudHealthListener(GameObject soldier)
	{
		healthScript = soldier.GetComponent<Health>();
		callBackRecieverDamage = healthScript.OnDamageTakenSignal.AddReceiver(OnDamageUpdate);
	}

	public override void RemoveHudHealthListener()
	{
		if (healthScript != null && callBackRecieverDamage != null)
		{
			healthScript.OnDamageTakenSignal.RemoveReceiver(callBackRecieverDamage);
		}
		isHealthUpdating = false;
		SpriteBackground.alpha = 0f;
		healthTweenDuration = 0f;
		OnAlphaTweenEnd();
	}

	private void OnHealthTweenTick()
	{
		if (isHealthUpdating)
		{
			healthTweenDuration -= Time.deltaTime;
			if (healthTweenDuration <= 0f)
			{
				isHealthUpdating = false;
				BackgroundTweenAlpha.style = UITweener.Style.Once;
				BackgroundTweenAlpha.gameObject.SetActive(false);
			}
		}
	}

	public override void OnDamageUpdate(Health UnitHealthComponent, int i, int k, float f)
	{
		healthTweenDuration = HealthFeedbackLifeTime;
		BackgroundTweenAlpha.style = UITweener.Style.Loop;
		if (!isHealthUpdating)
		{
			BackgroundTweenAlpha.gameObject.SetActive(true);
			BackgroundTweenAlpha.enabled = true;
			BackgroundTweenAlpha.Reset();
			BackgroundTweenAlpha.Play(true);
			isHealthUpdating = true;
		}
	}

	public override void OnScaleTweenEnd()
	{
		if (!isHealthUpdating)
		{
			BackgroundTweenAlpha.Play(false);
		}
	}

	public override void OnAlphaTweenEnd()
	{
		if (!isHealthUpdating)
		{
			BackgroundTweenAlpha.enabled = false;
		}
	}

	private void TriggerWeaponSwapFlash()
	{
		if (WeaponFlashAlphaTween != null && WeaponFlashScaleTween != null)
		{
			WeaponFlashAlphaTween.gameObject.SetActive(true);
			WeaponFlashAlphaTween.enabled = true;
			WeaponFlashAlphaTween.Reset();
			WeaponFlashAlphaTween.callWhenFinished = "OnWeaponSwapFlashEneded";
			WeaponFlashAlphaTween.eventReceiver = base.gameObject;
			WeaponFlashAlphaTween.Play(true);
			WeaponFlashScaleTween.enabled = true;
			WeaponFlashScaleTween.Reset();
			WeaponFlashScaleTween.Play(true);
		}
	}

	public override void OnWeaponSwapFlashEneded()
	{
		WeaponFlashAlphaTween.gameObject.SetActive(false);
	}

	public override void Update()
	{
		OnHealthTweenTick();
		CheckForNullColliders();
	}

	private void FadeOut()
	{
		Alpha.Reset2();
		Alpha.to = 0.5f;
		Alpha.from = Alpha.alpha;
		Alpha.enabled = true;
	}

	private void FadeIn()
	{
		Alpha.Reset2();
		Alpha.to = 1f;
		Alpha.from = Alpha.alpha;
		Alpha.enabled = true;
	}

	public override void ToggleAllTweeens(bool toggle)
	{
		PositionTween.enabled = toggle;
	}

	public override void TweenIn()
	{
		positionTweens = GetComponentsInChildren<TweenPosition>();
		TweenPosition[] array = positionTweens;
		foreach (TweenPosition tweenPosition in array)
		{
			tweenPosition.enabled = true;
			tweenPosition.Play(true);
		}
		if (base.GameStatistics != null)
		{
			base.GameStatistics.TweenIn();
		}
	}

	public override void TweenOut()
	{
		positionTweens = GetComponentsInChildren<TweenPosition>();
		TweenPosition[] array = positionTweens;
		foreach (TweenPosition tweenPosition in array)
		{
			tweenPosition.delay = 0f;
			tweenPosition.Play(false);
		}
		if (base.GameStatistics != null)
		{
			base.GameStatistics.TweenOut();
		}
	}

	public override void OnTriggerEnter(Collider collider)
	{
		if (!colliderList.Contains(collider.gameObject))
		{
			colliderList.Add(collider.gameObject);
			if (colliderList.Count == 1)
			{
				FadeOut();
			}
		}
	}

	public override void OnTriggerExit(Collider collider)
	{
		if (colliderList.Contains(collider.gameObject))
		{
			colliderList.Remove(collider.gameObject);
			if (colliderList.Count == 0)
			{
				FadeIn();
			}
		}
	}

	public override void CheckForNullColliders()
	{
		if (colliderList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < colliderList.Count; i++)
		{
			if (colliderList[i] == null || !colliderList[i].activeSelf)
			{
				colliderList.RemoveAt(i);
				break;
			}
		}
		if (colliderList.Count == 0)
		{
			FadeIn();
		}
	}

	public override void OnInitialize(GuiInGameManager manager, PlayerGUI pGui, int playerNumber, Vector2 pixelOffset, Camera camera)
	{
		SpriteWeaponBackground.spriteName = "hud_weapon_bg_p" + ID;
		SpriteAbilityBackground.spriteName = "hud_abi_bg_p" + ID;
	}

	public override void OnRespawn()
	{
		LabelDeath.enabled = false;
		LabelDeath.gameObject.SetActive(false);
		AbilityButton.gameObject.SetActive(true);
		UtilityButton1.gameObject.SetActive(true);
		UtilityButton2.gameObject.SetActive(true);
		UtilityButton3.gameObject.SetActive(true);
		LabelAmmoCurrent.gameObject.SetActive(true);
		LabelAmmoSpare.gameObject.SetActive(true);
		LabelAmmoSlash.gameObject.SetActive(true);
		LabelSecondaryAmmo.gameObject.SetActive(true);
		LabelSecondarySlash.gameObject.SetActive(true);
		LabelSecondaryAmmoSpare.gameObject.SetActive(true);
		LabelCash.gameObject.SetActive(true);
		SpriteAbilityBackground.gameObject.SetActive(true);
		killStreakCounter.gameObject.SetActive(true);
	}

	public override void DisablePlayerGUI()
	{
	}

	public override void ActivatePlayerGUI()
	{
	}

	public override void AddAbility(AbilitySlot slot, string image, int charges)
	{
		if (!abilities.ContainsKey(slot))
		{
			switch (slot)
			{
			case AbilitySlot.Grenade:
			{
				AbilityButtonNormal utilityButton = UtilityButton2;
				utilityButton.SetCharges(charges);
				abilities.Add(slot, utilityButton);
				break;
			}
			case AbilitySlot.SyringeSelf:
			{
				AbilityButtonNormal utilityButton = UtilityButton1;
				utilityButton.SetCharges(charges);
				abilities.Add(slot, utilityButton);
				break;
			}
			case AbilitySlot.Perk:
			{
				AbilityButtonNormal utilityButton = AbilityButton;
				utilityButton.SetIconImage(image);
				abilities.Add(slot, utilityButton);
				break;
			}
			case AbilitySlot.Dodge:
			{
				AbilityButtonNormal utilityButton = UtilityButton3;
				utilityButton.SetIconImage(image);
				abilities.Add(slot, utilityButton);
				break;
			}
			}
		}
	}

	protected override void SetAmmoText()
	{
		if (equippedWeapon != null)
		{
			MainAmmoLogo.gameObject.SetActive(true);
			LabelAmmoSlash.text = "|";
			if (equippedWeapon.LoadedAmmunition == 0)
			{
				LabelAmmoCurrent.color = Color.gray;
			}
			else
			{
				LabelAmmoCurrent.color = Color.white;
			}
			if (equippedWeapon.SpareAmmunition == 0)
			{
				LabelAmmoSpare.color = Color.gray;
			}
			else
			{
				LabelAmmoSpare.color = Color.white;
			}
			if (equippedWeapon.AmmunitionDisplay == AmmoDisplayType.Magazine)
			{
				LabelAmmoCurrent.text = equippedWeapon.LoadedAmmunition.ToString();
				LabelAmmoSpare.text = equippedWeapon.SpareAmmunition.ToString();
			}
			else if (equippedWeapon.AmmunitionDisplay == AmmoDisplayType.Shells)
			{
				LabelAmmoCurrent.text = equippedWeapon.LoadedAmmunition.ToString();
				LabelAmmoSpare.text = equippedWeapon.SpareAmmunition.ToString();
			}
			else if (equippedWeapon.AmmunitionDisplay == AmmoDisplayType.Total)
			{
				LabelAmmoCurrent.text = (equippedWeapon.LoadedAmmunition + equippedWeapon.SpareAmmunition).ToString();
				LabelAmmoSpare.text = string.Empty;
				LabelAmmoSlash.text = string.Empty;
			}
			else
			{
				MainAmmoLogo.gameObject.SetActive(false);
				LabelAmmoCurrent.text = string.Empty;
				LabelAmmoSpare.text = string.Empty;
				LabelAmmoSlash.text = string.Empty;
			}
			if (equippedWeapon.HasSecondaryModule())
			{
				SecondaryAttack secondaryAttackModule = equippedWeapon.SecondaryAttackModule;
				if (secondaryAttackModule.UseAmmunition)
				{
					if (secondaryAttackModule.LoadedAmmunition == 0)
					{
						LabelSecondaryAmmo.color = Color.gray;
					}
					else
					{
						LabelSecondaryAmmo.color = Color.white;
					}
					if (secondaryAttackModule.SpareAmmunition == 0)
					{
						LabelSecondaryAmmoSpare.color = Color.gray;
					}
					else
					{
						LabelSecondaryAmmoSpare.color = Color.white;
					}
					SecondaryAmmoLogo.gameObject.SetActive(true);
					if (equippedWeapon.SoldierPerk == Perk.Medic)
					{
						SecondaryAmmoLogo.spriteName = "hud_secondary_medic";
					}
					else
					{
						SecondaryAmmoLogo.spriteName = "hud_secondary_grenade";
					}
					LabelSecondaryAmmo.text = secondaryAttackModule.LoadedAmmunition.ToString();
					LabelSecondaryAmmoSpare.text = secondaryAttackModule.SpareAmmunition.ToString();
					LabelSecondarySlash.text = "|";
					LabelSecondaryAmmo.gameObject.SetActive(true);
					LabelSecondaryAmmoSpare.gameObject.SetActive(true);
					LabelSecondarySlash.gameObject.SetActive(true);
				}
				else
				{
					SecondaryAmmoLogo.gameObject.SetActive(false);
					LabelSecondaryAmmo.text = string.Empty;
					LabelSecondarySlash.text = string.Empty;
					LabelSecondaryAmmoSpare.text = string.Empty;
					LabelSecondaryAmmo.gameObject.SetActive(false);
					LabelSecondaryAmmoSpare.gameObject.SetActive(false);
					LabelSecondarySlash.gameObject.SetActive(false);
				}
			}
			else
			{
				SecondaryAmmoLogo.gameObject.SetActive(false);
				LabelSecondaryAmmo.text = string.Empty;
				LabelSecondarySlash.text = string.Empty;
				LabelSecondaryAmmoSpare.text = string.Empty;
				LabelSecondaryAmmo.gameObject.SetActive(false);
				LabelSecondaryAmmoSpare.gameObject.SetActive(false);
				LabelSecondarySlash.gameObject.SetActive(false);
			}
		}
		else
		{
			MainAmmoLogo.gameObject.SetActive(false);
			LabelAmmoCurrent.text = string.Empty;
			LabelAmmoSpare.text = string.Empty;
			LabelAmmoSlash.text = string.Empty;
		}
	}

	public override void EquipWeapon(InventoryWeaponSlot slot, Weapon newEquippedWeapon)
	{
		if (equippedSlot != InventoryWeaponSlot.None)
		{
			TriggerWeaponSwapFlash();
		}
		base.EquipWeapon(slot, newEquippedWeapon);
		SpriteWeaponIcon.gameObject.SetActive(true);
		SpriteWeaponIcon.spriteName = newEquippedWeapon.Image;
		SpriteWeaponIcon.MakePixelPerfect();
	}

	protected override void OnEquippedWeaponRemoved(InventoryWeaponSlot slot)
	{
		SpriteWeaponIcon.gameObject.SetActive(false);
	}

	private void SetWeaponSlotTexture()
	{
		foreach (KeyValuePair<InventoryWeaponSlot, InventoryWeapon> inventoryWeapon in inventoryWeapons)
		{
			inventoryWeapon.Value.SlotSprite.gameObject.SetActive(true);
			if (inventoryWeapon.Value.WeaponState == WeaponSlotState.Selected)
			{
				inventoryWeapon.Value.SlotSprite.spriteName = "weapon_slot_selected";
			}
			else if (inventoryWeapon.Value.WeaponState == WeaponSlotState.Passive && inventoryWeapon.Value.HasAmmo)
			{
				inventoryWeapon.Value.SlotSprite.spriteName = "weapon_slot_passive";
			}
			else if (inventoryWeapon.Value.WeaponState == WeaponSlotState.Passive && !inventoryWeapon.Value.HasAmmo && inventoryWeapon.Value.UseAmmunition)
			{
				inventoryWeapon.Value.SlotSprite.spriteName = "weapon_slot_empty";
			}
			else
			{
				inventoryWeapon.Value.SlotSprite.gameObject.SetActive(false);
			}
		}
	}

	public override void AddMoney(int baseAmount, int money)
	{
		moneyHandler.AddMoney(baseAmount, money, 0f);
	}

	public override void AddMoney(int baseAmount, int money, float multiplier)
	{
		moneyHandler.AddMoney(baseAmount, money, multiplier);
	}

	public override void SyncMoneyDisplay()
	{
		moneyHandler.SyncMoney();
	}

	public override void SetKillStreak(int killCount, float moneyMultiplier)
	{
		killStreakCounter.SetKillStreak(killCount, 0f);
	}

	public override string GetCurrentHexCodeColor()
	{
		return killStreakCounter.GetCurrentHexCodeColor();
	}

	public override void ResetKillStreak()
	{
		killStreakCounter.Reset();
	}

	public override void OnSoldierDespawn()
	{
		ResetKillStreak();
		RemoveAbility(AbilitySlot.Dodge);
		RemoveAbility(AbilitySlot.Grenade);
		RemoveAbility(AbilitySlot.Perk);
		RemoveAbility(AbilitySlot.SyringeSelf);
		RemoveInventoryWeapon(InventoryWeaponSlot.Melee);
		RemoveInventoryWeapon(InventoryWeaponSlot.Pistol);
		RemoveInventoryWeapon(InventoryWeaponSlot.Primary1);
		RemoveInventoryWeapon(InventoryWeaponSlot.Primary2);
		if (LabelDeath != null)
		{
			LabelDeath.gameObject.SetActive(true);
			LabelDeath.enabled = true;
		}
		if (SpriteWeaponIcon != null)
		{
			SpriteWeaponIcon.spriteName = "hud_player_dead";
			SpriteWeaponIcon.MakePixelPerfect();
			SpriteWeaponIcon.gameObject.SetActive(true);
		}
		AbilityButton.gameObject.SetActive(false);
		UtilityButton1.gameObject.SetActive(false);
		UtilityButton2.gameObject.SetActive(false);
		UtilityButton3.gameObject.SetActive(false);
		LabelAmmoCurrent.gameObject.SetActive(false);
		LabelAmmoSpare.gameObject.SetActive(false);
		LabelAmmoSlash.gameObject.SetActive(false);
		LabelSecondaryAmmo.gameObject.SetActive(false);
		LabelSecondarySlash.gameObject.SetActive(false);
		LabelSecondaryAmmoSpare.gameObject.SetActive(false);
		LabelCash.gameObject.SetActive(false);
		SpriteAbilityBackground.gameObject.SetActive(false);
		SpriteWeaponSlot1.gameObject.SetActive(false);
		SpriteWeaponSlot2.gameObject.SetActive(false);
		SpriteWeaponSlot3.gameObject.SetActive(false);
		SpriteWeaponSlot4.gameObject.SetActive(false);
		SecondaryAmmoLogo.gameObject.SetActive(false);
		MainAmmoLogo.gameObject.SetActive(false);
		killStreakCounter.gameObject.SetActive(false);
	}
}
