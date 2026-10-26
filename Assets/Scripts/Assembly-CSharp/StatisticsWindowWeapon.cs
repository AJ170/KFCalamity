using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class StatisticsWindowWeapon : MenuBase
{
	private class ElementData
	{
		public string Name;

		public string Title;

		public string TitleDescription;

		public string Image;

		public string Damage;

		public string HeadshotDamage;

		public string Description;

		public string Perk;

		public string Price;

		public bool isLocked;

		public float XpFraction;

		public float XpFrom;

		public float XpTo;

		public int Level;
	}

	public GuiList ElementList;

	public MenuTitleBar TitleBar;

	private WeaponStatisticsDisplayData[] weaponsDisplayData;

	private List<ElementData> elementDataList;

	private List<GuiList.ListButton> buttons;

	private bool isSelected;

	private bool isDirectionNegative;

	private MenuStatistics managerParent;

	private int visibleSideButtons = 2;

	private int startIndex;

	private AxisTouchButtonsWrapper titleBarButtons;

	private void Start()
	{
		if (!(managerParent != null))
		{
			return;
		}
		StatisticsWeaponDefinitions statisticsWeapons = managerParent.StatisticsWeapons;
		if (statisticsWeapons != null)
		{
			weaponsDisplayData = statisticsWeapons.WeaponData;
			buttons = ElementList.Create(visibleSideButtons, 4, 0.5f);
			elementDataList = new List<ElementData>();
			ElementList.SetManagerReference(this);
			ElementList.AdjustButtonPositions();
			SetStartValues();
			InitializeButtonData();
			SetWeaponButtonData(startIndex);
			GameObject @object = ElementList.CurrentSelected().Object;
			if (@object != null)
			{
				StatisticsPageWeapon component = @object.GetComponent<StatisticsPageWeapon>();
				if (component != null)
				{
					TitleBar.SetText(component.Name);
				}
			}
		}
		titleBarButtons = new AxisTouchButtonsWrapper(TitleBar.LeftArrow.gameObject, TitleBar.RightArrow.gameObject);
	}

	public void TweenOut()
	{
		List<GuiList.TweenValue> tweenValues = ElementList.GetTweenValues();
		float num = buttons.Count / 2;
		for (int i = 0; i < buttons.Count; i++)
		{
			if ((float)buttons[i].position == num)
			{
				buttons[i].Object.GetComponent<StatisticsPageWeapon>().TweenAlpha(0f, 0.3f);
			}
			else
			{
				buttons[i].Object.GetComponent<StatisticsPageWeapon>().Tween(tweenValues[buttons[i].position].position * 2f, TweenDirection.Left, 0, 0f, 0.3f);
			}
		}
		TitleBar.gameObject.SetActive(false);
		Invoke("DeActivate", 0.3f);
	}

	public void Open()
	{
		base.gameObject.SetActive(true);
		SetStartValues();
	}

	private void DeActivate()
	{
		base.gameObject.SetActive(false);
	}

	private void Kill()
	{
		Object.Destroy(base.gameObject);
	}

	public void SetManagerReference(MenuStatistics manager)
	{
		managerParent = manager;
	}

	public void Select()
	{
		isSelected = true;
		TitleBar.OnFocusGained();
	}

	public void Deselect()
	{
		isSelected = false;
		TitleBar.OnFocusLost();
	}

	private void OnTouchButtonPressed(ButtonActions sender)
	{
		int num = (int)sender.CustomData;
		GuiList.ListButton listButton = ElementList.CurrentSelected();
		GuiListButtonLevel component = listButton.Object.GetComponent<GuiListButtonLevel>();
		int num2 = (int)Mathf.Sign(num - component.Index);
		if (num - component.Index != 0)
		{
			if (Mathf.Abs(num - component.Index) > 1)
			{
				num2 *= -1;
			}
			Select();
			managerParent.Deselect();
			HandleHorizontalAxisInput(num2);
		}
	}

	private void SetStartValues()
	{
		List<GuiList.TweenValue> tweenValues = ElementList.GetTweenValues();
		float num = buttons.Count / 2;
		for (int i = 0; i < buttons.Count; i++)
		{
			StatisticsPageWeapon component = buttons[i].Object.GetComponent<StatisticsPageWeapon>();
			component.SetButtonList(ElementList);
			component.Initialize();
			buttons[i].Object.transform.localPosition = tweenValues[buttons[i].position].position;
			component.SetAlpha(tweenValues[buttons[i].position].Alpha);
			component.SetOnPressedCallback(OnTouchButtonPressed);
			if ((float)buttons[i].position == num)
			{
				buttons[i].Object.SetActive(true);
				buttons[i].Object.GetComponent<StatisticsPageWeapon>().SetAlpha(1f);
				buttons[i].Object.GetComponent<StatisticsPageWeapon>().SetMaxScale();
			}
			else
			{
				buttons[i].Object.GetComponent<StatisticsPageWeapon>().SetStartScale();
				buttons[i].Object.transform.localPosition = tweenValues[buttons[i].position].position * 2f;
				buttons[i].Object.GetComponent<StatisticsPageWeapon>().Tween(tweenValues[buttons[i].position].position, TweenDirection.Left, 0, tweenValues[buttons[i].position].Alpha, 0.3f);
			}
		}
		TitleBar.gameObject.SetActive(true);
	}

	public void InitializeButtonData()
	{
		MetaManager instance = MetaManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		for (int i = 0; i < weaponsDisplayData.Length; i++)
		{
			ElementData elementData = new ElementData();
			WeaponStatisticsDisplayData weaponStatisticsDisplayData = weaponsDisplayData[i];
			int num = instance.WeaponLevel(weaponStatisticsDisplayData.Name);
			bool flag = true;
			if (instance.IsWeaponUnlocked(weaponStatisticsDisplayData.Name))
			{
				flag = false;
			}
			string text = string.Empty;
			if (!weaponStatisticsDisplayData.unlockedFromStart && flag)
			{
				text = "[666666]";
				elementData.isLocked = true;
				if (weaponStatisticsDisplayData.HiddenIfLocked)
				{
					continue;
				}
			}
			int weaponMaxLevel = instance.WeaponMaxLevel;
			if (num < weaponMaxLevel)
			{
				float num2 = instance.WeaponExperienceByLevel(num + 1);
				float num3 = instance.WeaponTotalExperience(weaponStatisticsDisplayData.Name);
				float num4 = instance.WeaponExperienceByLevel(num);
				float xpFraction = (num3 - num4) / (num2 - num4);
				elementData.XpFraction = xpFraction;
				elementData.XpFrom = num3 - num4;
				elementData.XpTo = num2 - num4;
			}
			else
			{
				elementData.XpFraction = 1f;
				elementData.XpFrom = 0f;
				elementData.XpTo = 0f;
			}
			elementData.Level = num;
			string text2 = string.Empty;
			for (int j = 0; j < weaponStatisticsDisplayData.DescriptionLines.Count; j++)
			{
				if (!string.IsNullOrEmpty(weaponStatisticsDisplayData.DescriptionLines[j]))
				{
					text2 = text2 + weaponStatisticsDisplayData.DescriptionLines[j] + '\n';
				}
			}
			elementData.Name = weaponStatisticsDisplayData.Name;
			elementData.Description = text + text2;
			elementData.TitleDescription = text + Localizer.GetString("gui_stats_weapon_description_title");
			elementData.Title = text + Localizer.GetString(weaponStatisticsDisplayData.DisplayName) + " " + Localizer.GetString("gui_stats_weapon_level") + " " + num;
			elementData.Price = text + Localizer.GetString("gui_stats_weapon_price") + " \ufffd" + weaponStatisticsDisplayData.Price;
			elementData.Perk = text + Localizer.GetString("gui_stats_weapon_perk") + " " + Localizer.GetString(GetPerkLocalizerKey(weaponStatisticsDisplayData.Perk));
			elementData.Image = weaponStatisticsDisplayData.Image;
			elementData.Damage = text + Localizer.GetString("gui_stats_weapon_damage") + " " + weaponStatisticsDisplayData.Damage;
			if (weaponStatisticsDisplayData.CanDoHeadshot)
			{
				elementData.HeadshotDamage = text + Localizer.GetString("gui_stats_weapon_headshot") + " " + weaponStatisticsDisplayData.HeadshotDamage;
			}
			else
			{
				elementData.HeadshotDamage = string.Empty;
			}
			elementDataList.Add(elementData);
		}
	}

	private void UpdateButtonElementsData()
	{
		int num = 0;
		num = (isDirectionNegative ? (buttons.Count - 1) : 0);
		StatisticsPageWeapon component = buttons[0].Object.GetComponent<StatisticsPageWeapon>();
		StatisticsPageWeapon component2 = buttons[0].Object.GetComponent<StatisticsPageWeapon>();
		for (int i = 0; i < buttons.Count; i++)
		{
			if (buttons[i].position == num)
			{
				component = buttons[i].Object.GetComponent<StatisticsPageWeapon>();
				break;
			}
		}
		if (num == 0)
		{
			for (int j = 0; j < buttons.Count; j++)
			{
				if (buttons[j].position == 1)
				{
					component2 = buttons[j].Object.GetComponent<StatisticsPageWeapon>();
					if (component2.Index - 1 >= 0)
					{
						component.SetIndex(component2.Index - 1);
						component.CustomButtonData = component2.Index - 1;
					}
					else
					{
						component.CustomButtonData = elementDataList.Count - 1;
						component.SetIndex(elementDataList.Count - 1);
					}
					break;
				}
			}
		}
		else
		{
			for (int k = 0; k < buttons.Count; k++)
			{
				if (buttons[k].position == buttons.Count - 2)
				{
					component2 = buttons[k].Object.GetComponent<StatisticsPageWeapon>();
					if (component2.Index + 1 <= elementDataList.Count - 1)
					{
						component.CustomButtonData = component2.Index + 1;
						component.SetIndex(component2.Index + 1);
					}
					else
					{
						component.CustomButtonData = 0;
						component.SetIndex(0);
					}
					break;
				}
			}
		}
		ElementData elementData = elementDataList[component.Index];
		if (elementData != null)
		{
			component.Name = elementData.Name;
			component.SetXP(elementData.XpFraction);
			component.SetXpText((int)elementData.XpFrom, (int)elementData.XpTo);
			component.SetLock(elementData.isLocked);
			component.SetDescription(elementData.Description);
			component.SetText(elementData.Title);
			component.SetDescriptionTitle(elementData.TitleDescription);
			component.SetPrice(elementData.Price);
			component.SetWeaponDamage(elementData.Damage);
			component.SetPerk(elementData.Perk);
			component.SetImage(elementData.Image);
			component.SetHeadshotDamage(elementData.HeadshotDamage);
		}
	}

	public void SetWeaponButtonData(int indexOffset)
	{
		MetaManager instance = MetaManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		int num = buttons.Count / 2;
		int num2 = indexOffset - 1;
		int num3 = num;
		for (int i = 0; i < buttons.Count; i++)
		{
			if (buttons[i] == null)
			{
				continue;
			}
			if (num3 >= num)
			{
				num2 = ++num2 % elementDataList.Count;
			}
			else
			{
				num2 = (indexOffset - (num - num3)) % elementDataList.Count;
				if (num2 < 0)
				{
					num2 += elementDataList.Count;
				}
			}
			ElementData elementData = elementDataList[num2];
			StatisticsPageWeapon component = buttons[num3].Object.GetComponent<StatisticsPageWeapon>();
			component.SetXP(elementData.XpFraction);
			component.SetXpText((int)elementData.XpFrom, (int)elementData.XpTo);
			component.SetButtonList(ElementList);
			if (elementData.isLocked)
			{
				component.SetLock(true);
			}
			component.Name = elementData.Name;
			component.SetIndex(num2);
			component.CustomButtonData = num2;
			component.SetDescription(elementData.Description);
			component.SetText(elementData.Title);
			component.SetDescriptionTitle(elementData.TitleDescription);
			component.SetPrice(elementData.Price);
			component.SetWeaponDamage(elementData.Damage);
			component.SetPerk(elementData.Perk);
			component.SetImage(elementData.Image);
			component.SetHeadshotDamage(elementData.HeadshotDamage);
			num3 = ++num3 % buttons.Count;
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
			result = "gui_support_specialist";
			break;
		case Perk.None:
			result = string.Empty;
			break;
		}
		return result;
	}

	public override void TweenButton(GameObject button, Vector3 position, TweenDirection direction, int tweenDistance, float toAlpha, float tweenSpeed)
	{
		if (direction == TweenDirection.Left)
		{
			isDirectionNegative = true;
		}
		else
		{
			isDirectionNegative = false;
		}
		if (toAlpha == 1f)
		{
			managerParent.PlaySound(GuiMenuManager.Sound.CharButton);
		}
		button.GetComponent<GuiListButton>().Tween(position, direction, tweenDistance, toAlpha, tweenSpeed);
	}

	public override void OnTweenComplete()
	{
		GameObject @object = ElementList.CurrentSelected().Object;
		if (@object != null)
		{
			StatisticsPageWeapon component = @object.GetComponent<StatisticsPageWeapon>();
			if (component != null)
			{
				TitleBar.SetText(component.Name);
			}
		}
		UpdateButtonElementsData();
	}

	public override void HandleHorizontalAxisInput(int axis)
	{
		if (ElementList != null && axis != 0)
		{
			ElementList.SetSelection(axis);
		}
		TitleBar.OnHorizontalInput(axis);
	}

	public override void HandleVerticalAxisInput(int axis)
	{
		if (axis != 0 && axis >= 1 && managerParent != null)
		{
			managerParent.Select();
			Deselect();
		}
	}

	private void HandleInput()
	{
		int @int = InputManager.GetInt(InputAction.MenuHorizontal, InputAction.MenuHorizontalAlternate);
		int int2 = InputManager.GetInt(InputAction.MenuVertical, InputAction.MenuVerticalAlternate);
		bool @bool = InputManager.GetBool(InputAction.MenuBackward);
		if (@int == 0)
		{
			@int = titleBarButtons.GetInt();
		}
		HandleHorizontalAxisInput(@int);
		HandleVerticalAxisInput(int2);
		if (@bool)
		{
			managerParent.PlaySound(GuiMenuManager.Sound.Backward);
			managerParent.TweenOut();
		}
	}

	private void Update()
	{
		titleBarButtons.Update();
		HandleInput();
	}
}
