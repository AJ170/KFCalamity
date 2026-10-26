using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class MenuOptions : MenuBase
{
	[Serializable]
	public class OptionsBar
	{
		public string EditorName;

		public OptionTypes Option;

		public MenuCharacterTitleBar Bar;

		public bool Enabled = true;

		public int MinValue;

		public int MaxValue = 10;

		public bool IsSlideBar;

		public Vector2 SortValue;

		[HideInInspector]
		public int CurrentValue;

		[HideInInspector]
		public AxisTouchButtonsWrapper TouchButtons;
	}

	[Serializable]
	public class ControlInput
	{
		public string GuiKey;

		public InputMode InputMode;

		public string Image;
	}

	public enum OptionTypes
	{
		Music = 0,
		SFX = 1,
		Resolution = 2,
		Brightness = 3,
		StickType = 4,
		Overlay = 5,
		Language = 6
	}

	public TweenPosition PositionTween;

	public List<ControlInput> Controls;

	public UISprite TextureControls;

	public GameObject ConfirmWindow;

	public UILinkController ControllerLinkMenu;

	public GameObject ApplyButton;

	public GameObject BackButton;

	public GameObject RestoreDefaultsButton;

	public GameObject SettingsSaved;

	public GameObject ToggleOverlayCheckBox;

	public OptionsBar[] OptionBars;

	[Header("Resolution Support")]
	[Space(7f)]
	public bool Support16_9;

	public bool Support16_10;

	public bool Support4_3;

	private PopupWindow windowPopup;

	private string currentLanguage;

	private string[] allLanguages;

	private int listIndex;

	private int controlInputIndex;

	private float originalBrightness = 1f;

	private bool handleAxisInput;

	private bool handleInput;

	private bool hasEntered;

	private bool goingForward;

	private bool visibleConfirmWindow;

	private bool linkMenuOpen;

	private bool showOverlay;

	private bool togglePause;

	private bool hasChangeBeenMade;

	private float labelSaveSettingsTimer;

	private List<Resolution> supportedResolutions = new List<Resolution>();

	private UIAnchor[] anchors;

	private OptionManager optionManager;

	private OptionTypes selectedOption;

	private List<int> startValues = new List<int>();

	private OptionsBar ActivatedBar;

	private Vector2 lowestSortValues;

	private Vector2 highestSortValues;

	private void SetOption(OptionTypes option)
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(option);
		if (optionsBarForOption == null)
		{
			return;
		}
		switch (option)
		{
		case OptionTypes.Music:
			optionManager.MusicVolume = (float)optionsBarForOption.CurrentValue * 0.1f;
			break;
		case OptionTypes.SFX:
			optionManager.SfxVolume = (float)optionsBarForOption.CurrentValue * 0.1f;
			break;
		case OptionTypes.Resolution:
			if (optionsBarForOption.CurrentValue > -1)
			{
				optionManager.CurrentResolution = supportedResolutions[optionsBarForOption.CurrentValue];
			}
			break;
		case OptionTypes.Brightness:
			optionManager.Brightness = (float)optionsBarForOption.CurrentValue * 0.1f;
			break;
		case OptionTypes.Overlay:
			optionManager.ShowOverlay = showOverlay;
			break;
		case OptionTypes.StickType:
			optionManager.StickType = (TouchStickType)optionsBarForOption.CurrentValue;
			break;
		}
	}

	private void Awake()
	{
		if (OptionBars.Length > 0)
		{
			lowestSortValues = OptionBars[0].SortValue;
			highestSortValues = lowestSortValues;
		}
		OptionsBar[] optionBars = OptionBars;
		foreach (OptionsBar optionsBar in optionBars)
		{
			optionsBar.Bar.enabled = optionsBar.Enabled;
			if (optionsBar.SortValue.x < lowestSortValues.x)
			{
				lowestSortValues.x = optionsBar.SortValue.x;
			}
			else if (optionsBar.SortValue.x > highestSortValues.x)
			{
				highestSortValues.x = optionsBar.SortValue.x;
			}
			if (optionsBar.SortValue.y < lowestSortValues.y)
			{
				lowestSortValues.y = optionsBar.SortValue.y;
			}
			else if (optionsBar.SortValue.y > highestSortValues.y)
			{
				highestSortValues.y = optionsBar.SortValue.y;
			}
		}
	}

	private void Start()
	{
		optionManager = OptionManager.Instance;
		if (HasOptionsBarForOption(OptionTypes.Resolution))
		{
			InitResolutionBarValue();
		}
		InitLanguageBarValues();
		LoadSettings();
		OptionsBar optionsBarForOption = GetOptionsBarForOption(selectedOption);
		optionsBarForOption.Bar.OnFocusGained();
		TweenIn();
		anchors = GetComponentsInChildren<UIAnchor>();
		TweenPosition positionTween = PositionTween;
		positionTween.onFinished = (UITweener.OnFinished)Delegate.Combine(positionTween.onFinished, new UITweener.OnFinished(OnPositionTweenFinished));
		ButtonActions componentInChildren = ApplyButton.GetComponentInChildren<ButtonActions>();
		componentInChildren.SetOnReleaseCallback(OnApplySettings);
		ButtonActions componentInChildren2 = BackButton.GetComponentInChildren<ButtonActions>();
		componentInChildren2.SetOnReleaseCallback(OnBackPressed);
		ButtonActions component = ToggleOverlayCheckBox.GetComponent<ButtonActions>();
		component.SetOnReleaseCallback(OnToggleOverlayCheckBoxClicked);
		ButtonActions componentInChildren3 = RestoreDefaultsButton.GetComponentInChildren<ButtonActions>();
		componentInChildren3.SetOnReleaseCallback(OnRestoreDefaultsPressed);
		OptionsBar[] optionBars = OptionBars;
		foreach (OptionsBar optionsBar in optionBars)
		{
			if (optionsBar.IsSlideBar)
			{
				optionsBar.TouchButtons = new AxisTouchButtonsWrapper(optionsBar.Bar.LeftArrow.gameObject, optionsBar.Bar.RightArrow.gameObject);
			}
			startValues.Add(optionsBar.CurrentValue);
		}
		DeviceSwitchController.SubscribeToSwitch(this, OnDeviceModeChanged);
	}

	private void OnDestroy()
	{
		DeviceSwitchController.UnsubscribeToSwitch(this);
	}

	private void OnDeviceModeChanged(DeviceSwitchController.SwitchArgs args)
	{
		if (linkMenuOpen && args.NewDeviceMode == DeviceMode.Touch)
		{
			InputManager.Instance.CancelLinkProcess();
		}
	}

	private OptionsBar GetOptionsBarForOption(OptionTypes option)
	{
		return Array.Find(OptionBars, (OptionsBar a) => a.Option == option);
	}

	private bool HasOptionsBarForOption(OptionTypes option)
	{
		return Array.Exists(OptionBars, (OptionsBar a) => a.Option == option);
	}

	private void OnRestoreDefaultsPressed(ButtonActions sender)
	{
		OptionManager.Instance.RestoreDefaults();
		LoadSettings();
		startValues.Clear();
		startValues.AddRange(Array.ConvertAll(OptionBars, (OptionsBar a) => a.CurrentValue));
		ShowOptionsMessageLabel("gui_restore_defaults_message");
	}

	private void OnPositionTweenFinished(UITweener tweener)
	{
	}

	private void OnBackPressed(ButtonActions sender)
	{
		HandleBackButtonPressed();
	}

	private void OnSaveSettingsFadedInComplete()
	{
		labelSaveSettingsTimer = 1f;
	}

	private void OnSaveSettingsFadeOutComplete()
	{
		UILabel component = SettingsSaved.GetComponent<UILabel>();
		component.enabled = false;
	}

	private void ShowOptionsMessageLabel(string messageLocalizerKey)
	{
		UILabel component = SettingsSaved.GetComponent<UILabel>();
		component.text = Localizer.GetString(messageLocalizerKey);
		component.enabled = true;
		TweenAlpha component2 = SettingsSaved.GetComponent<TweenAlpha>();
		component2.callWhenFinished = "OnSaveSettingsFadedInComplete";
		component2.Play(true);
	}

	private void OnApplySettings(ButtonActions sender)
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(OptionTypes.Resolution);
		if (optionsBarForOption != null && optionsBarForOption.CurrentValue > -1)
		{
			Resolution resolution = supportedResolutions[optionsBarForOption.CurrentValue];
			if (resolution.width != Screen.width || resolution.height != Screen.height)
			{
				Screen.SetResolution(resolution.width, resolution.height, true);
			}
		}
		SaveSettings();
		ShowOptionsMessageLabel("gui_option_settings_saved");
		startValues.Clear();
		startValues.AddRange(Array.ConvertAll(OptionBars, (OptionsBar a) => a.CurrentValue));
	}

	private bool HasChanges()
	{
		for (int i = 0; i < OptionBars.Length; i++)
		{
			if (OptionBars[i].CurrentValue != startValues[i])
			{
				return true;
			}
		}
		return false;
	}

	private void GetAvailableResolutions()
	{
		List<Resolution> list = new List<Resolution>(Screen.resolutions);
		double num = 1.6;
		double num2 = 1.7777777777777777;
		double num3 = 1.3333333333333333;
		foreach (Resolution item in list)
		{
			double num4 = (double)item.width / (double)item.height;
			if (num4 == num && Support16_10)
			{
				supportedResolutions.Add(item);
			}
			else if (num4 == num2 && Support16_9)
			{
				supportedResolutions.Add(item);
			}
			else if (num4 == num3 && Support4_3)
			{
				supportedResolutions.Add(item);
			}
		}
	}

	private void InitLanguageBarValues()
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(OptionTypes.Language);
		if (optionsBarForOption != null)
		{
			allLanguages = Localizer.Instance.LanguageDisplayNames;
			optionsBarForOption.CurrentValue = Array.FindIndex(allLanguages, (string a) => a == Localizer.Instance.CurrentLanguage);
			optionsBarForOption.Bar.SetText(allLanguages[optionsBarForOption.CurrentValue]);
			optionsBarForOption.MaxValue = allLanguages.Length;
		}
	}

	private void InitResolutionBarValue()
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(OptionTypes.Resolution);
		if (optionsBarForOption != null)
		{
			GetAvailableResolutions();
			optionsBarForOption.CurrentValue = supportedResolutions.FindIndex((Resolution a) => a.width == Screen.width && a.height == Screen.height);
			if (optionsBarForOption.CurrentValue > -1)
			{
				Resolution resolution = supportedResolutions[optionsBarForOption.CurrentValue];
				optionsBarForOption.Bar.SetText(resolution.width + "x" + resolution.height);
			}
			else
			{
				optionsBarForOption.Bar.SetText(Screen.width + "x" + Screen.height);
			}
		}
	}

	private bool ResetIfPossible()
	{
		if (!ControllerLinkMenu.gameObject.activeSelf)
		{
			linkMenuOpen = false;
			PositionTween.callWhenFinished = "TweenComplete";
			TweenIn();
			return true;
		}
		return false;
	}

	private void Update()
	{
		OptionsBar[] optionBars = OptionBars;
		foreach (OptionsBar optionsBar in optionBars)
		{
			if (optionsBar.TouchButtons != null)
			{
				optionsBar.TouchButtons.Update();
			}
		}
		if (labelSaveSettingsTimer > 0f)
		{
			labelSaveSettingsTimer -= Time.deltaTime;
			if (labelSaveSettingsTimer <= 0f)
			{
				TweenAlpha component = SettingsSaved.GetComponent<TweenAlpha>();
				component.callWhenFinished = "OnSaveSettingsFadeOutComplete";
				component.Play(false);
			}
		}
		if (linkMenuOpen)
		{
			ResetIfPossible();
		}
		HandleInput();
		if (visibleConfirmWindow && windowPopup.HasInputBeenMade())
		{
			if (windowPopup.MessageConfirmed())
			{
				PositionTween.callWhenFinished = "OnEnterComplete";
				RemoveConfirmWindow();
				optionManager.LoadSettings();
				HandleButtonBackwardPressed();
			}
			else
			{
				RemoveConfirmWindow();
				TweenIn();
			}
			manager.PlaySound(GuiMenuManager.Sound.Forward);
		}
	}

	public void TweenIn()
	{
		handleInput = false;
		PositionTween.enabled = true;
		PositionTween.Play(true);
	}

	public void TweenOut()
	{
		handleInput = false;
		PositionTween.Play(false);
		ToggleAnchors(false);
	}

	private void ToggleAnchors(bool active)
	{
		UIAnchor[] array = anchors;
		foreach (UIAnchor uIAnchor in array)
		{
			uIAnchor.enabled = active;
		}
	}

	public void OnEnterComplete()
	{
		if (!hasEntered)
		{
			handleInput = true;
			handleAxisInput = true;
			hasEntered = true;
			ToggleAnchors(true);
		}
		else
		{
			manager.LoadState(MenuStates.Main);
		}
	}

	public void TweenComplete()
	{
		if (handleInput)
		{
			handleInput = false;
		}
		else
		{
			handleInput = true;
		}
	}

	public override void SetManagerReference(GuiMenuManager manager)
	{
		base.SetManagerReference(manager);
	}

	private InputMode GetCurrentControlSetting()
	{
		for (int i = 0; i < Controls.Count; i++)
		{
			if (i == controlInputIndex)
			{
				return Controls[i].InputMode;
			}
		}
		MonoBehaviour.print("[Menu Options] - Could not locate current controlSetting");
		return InputMode.Normal;
	}

	private void SetControlsSetting(InputMode mode)
	{
		for (int i = 0; i < Controls.Count; i++)
		{
			if (Controls[i].InputMode == mode)
			{
				controlInputIndex = i;
				if (TextureControls != null)
				{
					TextureControls.spriteName = Controls[i].Image;
				}
				break;
			}
		}
	}

	private void OnResolutionChange(int axis)
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(OptionTypes.Resolution);
		if ((optionsBarForOption.CurrentValue != 0 || axis >= 0) && (optionsBarForOption.CurrentValue != supportedResolutions.Count - 1 || axis <= 0))
		{
			optionsBarForOption.CurrentValue += axis;
			Resolution resolution = supportedResolutions[optionsBarForOption.CurrentValue];
			optionsBarForOption.Bar.SetText(resolution.width + "x" + resolution.height);
		}
	}

	private void OnControlSettingChange(int axis)
	{
		if (axis > 0)
		{
			if (controlInputIndex < Controls.Count - 1)
			{
				controlInputIndex++;
			}
			else
			{
				controlInputIndex = 0;
			}
		}
		else if (controlInputIndex == 0)
		{
			controlInputIndex = Controls.Count - 1;
		}
		else
		{
			controlInputIndex--;
		}
	}

	private void OnStickTypeChange(int axis)
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(OptionTypes.StickType);
		if (axis > 0)
		{
			if (optionsBarForOption.CurrentValue < optionsBarForOption.MaxValue - 1)
			{
				optionsBarForOption.CurrentValue++;
			}
			else
			{
				optionsBarForOption.CurrentValue = 0;
			}
		}
		else if (optionsBarForOption.CurrentValue == 0)
		{
			optionsBarForOption.CurrentValue = optionsBarForOption.MaxValue - 1;
		}
		else
		{
			optionsBarForOption.CurrentValue--;
		}
		SetStickOption();
	}

	private void SetStickOption()
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(OptionTypes.StickType);
		if (optionsBarForOption != null)
		{
			switch ((TouchStickType)optionsBarForOption.CurrentValue)
			{
			case TouchStickType.Dynamic:
				optionsBarForOption.Bar.SetText(Localizer.GetString("gui_options_stick_type_dynamic"));
				break;
			case TouchStickType.Locked:
				optionsBarForOption.Bar.SetText(Localizer.GetString("gui_options_stick_type_locked"));
				break;
			}
			SetOption(OptionTypes.StickType);
		}
	}

	private void SetLanguageOption()
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(OptionTypes.Language);
		optionsBarForOption.Bar.SetText(allLanguages[optionsBarForOption.CurrentValue]);
	}

	private void LoadSettings()
	{
		SetSliderValue01(OptionTypes.Music, optionManager.MusicVolume);
		SetSliderValue01(OptionTypes.SFX, optionManager.SfxVolume);
		SetSliderValue01(OptionTypes.Brightness, optionManager.Brightness);
		SetSliderValue(OptionTypes.StickType, (int)optionManager.StickType);
		SetStickOption();
		OptionsBar optionsBarForOption = GetOptionsBarForOption(OptionTypes.Overlay);
		optionsBarForOption.CurrentValue = (optionManager.ShowOverlay ? 1 : 0);
		SetToggleOverlayCheckboxValue(optionManager.ShowOverlay);
		originalBrightness = optionManager.Brightness;
	}

	private void SaveSettings()
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(OptionTypes.Resolution);
		OptionsBar optionsBarForOption2 = GetOptionsBarForOption(OptionTypes.StickType);
		OptionsBar optionsBarForOption3 = GetOptionsBarForOption(OptionTypes.Overlay);
		if (optionsBarForOption != null && optionsBarForOption.CurrentValue > -1)
		{
			optionManager.CurrentResolution = supportedResolutions[optionsBarForOption.CurrentValue];
		}
		originalBrightness = optionManager.Brightness;
		optionManager.IsStickTypeTemporary = false;
		SetOption(OptionTypes.StickType);
		SetOption(OptionTypes.Overlay);
		optionManager.InputMode = GetCurrentControlSetting();
		optionManager.SetSaveData(optionManager.Serialize());
		optionManager.Save();
	}

	private void CreateConfirmWindow()
	{
		visibleConfirmWindow = true;
		GameObject gameObject = NGUITools.AddChild(base.gameObject, ConfirmWindow);
		windowPopup = gameObject.GetComponent<PopupWindow>();
		windowPopup.ButtonIndex = 1;
		windowPopup.SetButton1(Localizer.GetString("gui_yes"), PopupWindow.PopupState.Confirmed);
		windowPopup.SetButton2(Localizer.GetString("gui_no"), PopupWindow.PopupState.Denied);
		windowPopup.SetPopupInfo(PopupType.Question, Localizer.GetString("gui_option_exit_message"));
		windowPopup.SetManagerReference(manager);
	}

	private void RemoveConfirmWindow()
	{
		handleAxisInput = true;
		visibleConfirmWindow = false;
		UnityEngine.Object.Destroy(windowPopup.gameObject);
	}

	private void OpenLinkControllerWindow()
	{
		if (InputManager.Instance != null)
		{
			linkMenuOpen = true;
			handleInput = false;
			InputManager.Instance.StartLinkProcess(true);
		}
	}

	private void HandleBackButtonPressed()
	{
		if (HasChanges())
		{
			Invoke("CreateConfirmWindow", 0.2f);
			PositionTween.callWhenFinished = "TweenComplete";
			TweenOut();
		}
		else
		{
			PositionTween.callWhenFinished = "OnEnterComplete";
			HandleButtonBackwardPressed();
		}
	}

	private void HandleInput()
	{
		if (!visibleConfirmWindow)
		{
			if (!handleInput)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
			{
				flag = InputManager.GetBool(InputAction.MenuForward);
				flag2 = InputManager.GetBool(InputAction.MenuBackward);
			}
			flag3 = InputManager.GetBool(InputAction.MenuU);
			flag4 = InputManager.GetBool(InputAction.MenuBumperLeft);
			bool @bool = InputManager.GetBool(InputAction.MenuY);
			int num = InputManager.GetInt(InputAction.MenuHorizontal, InputAction.MenuHorizontalAlternate);
			int @int = InputManager.GetInt(InputAction.MenuVertical, InputAction.MenuVerticalAlternate);
			if (@bool)
			{
				PositionTween.callWhenFinished = "OpenLinkControllerWindow";
				TweenOut();
			}
			if (flag3)
			{
				OnApplySettings(null);
			}
			if (flag4)
			{
				OnRestoreDefaultsPressed(null);
			}
			if (flag)
			{
				OptionsBar optionsBarForOption = GetOptionsBarForOption(selectedOption);
				if (optionsBarForOption != null && optionsBarForOption.Option == OptionTypes.Overlay)
				{
					OnToggleOverlayCheckBoxClicked(null);
				}
				else if (optionsBarForOption != null && optionsBarForOption.Bar.RequiresActivation)
				{
					optionsBarForOption.Bar.Active = !optionsBarForOption.Bar.Active;
					if (optionsBarForOption.Bar.Active)
					{
						ActivatedBar = optionsBarForOption;
					}
					else
					{
						ActivatedBar = null;
					}
				}
			}
			if (flag2)
			{
				HandleBackButtonPressed();
			}
			if (num == 0)
			{
				for (int i = 0; i < OptionBars.Length; i++)
				{
					OptionsBar optionsBar = OptionBars[i];
					if (optionsBar.TouchButtons == null)
					{
						continue;
					}
					int int2 = optionsBar.TouchButtons.GetInt();
					if (int2 != 0)
					{
						if (optionsBar.Option != selectedOption)
						{
							OptionsBar optionsBarForOption2 = GetOptionsBarForOption(selectedOption);
							optionsBarForOption2.Bar.OnFocusLost2();
							selectedOption = optionsBar.Option;
							listIndex = i;
							optionsBar.Bar.OnFocusGained2();
							break;
						}
						num = int2;
					}
				}
			}
			HandleHorizontalAxisInput(num);
			HandleVerticalAxisInput(@int);
		}
		else if (handleInput && InputManager.GetBool(InputAction.MenuBackward) && HasChanges())
		{
			RemoveConfirmWindow();
			TweenIn();
		}
	}

	private void SetSliderValue01(OptionTypes option, float value)
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(option);
		SetSliderValue01(optionsBarForOption, value);
	}

	private void SetSliderValue01(OptionsBar bar, float value)
	{
		bar.CurrentValue = Mathf.CeilToInt(value * 10f);
		if (bar.Bar.Sprite != null)
		{
			bar.Bar.Sprite.fillAmount = value;
		}
	}

	private void SetSliderValue(OptionTypes option, int difference)
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(option);
		if (optionsBarForOption != null)
		{
			SetSliderValue(optionsBarForOption, optionsBarForOption.CurrentValue + difference);
		}
	}

	private void SetSliderValue(OptionsBar bar, int value)
	{
		if (value <= bar.MaxValue && value >= bar.MinValue)
		{
			bar.CurrentValue = value;
			if (bar.Bar.Sprite != null)
			{
				bar.Bar.Sprite.fillAmount = (float)bar.CurrentValue * 0.1f;
			}
		}
	}

	private void SetToggleOverlayCheckboxValue(bool value)
	{
		showOverlay = value;
		ToggleOverlayCheckBox.GetComponent<UISprite>().spriteName = ((!value) ? "checkbox" : "checkbox_checked");
	}

	private void OnToggleOverlayCheckBoxClicked(ButtonActions sender)
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(OptionTypes.Overlay);
		if (DeviceSwitchController.CurrentDeviceMode != DeviceMode.Controller && selectedOption != OptionTypes.Overlay)
		{
			OptionsBar optionsBarForOption2 = GetOptionsBarForOption(selectedOption);
			optionsBarForOption2.Bar.OnFocusLost2();
			optionsBarForOption.Bar.OnFocusGained2();
			selectedOption = OptionTypes.Overlay;
		}
		optionsBarForOption.CurrentValue = ((optionsBarForOption.CurrentValue == 0) ? 1 : 0);
		SetToggleOverlayCheckboxValue((optionsBarForOption.CurrentValue != 0) ? true : false);
		hasChangeBeenMade = true;
	}

	private void OnLanguageBarChanged(int axis)
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(OptionTypes.Language);
		if (axis > 0)
		{
			if (optionsBarForOption.CurrentValue < optionsBarForOption.MaxValue - 1)
			{
				optionsBarForOption.CurrentValue++;
			}
			else
			{
				optionsBarForOption.CurrentValue = 0;
			}
		}
		else if (optionsBarForOption.CurrentValue == 0)
		{
			optionsBarForOption.CurrentValue = optionsBarForOption.MaxValue - 1;
		}
		else
		{
			optionsBarForOption.CurrentValue--;
		}
		SetLanguageOption();
	}

	public override void HandleButtonForwardPressed()
	{
		handleInput = false;
		handleAxisInput = false;
		SaveSettings();
		manager.PlaySound(GuiMenuManager.Sound.Forward);
		TweenOut();
	}

	public override void HandleButtonBackwardPressed()
	{
		handleInput = false;
		handleAxisInput = false;
		manager.PlaySound(GuiMenuManager.Sound.Backward);
		TweenOut();
	}

	private Vector2 WrapSortPosition(Vector2 sortPos, Vector2 axis)
	{
		Vector2 vector = sortPos + axis;
		if (!DoesBarWithSortValueExist(vector))
		{
			if (!DoesBarWithSortValueExist(sortPos + new Vector2(0f, axis.y)))
			{
				vector.y = ((!(axis.y > 0f)) ? highestSortValues.y : lowestSortValues.y);
			}
			if (!DoesBarWithSortValueExist(sortPos + new Vector2(axis.x, 0f)))
			{
				vector.x = ((!(axis.x > 0f)) ? highestSortValues.x : lowestSortValues.x);
			}
			if (!DoesBarWithSortValueExist(vector))
			{
				vector = WrapSortPosition(vector, axis);
			}
		}
		return vector;
	}

	private bool DoesBarWithSortValueExist(Vector2 sortValue)
	{
		return Array.Exists(OptionBars, (OptionsBar a) => a.SortValue == sortValue);
	}

	private void ChangeSelection(Vector2 axis)
	{
		axis.y = 0f - axis.y;
		OptionsBar optionsBarForOption = GetOptionsBarForOption(selectedOption);
		Vector2 newPos = WrapSortPosition(optionsBarForOption.SortValue, axis);
		OptionsBar optionsBar = Array.Find(OptionBars, (OptionsBar a) => a.SortValue == newPos);
		if (optionsBar == null)
		{
			Debug.LogError("Could not find a bar at the sort index: " + newPos.ToString());
			return;
		}
		manager.PlaySound(GuiMenuManager.Sound.MenuButton);
		optionsBarForOption.Bar.OnFocusLost2();
		optionsBar.Bar.OnFocusGained2();
		selectedOption = optionsBar.Option;
	}

	public override void HandleVerticalAxisInput(int axis)
	{
		if (axis != 0 && handleAxisInput && ActivatedBar == null)
		{
			ChangeSelection(new Vector2(0f, axis));
		}
	}

	public override void HandleHorizontalAxisInput(int axis)
	{
		OptionsBar optionsBarForOption = GetOptionsBarForOption(selectedOption);
		if (!optionsBarForOption.Bar.Active)
		{
			if (axis != 0 && handleAxisInput)
			{
				ChangeSelection(new Vector2(axis, 0f));
			}
			return;
		}
		optionsBarForOption.Bar.OnHorizontalInput(axis);
		if (axis != 0 && handleAxisInput)
		{
			switch (selectedOption)
			{
			case OptionTypes.Music:
			case OptionTypes.SFX:
			case OptionTypes.Brightness:
				SetSliderValue(selectedOption, axis);
				SetOption(selectedOption);
				break;
			case OptionTypes.Resolution:
				OnResolutionChange(axis);
				break;
			case OptionTypes.StickType:
				OnStickTypeChange(axis);
				break;
			case OptionTypes.Language:
				OnLanguageBarChanged(axis);
				break;
			}
			manager.PlaySound(GuiMenuManager.Sound.CharButton);
		}
	}

	private void ResetDelay()
	{
		handleAxisInput = true;
	}
}
