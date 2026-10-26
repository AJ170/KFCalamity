using Calamity.Input;
using UnityEngine;

public class InputControlHybrid : InputControl
{
	public InputControlHybrid(InputPlayer playerID, ControllerType controllerType, InputMode inputMode = InputMode.Normal)
		: base(playerID, controllerType, inputMode)
	{
	}

	protected override void Setup()
	{
		base.Setup();
		AddInput(InputAction.OpenWeaponTray, InputHelper.GetWrappedTouchButton("bottom_bar"));
		InputWrapper wrappedTapInput = InputHelper.GetWrappedTapInput();
		AddInput(InputAction.SkipSplashScreen, wrappedTapInput);
	}

	public override void SetInputMode(InputMode inputMode)
	{
	}

	protected override void SetupAction(MappedAction mappedAction)
	{
		InputWrapper inputWrapper = null;
		switch (mappedAction.Action)
		{
		case InputAction.MenuHorizontal:
			inputWrapper = InputHelper.GetWrappedSwipeInput(TKSwipeDirection.Horizontal, true);
			break;
		case InputAction.MenuVertical:
			inputWrapper = InputHelper.GetWrappedSwipeInput(TKSwipeDirection.Vertical);
			break;
		case InputAction.Pause:
			inputWrapper = InputHelper.GetWrappedKeyOrTouchButtonInput(KeyCode.Escape, InputActionType.Up, "pause_button");
			break;
		default:
			base.SetupAction(mappedAction);
			return;
		}
		if (inputWrapper != null)
		{
			AddInput(mappedAction.Action, inputWrapper);
		}
	}
}
