using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class PurchasePopup : MonoBehaviour
{
	public GuiButton ButtonAccept;

	public GuiButton ButtonDeny;

	public float InputDelay = 0.3f;

	public TweenPosition PositionTween;

	public TweenAlpha AlphaTween;

	private bool handleAxisInput;

	private bool handleInput;

	private bool inputMade;

	private int buttonIndex;

	private List<GuiButton> buttons = new List<GuiButton>();

	private GuiMenuManager manager;

	private string callWhenFinished = string.Empty;

	private void Start()
	{
		SetPopupInfo();
	}

	public void SetManagerReference(GuiMenuManager reference)
	{
		manager = reference;
	}

	public void OnEnterComplete()
	{
	}

	public bool HasInputBeenMade()
	{
		return inputMade;
	}

	public void OnTweenOutComplete()
	{
		Object.Destroy(base.gameObject);
	}

	private void OnPurchaseDenyTween()
	{
		manager.PurchasePopupDeny();
		Object.Destroy(base.gameObject);
	}

	public void SetPopupInfo()
	{
		buttons.Add(ButtonAccept);
		buttons.Add(ButtonDeny);
		SetTextAcceptButton(Localizer.GetString("gui_popup_purchase_accept"));
		SetTextDenyButton(Localizer.GetString("gui_popup_purchase_deny"));
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
		HandleInput();
	}

	private void HandleInput()
	{
		if (!handleInput)
		{
			return;
		}
		bool @bool = InputManager.GetBool(InputAction.MenuForward);
		bool bool2 = InputManager.GetBool(InputAction.MenuBackward);
		int @int = InputManager.GetInt(InputAction.MenuHorizontal, InputAction.MenuHorizontalAlternate);
		int axis = InputManager.GetInt(InputAction.MenuBumperLeft) * -1 + InputManager.GetInt(InputAction.MenuBumperRight);
		if (@bool)
		{
			inputMade = true;
			if (buttonIndex == 0)
			{
				manager.PlaySound(GuiMenuManager.Sound.Forward);
				callWhenFinished = "OnTweenOutComplete";
				manager.PurchasePopupAccept();
			}
			else
			{
				manager.PlaySound(GuiMenuManager.Sound.Backward);
				callWhenFinished = "OnPurchaseDenyTween";
				TweenOut();
			}
		}
		if (bool2)
		{
			manager.PlaySound(GuiMenuManager.Sound.Backward);
			callWhenFinished = "OnPurchaseDenyTween";
			TweenOut();
		}
		HandleHorizontalAxisInput(@int);
		HandleHorizontalAxisInput(axis);
	}

	public void TweenOut()
	{
		PositionTween.callWhenFinished = callWhenFinished;
		PositionTween.eventReceiver = base.gameObject;
		PositionTween.Play(false);
		handleInput = false;
	}

	public void HandleHorizontalAxisInput(int axis)
	{
		if (axis == 0 || !handleAxisInput)
		{
			return;
		}
		manager.PlaySound(GuiMenuManager.Sound.CharButton);
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
		Invoke("ResetDelay", InputDelay);
	}

	private void ResetDelay()
	{
		handleAxisInput = true;
	}

	private void OnDestroy()
	{
	}
}
