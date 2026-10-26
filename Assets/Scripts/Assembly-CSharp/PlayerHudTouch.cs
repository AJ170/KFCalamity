using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class PlayerHudTouch : PlayerHudBase
{
	private const string SLOT_TEXTURE_PREFIX_ACTIVE = "_white";

	private const string SLOT_TEXTURE_PREFIX_PASSIVE = "_gray";

	private const string SLOT_TEXTURE_PREFIX_EMPTY = "_red";

	public AbilityButtonNormal AbilityButton;

	public AbilityButtonNormal UtilityButton1;

	public AbilityButtonNormal UtilityButton2;

	public AbilityButtonNormal UtilityButton3;

	public AbilityButtonNormal DropWeaponButton;

	public AbilityButtonNormal ReloadButton;

	public AbilityButtonNormal AltFireButton;

	public AbilityButtonNormal AimButton;

	public WeaponButton WeaponSlot1;

	public WeaponButton WeaponSlot2;

	public WeaponButton WeaponSlot3;

	public WeaponButton WeaponSlot4;

	public UISprite WeaponXPBar;

	public UISprite WeaponXPBarBackground;

	public KillStreak killStreakCounter;

	public UILabel WeaponLevelLabel;

	public UILabel MoneyLabel;

	public UILabel AmmoCurrent;

	public UILabel AmmoSpare;

	public UILabel AmmoSlash;

	public UILabel SecondaryAmmoCurrent;

	public UILabel SecondaryAmmoSpare;

	public UILabel SecondaryAmmoSlash;

	public HudCashScroll ScrollingCash;

	public WeaponTrayDrag WeaponTray;

	public StickTouchRecognizer StickRecognizer;

	public TweenPosition ButtonBarTween;

	public GameObject ButtonDragObject;

	public ButtonBarDrag ButtonBar;

	public WeaponWidget WeaponWidget;

	public ControlOverlay[] ControlOverlays;

	public TweenPosition[] PauseTweens;

	public float ButtonBarTweenOutDistance = 500f;

	private List<TweenPosition> positionTweens = new List<TweenPosition>();

	private HudMoneyHandler moneyHandler = new HudMoneyHandler();

	private UIDragObject dragButtonsObject;

	private BoxCollider boxDragButtonCollider;

	private UIRoot root;

	private bool isButtonBarTweened;

	private ControllerType currentControllerType;

	private InputManager inputManager;

	private Color SLOT_EMPTY_COLOR = new Color32(161, 39, 39, byte.MaxValue);

	private Color SLOT_PASSIVE_COLOR = new Color32(100, 100, 100, byte.MaxValue);

	private Color SLOT_SELECTED_COLOR = Color.white;

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

	public override WeaponTrayDrag GetWeaponTray()
	{
		return WeaponTray;
	}

	public override AbilityButtonNormal GetAbilityButton(AbilitySlot slot)
	{
		object result;
		switch (slot)
		{
		case AbilitySlot.Grenade:
			result = UtilityButton2;
			break;
		case AbilitySlot.SyringeSelf:
			result = UtilityButton1;
			break;
		case AbilitySlot.SyringeTarget:
			result = UtilityButton1;
			break;
		case AbilitySlot.Perk:
			result = AbilityButton;
			break;
		case AbilitySlot.Dodge:
			result = UtilityButton3;
			break;
		default:
			result = null;
			break;
		}
		return (AbilityButtonNormal)result;
	}

	private WeaponButton GetWeaponButton(InventoryWeaponSlot slot)
	{
		object result;
		switch (slot)
		{
		case InventoryWeaponSlot.Melee:
			result = WeaponSlot1;
			break;
		case InventoryWeaponSlot.Pistol:
			result = WeaponSlot2;
			break;
		case InventoryWeaponSlot.Primary1:
			result = WeaponSlot3;
			break;
		case InventoryWeaponSlot.Primary2:
			result = WeaponSlot4;
			break;
		default:
			result = null;
			break;
		}
		return (WeaponButton)result;
	}

	protected override void OnSyncingGui(PlayerHudBase otherHud)
	{
		inventoryWeapons[InventoryWeaponSlot.Melee].SlotSprite = WeaponSlot1.IconSprite;
		inventoryWeapons[InventoryWeaponSlot.Pistol].SlotSprite = WeaponSlot2.IconSprite;
		inventoryWeapons[InventoryWeaponSlot.Primary1].SlotSprite = WeaponSlot3.IconSprite;
		inventoryWeapons[InventoryWeaponSlot.Primary2].SlotSprite = WeaponSlot4.IconSprite;
		killStreakCounter.SyncGui(otherHud.GetKillStreakCounter());
		WeaponButton[] weaponButtons = new WeaponButton[4] { WeaponSlot1, WeaponSlot2, WeaponSlot3, WeaponSlot4 };
		WeaponWidget.Initialize(inventoryWeapons, weaponButtons);
		WeaponWidget.WeaponList.SyncGUI(equippedSlot);
	}

	private void Awake()
	{
		if (GuiInGameManager.Instance != null && !GuiInGameManager.Instance.IsSwitchingGUI)
		{
			ActivateOverlay();
		}
		AmmoCurrent.text = "0 | 0";
		DeviceSwitchController.SubscribeToSwitch(this, OnDeviceModeChanged);
		positionTweens = new List<TweenPosition>(GetComponentsInChildren<TweenPosition>());
		isCreated = true;
		WeaponButton[] weaponButtons = new WeaponButton[4] { WeaponSlot1, WeaponSlot2, WeaponSlot3, WeaponSlot4 };
		WeaponWidget.Initialize(inventoryWeapons, weaponButtons);
		WeaponWidget.WeaponList.OnWeaponSwitch = OnWeaponSwitch;
		WeaponWidget.WeaponList.OnWeaponRemoved = OnWeaponRemoved;
	}

	private void OnWeaponSwitch(InventoryWeaponSlot slot)
	{
		Weapon weapon = inventoryWeapons[slot].Weapon;
		EquipWeapon(slot, weapon);
	}

	private void OnWeaponRemoved(InventoryWeaponSlot slot)
	{
		RemoveInventoryWeapon(slot);
	}

	private void OnDeviceModeChanged(DeviceSwitchController.SwitchArgs args)
	{
		if (args.OldDeviceMode == DeviceMode.Touch && args.NewDeviceMode != DeviceMode.Touch)
		{
			StickRecognizer.HideAllSticks();
		}
		if (args.OldDeviceMode != DeviceMode.Touch && args.NewDeviceMode == DeviceMode.Touch)
		{
			if (GuiInGameManager.Instance.IsShopOpen() || GuiInGameManager.Instance.IsEndScreenVisible || GuiInGameManager.Instance.IsGamePaused || !StickRecognizer.IsSticksEnabled)
			{
				StickRecognizer.HideAllSticks();
			}
			else
			{
				StickRecognizer.ShowAllSticks();
			}
		}
	}

	public override void Start()
	{
		root = NGUITools.FindInParents<UIRoot>(base.gameObject);
		moneyHandler.MoneyLabel = MoneyLabel;
		moneyHandler.MoneyScroll = ScrollingCash;
		moneyHandler.PlayerHud = this;
		moneyHandler.SyncMoney();
		ScrollingCash.MoneyHandler = moneyHandler;
		SetupWeaponSlot(InventoryWeaponSlot.Melee, WeaponSlot1.IconSprite);
		SetupWeaponSlot(InventoryWeaponSlot.Pistol, WeaponSlot2.IconSprite);
		SetupWeaponSlot(InventoryWeaponSlot.Primary1, WeaponSlot3.IconSprite);
		SetupWeaponSlot(InventoryWeaponSlot.Primary2, WeaponSlot4.IconSprite);
		if (GuiInGameManager.Instance.IsGamePaused || GuiInGameManager.Instance.IsShopOpen())
		{
			StickRecognizer.HideAllSticks();
			ButtonBar.gameObject.SetActive(false);
		}
		inputManager = InputManager.Instance;
		if (OptionManager.Instance.StickType == TouchStickType.Locked)
		{
			ResetJoystickPosition();
		}
		else if (OptionManager.Instance.StickType == TouchStickType.Dynamic)
		{
			StickRecognizer.HideAllSticks();
		}
		if (DeviceSwitchController.CurrentDeviceMode != DeviceMode.Touch)
		{
			StickRecognizer.HideAllSticks();
		}
		dragButtonsObject = ButtonDragObject.GetComponent<UIDragObject>();
		boxDragButtonCollider = ButtonDragObject.GetComponent<BoxCollider>();
	}

	public override void OnDestroy()
	{
		isCreated = false;
		DeviceSwitchController.UnsubscribeToSwitch(this);
	}

	private ControlOverlay FindOverlayByName(string overlayName)
	{
		return (ControlOverlays == null) ? null : Array.Find(ControlOverlays, (ControlOverlay a) => a.Name == overlayName);
	}

	private void ActivateOverlay()
	{
		GameSettings gameSettings = GameState.Instance.GameSettings;
		if (gameSettings != null && gameSettings.GameMode == GameModeSetting.Tutorial)
		{
			return;
		}
		OptionManager instance = OptionManager.Instance;
		if (!instance.ShowOverlay || DeviceSwitchController.CurrentDeviceMode != DeviceMode.Touch)
		{
			return;
		}
		ControlOverlay controlOverlay = null;
		string text = string.Empty;
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch)
		{
			if (instance.StickType == TouchStickType.Dynamic)
			{
				text = "Dynamic Overlay";
			}
			else if (instance.StickType == TouchStickType.Locked)
			{
				text = "Static Overlay";
			}
		}
		controlOverlay = FindOverlayByName(text);
		if (controlOverlay != null)
		{
			controlOverlay.ToggleActive(true);
		}
		else
		{
			Debug.LogError("No control Overlay with the name: " + text + " could be found!");
		}
	}

	public void ResetJoystickPosition()
	{
		UIRoot uIRoot = UIRoot.list[0];
		float num = (float)uIRoot.activeHeight / (float)Screen.height;
		float num2 = (float)Screen.width * num;
		float num3 = uIRoot.activeHeight;
		StickRecognizer.SetJoystickPosition(Stick.Left, new Vector3(num2 * 0.1f, num3 / 2f));
		StickRecognizer.SetJoystickPosition(Stick.Right, new Vector3(num2 - num2 * 0.1f, num3 / 2f));
	}

	protected override void OnWeaponSlotStateChanged(WeaponSlotState slotState, InventoryWeaponSlot slot, InventoryWeapon weapon)
	{
		WeaponWidget.OnWeaponSlotStateChanged(slotState, slot, weapon);
	}

	private void OnWeaponSlotSelected(InventoryWeaponSlot slot, InventoryWeapon weapon)
	{
		if (weapon.SlotSprite != null)
		{
			weapon.SlotSprite.spriteName = weapon.Weapon.TouchImage + "_white";
		}
		WeaponButton weaponButton = GetWeaponButton(slot);
		weaponButton.SetAmmoColor(SLOT_SELECTED_COLOR);
	}

	private void OnWeaponSlotPassive(InventoryWeaponSlot slot, InventoryWeapon weapon)
	{
		if (weapon.SlotSprite != null)
		{
			weapon.SlotSprite.spriteName = weapon.Weapon.TouchImage + "_gray";
		}
		WeaponButton weaponButton = GetWeaponButton(slot);
		weaponButton.SetAmmoColor(SLOT_PASSIVE_COLOR);
	}

	private void OnWeaponSlotEmpty(InventoryWeaponSlot slot, InventoryWeapon weapon)
	{
		weapon.SlotSprite.spriteName = weapon.Weapon.TouchImage + "_red";
		WeaponButton weaponButton = GetWeaponButton(slot);
		weaponButton.SetAmmoColor(SLOT_EMPTY_COLOR);
	}

	protected override void OnNoWeapon(InventoryWeaponSlot slot, InventoryWeapon weapon)
	{
		WeaponWidget.OnNoWeapon(slot);
	}

	public override void RemoveHudHealthListener()
	{
	}

	public override void OnDamageUpdate(Health UnitHealthComponent, int i, int k, float f)
	{
	}

	public override void OnScaleTweenEnd()
	{
	}

	public override void OnAlphaTweenEnd()
	{
	}

	public override void OnWeaponSwapFlashEneded()
	{
	}

	public override void Update()
	{
	}

	public override void OnPauseMenuChanged(bool isOpen)
	{
		if (GuiInGameManager.PauseMode == GuiInGameManager.PauseModeEnum.Normal)
		{
			ButtonBar.MovableFeedback.SetActive(isOpen);
			ToggleButtonsMovable(isOpen);
		}
	}

	public void ToggleButtonsMovable(bool isMovable)
	{
		if (AbilityButton != null)
		{
			AbilityButton.ToggleButtonActive(!isMovable);
		}
		if (UtilityButton1 != null)
		{
			UtilityButton1.ToggleButtonActive(!isMovable);
		}
		if (UtilityButton2 != null)
		{
			UtilityButton2.ToggleButtonActive(!isMovable);
		}
		if (UtilityButton3 != null)
		{
			UtilityButton3.ToggleButtonActive(!isMovable);
		}
		if (DropWeaponButton != null)
		{
			DropWeaponButton.ToggleButtonActive(!isMovable);
		}
		if (ReloadButton != null)
		{
			ReloadButton.ToggleButtonActive(!isMovable);
		}
		if (AltFireButton != null)
		{
			AltFireButton.ToggleButtonActive(!isMovable);
		}
		if (AimButton != null)
		{
			AimButton.ToggleButtonActive(!isMovable);
		}
		dragButtonsObject.enabled = isMovable;
		boxDragButtonCollider.enabled = isMovable;
	}

	public override void ToggleAllTweeens(bool toggle)
	{
		foreach (TweenPosition positionTween in positionTweens)
		{
			positionTween.enabled = toggle;
		}
	}

	public override void TweenIn()
	{
		TweenPosition[] pauseTweens = PauseTweens;
		foreach (TweenPosition tweenPosition in pauseTweens)
		{
			if (GuiInGameManager.PauseMode != GuiInGameManager.PauseModeEnum.WeaponTray || !(tweenPosition == ButtonBarTween))
			{
				tweenPosition.enabled = true;
				tweenPosition.Play(true);
			}
		}
		if (isButtonBarTweened)
		{
			ButtonBar.TweenIn();
			isButtonBarTweened = false;
		}
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch)
		{
			ResetJoystickPosition();
			StickRecognizer.ShowAllSticks();
		}
		if (!ButtonBar.gameObject.activeSelf)
		{
			ButtonBar.gameObject.SetActive(true);
		}
	}

	public override void TweenOut()
	{
		TweenPosition[] pauseTweens = PauseTweens;
		foreach (TweenPosition tweenPosition in pauseTweens)
		{
			if ((GuiInGameManager.PauseMode != GuiInGameManager.PauseModeEnum.WeaponTray || !(tweenPosition == ButtonBarTween)) && (GuiInGameManager.PauseMode != GuiInGameManager.PauseModeEnum.Normal || !(tweenPosition == ButtonBarTween)))
			{
				tweenPosition.delay = 0f;
				tweenPosition.Reset2();
				tweenPosition.Play(false);
			}
		}
		if (GuiInGameManager.PauseMode == GuiInGameManager.PauseModeEnum.Shop)
		{
			if (ButtonBarTween.cachedTransform.localPosition.x <= (float)(Screen.width / 2))
			{
				ButtonBarTween.from.x = 0f - ButtonBarTweenOutDistance;
			}
			else
			{
				ButtonBarTween.from.x = (float)Screen.width + ButtonBarTweenOutDistance;
			}
			ButtonBar.TweenOut();
			isButtonBarTweened = true;
		}
		if (OptionManager.Instance.StickType != 0 || GuiInGameManager.PauseMode != GuiInGameManager.PauseModeEnum.WeaponTray)
		{
			StickRecognizer.HideAllSticks();
		}
	}

	public override void OnTriggerEnter(Collider collider)
	{
	}

	public override void OnTriggerExit(Collider collider)
	{
	}

	public override void CheckForNullColliders()
	{
	}

	public override void OnInitialize(GuiInGameManager manager, PlayerGUI pGui, int playerNumber, Vector2 pixelOffset, Camera camera)
	{
	}

	public override void OnRespawn()
	{
		killStreakCounter.gameObject.SetActive(true);
		UtilityButton1.gameObject.SetActive(true);
		UtilityButton2.gameObject.SetActive(true);
		UtilityButton3.gameObject.SetActive(true);
		AbilityButton.gameObject.SetActive(true);
		DropWeaponButton.gameObject.SetActive(false);
		ReloadButton.gameObject.SetActive(true);
		AltFireButton.gameObject.SetActive(false);
		AimButton.gameObject.SetActive(false);
		WeaponSlot1.gameObject.SetActive(true);
		WeaponSlot2.gameObject.SetActive(true);
		WeaponSlot3.gameObject.SetActive(true);
		WeaponSlot4.gameObject.SetActive(true);
		WeaponWidget.gameObject.SetActive(true);
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

	public override void EquipWeapon(InventoryWeaponSlot slot, Weapon newEquippedWeapon)
	{
		base.EquipWeapon(slot, newEquippedWeapon);
		WeaponWidget.OnWeaponEquipped(slot, newEquippedWeapon);
	}

	protected override void OnEquippedWeaponRemoved(InventoryWeaponSlot slot)
	{
		switch (slot)
		{
		case InventoryWeaponSlot.Melee:
			WeaponSlot1.IconSprite.alpha = 0.3f;
			break;
		case InventoryWeaponSlot.Pistol:
			WeaponSlot2.IconSprite.alpha = 0.3f;
			break;
		case InventoryWeaponSlot.Primary1:
			WeaponSlot3.IconSprite.alpha = 0.3f;
			break;
		case InventoryWeaponSlot.Primary2:
			WeaponSlot4.IconSprite.alpha = 0.3f;
			break;
		case InventoryWeaponSlot.None:
			break;
		}
	}

	public override void AddInventoryWeapon(InventoryWeaponSlot slot, Weapon newWeapon, StoredWeaponData storedWeapons)
	{
		base.AddInventoryWeapon(slot, newWeapon, storedWeapons);
		WeaponWidget.OnWeaponAdded(slot, newWeapon);
	}

	public override void RemoveInventoryWeapon(InventoryWeaponSlot slot)
	{
		base.RemoveInventoryWeapon(slot);
		WeaponWidget.OnWeaponRemoved(slot);
	}

	public override void SetAbilityCharges(AbilitySlot slot, int charges)
	{
		if (abilities[slot] != null)
		{
			abilities[slot].SetCharges(charges);
		}
	}

	public override void StartReload(float duration)
	{
		if (ReloadButton != null)
		{
			ReloadButton.SetCooldown(duration);
		}
	}

	public override void StopReload()
	{
		if (ReloadButton != null)
		{
			ReloadButton.StopCooldown();
		}
	}

	protected override void SetAmmoText()
	{
	}

	public override void UpdateAmmunitionBySlot(InventoryWeaponSlot slot)
	{
		base.UpdateAmmunitionBySlot(slot);
		WeaponWidget.OnUpdateAmmunitionBySlot(slot);
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

	public override void SetWeaponExperience(int exp, int maxExp, int weaponLvl)
	{
		float num = (float)exp / (float)maxExp;
		Vector3 localScale = WeaponXPBarBackground.cachedTransform.localScale;
		WeaponXPBar.enabled = true;
		WeaponLevelLabel.enabled = true;
		WeaponXPBarBackground.enabled = true;
		WeaponXPBar.cachedTransform.localScale = new Vector3(localScale.x * num, localScale.y, localScale.z);
		WeaponLevelLabel.text = Localizer.GetString("gui_weapon_level") + " " + weaponLvl;
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
		killStreakCounter.gameObject.SetActive(false);
		UtilityButton1.gameObject.SetActive(false);
		UtilityButton2.gameObject.SetActive(false);
		UtilityButton3.gameObject.SetActive(false);
		AbilityButton.gameObject.SetActive(false);
		DropWeaponButton.gameObject.SetActive(false);
		ReloadButton.gameObject.SetActive(false);
		AltFireButton.gameObject.SetActive(false);
		AimButton.gameObject.SetActive(false);
		WeaponSlot1.gameObject.SetActive(false);
		WeaponSlot2.gameObject.SetActive(false);
		WeaponSlot3.gameObject.SetActive(false);
		WeaponSlot4.gameObject.SetActive(false);
		WeaponWidget.gameObject.SetActive(false);
	}
}
