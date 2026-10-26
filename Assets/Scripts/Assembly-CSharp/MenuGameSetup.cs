using System;
using System.Collections.Generic;
using Acheivements;
using Calamity.Input;
using UnityEngine;

public class MenuGameSetup : MenuBase
{
	[Serializable]
	public class Difficulty
	{
		public string DisplayName;

		public DifficultySetting Setting;
	}

	[Serializable]
	public class GameLength
	{
		public string DisplayName;

		public GameLengthSetting Setting;
	}

	public TweenPosition TopPositionTween;

	public TweenPosition BottomPositionTween;

	public TweenPosition AchievementsPositionTween;

	public UISprite MedalBeginner;

	public UISprite MedalBronze;

	public UISprite MedalSilver;

	public UISprite MedalGold;

	public UISprite SpeedBronze;

	public UISprite SpeedSilver;

	public UISprite SpeedGold;

	public UISprite ZedBronze;

	public UISprite ZedSilver;

	public UISprite ZedGold;

	public UISprite DocBronze;

	public UISprite DocSilver;

	public UISprite DocGold;

	public UISprite Lock;

	public UIPanel Panel;

	public GuiList LevelList;

	public UILabel TitleLevel;

	public UILabel TitleDifficulty;

	public UILabel TitleGameLength;

	public UILabel LabelDesc;

	public UILabel MedalInfoLabel;

	public UILabel TitleAchievements;

	public UILabel TitleAchievementsLevel;

	public UILabel AchivementButtonLabel;

	public MenuTitleBar LevelBar;

	public MenuTitleBar DifficultyBar;

	public MenuTitleBar GameLengthBar;

	public GameObject NextButton;

	public GameObject PrevButton;

	public TweenAlpha RightArrowTween;

	public TweenAlpha LeftArrowTween;

	private LevelDisplayData[] levelData;

	private List<GuiList.ListButton> buttons;

	private List<MenuTitleBar> TitleBars = new List<MenuTitleBar>();

	public List<GameLength> GameLengthList;

	public List<Difficulty> DifficultyList;

	private int listIndex;

	private int GameLengthIndex = 1;

	private int DifficultyIndex = 3;

	private bool handleAxisInput;

	private bool handleInput;

	private bool isDemo = true;

	private bool hasEntered;

	private bool goingForward;

	private bool hasChangeBeenMade;

	private string selectedLevel;

	private MetaManager metaManager;

	private OptionManager optionManager;

	private AxisTouchButtonsWrapper[] touchButtons;

	private TouchButtonWrapper achivementButton;

	private TouchButtonWrapper nextButton;

	private TouchButtonWrapper prevButton;

	private bool allowAchievements = true;

	private bool achivementsForward;

	private bool achievementsVisible;

	private void Awake()
	{
		if (!BuildRuntimeSettings.Current.SupportsTouch)
		{
			NextButton.SetActive(false);
			PrevButton.SetActive(false);
		}
	}

