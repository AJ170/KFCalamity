using System.Collections.Generic;
using UnityEngine;

public class ShopMoneyTab : ShopTabBase<ShopMoneyTab.TabMoneyButton>
{
	public class TabMoneyButton : TabButton
	{
		public string DisplayName;

		public Rect PerkRect;

		public Rect TitleRect;

		public Rect PriceRect;

		public ButtonType Type;

		public int MoneyValue;

		public int PlayerID;

		public GUIStyle PerkStyle;
	}

	public enum ButtonType
	{
		giveMoney = 0
	}

	private Color orangeColor = new Color(0.9f, 0.45f, 0f);

	private int moneyAmount = 50;

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

	public ShopMoneyTab(PlayerShopWindow shop, Rect tabRect, GUISkin skin)
		: base(shop, tabRect, skin, shop.InputID)
	{
		title = Localizer.GetString("gui_money");
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
			buttons[i].PerkRect = ScaledRect.getRect(15f, num + 32f, 64f, 64f, Alignment.TopLeft);
			buttons[i].TitleRect = ScaledRect.getRect(70f, num + 32f, 200f, 64f, Alignment.TopLeft);
			buttons[i].PriceRect = ScaledRect.getRect(235f, num + 32f, 100f, 64f, Alignment.TopLeft);
		}
	}

	public void SetGiveAwayAmount(int amount)
	{
		moneyAmount = amount;
	}

	public void AddButton(ButtonType type, GUIStyle style, string displayName, int ID, Perk perk)
	{
		TabMoneyButton tabMoneyButton = new TabMoneyButton();
		if (type == ButtonType.giveMoney)
		{
			tabMoneyButton.Name = "ammoMag";
			tabMoneyButton.DisplayName = displayName;
			tabMoneyButton.MoneyValue = moneyAmount;
			tabMoneyButton.PlayerID = ID;
			tabMoneyButton.PerkStyle = GetPerkStyle(perk);
		}
		tabMoneyButton.Style = style;
		tabMoneyButton.Type = type;
		buttons.Add(tabMoneyButton);
	}

	protected override void ButtonPressed(TabMoneyButton button)
	{
		if (shop.GiveMoney(button.PlayerID))
		{
			shop.PlaySound(GuiInGameManager.Sound.ShopSell);
			return;
		}
		shop.PlaySound(GuiInGameManager.Sound.ShopPurchaseUnable);
		ActionUnable = true;
	}

	public override void OnForwardPressed()
	{
		foreach (TabMoneyButton button in buttons)
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
				GUI.Label(buttons[i].TitleRect, buttons[i].DisplayName, mainTitleStyle);
			}
			else
			{
				GUI.DrawTexture(buttons[i].Rect, buttons[i].Style.normal.background);
				mainTitleStyle.normal.textColor = Color.white;
				subTitleStyle.normal.textColor = Color.white;
				GUI.Label(buttons[i].TitleRect, buttons[i].DisplayName, mainTitleStyle);
			}
			if (!shop.CanAfford(buttons[i].MoneyValue))
			{
				int num = shop.PlayerGUI.Money();
				if (num != 0)
				{
					mainTitleStyle.normal.textColor = orangeColor;
				}
				else
				{
					mainTitleStyle.normal.textColor = Color.red;
				}
				GUI.Label(buttons[i].PriceRect, "£" + num, mainTitleStyle);
			}
			else
			{
				mainTitleStyle.normal.textColor = Color.green;
				GUI.Label(buttons[i].PriceRect, "£" + buttons[i].MoneyValue, mainTitleStyle);
			}
			GUI.DrawTexture(buttons[i].PerkRect, buttons[i].PerkStyle.normal.background);
			GUI.DrawTexture(buttons[i].PerkRect, buttons[i].PerkStyle.hover.background);
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

	public void SetButtonData()
	{
		buttons.Clear();
		List<Player> allPlayers = PlayerManager.Instance.GetAllPlayers();
		allPlayers.Sort((Player player, Player player2) => player.ID.CompareTo(player2.ID));
		int iD = shop.PlayerGUI.GetID();
		int num = shop.NumberOfPlayers();
		if (num <= 1)
		{
			return;
		}
		for (int i = 0; i < allPlayers.Count; i++)
		{
			if (allPlayers[i].ID != iD)
			{
				AddButton(ButtonType.giveMoney, skin.GetStyle("button1"), Localizer.GetString("gui_shop_give_player") + " " + (allPlayers[i].ID + 1), allPlayers[i].ID, allPlayers[i].SelectedPerk);
			}
		}
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
		{
			SetSelectedButtonByIndex(0);
		}
		visible = true;
	}
}
