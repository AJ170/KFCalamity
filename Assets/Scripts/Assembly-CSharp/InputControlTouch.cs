using Calamity.Input;

public class InputControlTouch : InputControl
{
	public InputControlTouch(InputPlayer playerID, ControllerType controllerType, InputMode inputMode = InputMode.Normal)
		: base(playerID, controllerType, inputMode)
	{
	}

	protected override void Setup()
	{
		base.Setup();
	}

	public override void SetInputMode(InputMode inputMode)
	{
	}

	protected override void SetupAction(MappedAction mappedAction)
	{
		InputWrapper inputWrapper = null;
		switch (mappedAction.Action)
		{
		case InputAction.SkipSplashScreen:
			inputWrapper = InputHelper.GetWrappedTapInput();
			break;
		case InputAction.MenuHorizontal:
			inputWrapper = InputHelper.GetWrappedSwipeInput(TKSwipeDirection.Horizontal, true);
			break;
		case InputAction.MenuVertical:
			inputWrapper = InputHelper.GetWrappedSwipeInput(TKSwipeDirection.Vertical);
			break;
		case InputAction.Pause:
			inputWrapper = InputHelper.GetWrappedTouchButton("pause_button");
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
