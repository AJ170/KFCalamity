using UnityEngine;

public class ShopInventoryTab : ShopTabBase<ShopInventoryTab.InventoryWeaponButton>
{
	public class InventoryWeaponButton : TabButton
	{
		public Weapon Weapon;

		public InventoryWeaponSlot Slot;

		public Texture WeaponTexture;

		public string DisplayName;

		public string SlotDisplayName;

		public int CurrentAmmo;

		public int MaxAmmo;

		public int Price;

		public int level;

		public Rect AmmoRect;

		public Rect SecAmmoRect;

		public Rect LogoRect;

		public Rect TitleRect;

		public Rect PriceRect;

		public Rect AmountRect;

		public Rect EmptyTitleRect;

		public Rect levelRect;

		public WeaponType Type;

		public StoredWeaponData StoredWeaponData;

		public ButtonType ButtonType;
	}

	public enum ButtonType
	{
		Weapon = 0,
		Grenades = 1,
		Armor = 2
	}

	private int grenadeAmount;

	private int maxGrenades = 1;

	private int armorAmount;

	private Rect rectSideArm;

	private Rect rectGear;

	private Rect rectPrimary;

	private string ammoText;

	private string secAmmoText;

	private string emptyText;

	private Color armorColor = new Color(0.9f, 0.45f, 0f);

	private OuyaSDK.OuyaPlayer playerID;

	private bool ActionUnable
	{
		get
		{
			return shouldTint;
		}
		set
		{
			shouldTint = value;
		}
	}

	public ShopInventoryTab(PlayerShopWindow shop, Rect tabRect, GUISkin skin)
		: base(shop, tabRect, skin, shop.InputID)
	{
		ammoText = Localizer.GetString("gui_ammo");
		secAmmoText = Localizer.GetString("gui_ammo");
		emptyText = Localizer.GetString("gui_inventory_slot_empty");
		title = Localizer.GetString("gui_inventory");
		subTitleStyle.fontSize = ScaledFont.getFontSize(16);
		mainTitleStyle.fontSize = ScaledFont.getFontSize(30);
		InitializeWeaponSlots();
		SetSizeBasedOnResolution();
	}

	private void SetInventoryButtonData(int index, InventoryWeaponSlot slot, GUIStyle style, string slotDisplayName, string name, Perk perk, ButtonType buttonType, bool selected = false, Texture weaponTexture = null)
	{
		InventoryWeaponButton inventoryWeaponButton = buttons[index];
		inventoryWeaponButton.Slot = slot;
		inventoryWeaponButton.Style = style;
		inventoryWeaponButton.SlotDisplayName = slotDisplayName;
		inventoryWeaponButton.DisplayName = string.Empty;
		inventoryWeaponButton.Name = name;
		inventoryWeaponButton.Perk = perk;
		inventoryWeaponButton.ButtonType = buttonType;
		inventoryWeaponButton.Selected = selected;
		inventoryWeaponButton.WeaponTexture = weaponTexture;
		if (selected)
		{
			SetSelectedButtonByIndex(index);
		}
	}

	private void InitializeWeaponSlots()
	{
		if (buttons.Count == 0)
		{
			for (int i = 0; i < 5; i++)
			{
				buttons.Add(new InventoryWeaponButton());
			}
		}
		GUIStyle style = skin.GetStyle("button1");
		bool selected = DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller;
		for (int j = 0; j < buttons.Count; j++)
		{
			switch (j)
			{
			case 0:
				SetInventoryButtonData(j, InventoryWeaponSlot.Pistol, style, Localizer.GetString("gui_inventory_slot_1"), string.Empty, Perk.None, ButtonType.Weapon, selected);
				break;
			case 1:
				SetInventoryButtonData(j, InventoryWeaponSlot.Primary1, style, Localizer.GetString("gui_inventory_slot_2"), string.Empty, Perk.None, ButtonType.Weapon);
				break;
			case 2:
				SetInventoryButtonData(j, InventoryWeaponSlot.Primary2, style, Localizer.GetString("gui_inventory_slot_3"), string.Empty, Perk.None, ButtonType.Weapon);
				break;
			case 3:
				SetInventoryButtonData(j, InventoryWeaponSlot.None, style, Localizer.GetString("gui_inventory_slot_4"), "armor", Perk.None, ButtonType.Armor, false, skin.GetStyle("equipment_armor").normal.background);
				break;
			case 4:
				SetInventoryButtonData(j, InventoryWeaponSlot.None, style, Localizer.GetString("gui_inventory_slot_5"), "grenade", Perk.None, ButtonType.Grenades, false, skin.GetStyle("equipment_grenade").normal.background);
				break;
			}
		}
	}

