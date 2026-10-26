using System.Collections.Generic;
using UnityEngine;

public class ShopWeaponsTab : ShopTabBase<ShopWeaponsTab.TabWeaponButton>
{
	public class TabWeaponButton : TabButton
	{
		public GameObject WeaponPrefab;

		public Texture WeaponTexture;

		public Texture LockedTexture;

		public string DisplayName;

		public Rect WeaponLogoRect;

		public Rect TitleRect;

		public Rect PriceRect;

		public Rect levelRect;

		public int Price;

		public int level;
	}

	private MetaManager metaManager;

	private string lockedWeaponName;

	private string lockedWeaponDesc;

	private TabWeaponButton sellBuyWeaponButton;

	public TabWeaponButton SellBuyWeaponButton
	{
		get
		{
			return sellBuyWeaponButton;
		}
	}

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

	public ShopWeaponsTab(PlayerShopWindow shop, Rect tabRect, GUISkin skin, ShopController shopController)
		: base(shop, tabRect, skin, shop.InputID)
	{
		title = Localizer.GetString("gui_weapons");
		lockedWeaponName = Localizer.GetString("gui_shop_weapon_locked_name");
		lockedWeaponDesc = Localizer.GetString("gui_shop_weapon_locked_desc");
		subTitleStyle.fontSize = ScaledFont.getFontSize(18);
		mainTitleStyle.fontSize = ScaledFont.getFontSize(30);
		SetSizeBasedOnResolution();
		metaManager = MetaManager.Instance;
	}

	public override void SetSizeBasedOnResolution()
	{
		titleRect = ScaledRect.getRect(15f, 88f, 300f, 64f, Alignment.TopLeft);
		for (int i = 0; i < buttons.Count; i++)
		{
			float num = 113 + i * 74;
			buttons[i].levelRect = ScaledRect.getRect(235f, num + 63f, 120f, 50f, Alignment.TopLeft);
			buttons[i].Rect = ScaledRect.getRect(-15f, num + 20f, 350f, 89f, Alignment.TopLeft);
			buttons[i].WeaponLogoRect = ScaledRect.getRect(190f, num + 32f, 128f, 64f, Alignment.TopLeft);
			buttons[i].TitleRect = ScaledRect.getRect(30f, num + 23f, 200f, 64f, Alignment.TopLeft);
			buttons[i].PriceRect = ScaledRect.getRect(30f, num + 41f, 200f, 64f, Alignment.TopLeft);
		}
	}

	public override void Update()
	{
		if (enabled)
		{
			base.Update();
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
		GUI.Label(titleRect, title, mainTitleStyle);
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
			GUI.DrawTexture(buttons[i].WeaponLogoRect, buttons[i].WeaponTexture);
			if (!buttons[i].Locked)
			{
				GUI.Label(buttons[i].TitleRect, buttons[i].DisplayName, subTitleStyle);
				GUI.Label(buttons[i].levelRect, "Level " + buttons[i].level, subTitleStyle);
				if (shop.WeaponExistInInventory(buttons[i].WeaponPrefab))
				{
					subTitleStyle.normal.textColor = Color.red;
					GUI.Label(buttons[i].PriceRect, Localizer.GetString("gui_owned"), subTitleStyle);
				}
				else if (shop.CanAfford(buttons[i].Price) || (shop.SelectedWeaponData.Data != null && shop.CanAfford(buttons[i].Price - shop.GetWeaponValue())))
				{
					GUI.Label(buttons[i].PriceRect, "£" + buttons[i].Price, subTitleStyle);
				}
				else
				{
					subTitleStyle.normal.textColor = Color.red;
					GUI.Label(buttons[i].PriceRect, "£" + buttons[i].Price, subTitleStyle);
				}
			}
			else
			{
				GUI.Label(buttons[i].TitleRect, lockedWeaponName, subTitleStyle);
				GUI.Label(buttons[i].PriceRect, lockedWeaponDesc, subTitleStyle);
				GUI.DrawTexture(buttons[i].Rect, buttons[i].LockedTexture);
			}
		}
		GUI.EndGroup();
	}