	private void Start()
	{
		metaManager = MetaManager.Instance;
		LabelDesc.lineWidth = 320;
		LabelDesc.maxLineCount = 6;
		TitleBars.Add(LevelBar);
		TitleBars.Add(DifficultyBar);
		TitleBars.Add(GameLengthBar);
		TitleBars[listIndex].OnFocusGained();
		TitleLevel.text = Localizer.GetString("gui_game_option_title");
		TitleDifficulty.text = Localizer.GetString("gui_game_difficulty");
		TitleGameLength.text = Localizer.GetString("gui_game_length");
		TitleAchievements.text = Localizer.GetString("gui_end_screen_input_achievements");
		MedalInfoLabel.text = Localizer.GetString("gui_no_medals");
		MedalInfoLabel.enabled = false;
		TitleBars[0].SetText(Localizer.GetString(DifficultyList[DifficultyIndex].DisplayName));
		TitleBars[1].SetText(Localizer.GetString(DifficultyList[DifficultyIndex].DisplayName));
		TitleBars[2].SetText(Localizer.GetString(GameLengthList[GameLengthIndex].DisplayName));
		AchievementsPositionTween.onFinished = AchivementsOnFinished;
		HideMedals();
		GuiList.ListButton listButton = LevelList.CurrentSelected();
		if (listButton != null)
		{
			LevelDisplayData buttonData = GetButtonData(listButton);
			if (buttonData != null)
			{
				TitleBars[listIndex].SetText(Localizer.GetString(buttonData.guiName));
				TitleAchievementsLevel.text = TitleBars[listIndex].Label.text;
				LabelDesc.text = Localizer.GetString(buttonData.guiDesc);
				selectedLevel = buttonData.name;
				if (isDemo && buttonData.DemoLocked)
				{
					Lock.gameObject.SetActive(true);
				}
				else
				{
					ShowMedals();
				}
				manager.SetSelectedLevel(buttonData.name);
			}
		}
		manager.SetGameMode(GameModeSetting.Survival);
		if (BuildRuntimeSettings.Current.SupportsTouch)
		{
			touchButtons = new AxisTouchButtonsWrapper[3];
			touchButtons[0] = new AxisTouchButtonsWrapper("level_left_button", "level_right_button");
			touchButtons[1] = new AxisTouchButtonsWrapper("diff_left_button", "diff_right_button");
			touchButtons[2] = new AxisTouchButtonsWrapper("game_length_left_button", "game_length_right_button");
			nextButton = new TouchButtonWrapper("next_button");
			prevButton = new TouchButtonWrapper("back_button");
			nextButton.InitializeIngame();
			prevButton.InitializeIngame();
			achivementButton = new TouchButtonWrapper("achivementButton");
			for (int i = 0; i < touchButtons.Length; i++)
			{
				touchButtons[i].InitializeIngame();
			}
			achivementButton.InitializeIngame();
		}
		TweenPosition achievementsPositionTween = AchievementsPositionTween;
		achievementsPositionTween.onFinished = (UITweener.OnFinished)Delegate.Combine(achievementsPositionTween.onFinished, new UITweener.OnFinished(OnAchievementsTweenDone));
	}

	private void OnAchievementsTweenDone(UITweener tweener)
	{
		if (tweener.tweenFactor == 0f)
		{
			NextButton.SetActive(true);
		}
	}

	private void Update()
	{
		if (BuildRuntimeSettings.Current.SupportsTouch)
		{
			prevButton.Update();
			nextButton.Update();
			achivementButton.Update();
		}
		HandleInput();
		if (BuildRuntimeSettings.Current.SupportsTouch)
		{
			for (int i = 0; i < touchButtons.Length; i++)
			{
				touchButtons[i].Update();
			}
		}
	}

	public void OnEnterComplete()
	{
		if (!hasEntered)
		{
			handleInput = true;
			handleAxisInput = true;
			hasEntered = true;
		}
		else if (goingForward)
		{
			manager.LoadState(MenuStates.CharacterSelect);
		}
		else
		{
			manager.LoadState(MenuStates.Main);
		}
	}

	private void TweenOut()
	{
		TopPositionTween.Play(false);
		BottomPositionTween.Play(false);
	}

	public void SetDemoLock(bool demo)
	{
		isDemo = demo;
	}

