using UnityEngine;

public class ShopOptionsTab : ShopTabBase<ShopOptionsTab.TabOptionButton>
{
	public class TabOptionButton : TabButton
	{
		public string DisplayName;

		public Rect WeaponLogoRect;

		public Rect TitleRect;

		public ButtonType Type;

		public int MoneyValue;
	}

	public enum ButtonType
	{
		AmmunitionFill = 0,
		AmmunitionMag = 1,
		SecondaryAmmunitionMag = 2,
		Sell = 3,
		WeaponLists = 4,
		Purchase = 5
	}

	private string fullAmmoText;

	private int fillMoneyPrice;

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

	public ShopOptionsTab(PlayerShopWindow shop, Rect tabRect, GUISkin skin)
		: base(shop, tabRect, skin, shop.InputID)
	{
		fullAmmoText = Localizer.GetString("gui_shop_ammo_full");
		title = Localizer.GetString("gui_options");
		subTitleStyle.fontSize = ScaledFont.getFontSize(16);
		mainTitleStyle.fontSize = ScaledFont.getFontSize(30);
		SetSizeBasedOnResolution();
	}

	public override void SetSizeBasedOnResolution()
	{
		titleRect = ScaledRect.getRect(15f, 88f, 300f, 64f, Alignment.TopLeft);
		for (int i = 0; i < buttons.Count; i++)
		{
			float num = 113 + i * 74;
			buttons[i].Rect = ScaledRect.getRect(-15f, num + 20f, 350f, 89f, Alignment.TopLeft);
			buttons[i].WeaponLogoRect = ScaledRect.getRect(190f, num + 32f, 128f, 64f, Alignment.TopLeft);
			buttons[i].TitleRect = ScaledRect.getRect(30f, num + 32f, 300f, 64f, Alignment.TopLeft);
		}
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
			if (buttons[i].Type == ButtonType.WeaponLists)
			{
				GUI.Label(buttons[i].TitleRect, buttons[i].DisplayName, mainTitleStyle);
			}
			else if (buttons[i].Type == ButtonType.AmmunitionFill || buttons[i].Type == ButtonType.AmmunitionMag || buttons[i].Type == ButtonType.SecondaryAmmunitionMag)
			{
				if (buttons[i].Type == ButtonType.SecondaryAmmunitionMag)
				{
					if (shop.SelectedWeaponData.Data.SecondaryAmmunitionFull())
					{
						mainTitleStyle.normal.textColor = Color.gray;
						GUI.Label(buttons[i].TitleRect, fullAmmoText, mainTitleStyle);
					}
					else if (shop.CanAfford(buttons[i].MoneyValue))
					{
						GUI.Label(buttons[i].TitleRect, buttons[i].DisplayName + buttons[i].MoneyValue, mainTitleStyle);
					}
					else
					{
						mainTitleStyle.normal.textColor = Color.red;
						GUI.Label(buttons[i].TitleRect, buttons[i].DisplayName + buttons[i].MoneyValue, mainTitleStyle);
					}
				}
				else if (shop.SelectedWeaponData.Data.PrimaryAmmunitionFull())
				{
					mainTitleStyle.normal.textColor = Color.gray;
					GUI.Label(buttons[i].TitleRect, fullAmmoText, mainTitleStyle);
				}
				else if (buttons[i].Type == ButtonType.AmmunitionFill && fillMoneyPrice != -1 && shop.CanAfford(fillMoneyPrice))
				{
					GUI.Label(buttons[i].TitleRect, buttons[i].DisplayName + buttons[i].MoneyValue, mainTitleStyle);
				}
				else if (buttons[i].Type != 0 && shop.CanAfford(buttons[i].MoneyValue))
				{
					GUI.Label(buttons[i].TitleRect, buttons[i].DisplayName + buttons[i].MoneyValue, mainTitleStyle);
				}
				else
				{
					mainTitleStyle.normal.textColor = Color.red;
					GUI.Label(buttons[i].TitleRect, buttons[i].DisplayName + buttons[i].MoneyValue, mainTitleStyle);
				}
			}
			else
			{
				GUI.Label(buttons[i].TitleRect, buttons[i].DisplayName + buttons[i].MoneyValue, mainTitleStyle);
			}
		}
		GUI.EndGroup();
	}

	public void AddButton(ButtonType type, GUIStyle style)
	{
		Weapon weapon = shop.SelectedWeaponData.Weapon;
		TabOptionButton tabOptionButton = new TabOptionButton();
		if (type == ButtonType.AmmunitionMag)
		{
			tabOptionButton.Name = "ammoMag";
			tabOptionButton.DisplayName = Localizer.GetString("gui_buy") + " " + Localizer.GetString("gui_ammo") + " £";
			if (weapon.CalculateTotalAmmunitionCost(shop.PlayerGUI.Soldier, shop.SelectedWeaponData.Data) == 0)
			{
				tabOptionButton.MoneyValue = 0;
			}
			else
			{
				tabOptionButton.MoneyValue = weapon.CalculateBundleAmmunitionCost(shop.PlayerGUI.Soldier, shop.SelectedWeaponData.Data);
			}
		}
		else if (type == ButtonType.AmmunitionFill)
		{
			tabOptionButton.Name = "ammoFill";
			tabOptionButton.DisplayName = Localizer.GetString("gui_fill") + " " + Localizer.GetString("gui_ammo") + " £";
			tabOptionButton.MoneyValue = weapon.CalculateTotalAmmunitionCost(shop.PlayerGUI.Soldier, shop.SelectedWeaponData.Data);
			int num = weapon.CalculateBundleAmmunitionCost(shop.PlayerGUI.Soldier, shop.SelectedWeaponData.Data);
			if (num != 0)
			{
				int num2 = shop.PlayerGUI.Money();
				int num3 = Mathf.FloorToInt(num2 / num);
				if (num3 == 0)
				{
					fillMoneyPrice = -1;
				}
				else
				{
					fillMoneyPrice = num3 * num;
				}
			}
			else
			{
				fillMoneyPrice = -1;
			}
		}
		else if (type == ButtonType.Sell)
		{
			tabOptionButton.Name = "sell";
			tabOptionButton.DisplayName = Localizer.GetString("gui_sell") + " " + Localizer.GetString("gui_weapon") + " £";
			tabOptionButton.MoneyValue = weapon.CalculateSellPrice(shop.PlayerGUI.Soldier);
		}
		else if (type != ButtonType.WeaponLists)
		{
			bool useAmmunition;
			int num4;
			if (type == ButtonType.SecondaryAmmunitionMag && weapon.HasSecondaryModule())
			{
				useAmmunition = weapon.SecondaryAttackModule.UseAmmunition;
			}
			else
				num4 = 0;
		}
		else
		{
			tabOptionButton.Name = "weapons";
			tabOptionButton.DisplayName = Localizer.GetString("gui_browse_weapons");
		}
		tabOptionButton.Style = style;
		tabOptionButton.Type = type;
		buttons.Add(tabOptionButton);
	}

	private void UpdateButtonMoneyValue()
	{
		Weapon weapon = shop.SelectedWeaponData.Weapon;
		if (!(weapon != null))
		{
			return;
		}
		GameObject soldier = shop.PlayerGUI.Soldier;
		foreach (TabOptionButton button in buttons)
		{
			if (button.Type == ButtonType.AmmunitionFill)
			{
				button.MoneyValue = weapon.CalculateTotalAmmunitionCost(soldier, shop.SelectedWeaponData.Data);
				int num = weapon.CalculateBundleAmmunitionCost(soldier, shop.SelectedWeaponData.Data);
				if (num != 0)
				{
					int num2 = shop.PlayerGUI.Money();
					int num3 = Mathf.FloorToInt(num2 / num);
					if (num3 == 0)
					{
						fillMoneyPrice = -1;
					}
					else
					{
						fillMoneyPrice = num3 * num;
					}
				}
				else
				{
					fillMoneyPrice = -1;
				}
			}
			if (button.Type == ButtonType.AmmunitionMag)
			{
				if (weapon.CalculateTotalAmmunitionCost(soldier, shop.SelectedWeaponData.Data) == 0)
				{
					button.MoneyValue = 0;
				}
				else
				{
					button.MoneyValue = weapon.CalculateBundleAmmunitionCost(soldier, shop.SelectedWeaponData.Data);
				}
			}
			if (button.Type == ButtonType.SecondaryAmmunitionMag)
			{
				if (!shop.SelectedWeaponData.Data.SecondaryAmmunitionFull())
				{
					int moneyValue = weapon.SecondaryAttackModule.CalculateBundleAmmunitionCost(soldier, shop.SelectedWeaponData.Data);
					button.MoneyValue = moneyValue;
				}
				else
				{
					button.MoneyValue = 0;
				}
			}
		}
	}

	protected override void ButtonPressed(TabOptionButton button)
	{
		if (button.Type == ButtonType.AmmunitionFill)
		{
			if (shop.PurchaseAmmoFill())
			{
				UpdateButtonMoneyValue();
				shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseOther);
			}
			else
			{
				shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseUnable);
			}
		}
		else if (button.Type == ButtonType.AmmunitionMag)
		{
			if (shop.PurchaseAmmoMag())
			{
				UpdateButtonMoneyValue();
				shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseOther);
			}
			else
			{
				shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseUnable);
			}
		}
		else if (button.Type == ButtonType.SecondaryAmmunitionMag)
		{
			if (shop.PurchaseSecondaryAmmo())
			{
				UpdateButtonMoneyValue();
				shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseOther);
			}
			else
			{
				shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseUnable);
			}
		}
		else if (button.Type == ButtonType.Purchase)
		{
			shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.WeaponCategories, "forward", button.Name, null);
			shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseOther);
		}
		else if (button.Type == ButtonType.Sell)
		{
			if (SellRequest(button))
			{
				shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.Inventory, "forward", button.Name, null);
			}
			shop.PlaySound(GuiInGameManager.Sound.ShopSell);
		}
		else if (button.Type == ButtonType.WeaponLists)
		{
			if (shop.SelectedWeaponData.Slot == InventoryWeaponSlot.Pistol)
			{
				shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.WeaponList, "forward", button.Name, null);
			}
			else
			{
				shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.WeaponCategories, "forward", button.Name, null);
			}
			shop.PlaySound(GuiInGameManager.Sound.ShopButton);
		}
	}

	public override void OnForwardPressed()
	{
		foreach (TabOptionButton button in buttons)
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
		shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.Inventory, "backward", null, null);
		shop.PlaySound(GuiInGameManager.Sound.Forward);
	}

	private bool SellRequest(TabOptionButton button)
	{
		if (!shop.IsActiveSlotAvailable())
		{
			shop.SellWeapon();
			return true;
		}
		return false;
	}

	public void SetDataBasedOnSelectedWeapon()
	{
		buttons.Clear();
		if (shop.SelectedWeaponData.Data == null)
		{
			AddButton(ButtonType.Purchase, skin.GetStyle("button1"));
		}
		else
		{
			Weapon weapon = shop.SelectedWeaponData.Weapon;
			bool useAmmunition;
			int num;
			if (weapon.HasSecondaryModule())
			{
				useAmmunition = weapon.SecondaryAttackModule.UseAmmunition;
			}
			else
				num = 0;
			AddButton(ButtonType.AmmunitionMag, skin.GetStyle("button1"));
			AddButton(ButtonType.AmmunitionFill, skin.GetStyle("button1"));
			AddButton(ButtonType.WeaponLists, skin.GetStyle("button1"));
			AddButton(ButtonType.Sell, skin.GetStyle("button1"));
		}
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller && buttons.Count > 0)
		{
			SetSelectedButtonByIndex(0);
		}
	}
}