	public override void ResetUnableTint()
	{
		ActionUnable = false;
		tintIndex = 0;
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

	public void AddButton(Weapon weapon, GUIStyle style, bool disabled, bool locked, bool selected = false)
	{
		TabWeaponButton tabWeaponButton = new TabWeaponButton();
		tabWeaponButton.WeaponPrefab = weapon.gameObject;
		tabWeaponButton.Disabled = disabled;
		tabWeaponButton.Locked = locked;
		tabWeaponButton.Name = weapon.Name;
		tabWeaponButton.level = MetaManager.Instance.WeaponLevel(weapon.Name);
		tabWeaponButton.Price = weapon.CalculatePurchaseCost(shop.PlayerGUI.Soldier);
		tabWeaponButton.DisplayName = Localizer.GetString(weapon.Image);
		tabWeaponButton.WeaponTexture = skin.GetStyle(weapon.Image).normal.background;
		tabWeaponButton.LockedTexture = skin.GetStyle("button_locked").normal.background;
		tabWeaponButton.Selected = selected;
		tabWeaponButton.Style = style;
		buttons.Add(tabWeaponButton);
	}

	private GUIStyle GetPerkStyle(Perk perk)
	{
		switch (perk)
		{
		case Perk.Berserker:
			return skin.GetStyle("gui_berserker");
		case Perk.Commando:
			return skin.GetStyle("gui_commando");
		case Perk.Demolition:
			return skin.GetStyle("gui_demolition");
		case Perk.Firebug:
			return skin.GetStyle("gui_firebug");
		case Perk.Medic:
			return skin.GetStyle("gui_medic");
		case Perk.Sharpshooter:
			return skin.GetStyle("gui_sharpshooter");
		case Perk.SupportSpecialist:
			return skin.GetStyle("gui_support");
		default:
			return skin.GetStyle("gui_commando");
		}
	}

	protected override void ButtonPressed(TabWeaponButton button)
	{
		if (PurchaseRequest(button))
		{
			shop.IsActiveSlotAvailable();
			shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.Inventory, "forward", button.Name, null);
			shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseWeapon);
		}
		else
		{
			shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseUnable);
		}
	}

	public override void OnForwardPressed()
	{
		foreach (TabWeaponButton button in buttons)
		{
			if (button.Selected && !button.Disabled && !button.Locked)
			{
				ButtonPressed(button);
				break;
			}
		}
	}

	public override void OnBackwardPressed()
	{
		if (shop.SelectedWeaponData.Slot == InventoryWeaponSlot.Pistol)
		{
			shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.Inventory, "backward", null, null);
		}
		else
		{
			shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.WeaponCategories, "backward", null, null);
		}
		shop.PlaySound(GuiInGameManager.Sound.Backward);
	}

	private bool PurchaseRequest(TabWeaponButton button)
	{
		if (shop.CanAfford(button.Price) && shop.IsActiveSlotAvailable())
		{
			shop.PurchaseWeapon(button.WeaponPrefab, button.Price);
			return true;
		}
		if (shop.CanAfford(button.Price - shop.GetWeaponValue()) && !shop.IsActiveSlotAvailable())
		{
			sellBuyWeaponButton = button;
			shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.SellBuy, "forward", button.Name, null);
			shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseOther);
			return false;
		}
		return false;
	}

	public override void RemoveSelection()
	{
		foreach (TabWeaponButton button in buttons)
		{
			button.Selected = false;
		}
		SetSelectedButtonByIndex(-1);
	}

	public override void Start()
	{
		enabled = true;
	}

	public override void Stop()
	{
		enabled = false;
	}

	public void SetWeaponTabData(Perk perk)
	{
		buttons.Clear();
		Dictionary<InventoryWeaponSlot, StoredWeaponData> weapons = shop.PlayerGUI.Weapons;
		if (weapons == null)
		{
			return;
		}
		if (shop.SelectedWeaponData.Slot == InventoryWeaponSlot.Pistol)
		{
			List<Weapon> weaponsBySlot = shop.GetWeaponsBySlot(WeaponSlot.Pistol);
			Weapon weapon = ((!weapons.ContainsKey(InventoryWeaponSlot.Pistol)) ? null : weapons[InventoryWeaponSlot.Pistol].Weapon);
			for (int i = 0; i < weaponsBySlot.Count; i++)
			{
				Weapon weapon2 = weaponsBySlot[i];
				bool disabled = false;
				bool locked = false;
				Weapon.ShopDisplayModes shopDisplayMode = weapon2.ShopDisplayMode;
				if (weapon != null && weapon == weapon2)
				{
					disabled = true;
				}
				if (shopDisplayMode == Weapon.ShopDisplayModes.DontDisplay)
				{
					continue;
				}
				if (shopDisplayMode != Weapon.ShopDisplayModes.AlwaysUnlocked && !metaManager.IsWeaponUnlocked(weapon2.Name))
				{
					if (shopDisplayMode == Weapon.ShopDisplayModes.OnlyIfUnlocked)
					{
						continue;
					}
					locked = true;
				}
				AddButton(weapon2, skin.GetStyle("button1"), disabled, locked, i == 0);
			}
		}
		else
		{
			List<Weapon> weaponsBySlot = shop.GetWeaponsByPerk(perk);
			foreach (Weapon item in weaponsBySlot)
			{
				bool disabled2 = false;
				bool locked2 = false;
				Weapon.ShopDisplayModes shopDisplayMode2 = item.ShopDisplayMode;
				foreach (StoredWeaponData value in weapons.Values)
				{
					if (value.Weapon == item)
					{
						disabled2 = true;
						break;
					}
				}
				if (item.Slot == WeaponSlot.Pistol || shopDisplayMode2 == Weapon.ShopDisplayModes.DontDisplay)
				{
					continue;
				}
				if (shopDisplayMode2 != Weapon.ShopDisplayModes.AlwaysUnlocked && !metaManager.IsWeaponUnlocked(item.Name))
				{
					if (shopDisplayMode2 == Weapon.ShopDisplayModes.OnlyIfUnlocked)
					{
						continue;
					}
					locked2 = true;
				}
				AddButton(item, skin.GetStyle("button1"), disabled2, locked2);
			}
		}
		if (buttons.Count > 0)
		{
			buttons = SortWeapons(buttons);
			if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
			{
				SetSelectedButtonByIndex(0);
			}
		}
	}

	private List<TabWeaponButton> SortWeapons(List<TabWeaponButton> weapons)
	{
		List<TabWeaponButton> list = new List<TabWeaponButton>();
		List<TabWeaponButton> list2 = new List<TabWeaponButton>();
		int count = weapons.Count;
		for (int i = 0; i < count; i++)
		{
			if (weapons[i].Locked)
			{
				list.Add(weapons[i]);
			}
			else
			{
				list2.Add(weapons[i]);
			}
		}
		if (list.Count > 1)
		{
			list = SortWeaponsByPrice(list);
		}
		if (list2.Count > 1)
		{
			list2 = SortWeaponsByPrice(list2);
		}
		weapons = new List<TabWeaponButton>();
		for (int j = 0; j < list2.Count; j++)
		{
			weapons.Add(list2[j]);
		}
		for (int k = 0; k < list.Count; k++)
		{
			weapons.Add(list[k]);
		}
		return weapons;
	}

	private List<TabWeaponButton> SortWeaponsByPrice(List<TabWeaponButton> weapons)
	{
		weapons.Sort((TabWeaponButton button1, TabWeaponButton button2) => button1.Price.CompareTo(button2.Price));
		return weapons;
	}
}