	private void ShowMedals()
	{
		LevelDisplayData buttonData = GetButtonData(LevelList.CurrentSelected());
		if (buttonData.name != "random")
		{
			if (metaManager.IsLevelCompleted(buttonData.name, DifficultySetting.Beginner))
			{
				MedalBeginner.spriteName = "beginner_medal";
			}
			if (metaManager.IsLevelCompleted(buttonData.name, DifficultySetting.Normal))
			{
				MedalBronze.spriteName = "bronze_medal";
			}
			if (metaManager.IsLevelCompleted(buttonData.name, DifficultySetting.Hard))
			{
				MedalSilver.spriteName = "silver_medal";
			}
			if (metaManager.IsLevelCompleted(buttonData.name, DifficultySetting.Hell))
			{
				MedalGold.spriteName = "gold_medal";
			}
			if (metaManager.IsStatAchieved(buttonData.name, AchievementId.SpeedRunner, AcheivementStatus.Gold))
			{
				SpeedGold.spriteName = "SASpeedRunnerGold";
				SpeedSilver.spriteName = "SASpeedRunnerSilver";
				SpeedBronze.spriteName = "SASpeedRunnerBronze";
			}
			else if (metaManager.IsStatAchieved(buttonData.name, AchievementId.SpeedRunner, AcheivementStatus.Silver))
			{
				SpeedSilver.spriteName = "SASpeedRunnerSilver";
				SpeedBronze.spriteName = "SASpeedRunnerBronze";
			}
			else if (metaManager.IsStatAchieved(buttonData.name, AchievementId.SpeedRunner, AcheivementStatus.Bronze))
			{
				SpeedBronze.spriteName = "SASpeedRunnerBronze";
			}
			if (metaManager.IsStatAchieved(buttonData.name, AchievementId.ZedSlayer, AcheivementStatus.Gold))
			{
				ZedGold.spriteName = "SAZedSlayerGold";
				ZedSilver.spriteName = "SAZedSlayerSilver";
				ZedBronze.spriteName = "SAZedSlayerBronze";
			}
			else if (metaManager.IsStatAchieved(buttonData.name, AchievementId.ZedSlayer, AcheivementStatus.Silver))
			{
				ZedSilver.spriteName = "SAZedSlayerSilver";
				ZedBronze.spriteName = "SAZedSlayerBronze";
			}
			else if (metaManager.IsStatAchieved(buttonData.name, AchievementId.ZedSlayer, AcheivementStatus.Bronze))
			{
				ZedBronze.spriteName = "SAZedSlayerBronze";
			}
			if (metaManager.IsStatAchieved(buttonData.name, AchievementId.MasterDoc, AcheivementStatus.Gold))
			{
				DocGold.spriteName = "SAMasterDocGold";
				DocSilver.spriteName = "SAMasterDocSilver";
				DocBronze.spriteName = "SAMasterDocBronze";
			}
			else if (metaManager.IsStatAchieved(buttonData.name, AchievementId.MasterDoc, AcheivementStatus.Silver))
			{
				DocSilver.spriteName = "SAMasterDocSilver";
				DocBronze.spriteName = "SAMasterDocBronze";
			}
			else if (metaManager.IsStatAchieved(buttonData.name, AchievementId.MasterDoc, AcheivementStatus.Bronze))
			{
				DocBronze.spriteName = "SAMasterDocBronze";
			}
			MedalBeginner.enabled = true;
			MedalBronze.enabled = true;
			MedalSilver.enabled = true;
			MedalGold.enabled = true;
			SpeedBronze.enabled = true;
			SpeedSilver.enabled = true;
			SpeedGold.enabled = true;
			ZedBronze.enabled = true;
			ZedSilver.enabled = true;
			ZedGold.enabled = true;
			DocBronze.enabled = true;
			DocSilver.enabled = true;
			DocGold.enabled = true;
		}
	}

