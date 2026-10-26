using System.Collections.Generic;
using UnityEngine;

public class ShopPurchaseCategoryTab : ShopTabBase<ShopPurchaseCategoryTab.TabCategoryButton>
{
	public class TabCategoryButton : TabButton
	{
		public GUIStyle perkStyle;

		public string displayName;

		public Rect perkRect;

		public Rect weaponLogoRect;

		public Rect titleRect;
	}

	private class CategoryInfo
	{
		public Perk Perk;

		public string PerkString;

		public CategoryInfo(Perk perk, string perkString)
		{
			Perk = perk;
			PerkString = perkString;
		}
	}

	public ShopPurchaseCategoryTab(PlayerShopWindow shop, Rect tabRect, GUISkin skin)
		: base(shop, tabRect, skin, shop.InputID)
	{
		title = Localizer.GetString("gui_categories");
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
			buttons[i].perkRect = ScaledRect.getRect(15f, num + 32f, 64f, 64f, Alignment.TopLeft);
			buttons[i].weaponLogoRect = ScaledRect.getRect(190f, num + 32f, 128f, 64f, Alignment.TopLeft);
			buttons[i].titleRect = ScaledRect.getRect(70f, num + 32f, 200f, 64f, Alignment.TopLeft);
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
				GUI.DrawTexture(buttons[i].Rect, buttons[i].Style.hover.background);
			}
			else
			{
				GUI.DrawTexture(buttons[i].Rect, buttons[i].Style.normal.background);
				mainTitleStyle.normal.textColor = Color.white;
				subTitleStyle.normal.textColor = Color.white;
			}
			GUI.DrawTexture(buttons[i].perkRect, buttons[i].perkStyle.normal.background);
			GUI.DrawTexture(buttons[i].perkRect, buttons[i].perkStyle.hover.background);
			GUI.DrawTexture(buttons[i].weaponLogoRect, buttons[i].perkStyle.active.background);
			GUI.Label(buttons[i].titleRect, buttons[i].displayName, subTitleStyle);
		}
		GUI.EndGroup();
	}

	public void AddCategories(string name, Perk perk, GUIStyle style)
	{
		TabCategoryButton tabCategoryButton = new TabCategoryButton();
		tabCategoryButton.Name = name;
		tabCategoryButton.Perk = perk;
		tabCategoryButton.displayName = Localizer.GetString(name);
		tabCategoryButton.Style = style;
		tabCategoryButton.perkStyle = GetPerkStyle(perk);
		buttons.Add(tabCategoryButton);
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

	protected override void ButtonPressed(TabCategoryButton button)
	{
		shop.SetWeaponTabData(button.Perk);
		shop.ShopOptionRequest(ShopOptions.ChangeTab, PlayerShopWindow.TabEnums.WeaponList, "forward", button.Name, null);
		shop.PlaySound(GuiInGameManager.Sound.ShopButton);
	}

	public override void OnForwardPressed()
	{
		foreach (TabCategoryButton button in buttons)
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
		shop.PlaySound(GuiInGameManager.Sound.ShopButton);
	}

	public override void RemoveSelection()
	{
		foreach (TabCategoryButton button in buttons)
		{
			button.Selected = false;
		}
	}

	public void SetDataBasedOnSelectedSlot(InventoryWeaponSlot slot)
	{
		buttons.Clear();
		if (slot == InventoryWeaponSlot.Pistol)
		{
			AddCategories("gui_category_pistols", Perk.Medic, skin.GetStyle("button1"));
		}
		else
		{
			if (shop == null || shop.PlayerGUI == null || shop.PlayerGUI.Player == null)
			{
				return;
			}
			Perk selectedPerk = shop.PlayerGUI.Player.SelectedPerk;
			List<CategoryInfo> list = new List<CategoryInfo>();
			list.Add(new CategoryInfo(Perk.Medic, "gui_category_medic"));
			list.Add(new CategoryInfo(Perk.SupportSpecialist, "gui_category_support"));
			list.Add(new CategoryInfo(Perk.Sharpshooter, "gui_category_sharpshooter"));
			list.Add(new CategoryInfo(Perk.Commando, "gui_category_commando"));
			list.Add(new CategoryInfo(Perk.Berserker, "gui_category_berserker"));
			list.Add(new CategoryInfo(Perk.Firebug, "gui_category_firebug"));
			list.Add(new CategoryInfo(Perk.Demolition, "gui_category_demolition"));
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Perk == selectedPerk)
				{
					AddCategories(list[i].PerkString, list[i].Perk, skin.GetStyle("button1"));
					break;
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j].Perk != selectedPerk)
				{
					AddCategories(list[j].PerkString, list[j].Perk, skin.GetStyle("button1"));
				}
			}
			if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
			{
				SetSelectedButtonByIndex(0);
			}
		}
	}

	public override void Start()
	{
		base.Start();
		if (base.CurrentSelected == -1)
		{
			SetSelectedButtonByIndex(0);
		}
	}
}
