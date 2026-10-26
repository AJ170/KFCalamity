using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public enum PauseOptions
	{
		None = 0,
		Return = 1,
		Exit = 2,
		Menu = 3,
		Controls = 4,
		Options = 5
	}

	[Serializable]
	public class PauseButton
	{
		public PauseMenuButton Button;

		public PauseOptions Type;

		public string label;

		[HideInInspector]
		public int Index;
	}

	public TweenPosition PositionTween;

	public List<PauseButton> Buttons;

	public UILabel LabelTitle;

	public UISprite SpriteControls;

	public PauseMenuOptions Options;

	private GuiInGameManager manager;

	private bool handleInput;

	private bool handleAxisInput;

	private bool controlsVisible;

	private bool shouldUpdate = true;

	private int buttonIndex;

	private float delayBetweenFocusChanges = 0.35f;

	private float delayTimeStamp;

	private ButtonActions[] buttonActions;

	private void Start()
	{
		LabelTitle.text = Localizer.GetString("gui_game_paused");
		for (int i = 0; i < Buttons.Count; i++)
		{
			PauseButton pauseButton = Buttons[i];
			if (pauseButton != null)
			{
				pauseButton.Button.SetText(Localizer.GetString(pauseButton.label));
				pauseButton.Index = i;
				ButtonActions component = pauseButton.Button.GetComponent<ButtonActions>();
				component.CustomData = pauseButton;
				component.SetOnClickCallback(OnPauseButtonPressed);
			}
		}
		Buttons[buttonIndex].Button.OnSelect();
		base.gameObject.SetActive(false);
	}

	private void OnPauseButtonPressed(ButtonActions sender)
	{
		PauseButton button = sender.CustomData as PauseButton;
		Buttons[buttonIndex].Button.OnDeselect();
		button.Button.OnSelect();
		buttonIndex = Buttons.FindIndex((PauseButton a) => a == button);
		HandleSelected(button.Type);
	}

	private void Update()
	{
		if (!shouldUpdate)
		{
			return;
		}
		if (!handleInput)
		{
			delayTimeStamp += Time.fixedDeltaTime * 2f;
			if (delayTimeStamp >= delayBetweenFocusChanges)
			{
				Delay();
			}
		}
		else
		{
			HandleInput();
		}
	}

	public void TweenIn()
	{
		base.gameObject.SetActive(true);
		PositionTween.callWhenFinished = "ActivateInput";
		PositionTween.enabled = true;
		PositionTween.Play(true);
	}

	public void TweenOut()
	{
		PositionTween.callWhenFinished = "OnTweenComplete";
		PositionTween.Play(false);
	}

	private void OpenOptionsMenu()
	{
		PositionTween.callWhenFinished = string.Empty;
		shouldUpdate = false;
		Options.gameObject.SetActive(true);
		Options.TweenIn();
		if (Options.IsInitialized)
		{
			Options.LoadSettings();
		}
	}

	public void ActivateInput()
	{
		shouldUpdate = true;
		handleInput = true;
		handleAxisInput = true;
	}

	public void OnTweenComplete()
	{
		handleInput = false;
		handleAxisInput = false;
		base.gameObject.SetActive(false);
	}

	public void SetManagerReference(GuiInGameManager manager)
	{
		this.manager = manager;
	}

	private void SetControlsVisibility()
	{
		if (controlsVisible)
		{
			LabelTitle.gameObject.SetActive(true);
			foreach (PauseButton button in Buttons)
			{
				button.Button.gameObject.SetActive(true);
			}
			handleAxisInput = true;
			controlsVisible = false;
			SpriteControls.gameObject.SetActive(false);
			return;
		}
		LabelTitle.gameObject.SetActive(false);
		foreach (PauseButton button2 in Buttons)
		{
			button2.Button.gameObject.SetActive(false);
		}
		handleAxisInput = false;
		controlsVisible = true;
		SpriteControls.gameObject.SetActive(true);
	}

	private void HandleSelected(PauseOptions option)
	{
		switch (option)
		{
		case PauseOptions.Menu:
			manager.ReturnToMainMenu();
			break;
		case PauseOptions.Return:
			manager.ClosePauseMenu();
			break;
		case PauseOptions.Options:
			PositionTween.callWhenFinished = "OpenOptionsMenu";
			PositionTween.Play(false);
			break;
		case PauseOptions.Exit:
			Application.Quit();
			break;
		}
	}

	private void HandleInput()
	{
		if (handleInput)
		{
			bool @bool = InputManager.GetBool(InputAction.MenuForward);
			bool flag = InputManager.GetBool(InputAction.MenuBackward) || InputManager.GetPause();
			int @int = InputManager.GetInt(InputAction.MenuVertical, InputAction.MenuVerticalAlternate);
			if (@bool)
			{
				handleInput = false;
				manager.PlaySound(GuiInGameManager.Sound.Forward);
				PauseOptions type = Buttons[buttonIndex].Type;
				HandleSelected(type);
			}
			else if (flag)
			{
				handleInput = false;
				manager.PlaySound(GuiInGameManager.Sound.Forward);
				manager.ClosePauseMenu();
			}
			if (DeviceSwitchController.CurrentDeviceMode != DeviceMode.Touch)
			{
				HandleVerticalAxisInput(@int);
			}
		}
	}

	public void HandleVerticalAxisInput(int axis)
	{
		if (axis == 0 || !handleInput || !handleAxisInput)
		{
			return;
		}
		Buttons[buttonIndex].Button.OnDeselect();
		manager.PlaySound(GuiInGameManager.Sound.MenuButton);
		if (axis > 0)
		{
			if (buttonIndex == 0)
			{
				buttonIndex = Buttons.Count - 1;
			}
			else
			{
				buttonIndex--;
			}
		}
		else if (axis < 0)
		{
			if (buttonIndex == Buttons.Count - 1)
			{
				buttonIndex = 0;
			}
			else
			{
				buttonIndex++;
			}
		}
		handleInput = false;
		Buttons[buttonIndex].Button.OnSelect();
	}

	private void Delay()
	{
		handleInput = true;
		delayTimeStamp = 0f;
	}

	public void Open()
	{
		TweenIn();
	}

	public void Close()
	{
		TweenOut();
	}
}