	private void HideMedals()
	{
		MedalBeginner.spriteName = "beginner_medal_locked";
		MedalBronze.spriteName = "bronze_medal_locked";
		MedalSilver.spriteName = "silver_medal_locked";
		MedalGold.spriteName = "gold_medal_locked";
		SpeedBronze.spriteName = "SASpeedRunnerBronze_locked";
		SpeedSilver.spriteName = "SASpeedRunnerSilver_locked";
		SpeedGold.spriteName = "SASpeedRunnerGold_locked";
		ZedBronze.spriteName = "SAZedSlayerBronze_locked";
		ZedSilver.spriteName = "SAZedSlayerSilver_locked";
		ZedGold.spriteName = "SAZedSlayerGold_locked";
		DocBronze.spriteName = "SAMasterDocBronze_locked";
		DocSilver.spriteName = "SAMasterDocSilver_locked";
		DocGold.spriteName = "SAMasterDocGold_locked";
		LevelDisplayData buttonData = GetButtonData(LevelList.CurrentSelected());
		if (buttonData.name == "random")
		{
			MedalBeginner.enabled = false;
			MedalBronze.enabled = false;
			MedalSilver.enabled = false;
			MedalGold.enabled = false;
			DocBronze.enabled = false;
			DocSilver.enabled = false;
			DocGold.enabled = false;
			ZedBronze.enabled = false;
			ZedSilver.enabled = false;
			ZedGold.enabled = false;
			SpeedBronze.enabled = false;
			SpeedSilver.enabled = false;
			SpeedGold.enabled = false;
		}
		else
		{
			MedalBeginner.enabled = true;
			MedalBronze.enabled = true;
			MedalSilver.enabled = true;
			MedalGold.enabled = true;
			DocBronze.enabled = true;
			DocSilver.enabled = true;
			DocGold.enabled = true;
			ZedBronze.enabled = true;
			ZedSilver.enabled = true;
			ZedGold.enabled = true;
			SpeedBronze.enabled = true;
			SpeedSilver.enabled = true;
			SpeedGold.enabled = true;
		}
	}

	private void PopulateLevelList()
	{
		if (levelData == null)
		{
			return;
		}
		buttons = LevelList.Create(1, levelData.Length - 1, 0.5f);
		GameSettings settings = manager.Settings;
		optionManager = OptionManager.Instance;
		settings.Difficulty = optionManager.DifficultySetting;
		settings.GameLength = optionManager.GameLengthSetting;
		settings.LevelName = optionManager.LevelSetting;
		if (settings.LevelName != null && settings.LevelName != "tutorial_scene")
		{
			if (settings.LevelName == string.Empty || settings.LevelName == "random")
			{
				settings.LevelChosenAtRandom = true;
			}
			else
			{
				settings.LevelChosenAtRandom = false;
			}
			if (settings.LevelChosenAtRandom)
			{
				settings.LevelName = "random";
			}
			for (int i = 0; i < levelData.Length; i++)
			{
				if (levelData[i].name == settings.LevelName)
				{
					SetLevelButtonData(i);
					break;
				}
			}
			for (int j = 0; j < DifficultyList.Count; j++)
			{
				if (DifficultyList[j].Setting == settings.Difficulty)
				{
					DifficultyIndex = j;
					break;
				}
			}
			for (int k = 0; k < GameLengthList.Count; k++)
			{
				if (GameLengthList[k].Setting == settings.GameLength)
				{
					GameLengthIndex = k;
					break;
				}
			}
		}
		else
		{
			SetLevelButtonData(5);
		}
		LevelList.AdjustButtonPositions();
		LevelList.SetManagerReference(this);
		SetLevelListStartValues();
	}

	private void OnRightArrowReleased()
	{
		UISprite component = RightArrowTween.GetComponent<UISprite>();
		component.spriteName = "arrow_right_white_outline";
	}

	private void OnLeftArrowReleased()
	{
		UISprite component = LeftArrowTween.GetComponent<UISprite>();
		component.spriteName = "arrow_left_white_outline";
	}