	public override void SetSizeBasedOnResolution()
	{
		titleRect = ScaledRect.getRect(30f, 88f, 300f, 64f, Alignment.TopLeft);
		rectSideArm = ScaledRect.getRect(30f, 88f, 300f, 64f, Alignment.TopLeft);
		rectPrimary = ScaledRect.getRect(30f, 217f, 300f, 64f, Alignment.TopLeft);
		rectGear = ScaledRect.getRect(30f, 424f, 300f, 64f, Alignment.TopLeft);
		for (int i = 0; i < buttons.Count; i++)
		{
			float num;
			if (i > 2)
			{
				num = 113 + (i + 1) * 84;
				if (i > 3)
				{
					buttons[i].LogoRect = ScaledRect.getRect(255f, num + 32f + 5f, 27f, 50f, Alignment.TopLeft);
				}
				else
				{
					buttons[i].LogoRect = ScaledRect.getRect(255f, num + 32f + 5f, 33f, 46f, Alignment.TopLeft);
				}
				buttons[i].PriceRect = ScaledRect.getRect(35f, num + 32f, 200f, 64f, Alignment.TopLeft);
				buttons[i].AmountRect = ScaledRect.getRect(250f, num + 32f, 100f, 64f, Alignment.TopRight);
				buttons[i].TitleRect = ScaledRect.getRect(15f, num - 10f, 300f, 64f, Alignment.TopLeft);
			}
			else
			{
				num = ((i != 0) ? ((float)(158 + i * 84)) : ((float)(113 + i * 84)));
				buttons[i].levelRect = ScaledRect.getRect(235f, num + 63f, 120f, 50f, Alignment.TopLeft);
				buttons[i].EmptyTitleRect = ScaledRect.getRect(30f, num + 32f, 200f, 64f, Alignment.TopLeft);
				buttons[i].TitleRect = ScaledRect.getRect(30f, num + 12f, 300f, 64f, Alignment.TopLeft);
				buttons[i].AmmoRect = ScaledRect.getRect(30f, num + 32f, 200f, 64f, Alignment.TopLeft);
				buttons[i].SecAmmoRect = ScaledRect.getRect(30f, num + 52f, 200f, 64f, Alignment.TopLeft);
				buttons[i].LogoRect = ScaledRect.getRect(190f, num + 32f, 128f, 64f, Alignment.TopLeft);
			}
			buttons[i].Rect = ScaledRect.getRect(-15f, num + 20f, 350f, 89f, Alignment.TopLeft);
		}
	}

	public void ClearInventory()
	{
		RemoveWeapon(InventoryWeaponSlot.Pistol);
		RemoveWeapon(InventoryWeaponSlot.Primary1);
		RemoveWeapon(InventoryWeaponSlot.Primary2);
	}

	public void UpdateInventory()
	{
		UpdateArmor();
		UpdateGrenades();
		for (int i = 0; i < buttons.Count; i++)
		{
			if (buttons[i].StoredWeaponData != null)
			{
				buttons[i].level = MetaManager.Instance.WeaponLevel(buttons[i].Name);
			}
		}
	}

