using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class MenuStatistics : MenuBase
{
	public enum Categories
	{
		LifeTime = 0,
		Perks = 1,
		Weapons = 2,
		None = 3
	}

	public MenuTitleBar TitleBar;

	public List<StatSetting> StatCategories;

	public TweenAlpha Alpha;

	public TweenPosition PositionTween;

	public GuiButton BackButton;

	private bool handleAxisInput = true;

	private bool handleInput = true;

	private bool isDemo = true;

	private WeaponStatisticsDisplayData[] weaponsDisplayData;

	private PerkMenuDefinitions perkMenuDefines;

	private PerkStatisticsDisplayData[] perksDisplayData;

	private int titleIndex;

	private GameObject activePrefab;

	private Categories prefabCategory;

	private static int currentFormatLevel;

	private static bool currentFormatLocked;

	private StatisticsPerkDefinitions perkDefines;

	private StatisticsWeaponDefinitions weaponDefines;

	private AxisTouchButtonsWrapper titleBarButtons;

	public StatisticsPerkDefinitions StatisticsPerk
	{
		get
		{
			return perkDefines;
		}
	}

	public StatisticsWeaponDefinitions StatisticsWeapons
	{
		get
		{
			return weaponDefines;
		}
	}

	public PerkMenuDefinitions PerkMenuDefines
	{
		get
		{
			return perkMenuDefines;
		}
		set
		{
			perkMenuDefines = value;
		}
	}

	public bool IsDemo
	{
		get
		{
			return isDemo;
		}
		set
		{
			isDemo = value;
		}
	}

	private void Start()
	{
		TitleBar.OnFocusGained();
		TitleBar.SetText(Localizer.GetString(StatCategories[titleIndex].DisplayName));
		weaponDefines = manager.StatisticsWeapons;
		perkDefines = manager.StatisticsPerk;
		if (!manager.LocalizedWeaponStats)
		{
			if (weaponDefines != null)
			{
				weaponsDisplayData = weaponDefines.WeaponData;
				for (int i = 0; i < weaponsDisplayData.Length; i++)
				{
					WeaponStatisticsDisplayData weaponStatisticsDisplayData = weaponsDisplayData[i];
					if (weaponStatisticsDisplayData == null)
					{
						continue;
					}
					if (MetaManager.Instance.IsWeaponUnlocked(weaponStatisticsDisplayData.Name))
					{
						currentFormatLocked = false;
					}
					else if (weaponStatisticsDisplayData.unlockedFromStart)
					{
						currentFormatLocked = false;
					}
					else
					{
						currentFormatLocked = true;
					}
					currentFormatLevel = MetaManager.Instance.WeaponLevel(weaponStatisticsDisplayData.Name);
					List<string> descriptionLines = weaponStatisticsDisplayData.DescriptionLines;
					for (int j = 0; j < descriptionLines.Count; j++)
					{
						if (descriptionLines[j] != null)
						{
							descriptionLines[j] = FormatString(Localizer.GetString(descriptionLines[j]));
						}
					}
				}
			}
			manager.LocalizedWeaponStats = true;
		}
		if (!manager.LocalizedPerkStats)
		{
			if (perkDefines != null)
			{
				perksDisplayData = perkDefines.PerkData;
				for (int k = 0; k < perksDisplayData.Length; k++)
				{
					PerkStatisticsDisplayData perkStatisticsDisplayData = perksDisplayData[k];
					if (perkStatisticsDisplayData == null)
					{
						continue;
					}
					currentFormatLevel = MetaManager.Instance.PerkLevel(perkStatisticsDisplayData.Perk);
					currentFormatLocked = false;
					List<string> abilityLines = perkStatisticsDisplayData.AbilityLines;
					for (int l = 0; l < abilityLines.Count; l++)
					{
						if (abilityLines[l] != null)
						{
							abilityLines[l] = FormatString(Localizer.GetString(abilityLines[l]));
						}
					}
					abilityLines = perkStatisticsDisplayData.DescriptionLines;
					for (int m = 0; m < abilityLines.Count; m++)
					{
						if (abilityLines[m] != null)
						{
							abilityLines[m] = FormatString(Localizer.GetString(abilityLines[m]));
						}
					}
					abilityLines = perkStatisticsDisplayData.AdvancedDescriptionLines;
					for (int n = 0; n < abilityLines.Count; n++)
					{
						if (abilityLines[n] != null)
						{
							abilityLines[n] = FormatString(Localizer.GetString(abilityLines[n]));
						}
					}
				}
			}
			manager.LocalizedPerkStats = true;
		}
		HandleWindowChange();
		BackButton.SetOnReleaseCallback(OnBackButtonPressed);
		titleBarButtons = new AxisTouchButtonsWrapper(TitleBar.LeftArrow.gameObject, TitleBar.RightArrow.gameObject);
	}

	public void PlaySound(GuiMenuManager.Sound type)
	{
		if (manager != null)
		{
			manager.PlaySound(type);
		}
	}

	private bool IsPerkLocked(Perk perk)
	{
		if (perkMenuDefines != null)
		{
			for (int i = 0; i < perkMenuDefines.PerkData.Length; i++)
			{
				if (perkMenuDefines.PerkData[i] != null && perkMenuDefines.PerkData[i].Perk == perk)
				{
					return perkMenuDefines.PerkData[i].DemoLocked;
				}
			}
		}
		return false;
	}

	private static string FormatString(string text)
	{
		int num = 0;
		int num2 = 0;
		num = text.IndexOf('<');
		if (num != -1)
		{
			int num3 = text.IndexOf('>');
			if (num3 != -1)
			{
				num2 = num3 - num;
				string text2 = text.Substring(num, num2 + 1);
				string newValue = FormatLine(text2.Substring(1, text2.Length - 2));
				text = text.Replace(text2, newValue);
				text = FormatString(text);
			}
		}
		return text;
	}

	private static string FormatLine(string line)
	{
		string text = line;
		string text2 = line;
		int num = line.IndexOf('=');
		if (num != -1)
		{
			text = line.Substring(0, num).Trim();
			text2 = line.Substring(num + 1);
			text2 = text2.Trim();
		}
		switch (text)
		{
		case "level":
			text2 = currentFormatLevel.ToString();
			break;
		case "levelmulti":
			text2 = (float.Parse(text2) * (float)currentFormatLevel).ToString();
			break;
		case "levelreq":
			text2 = ((int.Parse(text2) <= currentFormatLevel) ? string.Empty : "[666666]");
			break;
		case "color":
			text2 = ((!currentFormatLocked) ? ("[" + text2 + "]") : string.Empty);
			break;
		case "colorlevelreq":
		{
			string s = "0";
			string text3 = string.Empty;
			int num2 = text2.IndexOf(',');
			if (num2 != -1)
			{
				s = text2.Substring(0, num2).Trim();
				text3 = text2.Substring(num2 + 1);
				text3 = text3.Trim();
			}
			text2 = ((int.Parse(s) >= currentFormatLevel || currentFormatLocked) ? string.Empty : ("[" + text3 + "]"));
			break;
		}
		case "/colorlevelreq":
			text2 = ((int.Parse(text2) >= currentFormatLevel || currentFormatLocked) ? string.Empty : "[-]");
			break;
		case "/color":
			text2 = ((!currentFormatLocked) ? "[-]" : string.Empty);
			break;
		case "br":
			text2 = "\n";
			break;
		}
		return text2;
	}

	private void Update()
	{
		titleBarButtons.Update();
		HandleInput();
	}

	public void Select()
	{
		TitleBar.OnFocusGained();
		handleInput = true;
	}

	public void Deselect()
	{
		TitleBar.OnFocusLost();
	}

	private void OnBackButtonPressed(ButtonActions sender)
	{
		PlaySound(GuiMenuManager.Sound.Backward);
		TweenOut();
	}

	private void HandleInput()
	{
		if (handleInput)
		{
			bool @bool = InputManager.GetBool(InputAction.MenuForward);
			bool bool2 = InputManager.GetBool(InputAction.MenuBackward);
			int @int = InputManager.GetInt(InputAction.MenuHorizontal, InputAction.MenuHorizontalAlternate);
			int int2 = InputManager.GetInt(InputAction.MenuVertical, InputAction.MenuVerticalAlternate);
			if (@bool)
			{
				HandleButtonForwardPressed();
			}
			if (bool2)
			{
				PlaySound(GuiMenuManager.Sound.Backward);
				TweenOut();
			}
			HandleHorizontalAxisInput(@int);
			HandleVerticalAxisInput(int2);
		}
	}

	public override void HandleHorizontalAxisInput(int axis)
	{
		if (axis == 0)
		{
			axis = titleBarButtons.GetInt();
		}
		TitleBar.OnHorizontalInput(axis);
		if (axis == 0 || !handleAxisInput)
		{
			return;
		}
		switch (axis)
		{
		case -1:
			titleIndex--;
			if (titleIndex < 0)
			{
				titleIndex = StatCategories.Count - 1;
			}
			break;
		case 1:
			titleIndex++;
			if (titleIndex > StatCategories.Count - 1)
			{
				titleIndex = 0;
			}
			break;
		}
		if (axis != 0)
		{
		}
		PlaySound(GuiMenuManager.Sound.CharButton);
		HandleWindowChange();
		handleAxisInput = false;
		Invoke("ResetDelay", 0.3f);
	}

	public override void HandleVerticalAxisInput(int axis)
	{
		if (axis == 0 || !handleAxisInput)
		{
			return;
		}
		Categories setting = StatCategories[titleIndex].Setting;
		if (axis < 1)
		{
			switch (setting)
			{
			case Categories.Weapons:
				activePrefab.GetComponent<StatisticsWindowWeapon>().Select();
				TitleBar.OnFocusLost();
				handleInput = false;
				PlaySound(GuiMenuManager.Sound.CharButton);
				break;
			case Categories.Perks:
				activePrefab.GetComponent<StatisticsWindowPerk>().Select();
				TitleBar.OnFocusLost();
				handleInput = false;
				PlaySound(GuiMenuManager.Sound.CharButton);
				break;
			}
		}
		else
		{
			if (titleIndex != 0 && !handleInput)
			{
				PlaySound(GuiMenuManager.Sound.CharButton);
			}
			switch (setting)
			{
			case Categories.Weapons:
				handleInput = true;
				break;
			case Categories.Perks:
				handleInput = true;
				break;
			}
		}
		handleAxisInput = false;
		Invoke("ResetDelay", 0.3f);
	}

	private void HandleWindowChange()
	{
		StatSetting statSetting = StatCategories[titleIndex];
		TitleBar.SetText(Localizer.GetString(statSetting.DisplayName));
		if (activePrefab != null)
		{
			if (prefabCategory == Categories.LifeTime)
			{
				activePrefab.GetComponent<StatisticsWindowLifeTime>().TweenOut();
			}
			else if (prefabCategory == Categories.Perks)
			{
				activePrefab.GetComponent<StatisticsWindowPerk>().TweenOut();
			}
			else if (prefabCategory == Categories.Weapons)
			{
				activePrefab.GetComponent<StatisticsWindowWeapon>().TweenOut();
			}
		}
		if (statSetting.Setting == Categories.Weapons)
		{
			if (statSetting.LoadedPrefab != null)
			{
				activePrefab = statSetting.LoadedPrefab;
				activePrefab.GetComponent<StatisticsWindowWeapon>().Open();
			}
			else if (statSetting.Prefab != null)
			{
				activePrefab = NGUITools.AddChild(base.gameObject, statSetting.Prefab);
				activePrefab.GetComponent<StatisticsWindowWeapon>().SetManagerReference(this);
				statSetting.LoadedPrefab = activePrefab;
			}
			prefabCategory = Categories.Weapons;
		}
		else if (statSetting.Setting == Categories.Perks)
		{
			if (statSetting.LoadedPrefab != null)
			{
				activePrefab = statSetting.LoadedPrefab;
				activePrefab.GetComponent<StatisticsWindowPerk>().Open();
			}
			else if (statSetting.Prefab != null)
			{
				activePrefab = NGUITools.AddChild(base.gameObject, statSetting.Prefab);
				activePrefab.transform.localPosition = new Vector3(0f, 0f, 10f);
				activePrefab.GetComponent<StatisticsWindowPerk>().SetManagerReference(this);
				statSetting.LoadedPrefab = activePrefab;
			}
			prefabCategory = Categories.Perks;
		}
		else if (statSetting.Setting == Categories.LifeTime)
		{
			if (statSetting.LoadedPrefab != null)
			{
				activePrefab = statSetting.LoadedPrefab;
				activePrefab.GetComponent<StatisticsWindowLifeTime>().Open();
			}
			else if (statSetting.Prefab != null)
			{
				activePrefab = NGUITools.AddChild(base.gameObject, statSetting.Prefab);
				statSetting.LoadedPrefab = activePrefab;
			}
			prefabCategory = Categories.LifeTime;
		}
	}

	public void TweenOut()
	{
		Alpha.callWhenFinished = "LoadState";
		Alpha.Play(false);
	}

	public void LoadState()
	{
		if (manager != null)
		{
			manager.ChangeGui(MenuStates.Main);
		}
	}

	public override void OnTweenComplete()
	{
		base.OnTweenComplete();
	}

	public override void SetManagerReference(GuiMenuManager manager)
	{
		base.SetManagerReference(manager);
	}

	private void ResetDelay()
	{
		handleAxisInput = true;
	}
}