	private void OnTouchInput(ButtonActions sender)
	{
		if (!handleInput || achievementsVisible)
		{
			return;
		}
		int num = (int)sender.CustomData;
		GuiList.ListButton listButton = LevelList.CurrentSelected();
		GuiListButtonLevel component = listButton.Object.GetComponent<GuiListButtonLevel>();
		int num2 = (int)Mathf.Sign(num - component.Index);
		if (num - component.Index != 0)
		{
			if (Mathf.Abs(num - component.Index) > 1)
			{
				num2 *= -1;
			}
			if (num2 > 0)
			{
				UISprite component2 = RightArrowTween.GetComponent<UISprite>();
				component2.spriteName = "arrow_right_black_outline";
				Invoke("OnRightArrowReleased", 0.1f);
			}
			else
			{
				UISprite component3 = LeftArrowTween.GetComponent<UISprite>();
				component3.spriteName = "arrow_left_black_outline";
				Invoke("OnLeftArrowReleased", 0.1f);
			}
			SetSelected(0);
			HandleHorizontalAxisInput(num2);
		}
		else
		{
			GoForward();
		}
	}

	private void SetLevelListStartValues()
	{
		List<GuiList.TweenValue> tweenValues = LevelList.GetTweenValues();
		float num = buttons.Count / 2;
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].Object.transform.localPosition = tweenValues[buttons[i].position].position;
			GuiListButtonLevel component = buttons[i].Object.GetComponent<GuiListButtonLevel>();
			component.SetAlpha(tweenValues[buttons[i].position].Alpha);
			component.SetOnPressedCallback(OnTouchInput);
			if ((float)buttons[i].position == num)
			{
				component.SetMaxScale();
			}
			else
			{
				component.SetStartScale();
			}
			if (tweenValues[buttons[i].position].Alpha == 0f)
			{
				BoxCollider component2 = buttons[i].Object.GetComponent<BoxCollider>();
				component2.enabled = false;
			}
		}
	}

	public void SetLevelButtonData(int indexOffset)
	{
		int num = buttons.Count / 2;
		int num2 = indexOffset;
		int num3 = num;
		for (int i = 0; i < buttons.Count; i++)
		{
			GuiListButtonLevel component = buttons[num3].Object.GetComponent<GuiListButtonLevel>();
			component.SetButtonList(LevelList);
			component.Name = levelData[num2].name;
			component.SetImage(levelData[num2].image);
			component.SetDepth(i + 101);
			component.CustomButtonData = num2;
			component.SetIndex(num2);
			num2 = ++num2 % buttons.Count;
			num3 = ++num3 % buttons.Count;
		}
	}

	public void SetLevelData(LevelDisplayData[] levelData)
	{
		this.levelData = levelData;
		PopulateLevelList();
	}

	public override void TweenButton(GameObject button, Vector3 position, TweenDirection direction, int tweenDistance, float toAlpha, float tweenSpeed)
	{
		Lock.gameObject.SetActive(false);
		handleInput = false;
		HideMedals();
		if (toAlpha == 1f)
		{
			manager.PlaySound(GuiMenuManager.Sound.CharButton);
		}
		BoxCollider component = button.GetComponent<BoxCollider>();
		if (component != null)
		{
			if (toAlpha > 0f)
			{
				component.enabled = true;
			}
			else
			{
				component.enabled = false;
			}
		}
		LabelDesc.text = string.Empty;
		button.GetComponent<GuiListButton>().Tween(position, direction, tweenDistance, toAlpha, tweenSpeed);
	}

	public override void SetManagerReference(GuiMenuManager manager)
	{
		base.SetManagerReference(manager);
	}

	private int HandleTouchAxis()
	{
		int num = 0;
		for (int i = 0; i < touchButtons.Length; i++)
		{
			num = touchButtons[i].GetInt();
			if (num != 0)
			{
				if (listIndex != i)
				{
					SetSelected(i);
				}
				break;
			}
		}
		return num;
	}

	private void HandleInput()
	{
		if (!handleInput)
		{
			return;
		}
		bool supportsTouch = BuildRuntimeSettings.Current.SupportsTouch;
		bool @bool = InputManager.GetBool(InputAction.MenuForward);
		bool bool2 = InputManager.GetBool(InputAction.MenuBackward);
		bool flag = InputManager.GetBool(InputAction.Grenade);
		int num = InputManager.GetInt(InputAction.MenuHorizontal, InputAction.MenuHorizontalAlternate);
		int @int = InputManager.GetInt(InputAction.MenuVertical, InputAction.MenuVerticalAlternate);
		if (num == 0)
		{
			num = (supportsTouch ? HandleTouchAxis() : 0);
		}
		if (!flag)
		{
			flag = supportsTouch && achivementButton.GetBool();
		}
		if (@bool)
		{
			HandleButtonForwardPressed();
		}
		if (bool2)
		{
			HandleButtonBackwardPressed();
		}
		if (flag)
		{
			ToggleAchievements();
		}
		if (handleAxisInput)
		{
			HandleHorizontalAxisInput(num);
			HandleVerticalAxisInput(@int);
		}
		if (BuildRuntimeSettings.Current.SupportsTouch)
		{
			if (nextButton.GetBool())
			{
				GoForward();
			}
			if (prevButton.GetBool())
			{
				HandleButtonBackwardPressed();
			}
		}
	}

	public void GoForward()
	{
		if (isDemo)
		{
			LevelDisplayData buttonData = GetButtonData(LevelList.CurrentSelected());
			if (buttonData != null)
			{
				if (!buttonData.DemoLocked)
				{
					handleInput = false;
					handleAxisInput = false;
					manager.PlaySound(GuiMenuManager.Sound.Forward);
					SaveSettings();
					goingForward = true;
					TweenOut();
				}
				else
				{
					manager.PlaySound(GuiMenuManager.Sound.UnConfim);
				}
			}
		}
		else
		{
			handleInput = false;
			handleAxisInput = false;
			manager.PlaySound(GuiMenuManager.Sound.Forward);
			SaveSettings();
			goingForward = true;
			TweenOut();
			manager.LoadState(MenuStates.CharacterSelect);
		}
	}

	public override void HandleButtonForwardPressed()
	{
		GoForward();
	}

	public override void HandleButtonBackwardPressed()
	{
		handleInput = false;
		handleAxisInput = false;
		SaveSettings();
		manager.PlaySound(GuiMenuManager.Sound.Backward);
		AchievementsPositionTween.Play(false);
		TweenOut();
	}

	private void ToggleAchievements()
	{
		achievementsVisible = !achievementsVisible;
		if (achievementsVisible)
		{
			AchievementsPositionTween.gameObject.SetActive(achievementsVisible);
		}
		AchievementsPositionTween.Play(achievementsVisible);
		if (!achievementsVisible)
		{
			AchivementButtonLabel.text = Localizer.GetString("gui_end_screen_input_medals");
		}
		else
		{
			AchivementButtonLabel.text = Localizer.GetString("gui_hide");
		}
		if (achievementsVisible)
		{
			NextButton.SetActive(false);
		}
	}

	private void AchivementsOnFinished(UITweener tween)
	{
		if (!achievementsVisible)
		{
			AchievementsPositionTween.gameObject.SetActive(false);
		}
	}

	public void SetSelected(int index)
	{
		manager.PlaySound(GuiMenuManager.Sound.MenuButton);
		TitleBars[listIndex].OnFocusLost();
		listIndex = index;
		TitleBars[listIndex].OnFocusGained();
	}

	public override void HandleVerticalAxisInput(int axis)
	{
		if (axis == 0)
		{
			return;
		}
		if (axis < 1)
		{
			if (listIndex + 1 <= TitleBars.Count - 1)
			{
				manager.PlaySound(GuiMenuManager.Sound.MenuButton);
				TitleBars[listIndex].OnFocusLost();
				listIndex++;
				TitleBars[listIndex].OnFocusGained();
			}
		}
		else if (listIndex - 1 >= 0)
		{
			manager.PlaySound(GuiMenuManager.Sound.MenuButton);
			TitleBars[listIndex].OnFocusLost();
			listIndex--;
			TitleBars[listIndex].OnFocusGained();
		}
		handleAxisInput = false;
		Invoke("ResetDelay", 0.2f);
	}

	public override void HandleHorizontalAxisInput(int axis)
	{
		TitleBars[listIndex].OnHorizontalInput(axis);
		if (axis == 0)
		{
			return;
		}
		hasChangeBeenMade = true;
		if (listIndex > 0)
		{
			manager.PlaySound(GuiMenuManager.Sound.CharButton);
		}
		if (listIndex == 0)
		{
			if (axis != 0)
			{
				LevelList.SetSelection(axis);
				LeftArrowTween.Play(true);
				RightArrowTween.Play(true);
			}
		}
		else if (listIndex == 1)
		{
			switch (axis)
			{
			case -1:
				DifficultyIndex--;
				if (DifficultyIndex < 0)
				{
					DifficultyIndex = DifficultyList.Count - 1;
				}
				break;
			case 1:
				DifficultyIndex++;
				if (DifficultyIndex > DifficultyList.Count - 1)
				{
					DifficultyIndex = 0;
				}
				break;
			}
			TitleBars[listIndex].SetText(Localizer.GetString(DifficultyList[DifficultyIndex].DisplayName));
		}
		else if (listIndex == 2)
		{
			switch (axis)
			{
			case -1:
				GameLengthIndex--;
				if (GameLengthIndex < 0)
				{
					GameLengthIndex = GameLengthList.Count - 1;
				}
				break;
			case 1:
				GameLengthIndex++;
				if (GameLengthIndex > GameLengthList.Count - 1)
				{
					GameLengthIndex = 0;
				}
				break;
			}
			TitleBars[listIndex].SetText(Localizer.GetString(GameLengthList[GameLengthIndex].DisplayName));
		}
		handleAxisInput = false;
		Invoke("ResetDelay", 0.2f);
	}

	public override void OnTweenComplete()
	{
		handleInput = true;
		LevelDisplayData buttonData = GetButtonData(LevelList.CurrentSelected());
		if (buttonData != null)
		{
			if (isDemo && buttonData.DemoLocked)
			{
				LabelDesc.color = Color.gray;
				Lock.gameObject.SetActive(true);
			}
			else
			{
				LabelDesc.color = Color.white;
				ShowMedals();
			}
			TitleBars[0].SetText(Localizer.GetString(buttonData.guiName));
			TitleAchievementsLevel.text = TitleBars[0].Label.text;
			LabelDesc.text = Localizer.GetString(buttonData.guiDesc);
			manager.SetSelectedLevel(buttonData.name);
			selectedLevel = buttonData.name;
			LeftArrowTween.Play(false);
			RightArrowTween.Play(false);
		}
	}

	public LevelDisplayData GetButtonData(GuiList.ListButton button)
	{
		GuiListButton component = button.Object.GetComponent<GuiListButton>();
		if (component != null)
		{
			for (int i = 0; i < levelData.Length; i++)
			{
				if (levelData[i].name == component.Name)
				{
					return levelData[i];
				}
			}
		}
		return null;
	}

	private void ResetDelay()
	{
		handleAxisInput = true;
	}

	private void SaveSettings()
	{
		if (hasChangeBeenMade)
		{
			manager.SetGameDiffictulty(DifficultyList[DifficultyIndex].Setting);
			manager.SetGameLength(GameLengthList[GameLengthIndex].Setting);
			optionManager = OptionManager.Instance;
			optionManager.LevelSetting = selectedLevel;
			optionManager.DifficultySetting = DifficultyList[DifficultyIndex].Setting;
			optionManager.GameLengthSetting = GameLengthList[GameLengthIndex].Setting;
			optionManager.SetSaveData(optionManager.Serialize());
			optionManager.Save();
		}
	}
}
