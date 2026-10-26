using UnityEngine;

public class ShopSellBuyTab : ShopTabBase<ShopSellBuyTab.TabSellBuyButton>
{
	public class TabSellBuyButton : TabButton
	{
		public Texture WeaponTexture;

		public Texture LockedTexture;

		public string DisplayName;

		public Rect TitleRect;
	}

	private ShopWeaponsTab.TabWeaponButton shopWeaponsTabButton;

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

	public ShopSellBuyTab(PlayerShopWindow shop, Rect tabRect, GUISkin skin, ShopController shopController)
		: base(shop, tabRect, skin, shop.InputID)
	{
		subTitleStyle.fontSize = ScaledFont.getFontSize(22);
		mainTitleStyle.fontSize = ScaledFont.getFontSize(30);
		AddButton(skin.GetStyle("button1"), Localizer.GetString("gui_yes"));
		AddButton(skin.GetStyle("button1"), Localizer.GetString("gui_no"));
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
		{
			SetSelectedButtonByIndex(0);
		}
		SetSizeBasedOnResolution();
	}

	public override void SetSizeBasedOnResolution()
	{
		titleRect = ScaledRect.getRect(15f, 88f, 300f, 164f, Alignment.TopLeft);
		for (int i = 0; i < buttons.Count; i++)
		{
			float num = 113 + (i + 2) * 74;
			buttons[i].Rect = ScaledRect.getRect(-15f, num + 20f, 350f, 89f, Alignment.TopLeft);
			buttons[i].TitleRect = ScaledRect.getRect(30f, num + 23f, 300f, 90f, Alignment.TopLeft);
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
		GUI.Label(titleRect, title, subTitleStyle);
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
			GUI.Label(buttons[i].TitleRect, buttons[i].DisplayName, mainTitleStyle);
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

	public void AddButton(GUIStyle style, string text)
	{
		TabSellBuyButton tabSellBuyButton = new TabSellBuyButton();
		tabSellBuyButton.Disabled = false;
		tabSellBuyButton.Locked = false;
		tabSellBuyButton.Name = "WEAPON NAME";
		tabSellBuyButton.DisplayName = text;
		tabSellBuyButton.LockedTexture = skin.GetStyle("button_locked").normal.background;
		tabSellBuyButton.Style = style;
		buttons.Add(tabSellBuyButton);
	}

	public override void HandleTouch(Vector2 location)
	{
		int num = 0;
		foreach (TabSellBuyButton button in buttons)
		{
			Rect rect = button.Rect;
			rect.x += shop.tabRect.x;
			if (rect.Contains(location))
			{
				if (num == 0)
				{
					ButtonPressed(button);
					break;
				}
				OnBackwardPressed();
			}
			num++;
		}
	}

	protected override void ButtonPressed(TabSellBuyButton button)
	{
		if (shopWeaponsTabButton != null)
		{
			shop.SellWeapon();
			shop.PurchaseWeapon(shopWeaponsTabButton.WeaponPrefab, shopWeaponsTabButton.Price);
			shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.Inventory, "forward", shopWeaponsTabButton.Name, null);
		}
		else
		{
			Debug.LogError("The variable shopWeaponsTabButton was null");
		}
	}

	public override void OnForwardPressed()
	{
		if (buttons[0].Selected)
		{
			ButtonPressed(buttons[0]);
		}
		else
		{
			OnBackwardPressed();
		}
	}

	public override void OnBackwardPressed()
	{
		shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.WeaponList, "backward", null, null);
		shop.PlaySound(GuiInGameManager.Sound.Backward);
	}

	public void SetData(ShopWeaponsTab.TabWeaponButton button)
	{
		string @string = Localizer.GetString("gui_shop_buy_sell_popup");
		@string = @string.Replace("XX", Localizer.GetString(shop.GetWeapon().Image));
		@string = @string.Replace("YY", "£" + shop.GetWeaponValue());
		title = @string;
		shopWeaponsTabButton = button;
	}

	public override void Start()
	{
		base.Start();
		SetSelectedButtonByIndex(0);
	}
}
