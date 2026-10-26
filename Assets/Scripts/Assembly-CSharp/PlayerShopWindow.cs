using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class PlayerShopWindow
{
	public enum TabEnums
	{
		Inventory = 1,
		Options = 2,
		WeaponCategories = 3,
		WeaponList = 4,
		Money = 5,
		None = 6,
		SellBuy = 7
	}

	public class SelectedWeaponSlot
	{
		public InventoryWeaponSlot Slot;

		public StoredWeaponData Data;

		public Weapon Weapon;
	}

	public Rect tabRect;

	private Dictionary<TabEnums, IShopTab> tabs = new Dictionary<TabEnums, IShopTab>();

	private GUISkin skin;

	private GUIStyle mainTitleStyle;

	private GUIStyle subTitleStyle;

	private PlayerGUI playerGui;

	private Rect titleBarRect;

	private Rect perkRect;

	private Rect titleRect;

	private Rect moneyRect;

	private Rect statusTextRect;

	private Rect statusIconRect;

	private Rect bgRect;

	private Rect readyRect;

	private Rect backButtonRect;

	private Rect backTextRect;

	private Texture backgroundTexture;

	private Texture oButtonTexture;

	private Texture perkImage;

	private Texture perkBg;

	private TabEnums activeTab;

	private TabEnums nextTab;

	private string title;

	private string statusText3;

	private string readyText;

	private int playerNumber;

	private int inputId;

	private bool enabled;

	private bool movingTab;

	private bool isShoppingDone;

	private float nextTabLerpPositionX;

	private float nextLerpMutliplier;

	private float activeLerpMultiplier;

	private float activeTabLerpPositionX;

	private float tabLerpDistance;

	public SelectedWeaponSlot SelectedWeaponData = new SelectedWeaponSlot();

	private ShopController shopManager;

	private TouchButtonWrapper backButton;

	private GameObject backButtonObject;

	private TKTapRecognizer tapRecognizer;

	public TKTapRecognizer TapRecognizer
	{
		get
		{
			return tapRecognizer;
		}
	}

	public int InputID
	{
		get
		{
			return inputId;
		}
		set
		{
			inputId = value;
		}
	}

	public PlayerGUI PlayerGUI
	{
		get
		{
			return playerGui;
		}
	}

	public int PlayerNumber
	{
		get
		{
			return playerNumber;
		}
	}

	public PlayerShopWindow(ShopController shopManager, PlayerGUI playerGui, GUISkin skin, int playerNumber, int controlId, Perk perk)
	{
		this.shopManager = shopManager;
		InputID = controlId;
		this.playerGui = playerGui;
		if (perk != Perk.None && skin != null)
		{
			perkImage = skin.GetStyle(GetPerkLocalizerKey(perk)).hover.background;
			perkBg = skin.GetStyle(GetPerkLocalizerKey(perk)).normal.background;
		}
		statusText3 = Localizer.GetString("gui_shop_re_open");
		readyText = Localizer.GetString("gui_ready");
		title = Localizer.GetString("gui_player") + " " + (playerNumber + 1);
		oButtonTexture = skin.GetStyle("button_o").normal.background;
		this.playerNumber = playerNumber;
		mainTitleStyle = new GUIStyle(skin.label);
		mainTitleStyle.alignment = TextAnchor.MiddleLeft;
		mainTitleStyle.fontSize = ScaledFont.getFontSize(30);
		subTitleStyle = new GUIStyle(mainTitleStyle);
		subTitleStyle.fontSize = ScaledFont.getFontSize(24);
		bgRect = ScaledRect.getRect(0f, 0f, 320f, 720f, Alignment.TopLeft);
		titleBarRect = ScaledRect.getRect(-15f, 0f, 350f, 89f, Alignment.TopLeft);
		titleRect = ScaledRect.getRect(75f, 15f, 200f, 64f, Alignment.TopLeft);
		perkRect = ScaledRect.getRect(20f, 13f, 64f, 64f, Alignment.TopLeft);
		moneyRect = ScaledRect.getRect(205f, 15f, 200f, 64f, Alignment.TopLeft);
		float num = 0f;
		if (PlayerManager.Instance.NumberOfPlayers == 1)
		{
			num = 1.5f;
		}
		else if (PlayerManager.Instance.MaxPlayers == 2)
		{
			if (playerNumber == 0)
			{
				num = 1f;
			}
			if (playerNumber == 1)
			{
				num = 2f;
			}
		}
		else
		{
			num = playerNumber;
		}
		this.skin = skin;
		readyRect = ScaledRect.getRect(ScaledRect.customWidth / 4f * num + 120f, 240f, 200f, 64f, Alignment.TopLeft);
		statusTextRect = ScaledRect.getRect(ScaledRect.customWidth / 4f * num + 59f, 280f, 300f, 64f, Alignment.TopLeft);
		statusIconRect = ScaledRect.getRect(ScaledRect.customWidth / 4f * num + 95f, 280f, 64f, 64f, Alignment.TopLeft);
		tabRect = ScaledRect.getRect(ScaledRect.customWidth / 4f * num, 0f, ScaledRect.customWidth / 4f, ScaledRect.customHeight, Alignment.TopLeft);
		tabLerpDistance = tabRect.width;
		AddTab(TabEnums.Inventory, new ShopInventoryTab(this, new Rect(0f, tabRect.y, tabRect.width, tabRect.height), skin));
		AddTab(TabEnums.Options, new ShopOptionsTab(this, new Rect(0f, tabRect.y, tabRect.width, tabRect.height), skin));
		AddTab(TabEnums.WeaponCategories, new ShopPurchaseCategoryTab(this, new Rect(0f, tabRect.y, tabRect.width, tabRect.height), skin));
		AddTab(TabEnums.WeaponList, new ShopWeaponsTab(this, new Rect(0f, tabRect.y, tabRect.width, tabRect.height), skin, shopManager));
		AddTab(TabEnums.Money, new ShopMoneyTab(this, new Rect(0f, tabRect.y, tabRect.width, tabRect.height), skin));
		AddTab(TabEnums.SellBuy, new ShopSellBuyTab(this, new Rect(0f, tabRect.y, tabRect.width, tabRect.height), skin, shopManager));
		activeTab = TabEnums.Inventory;
		tabs[activeTab].Start();
		SetBackground(playerNumber);
		enabled = true;
		tapRecognizer = new TKTapRecognizer();
	}

	public void OnBackPressed()
	{
		tabs[activeTab].OnBackwardPressed();
	}

	public void OnForwardPressed()
	{
		tabs[activeTab].OnForwardPressed();
	}

	public void SetBackButtonObject(GameObject backButton)
	{
		backButtonObject = backButton;
	}

	private void tapRecognizer_gestureRecognizedEvent(TKTapRecognizer obj)
	{
		if (PlayerManager.Instance.NumberOfPlayers > 1 || DeviceSwitchController.CurrentDeviceMode != DeviceMode.Touch)
		{
			return;
		}
		Vector2 vector = obj.touchLocation();
		vector.y = (float)Screen.height - vector.y;
		Rect rect = backButtonRect;
		rect.x += tabRect.x;
		if (!isShoppingDone)
		{
			if (rect.Contains(vector))
			{
				tabs[activeTab].OnBackwardPressed();
			}
			else
			{
				tabs[activeTab].HandleTouch(vector);
			}
		}
	}

	public void Update()
	{
		if (enabled)
		{
			if (backButton != null)
			{
				backButton.Update();
			}
			TabLerp();
			HandleInput();
			tabs[activeTab].Update();
		}
	}

	private void HandleInput()
	{
		if (enabled)
		{
			tabs[activeTab].HandleButtonPressed();
			if (!isShoppingDone)
			{
				int @int = InputManager.GetInt(InputAction.MenuVertical, InputAction.MenuVerticalAlternate, InputID);
				bool @bool = InputManager.GetBool(InputAction.MenuY, InputID);
				bool bool2 = InputManager.GetBool(InputAction.MenuU, InputID);
				if (@int != 0)
				{
					tabs[activeTab].SetSelectedButton(@int);
					PlaySound(GuiInGameManager.Sound.ShopList);
				}
				if (@bool)
				{
					OnAmmoAndArmorButtonPressed();
				}
				if (bool2 && activeTab != TabEnums.Money)
				{
					OpenMoneyTab();
				}
			}
		}
		if (backButton != null && backButton.GetBool())
		{
			OnBackPressed();
		}
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

	public void Stop()
	{
		enabled = false;
	}

	public void SetShopStatus()
	{
		if (isShoppingDone)
		{
			isShoppingDone = false;
		}
		else
		{
			isShoppingDone = true;
		}
	}

	public void PlaySound(GuiInGameManager.Sound sound)
	{
		PlayerGUI.GuiManager.PlaySound(sound);
	}

	public void Open()
	{
		if (PlayerGUI.Soldier != null)
		{
			TouchKit.addGestureRecognizer(tapRecognizer);
			tapRecognizer.gestureRecognizedEvent += tapRecognizer_gestureRecognizedEvent;
			Soldier component = PlayerGUI.Soldier.GetComponent<Soldier>();
			if (component != null)
			{
				((ShopMoneyTab)tabs[TabEnums.Money]).SetGiveAwayAmount(component.MoneyToGiveAwayInShop);
			}
			((ShopInventoryTab)tabs[TabEnums.Inventory]).UpdateInventory();
			enabled = true;
			if (backButtonObject != null && PlayerManager.Instance.NumberOfPlayers == 1)
			{
				backButtonObject.SetActive(true);
			}
			if (backButton == null && PlayerManager.Instance.NumberOfPlayers == 1)
			{
				backButton = new TouchButtonWrapper("back_button");
				backButton.InitializeIngame();
			}
			TapRecognizer.enabled = true;
		}
		else
		{
			Close();
		}
	}

	public void Close()
	{
		activeTab = TabEnums.Inventory;
		isShoppingDone = false;
		enabled = false;
		PlayerGUI.Hud.SyncMoneyDisplay();
		TouchKit.removeGestureRecognizer(tapRecognizer);
		tapRecognizer.gestureRecognizedEvent -= tapRecognizer_gestureRecognizedEvent;
	}

	private void SetBackground(int playerId)
	{
		switch (playerId)
		{
		case 0:
			backgroundTexture = skin.GetStyle("window_bgs").normal.background;
			break;
		case 1:
			backgroundTexture = skin.GetStyle("window_bgs").hover.background;
			break;
		case 2:
			backgroundTexture = skin.GetStyle("window_bgs").active.background;
			break;
		case 3:
			backgroundTexture = skin.GetStyle("window_bgs").focused.background;
			break;
		}
	}

	public void Display()
	{
		if (!enabled)
		{
			return;
		}
		if (!isShoppingDone)
		{
			string text = ((!(tabs[activeTab].ToString() == "ShopInventoryTab")) ? Localizer.GetString("gui_back") : Localizer.GetString("gui_close"));
			GuiInGameManager.Instance.ShopBackButtonLabel.text = text;
			GUI.BeginGroup(tabRect);
			GUI.DrawTexture(bgRect, backgroundTexture);
			GUI.DrawTexture(titleBarRect, skin.GetStyle("button1").normal.background);
			GUI.Label(titleRect, title, mainTitleStyle);
			GUI.DrawTexture(perkRect, perkBg);
			GUI.DrawTexture(perkRect, perkImage);
			if (PlayerGUI.Soldier != null)
			{
				GUI.Label(moneyRect, "£ " + PlayerGUI.Money(), mainTitleStyle);
			}
			tabs[activeTab].Display();
			if (movingTab)
			{
				tabs[nextTab].Display();
			}
			GUI.EndGroup();
		}
		else
		{
			mainTitleStyle.normal.textColor = Color.white;
			GUI.Label(readyRect, readyText, mainTitleStyle);
		}
	}

	private void AddTab(TabEnums tabEnum, IShopTab tab)
	{
		tabs.Add(tabEnum, tab);
	}

	private void SetActiveTab(TabEnums tabEnum, string direction)
	{
		if (tabEnum != TabEnums.None)
		{
			nextTab = tabEnum;
			movingTab = true;
			if (direction == "forward")
			{
				activeTabLerpPositionX = 0f - tabLerpDistance;
				nextTabLerpPositionX = 0f;
				tabs[nextTab].SetTabPosX(tabLerpDistance);
			}
			else if (direction == "backward")
			{
				activeTabLerpPositionX = tabLerpDistance;
				nextTabLerpPositionX = 0f;
				tabs[nextTab].SetTabPosX(0f - tabLerpDistance);
			}
			nextLerpMutliplier = 1f;
			activeLerpMultiplier = 1f;
			tabs[activeTab].Stop();
		}
	}

	private void TabLerp()
	{
		if (movingTab)
		{
			nextLerpMutliplier -= Time.deltaTime / nextTabLerpPositionX;
			activeLerpMultiplier -= Time.deltaTime / activeTabLerpPositionX;
			tabs[nextTab].SetTabPosX(Mathf.Lerp(tabs[nextTab].TabRect.x, nextTabLerpPositionX, Time.deltaTime * 20f));
			tabs[activeTab].SetTabPosX(Mathf.Lerp(tabs[activeTab].TabRect.x, activeTabLerpPositionX, Time.deltaTime * 20f));
			if (tabs[nextTab].TabRect.x <= 1.5f && tabs[nextTab].TabRect.x >= -1.5f)
			{
				tabs[nextTab].SetTabPosX(nextTabLerpPositionX);
				tabs[activeTab].SetTabPosX(activeTabLerpPositionX);
				activeTab = nextTab;
				tabs[activeTab].Start();
				movingTab = false;
			}
		}
	}

	public void ShopOptionRequest(ShopOptions option, TabEnums window, string arg_1, string arg_2, string arg_3)
	{
		switch (option)
		{
		case ShopOptions.ChangeTab:
			SetActiveTab(window, arg_1);
			switch (window)
			{
			case TabEnums.Inventory:
				((ShopInventoryTab)tabs[window]).UpdateInventory();
				break;
			case TabEnums.SellBuy:
				SetSellBuyTabData();
				break;
			}
			break;
		case ShopOptions.Purchase:
			break;
		case ShopOptions.Sell:
			break;
		case ShopOptions.Trade:
			break;
		}
	}

	public void SetWeaponTabData(Perk perk)
	{
		ShopWeaponsTab shopWeaponsTab = tabs[TabEnums.WeaponList] as ShopWeaponsTab;
		shopWeaponsTab.SetWeaponTabData(perk);
		shopWeaponsTab.SetSizeBasedOnResolution();
	}

	public void OpenMoneyTab()
	{
		if (shopManager != null)
		{
			if (shopManager.HasMultiplePlayers())
			{
				ShopMoneyTab shopMoneyTab = tabs[TabEnums.Money] as ShopMoneyTab;
				shopMoneyTab.SetButtonData();
				shopMoneyTab.SetSizeBasedOnResolution();
				SetActiveTab(TabEnums.Money, "forward");
				PlaySound(GuiInGameManager.Sound.Forward);
			}
			else
			{
				PlaySound(GuiInGameManager.Sound.ShopPurchaseUnable);
			}
		}
	}

	public void SetSellBuyTabData()
	{
		ShopWeaponsTab shopWeaponsTab = (ShopWeaponsTab)tabs[TabEnums.WeaponList];
		((ShopSellBuyTab)tabs[TabEnums.SellBuy]).SetData(shopWeaponsTab.SellBuyWeaponButton);
	}

	public List<Weapon> GetWeaponsByPerk(Perk perk)
	{
		return WeaponFactory.Instance.WeaponTemplatesByPerk(perk);
	}

	public List<Weapon> GetWeaponsBySlot(WeaponSlot slot)
	{
		return WeaponFactory.Instance.WeaponTemplatesBySlot(slot);
	}

	public void SetSelectedWeaponSlot(InventoryWeaponSlot slot, Weapon weapon, StoredWeaponData weaponData)
	{
		SelectedWeaponData.Weapon = weapon;
		SelectedWeaponData.Data = weaponData;
		SelectedWeaponData.Slot = slot;
		ShopOptionsTab shopOptionsTab = tabs[TabEnums.Options] as ShopOptionsTab;
		shopOptionsTab.SetDataBasedOnSelectedWeapon();
		shopOptionsTab.SetSizeBasedOnResolution();
	}

	public void SetWeaponCategories(InventoryWeaponSlot slot)
	{
		ShopPurchaseCategoryTab shopPurchaseCategoryTab = tabs[TabEnums.WeaponCategories] as ShopPurchaseCategoryTab;
		shopPurchaseCategoryTab.SetDataBasedOnSelectedSlot(slot);
		shopPurchaseCategoryTab.SetSizeBasedOnResolution();
	}

	public void SetEquipedWeaponReference(Weapon equippedWeapon)
	{
		((ShopInventoryTab)tabs[TabEnums.Inventory]).SetEquipedWeaponReference(equippedWeapon);
	}

	public bool WeaponExistInInventory(GameObject weaponPrefab)
	{
		if (PlayerGUI.Inventory != null)
		{
			return PlayerGUI.Inventory.HasWeapon(weaponPrefab);
		}
		return false;
	}

	public void AddInventoryWeapon(InventoryWeaponSlot slot, Weapon newWeapon, StoredWeaponData storedWeapons)
	{
		((ShopInventoryTab)tabs[TabEnums.Inventory]).AddWeapon(slot, newWeapon, storedWeapons);
	}

	public void RemoveInventoryWeapon(InventoryWeaponSlot slot)
	{
		((ShopInventoryTab)tabs[TabEnums.Inventory]).RemoveWeapon(slot);
	}

	public void PurchaseWeapon(GameObject weaponPrefab, int price)
	{
		if (PlayerGUI.Soldier != null)
		{
			StoredWeaponData storedWeaponData = new StoredWeaponData(weaponPrefab, InventoryWeaponSlot.None);
			storedWeaponData.PurchasePrice = price;
			PlayerGUI.Inventory.AddWeaponToSlot(weaponPrefab, SelectedWeaponData.Slot, storedWeaponData);
			PlayerGUI.Inventory.AutoSwitchIfHigherThanEquipped(SelectedWeaponData.Slot);
			PlayerGUI.Stats.RemoveMoney(price);
			PlayerGUI.OnMoneyChange();
			GA.API.Design.NewEvent("Shop:Bought:" + storedWeaponData.Weapon.Name, PlayerGUI.Stats.Money);
		}
	}

	public void SellWeapon()
	{
		if (SelectedWeaponData == null)
		{
			return;
		}
		Weapon weapon = SelectedWeaponData.Weapon;
		if (weapon != null && PlayerGUI.Soldier != null)
		{
			bool flag = PlayerGUI.Inventory.IsActiveWeapon(SelectedWeaponData.Slot);
			PlayerGUI.Inventory.RemoveWeapon(SelectedWeaponData.Slot);
			if (flag)
			{
				PlayerGUI.Inventory.AutoSwitchWeapon(true);
			}
			PlayerGUI.Stats.AddMoney(weapon.CalculateSellPrice(PlayerGUI.Soldier));
			PlayerGUI.OnMoneyChange();
			GA.API.Design.NewEvent("Shop:Sold:" + weapon.Name, PlayerGUI.Stats.Money);
		}
	}

	public int GetWeaponValue()
	{
		if (SelectedWeaponData != null)
		{
			Weapon weapon = SelectedWeaponData.Weapon;
			if (weapon != null && PlayerGUI.Soldier != null)
			{
				return weapon.CalculateSellPrice(PlayerGUI.Soldier);
			}
		}
		return -1;
	}

	public Weapon GetWeapon()
	{
		if (SelectedWeaponData != null)
		{
			return SelectedWeaponData.Weapon;
		}
		return null;
	}

	public void OnAmmoAndArmorButtonPressed()
	{
		bool flag = false;
		if (FillAllAmmo() || (RefillGrenadesAndArmor() ? true : false))
		{
			PlaySound(GuiInGameManager.Sound.ShopPurchaseOther);
			((ShopInventoryTab)tabs[TabEnums.Inventory]).UpdateInventory();
		}
		else
		{
			PlaySound(GuiInGameManager.Sound.ShopPurchaseUnable);
		}
	}

	public bool RefillGrenadesAndArmor()
	{
		bool result = false;
		if (PurchaseArmor())
		{
			result = true;
		}
		if (PlayerGUI.Soldier != null)
		{
			Ability ability = PlayerGUI.Soldier.GetComponent<PlayerAbilityController>().AbilityBySlot(AbilitySlot.Grenade);
			if (ability != null)
			{
				for (int i = 0; i < ability.MaxCharges; i++)
				{
					if (!PurchaseGrenade())
					{
						break;
					}
					result = true;
				}
			}
		}
		return result;
	}

	public bool FillAllAmmo()
	{
		bool result = false;
		if (FillWeaponAmmoBySlot(InventoryWeaponSlot.Primary1))
		{
			result = true;
		}
		if (FillWeaponAmmoBySlot(InventoryWeaponSlot.Primary2))
		{
			result = true;
		}
		if (FillWeaponAmmoBySlot(InventoryWeaponSlot.Pistol))
		{
			result = true;
		}
		if (FillSecondaryAmmoBySlot(InventoryWeaponSlot.Primary1))
		{
			result = true;
		}
		if (FillSecondaryAmmoBySlot(InventoryWeaponSlot.Primary2))
		{
			result = true;
		}
		return result;
	}

	public bool FillSecondaryAmmoBySlot(InventoryWeaponSlot slot)
	{
		if (PlayerGUI.Inventory != null && PlayerGUI.Soldier != null && PlayerGUI.Weapons.ContainsKey(slot))
		{
			PlayerWeaponController inventory = PlayerGUI.Inventory;
			Weapon weapon = PlayerGUI.Weapons[slot].Weapon;
			StoredWeaponData storedWeaponData = PlayerGUI.Weapons[slot];
			if (weapon != null && weapon.Type != WeaponType.Melee && weapon.HasSecondaryModule() && weapon.SecondaryAttackModule.UseAmmunition && storedWeaponData.SecondaryAmmunitionDeficit() > 0)
			{
				SecondaryAttack secondaryAttackModule = weapon.SecondaryAttackModule;
				int num = secondaryAttackModule.CalculateBundleAmmunitionCost(PlayerGUI.Soldier, storedWeaponData);
				if (CanAfford(num))
				{
					int a = Mathf.FloorToInt(PlayerGUI.Money() / num);
					int num2 = Mathf.Min(a, secondaryAttackModule.MissingAmmunitionBundles(storedWeaponData));
					int money = num2 * num;
					PlayerGUI.Stats.RemoveMoney(money);
					PlayerGUI.OnMoneyChange();
					inventory.RefillSecondaryAmmunitionForSlot(slot, num2);
					return true;
				}
			}
		}
		return false;
	}

	public bool FillWeaponAmmoBySlot(InventoryWeaponSlot slot)
	{
		if (PlayerGUI.Inventory != null && PlayerGUI.Soldier != null && PlayerGUI.Weapons.ContainsKey(slot))
		{
			PlayerWeaponController inventory = PlayerGUI.Inventory;
			Weapon weapon = PlayerGUI.Weapons[slot].Weapon;
			StoredWeaponData storedWeaponData = PlayerGUI.Weapons[slot];
			if (weapon != null && weapon.Type != WeaponType.Melee && storedWeaponData.TotalAmmunitionDeficit() > 0)
			{
				int num = weapon.CalculateTotalAmmunitionCost(PlayerGUI.Soldier, storedWeaponData);
				if (CanAfford(num))
				{
					PlayerGUI.Stats.RemoveMoney(num);
					PlayerGUI.OnMoneyChange();
					inventory.RefillTotalAmmunitionForSlot(slot);
					return true;
				}
				int num2 = weapon.CalculateBundleAmmunitionCost(PlayerGUI.Soldier, storedWeaponData);
				if (CanAfford(num2))
				{
					int a = Mathf.FloorToInt(PlayerGUI.Money() / num2);
					int num3 = Mathf.Min(a, weapon.MissingAmmunitionBundles(storedWeaponData));
					int money = num3 * num2;
					PlayerGUI.Stats.RemoveMoney(money);
					PlayerGUI.OnMoneyChange();
					inventory.RefillMagazineForSlot(slot, num3);
					return true;
				}
			}
		}
		return false;
	}

	public bool PurchaseAmmoMag()
	{
		if (SelectedWeaponData != null)
		{
			Weapon weapon = SelectedWeaponData.Weapon;
			StoredWeaponData data = SelectedWeaponData.Data;
			if (PlayerGUI.Soldier != null && weapon != null && data != null && CanAfford(weapon.CalculateBundleAmmunitionCost(PlayerGUI.Soldier, data)) && data.TotalAmmunitionDeficit() > 0)
			{
				PlayerGUI.Stats.RemoveMoney(weapon.CalculateBundleAmmunitionCost(PlayerGUI.Soldier, data));
				PlayerGUI.Inventory.RefillMagazineForSlot(SelectedWeaponData.Slot);
				PlayerGUI.OnMoneyChange();
				return true;
			}
		}
		return false;
	}

	public bool PurchaseAmmoFill()
	{
		if (SelectedWeaponData != null)
		{
			Weapon weapon = SelectedWeaponData.Weapon;
			StoredWeaponData data = SelectedWeaponData.Data;
			if (PlayerGUI.Soldier != null && weapon != null && data != null && data.TotalAmmunitionDeficit() > 0)
			{
				if (CanAfford(weapon.CalculateTotalAmmunitionCost(PlayerGUI.Soldier, data)))
				{
					PlayerGUI.Stats.RemoveMoney(weapon.CalculateTotalAmmunitionCost(PlayerGUI.Soldier, data));
					PlayerGUI.OnMoneyChange();
					PlayerGUI.Inventory.RefillTotalAmmunitionForSlot(SelectedWeaponData.Slot);
					return true;
				}
				int num = weapon.CalculateBundleAmmunitionCost(PlayerGUI.Soldier, data);
				if (num != 0)
				{
					int num2 = PlayerGUI.Money();
					int num3 = Mathf.FloorToInt(num2 / num);
					int money = num3 * num;
					PlayerGUI.Stats.RemoveMoney(money);
					PlayerGUI.OnMoneyChange();
					PlayerGUI.Inventory.RefillMagazineForSlot(SelectedWeaponData.Slot, num3);
					return true;
				}
			}
		}
		return false;
	}

	public bool PurchaseSecondaryAmmo()
	{
		if (SelectedWeaponData != null)
		{
			Weapon weapon = SelectedWeaponData.Weapon;
			StoredWeaponData data = SelectedWeaponData.Data;
			if (PlayerGUI.Soldier != null && weapon != null && data != null && weapon.HasSecondaryModule() && weapon.SecondaryAttackModule.UseAmmunition && CanAfford(weapon.SecondaryAttackModule.CalculateBundleAmmunitionCost(PlayerGUI.Soldier, SelectedWeaponData.Data)) && data.SecondaryAmmunitionDeficit() > 0)
			{
				PlayerGUI.Stats.RemoveMoney(weapon.SecondaryAttackModule.CalculateBundleAmmunitionCost(PlayerGUI.Soldier, SelectedWeaponData.Data));
				PlayerGUI.OnMoneyChange();
				PlayerGUI.Inventory.RefillSecondaryAmmunitionForSlot(SelectedWeaponData.Slot);
				return true;
			}
		}
		return false;
	}

	public bool PurchaseGrenade()
	{
		if (PlayerGUI.Soldier != null)
		{
			Ability ability = PlayerGUI.Soldier.GetComponent<PlayerAbilityController>().AbilityBySlot(AbilitySlot.Grenade);
			if (ability != null)
			{
				int num = Soldier.CalculateGrenadeCost(PlayerGUI.Soldier);
				if (CanAfford(num) && ability.ChargesLeft() < ability.MaxCharges)
				{
					ability.RefillCharges(1);
					PlayerGUI.Stats.RemoveMoney(num);
					PlayerGUI.OnMoneyChange();
					return true;
				}
			}
		}
		return false;
	}

	public bool PurchaseArmor()
	{
		GameObject soldier = PlayerGUI.Soldier;
		if (soldier != null)
		{
			Health component = soldier.GetComponent<Health>();
			if (component != null && component.CurrentArmor < component.MaxArmor)
			{
				int num = PlayerGUI.Money();
				int num2 = Soldier.CalculateArmorCost(soldier);
				if (CanAfford(num2))
				{
					component.RefillArmor(component.MaxArmor);
					PlayerGUI.Stats.RemoveMoney(num2);
					PlayerGUI.OnMoneyChange();
					return true;
				}
				if (num > 0)
				{
					component.RefillArmor(Soldier.CalculateArmorDeficitByMoney(soldier, num));
					PlayerGUI.Stats.RemoveMoney(num);
					PlayerGUI.OnMoneyChange();
					return true;
				}
			}
		}
		return false;
	}

	public int NumberOfPlayers()
	{
		if (shopManager != null)
		{
			return shopManager.GetNumberOfPlayers();
		}
		return 0;
	}

	public bool GiveMoney(int playerID)
	{
		if (PlayerGUI != null && shopManager != null)
		{
			int num = 50;
			Soldier component = PlayerGUI.Soldier.GetComponent<Soldier>();
			if (component != null)
			{
				num = component.MoneyToGiveAwayInShop;
			}
			if (!CanAfford(num))
			{
				int num2 = PlayerGUI.Money();
				if (num2 <= 0)
				{
					return false;
				}
				num = num2;
			}
			if (shopManager.HasMultiplePlayers())
			{
				PlayerStats playerStatsByID = shopManager.GetPlayerStatsByID(playerID);
				PlayerGUI.Stats.RemoveMoney(num);
				playerStatsByID.AddMoney(num);
				PlayerGUI.OnMoneyChange();
				return true;
			}
		}
		return false;
	}

	public bool CanAfford(int price)
	{
		if (PlayerGUI.Money() >= price)
		{
			return true;
		}
		return false;
	}

	public void ClearInventory()
	{
		if (SelectedWeaponData != null)
		{
			SelectedWeaponData.Data = null;
			SelectedWeaponData.Slot = InventoryWeaponSlot.None;
			SelectedWeaponData.Weapon = null;
		}
		((ShopInventoryTab)tabs[TabEnums.Inventory]).ClearInventory();
	}

	public bool IsWaitingForPlayers()
	{
		return isShoppingDone;
	}

	public bool IsActiveSlotAvailable()
	{
		if (PlayerGUI.Inventory != null && PlayerGUI.Inventory.IsSlotAvailable(SelectedWeaponData.Slot))
		{
			return true;
		}
		return false;
	}
}
