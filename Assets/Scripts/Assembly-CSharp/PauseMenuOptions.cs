using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class PauseMenuOptions : MenuBase
{
	[Serializable]
	public class ControlInput
	{
		public string GuiKey;

		public InputMode InputMode;

		public string Image;
	}

	public TweenPosition PositionTween;

	public TweenPosition InfoPositionTween;

	public List<ControlInput> Controls;

	public UILabel LabelMusic;

	public UILabel LabelSFX;

	public UILabel LabelInput;

	public UILabel LabelTitle;

	public UILabel Olabel;

	public UILabel Alabel;

	public UILabel Ylabel;

	public UISprite SliderMusic;

	public UISprite SliderSFX;

	public UISprite TextureControls;

	public MenuCharacterTitleBar BarMusic;

	public MenuCharacterTitleBar BarSFX;

	public MenuCharacterTitleBar BarControls;

	public GameObject ConfirmWindow;

	private PauseMenuOptionsPopup windowPopup;

	private List<MenuCharacterTitleBar> sliderBars = new List<MenuCharacterTitleBar>();

	private List<UISprite> sliderSprites = new List<UISprite>();

	private int listIndex;

	private int maxVolumeIndex = 10;

	private int minVolumeIndex;

	private int musicIndex = 10;

	private int sfxIndex = 10;

	private int controlInputIndex;

	private bool handleAxisInput;

	private bool handleInput;

	private bool hasEntered;

	private bool goingForward;

	private bool visibleConfirmWindow;

	private bool hasChangeBeenMade;

	private bool initialized;

	private float delayBetweenFocusChanges = 0.15f;

	private float delayTimeStamp;

	public GuiInGameManager GuiManager;

	public PauseMenu Menu;

	private OptionManager optionManager;

	public bool IsInitialized
	{
		get
		{
			return initialized;
		}
	}

	private void Start()
	{
		if (!initialized)
		{
			Initialize();
		}
	}

	private void Initialize()
	{
		optionManager = OptionManager.Instance;
		LabelMusic.text = Localizer.GetString("gui_option_music");
		LabelSFX.text = Localizer.GetString("gui_option_sfx");
		LabelTitle.text = Localizer.GetString("gui_options");
		LabelInput.text = Localizer.GetString("gui_controls");
		Olabel.text = " - " + Localizer.GetString("gui_save");
		Alabel.text = " - " + Localizer.GetString("gui_back");
		Ylabel.text = " - " + Localizer.GetString("gui_options_controller_button");
		sliderSprites.Add(SliderMusic);
		sliderSprites.Add(SliderSFX);
		sliderBars.Add(BarMusic);
		sliderBars.Add(BarSFX);
		sliderBars.Add(BarControls);
		LoadSettings();
		sliderBars[listIndex].OnFocusGained();
		TweenIn();
		initialized = true;
	}

	private void Update()
	{
		if (!handleAxisInput)
		{
			delayTimeStamp += Time.fixedDeltaTime * 2f;
			if (delayTimeStamp >= delayBetweenFocusChanges)
			{
				delayTimeStamp = 0f;
				ResetDelay();
			}
		}
		else
		{
			HandleInput();
		}
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
			GuiManager.PlaySound(GuiInGameManager.Sound.Forward);
		}
	}

	public void TweenIn()
	{
		InfoPositionTween.enabled = true;
		InfoPositionTween.Play(true);
		PositionTween.enabled = true;
		PositionTween.Play(true);
	}

	public void TweenOut()
	{
		handleInput = false;
		InfoPositionTween.Play(false);
		PositionTween.Play(false);
	}

	public void OnEnterComplete()
	{
		if (!hasEntered)
		{
			handleInput = true;
			handleAxisInput = true;
			hasEntered = true;
		}
		else
		{
			hasEntered = false;
			Menu.Open();
			base.gameObject.SetActive(false);
		}
	}

	public void EnableInput()
	{
		handleInput = true;
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
				sliderBars[2].SetText(Localizer.GetString(Controls[i].GuiKey));
				if (TextureControls != null)
				{
					TextureControls.spriteName = Controls[i].Image;
				}
				break;
			}
		}
	}

	private void OnControlSettingChange(int axis)
	{
		hasChangeBeenMade = true;
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
		SetControlsSetting(GetCurrentControlSetting());
	}

	public void LoadSettings()
	{
		SliderSFX.fillAmount = optionManager.SfxVolume;
		SliderMusic.fillAmount = optionManager.MusicVolume;
		musicIndex = Mathf.CeilToInt(optionManager.MusicVolume * 10f);
		sfxIndex = Mathf.CeilToInt(optionManager.SfxVolume * 10f);
		SetControlsSetting(optionManager.InputMode);
	}

	private void SaveSettings()
	{
		if (optionManager == null)
		{
			optionManager = OptionManager.Instance;
		}
		optionManager.InputMode = GetCurrentControlSetting();
		optionManager.SetSaveData(optionManager.Serialize());
		optionManager.Save();
	}

	private void CreateConfirmWindow()
	{
		PositionTween.callWhenFinished = string.Empty;
		visibleConfirmWindow = true;
		GameObject gameObject = NGUITools.AddChild(base.gameObject, ConfirmWindow);
		windowPopup = gameObject.GetComponent<PauseMenuOptionsPopup>();
		windowPopup.ButtonIndex = 1;
		windowPopup.SetTextAcceptButton(Localizer.GetString("gui_yes"));
		windowPopup.SetTextDenyButton(Localizer.GetString("gui_no"));
		windowPopup.SetPopupInfo(PopupType.Question, Localizer.GetString("gui_option_exit_message"));
		windowPopup.SetInGameManagerReference(GuiManager);
	}

	private void RemoveConfirmWindow()
	{
		handleAxisInput = true;
		handleInput = true;
		visibleConfirmWindow = false;
		UnityEngine.Object.Destroy(windowPopup.gameObject);
	}

	private void OpenLinkControllerWindow()
	{
		if (InputManager.Instance != null)
		{
			InputManager.Instance.StartLinkProcess(true);
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
			for (int i = 0; i < 4; i++)
			{
				if (InputManager.GetBool(InputAction.MenuY, i))
				{
					OpenLinkControllerWindow();
					break;
				}
			}
			bool @bool = InputManager.GetBool(InputAction.MenuForward);
			bool bool2 = InputManager.GetBool(InputAction.MenuBackward);
			int @int = InputManager.GetInt(InputAction.MenuHorizontal, InputAction.MenuHorizontalAlternate);
			int int2 = InputManager.GetInt(InputAction.MenuVertical, InputAction.MenuVerticalAlternate);
			if (@bool)
			{
				PositionTween.callWhenFinished = "OnEnterComplete";
				HandleButtonForwardPressed();
			}
			if (bool2)
			{
				if (hasChangeBeenMade)
				{
					PositionTween.callWhenFinished = "CreateConfirmWindow";
					TweenOut();
				}
				else
				{
					PositionTween.callWhenFinished = "OnEnterComplete";
					HandleButtonBackwardPressed();
				}
			}
			HandleHorizontalAxisInput(@int);
			HandleVerticalAxisInput(int2);
		}
		else if (handleInput && InputManager.GetBool(InputAction.MenuBackward) && hasChangeBeenMade)
		{
			RemoveConfirmWindow();
			TweenIn();
		}
	}

	private void OnVolumeChange(int axis)
	{
		if (listIndex == 0)
		{
			if (axis == 1)
			{
				if (musicIndex < maxVolumeIndex)
				{
					musicIndex++;
					hasChangeBeenMade = true;
				}
			}
			else if (musicIndex > minVolumeIndex)
			{
				musicIndex--;
				hasChangeBeenMade = true;
			}
			sliderSprites[listIndex].fillAmount = (float)musicIndex * 0.1f;
			optionManager.MusicVolume = (float)musicIndex * 0.1f;
		}
		else
		{
			if (listIndex != 1)
			{
				return;
			}
			if (axis == 1)
			{
				if (sfxIndex < maxVolumeIndex)
				{
					sfxIndex++;
					hasChangeBeenMade = true;
				}
			}
			else if (sfxIndex > minVolumeIndex)
			{
				sfxIndex--;
				hasChangeBeenMade = true;
			}
			sliderSprites[listIndex].fillAmount = (float)sfxIndex * 0.1f;
			optionManager.SfxVolume = (float)sfxIndex * 0.1f;
		}
	}

	public override void HandleButtonForwardPressed()
	{
		handleInput = false;
		handleAxisInput = false;
		SaveSettings();
		GuiManager.PlaySound(GuiInGameManager.Sound.Forward);
		TweenOut();
	}

	public override void HandleButtonBackwardPressed()
	{
		handleInput = false;
		handleAxisInput = false;
		hasChangeBeenMade = false;
		GuiManager.PlaySound(GuiInGameManager.Sound.Backward);
		TweenOut();
	}

	public override void HandleVerticalAxisInput(int axis)
	{
		if (axis == 0 || !handleAxisInput)
		{
			return;
		}
		if (axis < 1)
		{
			if (listIndex + 1 <= sliderBars.Count - 1)
			{
				GuiManager.PlaySound(GuiInGameManager.Sound.ShopButton);
				sliderBars[listIndex].OnFocusLost2();
				listIndex++;
				sliderBars[listIndex].OnFocusGained2();
			}
		}
		else if (listIndex - 1 >= 0)
		{
			GuiManager.PlaySound(GuiInGameManager.Sound.ShopButton);
			sliderBars[listIndex].OnFocusLost2();
			listIndex--;
			sliderBars[listIndex].OnFocusGained2();
		}
		handleAxisInput = false;
	}

	public override void HandleHorizontalAxisInput(int axis)
	{
		sliderBars[listIndex].OnHorizontalInput(axis);
		if (axis != 0 && handleAxisInput)
		{
			if (listIndex < 2)
			{
				OnVolumeChange(axis);
			}
			else if (listIndex == 2)
			{
				OnControlSettingChange(axis);
			}
			GuiManager.PlaySound(GuiInGameManager.Sound.ShopList);
			handleAxisInput = false;
		}
	}

	private void ResetDelay()
	{
		handleAxisInput = true;
		if (sliderBars[listIndex] != null)
		{
			sliderBars[listIndex].OnButtonRelease();
		}
	}
}