	public override void Display()
	{
		if (!visible)
		{
			return;
		}
		mainTitleStyle.normal.textColor = Color.white;
		subTitleStyle.normal.textColor = Color.white;
		GUI.BeginGroup(tabRect);
		backupColor = GUI.color;
		GUI.Label(rectSideArm, Localizer.GetString("gui_shop_title_sidearm"), mainTitleStyle);
		GUI.Label(rectPrimary, Localizer.GetString("gui_shop_title_primary"), mainTitleStyle);
		GUI.Label(rectGear, Localizer.GetString("gui_shop_title_gear"), mainTitleStyle);
		for (int i = 0; i < buttons.Count; i++)
		{
			if (buttons[i].Selected && ShopController.UseSelectionInShop)
			{
				mainTitleStyle.normal.textColor = Color.black;
				subTitleStyle.normal.textColor = Color.black;
				if (ShouldTintUnableColor())
				{
					GUI.color = Color.red;
					GUI.DrawTexture(buttons[i].Rect, buttons[i].Style.hover.background);
					GUI.color = backupColor;
				}
				else
				{
					GUI.DrawTexture(buttons[i].Rect, buttons[i].Style.hover.background);
				}
			}
			else
			{
				GUI.DrawTexture(buttons[i].Rect, buttons[i].Style.normal.background);
				mainTitleStyle.normal.textColor = Color.white;
				subTitleStyle.normal.textColor = Color.white;
			}
			if (i > 2)
			{
				if (buttons[i].Price == 0)
				{
					mainTitleStyle.normal.textColor = Color.gray;
				}
				else if (!shop.CanAfford(buttons[i].Price))
				{
					if (buttons[i].ButtonType == ButtonType.Armor && shop.PlayerGUI.Money() != 0)
					{
						mainTitleStyle.normal.textColor = armorColor;
					}
					else
					{
						mainTitleStyle.normal.textColor = Color.red;
					}
				}
				if (i == 4)
				{
					GUI.DrawTexture(buttons[i].LogoRect, buttons[i].WeaponTexture);
					GUI.Label(buttons[i].PriceRect, buttons[i].DisplayName, mainTitleStyle);
					mainTitleStyle.alignment = TextAnchor.MiddleRight;
					GUI.Label(buttons[i].AmountRect, grenadeAmount + "/" + maxGrenades, mainTitleStyle);
					mainTitleStyle.alignment = TextAnchor.MiddleLeft;
				}
				else
				{
					mainTitleStyle.alignment = TextAnchor.MiddleRight;
					GUI.Label(buttons[i].AmountRect, armorAmount + "%", mainTitleStyle);
					mainTitleStyle.alignment = TextAnchor.MiddleLeft;
					GUI.DrawTexture(buttons[i].LogoRect, buttons[i].WeaponTexture);
					GUI.Label(buttons[i].PriceRect, buttons[i].DisplayName, mainTitleStyle);
				}
				continue;
			}
			if (buttons[i].Name == string.Empty)
			{
				GUI.Label(buttons[i].EmptyTitleRect, emptyText, mainTitleStyle);
			}
			else
			{
				GUI.DrawTexture(buttons[i].LogoRect, buttons[i].WeaponTexture);
				GUI.Label(buttons[i].levelRect, Localizer.GetString("gui_level") + " " + buttons[i].level, subTitleStyle);
				if (buttons[i].Type == WeaponType.Ranged)
				{
					StoredWeaponData storedWeaponData = buttons[i].StoredWeaponData;
					GUI.Label(buttons[i].AmmoRect, ammoText + " " + (storedWeaponData.LoadedAmmunition + storedWeaponData.SpareAmmunition) + "/" + buttons[i].MaxAmmo, subTitleStyle);
					bool useAmmunition;
					int num;
					if (storedWeaponData.Weapon.HasSecondaryModule())
					{
						useAmmunition = storedWeaponData.Weapon.SecondaryAttackModule.UseAmmunition;
					}
					else
						num = 0;
				}
			}
			GUI.Label(buttons[i].TitleRect, buttons[i].DisplayName, subTitleStyle);
		}
		GUI.EndGroup();
	}

	private bool ShouldTintUnableColor()
	{
		bool result = false;
		if (ActionUnable)
		{
			if (tintIndex >= 0 && tintIndex < 11)
			{
				result = true;
				tintIndex++;
			}
			else if (tintIndex >= 21 && tintIndex < 31)
			{
				result = true;
				tintIndex++;
			}
			else if (tintIndex == 31)
			{
				ActionUnable = false;
				tintIndex = 0;
			}
			else
			{
				tintIndex++;
			}
		}
		return result;
	}

	public override void ResetUnableTint()
	{
		ActionUnable = false;
		tintIndex = 0;
	}

	public void SetEquipedWeaponReference(Weapon equippedWeapon)
	{
	}

	public void AddWeapon(InventoryWeaponSlot slot, Weapon newWeapon, StoredWeaponData storedWeaponData)
	{
		foreach (InventoryWeaponButton button in buttons)
		{
			if (button.Slot == slot)
			{
				button.Weapon = newWeapon;
				button.Name = newWeapon.Name;
				button.level = MetaManager.Instance.WeaponLevel(newWeapon.Name);
				button.Perk = newWeapon.SoldierPerk;
				button.WeaponTexture = skin.GetStyle(newWeapon.Image).normal.background;
				button.DisplayName = Localizer.GetString(newWeapon.Image);
				button.StoredWeaponData = storedWeaponData;
				button.MaxAmmo = newWeapon.MaxTotalAmmunition;
				button.Price = newWeapon.CalculateSellPrice(shop.PlayerGUI.Soldier);
				button.Type = newWeapon.Type;
				break;
			}
		}
	}

