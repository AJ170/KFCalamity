using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class PopupWindow : MonoBehaviour
{
	public enum ButtonOrientation
	{
		Horizontal = 0,
		Vertical = 1
	}

	public enum PopupState
	{
		WaitingForInput = 0,
		Confirmed = 1,
		Denied = 2,
		Option1 = 3,
		Option2 = 4
	}

	public delegate void OnConfirm();

	public delegate void OnDeny();

	public ButtonOrientation InputOrientation;

	public GuiButton Button1;

	public GuiButton Button2;

	public UILabel TextLabel;

	private bool handleAxisInput;

	private int buttonIndex;

	private List<GuiButton> buttons = new List<GuiButton>();

	private GuiMenuManager manager;

	private GuiInGameManager managerInGame;

	private PopupState currentState;

	private TouchButtonWrapper confirmButton;

	private TouchButtonWrapper cancelButton;

	protected bool handleInput;

	public OnConfirm onConfirm;

	public OnDeny onDeny;

	public int ButtonIndex
	{
		get
		{
			return buttonIndex;
		}
		set
		{
			buttonIndex = value;
		}
	}

	public PopupState Status
	{
		get
		{
			return currentState;
		}
	}

	protected virtual void OnStart()
	{
	}

	private void Start()
	{
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller && buttons != null && buttonIndex < buttons.Count)
		{
			buttons[buttonIndex].OnSelect();
		}
		SetupTouchButtons();
		currentState = PopupState.WaitingForInput;
		OnStart();
	}

	private void SetupTouchButtons()
	{
		if (Button1 != null)
		{
			confirmButton = new TouchButtonWrapper(Button1.gameObject);
		}
		if (Button2 != null)
		{
			cancelButton = new TouchButtonWrapper(Button2.gameObject);
		}
	}

	public void SetManagerReference(GuiMenuManager reference)
	{
		manager = reference;
	}

	public void SetInGameManagerReference(GuiInGameManager reference)
	{
		managerInGame = reference;
	}

	public bool HasInputBeenMade()
	{
		return currentState != PopupState.WaitingForInput;
	}

	public bool MessageConfirmed()
	{
		return currentState == PopupState.Confirmed;
	}

	public virtual void SetPopupInfo(PopupType type, string text)
	{
		switch (type)
		{
		case PopupType.Statement:
			AddButton(Button1);
			break;
		case PopupType.Question:
			AddButton(Button1);
			AddButton(Button2);
			break;
		}
		TextLabel.text = text;
		handleAxisInput = true;
		handleInput = true;
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller && buttonIndex < buttons.Count)
		{
			buttons[buttonIndex].OnSelect();
		}
	}

	public virtual void SetMultiOptionPopupWindowInfo(string mainHeader, string header1, string header2, string text1, string text2)
	{
		throw new NotImplementedException("SetMultiOptionPopupWindowInfo: Can only be called on the MultiOptionPopupWindow class");
	}

	public void SetButton1(string text, PopupState buttonState)
	{
		if (Button1 != null)
		{
			Button1.SetText(text);
			Button1.CustomButtonData = buttonState;
		}
		else
		{
			Debug.LogError("Tried to set text for Button1, but no button object could be found!", this);
		}
	}

	public void SetButton2(string text, PopupState buttonState)
	{
		if (Button2 != null)
		{
			Button2.SetText(text);
			Button2.CustomButtonData = buttonState;
		}
		else
		{
			Debug.LogError("Tried to set text for Button2, but no button object could be found!", this);
		}
	}

	protected virtual void Update()
	{
		if (confirmButton != null)
		{
			confirmButton.Update();
		}
		if (cancelButton != null)
		{
			cancelButton.Update();
		}
		HandleInput();
	}

	private void HandleInput()
	{
		if (!handleInput)
		{
			return;
		}
		bool flag = DeviceSwitchController.CurrentControllerType == ControllerType.TouchControls;
		bool flag2 = false;
		bool flag3 = false;
		if (!flag)
		{
			flag2 = InputManager.GetBool(InputAction.MenuForward);
			flag3 = InputManager.GetBool(InputAction.MenuBackward);
		}
		if (!flag2)
		{
			if (confirmButton != null)
			{
				flag2 = confirmButton.GetBool();
			}
			if (cancelButton != null)
			{
				flag3 = cancelButton.GetBool();
			}
		}
		if (!flag3 && cancelButton != null)
		{
			flag3 = cancelButton.GetBool();
		}
		if (flag2)
		{
			buttons[buttonIndex].OnSelect();
		}
		else if (flag3)
		{
			buttonIndex = 1;
			buttons[buttonIndex].OnSelect();
		}
		if (flag2 || flag3)
		{
			currentState = (PopupState)(int)buttons[buttonIndex].CustomButtonData;
		}
		if (flag2)
		{
			handleAxisInput = false;
			handleInput = false;
			if (buttonIndex == 0)
			{
				if (onConfirm != null)
				{
					onConfirm();
				}
			}
			else if (onDeny != null)
			{
				onDeny();
			}
		}
		else if (flag3)
		{
			handleInput = false;
			handleAxisInput = false;
			if (onDeny != null)
			{
				onDeny();
			}
		}
		int axis = ((InputOrientation != 0) ? InputManager.GetInt(InputAction.MenuVertical, InputAction.MenuVerticalAlternate) : InputManager.GetInt(InputAction.MenuHorizontal, InputAction.MenuHorizontalAlternate));
		int axis2 = InputManager.GetInt(InputAction.MenuBumperLeft) * -1 + InputManager.GetInt(InputAction.MenuBumperRight);
		HandleAxisInput(axis);
		HandleAxisInput(axis2);
	}

	public void HandleAxisInput(int axis)
	{
		if (axis == 0 || !handleAxisInput)
		{
			return;
		}
		if (manager != null)
		{
			manager.PlaySound(GuiMenuManager.Sound.CharButton);
		}
		else if (managerInGame != null)
		{
			managerInGame.PlaySound(GuiInGameManager.Sound.ShopList);
		}
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller && buttonIndex < buttons.Count)
		{
			buttons[buttonIndex].OnDeselect();
		}
		if (axis < 0)
		{
			if (buttonIndex == 0)
			{
				buttonIndex = buttons.Count - 1;
			}
			else
			{
				buttonIndex--;
			}
		}
		else if (axis > 0)
		{
			if (buttonIndex == buttons.Count - 1)
			{
				buttonIndex = 0;
			}
			else
			{
				buttonIndex++;
			}
		}
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller && buttonIndex < buttons.Count)
		{
			buttons[buttonIndex].OnSelect();
		}
	}

	private void OnDestroy()
	{
	}

	protected void AddButton(GuiButton button)
	{
		buttons.Add(button);
	}
}
