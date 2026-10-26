using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class StatisticsWindowPerk : MenuBase
{
	private class ElementData
	{
		public string Name;

		public string Title;

		public string Image;

		public string Description;

		public Perk Perk;

		public float XpFraction;

		public float XpFrom;

		public float XpTo;
	}

	public GuiList ElementList;

	public MenuTitleBar TitleBar;

	private PerkStatisticsDisplayData[] perkDisplayData;

	private List<GuiList.ListButton> buttons;

	private bool isSelected;

	private bool isDirectionNegative;

	private MenuStatistics managerParent;

	private List<ElementData> elementDataList;

	private AxisTouchButtonsWrapper titleBarButtons;

	private void Start()
	{
		if (!(managerParent != null))
		{
			return;
		}
		StatisticsPerkDefinitions statisticsPerk = managerParent.StatisticsPerk;
		if (statisticsPerk != null)
		{
			elementDataList = new List<ElementData>();
			perkDisplayData = statisticsPerk.PerkData;
			buttons = ElementList.Create(2, 5, 0.5f);
			ElementList.SetManagerReference(this);
			ElementList.AdjustButtonPositions();
			SetStartValues();
			InitializeButtonData();
			SetButtonData(0);
			GameObject @object = ElementList.CurrentSelected().Object;
			if (@object != null)
			{
				StatisticsPagePerk component = @object.GetComponent<StatisticsPagePerk>();
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
				buttons[i].Object.GetComponent<StatisticsPagePerk>().TweenAlpha(0f, 0.3f);
			}
			else
			{
				buttons[i].Object.GetComponent<StatisticsPagePerk>().Tween(tweenValues[buttons[i].position].position * 2f, TweenDirection.Left, 0, 0f, 0.3f);
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
			StatisticsPagePerk component = buttons[i].Object.GetComponent<StatisticsPagePerk>();
			component.SetButtonList(ElementList);
			component.Initialize();
			buttons[i].Object.transform.localPosition = tweenValues[buttons[i].position].position;
			component.SetAlpha(tweenValues[buttons[i].position].Alpha);
			component.SetOnPressedCallback(OnTouchButtonPressed);
			if ((float)buttons[i].position == num)
			{
				buttons[i].Object.SetActive(true);
				component.SetAlpha(1f);
				component.SetMaxScale();
			}
			else
			{
				buttons[i].Object.transform.localPosition = tweenValues[buttons[i].position].position * 2f;
				component.Tween(tweenValues[buttons[i].position].position, TweenDirection.Left, 0, tweenValues[buttons[i].position].Alpha, 0.3f);
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
		for (int i = 0; i < perkDisplayData.Length; i++)
		{
			ElementData elementData = new ElementData();
			PerkStatisticsDisplayData perkStatisticsDisplayData = perkDisplayData[i];
			int num = instance.PerkLevel(perkStatisticsDisplayData.Perk);
			int perkMaxLevel = instance.PerkMaxLevel;
			if (num < perkMaxLevel)
			{
				float num2 = instance.PerkExperienceByLevel(num + 1);
				float num3 = instance.PerkTotalExperience(perkStatisticsDisplayData.Perk);
				float num4 = instance.PerkExperienceByLevel(num);
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
			elementData.Perk = perkStatisticsDisplayData.Perk;
			elementData.Name = Localizer.GetString(perkStatisticsDisplayData.DisplayName);
			elementData.Image = perkStatisticsDisplayData.Image;
			string empty = string.Empty;
			empty = empty + Localizer.GetString("gui_stats_perk_ability_title") + '\n';
			for (int j = 0; j < perkStatisticsDisplayData.AbilityLines.Count; j++)
			{
				if (perkStatisticsDisplayData.AbilityLines[j] != null)
				{
					empty = empty + perkStatisticsDisplayData.AbilityLines[j] + '\n';
				}
			}
			string text = empty;
			empty = text + '\n' + Localizer.GetString("gui_stats_perk_bonus_title") + '\n';
			for (int k = 0; k < perkStatisticsDisplayData.AbilityLines.Count; k++)
			{
				if (perkStatisticsDisplayData.DescriptionLines[k] != null)
				{
					empty = empty + perkStatisticsDisplayData.DescriptionLines[k] + '\n';
				}
			}
			text = empty;
			empty = text + '\n' + Localizer.GetString("gui_stats_perk_advanced_title") + '\n';
			for (int l = 0; l < perkStatisticsDisplayData.AbilityLines.Count; l++)
			{
				if (perkStatisticsDisplayData.AdvancedDescriptionLines[l] != null)
				{
					empty = empty + perkStatisticsDisplayData.AdvancedDescriptionLines[l] + '\n';
				}
			}
			elementData.Description = empty;
			elementData.Title = Localizer.GetString(perkStatisticsDisplayData.DisplayName) + " " + Localizer.GetString("gui_stats_weapon_level") + " " + num;
			elementDataList.Add(elementData);
		}
	}

	private void UpdateButtonElementsData()
	{
		int num = 0;
		num = (isDirectionNegative ? (buttons.Count - 1) : 0);
		StatisticsPagePerk component = buttons[0].Object.GetComponent<StatisticsPagePerk>();
		StatisticsPagePerk component2 = buttons[0].Object.GetComponent<StatisticsPagePerk>();
		for (int i = 0; i < buttons.Count; i++)
		{
			if (buttons[i].position == num)
			{
				component = buttons[i].Object.GetComponent<StatisticsPagePerk>();
				break;
			}
		}
		if (num == 0)
		{
			for (int j = 0; j < buttons.Count; j++)
			{
				if (buttons[j].position == 1)
				{
					component2 = buttons[j].Object.GetComponent<StatisticsPagePerk>();
					if (component2.Index - 1 >= 0)
					{
						component.CustomButtonData = component2.Index - 1;
						component.SetIndex(component2.Index - 1);
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
					component2 = buttons[k].Object.GetComponent<StatisticsPagePerk>();
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
			component.SetAbilityTitle(elementData.Description);
			component.SetText(elementData.Title);
			component.SetImage(elementData.Image);
			component.SetXP(elementData.XpFraction);
			component.SetXpText((int)elementData.XpFrom, (int)elementData.XpTo);
		}
	}

	public void SetButtonData(int indexOffset)
	{
		MetaManager instance = MetaManager.Instance;
		if (!(managerParent != null))
		{
			return;
		}
		PerkMenuDefinitions perkMenuDefines = managerParent.PerkMenuDefines;
		if (!(instance != null) || !(perkMenuDefines != null) || perkMenuDefines.PerkData.Length <= 0)
		{
			return;
		}
		int num = buttons.Count / 2;
		int num2 = indexOffset - 1;
		int num3 = num;
		bool flag = false;
		for (int i = 0; i < buttons.Count; i++)
		{
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
			StatisticsPagePerk component = buttons[num3].Object.GetComponent<StatisticsPagePerk>();
			ElementData elementData = elementDataList[num2];
			int num4 = instance.PerkLevel(elementData.Perk);
			component.SetXP(elementData.XpFraction);
			component.SetXpText((int)elementData.XpFrom, (int)elementData.XpTo);
			component.SetIndex(num2);
			component.CustomButtonData = num2;
			component.SetButtonList(ElementList);
			component.Name = elementData.Name;
			component.SetImage(elementData.Image);
			if (!flag)
			{
				component.SetAbilityTitle(elementData.Description);
				component.SetText(elementData.Name + " " + Localizer.GetString("gui_stats_weapon_level") + " " + num4);
			}
			else
			{
				component.SetAbilityTitle("[-][-][-][-][666666]" + elementData.Description + "[-]");
				component.SetText("[-][-][-][-][66666]" + elementData.Image + " " + Localizer.GetString("gui_stats_weapon_level") + " " + num4 + "[-]");
			}
			num3 = ++num3 % buttons.Count;
		}
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
			StatisticsPagePerk component = @object.GetComponent<StatisticsPagePerk>();
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
