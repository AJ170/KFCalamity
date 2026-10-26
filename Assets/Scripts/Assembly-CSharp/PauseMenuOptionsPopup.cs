using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class PauseMenuOptionsPopup : MonoBehaviour
{
	public GuiButton ButtonAccept;

	public GuiButton ButtonDeny;

	public UILabel TextLabel;

	private bool handleAxisInput;

	private bool handleInput;

	private bool inputMade;

	private bool wasMessageConfirmed;

	private int buttonIndex;

	private List<GuiButton> buttons = new List<GuiButton>();

	private GuiMenuManager manager;

	private GuiInGameManager managerInGame;

	private float delayBetweenFocusChanges = 0.35f;

	private float delayTimeStamp;

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

	private void Start()
	{
	}

	public void SetManagerReference(GuiMenuManager reference)
	{
		manager = reference;
	}

	public void SetInGameManagerReference(GuiInGameManager reference)
	{
		managerInGame = reference;
	}

	public void OnEnterComplete()
	{
	}

	public bool HasInputBeenMade()
	{
		return inputMade;
	}

	public bool MessageConfirmed()
	{
		return wasMessageConfirmed;
	}

	public void SetPopupInfo(PopupType type, string text)
	{
		if (type != PopupType.Statement && type == PopupType.Question)
		{
			buttons.Add(ButtonAccept);
			buttons.Add(ButtonDeny);
		}
		TextLabel.text = text;
		handleAxisInput = true;
		handleInput = true;
		buttons[buttonIndex].OnSelect();
	}

	public void SetTextAcceptButton(string text)
	{
		ButtonAccept.SetText(text);
	}

	public void SetTextDenyButton(string text)
	{
		ButtonDeny.SetText(text);
	}

	private void Update()
	{
		if (!handleAxisInput)
		{
			delayTimeStamp += Time.fixedDeltaTime * 2f;
			if (delayTimeStamp >= delayBetweenFocusChanges)
			{
				ResetDelay();
			}
		}
		else
		{
			HandleInput();
		}
	}

	private void HandleInput()
	{
		if (!handleInput)
		{
			return;
		}
		bool @bool = InputManager.GetBool(InputAction.MenuForward);
		int @int = InputManager.GetInt(InputAction.MenuHorizontal, InputAction.MenuHorizontalAlternate);
		int axis = InputManager.GetInt(InputAction.MenuBumperLeft) * -1 + InputManager.GetInt(InputAction.MenuBumperRight);
		if (@bool)
		{
			inputMade = true;
			if (buttonIndex == 0)
			{
				handleInput = false;
				handleAxisInput = false;
				wasMessageConfirmed = true;
			}
			else
			{
				handleInput = false;
				handleAxisInput = false;
				wasMessageConfirmed = false;
			}
		}
		HandleHorizontalAxisInput(@int);
		HandleHorizontalAxisInput(axis);
	}

	public void HandleHorizontalAxisInput(int axis)
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
		buttons[buttonIndex].OnDeselect();
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
		handleAxisInput = false;
		buttons[buttonIndex].OnSelect();
	}

	private void ResetDelay()
	{
		delayTimeStamp = 0f;
		handleAxisInput = true;
	}

	private void OnDestroy()
	{
	}
}