	public void RemoveWeapon(InventoryWeaponSlot slot)
	{
		foreach (InventoryWeaponButton button in buttons)
		{
			if (button.Slot == slot)
			{
				button.Name = string.Empty;
				button.Perk = Perk.None;
				button.WeaponTexture = null;
				button.StoredWeaponData = null;
				button.DisplayName = string.Empty;
				button.Type = WeaponType.Ranged;
				break;
			}
		}
	}

	protected override void ButtonPressed(InventoryWeaponButton button)
	{
		if (button.ButtonType == ButtonType.Armor)
		{
			if (RequestArmorPurchase())
			{
				shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseOther);
				return;
			}
			ActionUnable = true;
			shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseUnable);
			return;
		}
		if (button.ButtonType == ButtonType.Grenades)
		{
			if (RequestGrenadePurchase())
			{
				shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseOther);
				return;
			}
			ActionUnable = true;
			shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseUnable);
			return;
		}
		shop.SetSelectedWeaponSlot(button.Slot, button.Weapon, button.StoredWeaponData);
		shop.SetWeaponCategories(button.Slot);
		shop.SetWeaponTabData(Perk.None);
		if (button.StoredWeaponData == null)
		{
			if (button.Slot == InventoryWeaponSlot.Pistol)
			{
				shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.WeaponList, "forward", button.Name, null);
			}
			else
			{
				shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.WeaponCategories, "forward", button.Name, null);
			}
		}
		else
		{
			shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.Options, "forward", button.Name, null);
		}
		shop.PlaySound(GuiInGameManager.Sound.ShopButton);
	}

	public override void OnForwardPressed()
	{
		if (shop.IsWaitingForPlayers())
		{
			shop.SetShopStatus();
			return;
		}
		foreach (InventoryWeaponButton button in buttons)
		{
			if (button.Selected)
			{
				ButtonPressed(button);
				break;
			}
		}
	}

	public override void OnBackwardPressed()
	{
		if (!shop.IsWaitingForPlayers())
		{
			shop.SetShopStatus();
			shop.PlaySound(GuiInGameManager.Sound.Forward);
		}
	}

	private bool RequestArmorPurchase()
	{
		bool result = shop.PurchaseArmor();
		UpdateArmor();
		return result;
	}

	private bool RequestGrenadePurchase()
	{
		bool result = shop.PurchaseGrenade();
		UpdateGrenades();
		return result;
	}

	private void UpdateArmor()
	{
		armorAmount = 0;
		if (!(shop.PlayerGUI.Soldier != null))
		{
			return;
		}
		Health component = shop.PlayerGUI.Soldier.GetComponent<Health>();
		if (!(component != null))
		{
			return;
		}
		armorAmount = component.CurrentArmor / 2;
		foreach (InventoryWeaponButton button in buttons)
		{
			if (button.ButtonType == ButtonType.Armor)
			{
				if (component.CurrentArmor < component.MaxArmor)
				{
					int price = Soldier.CalculateArmorCost(shop.PlayerGUI.Soldier);
					button.DisplayName = Localizer.GetString("gui_buy") + " £" + price;
					button.Price = price;
				}
				else
				{
					button.DisplayName = Localizer.GetString("gui_full");
					button.Price = 0;
				}
				break;
			}
		}
	}

	private void UpdateGrenades()
	{
		GameObject soldier = shop.PlayerGUI.Soldier;
		if (!(soldier != null))
		{
			return;
		}
		Ability ability = soldier.GetComponent<PlayerAbilityController>().AbilityBySlot(AbilitySlot.Grenade);
		if (!(ability != null))
		{
			return;
		}
		foreach (InventoryWeaponButton button in buttons)
		{
			if (button.ButtonType == ButtonType.Grenades)
			{
				maxGrenades = ability.MaxCharges;
				grenadeAmount = ability.ChargesLeft();
				if (grenadeAmount < maxGrenades)
				{
					int price = Soldier.CalculateGrenadeCost(soldier);
					button.DisplayName = Localizer.GetString("gui_buy") + " £" + price;
					button.Price = price;
				}
				else
				{
					button.DisplayName = Localizer.GetString("gui_full");
					button.Price = 0;
				}
				break;
			}
		}
	}
}
