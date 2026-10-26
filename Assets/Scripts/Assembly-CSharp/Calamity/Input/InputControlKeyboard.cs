using UnityEngine;

namespace Calamity.Input
{
	public class InputControlKeyboard : InputControl
	{
		public InputControlKeyboard(InputPlayer playerID, ControllerType controllerType, InputMode inputMode = InputMode.Normal)
			: base(playerID, controllerType, inputMode)
		{
		}

		protected override void Setup()
		{
			base.Setup();
		}

		protected override void SetupAction(MappedAction mappedAction)
		{
			InputWrapper inputWrapper = null;
			switch (mappedAction.Action)
			{
			case InputAction.SkipSplashScreen:
				inputWrapper = InputHelper.GetWrappedAnyKeyInput(InputActionType.Up);
				break;
			case InputAction.MenuForward:
				inputWrapper = InputHelper.GetWrappedKey(KeyCode.Return, InputActionType.Up);
				break;
			case InputAction.Pause:
				inputWrapper = InputHelper.GetWrappedKeyOrTouchButtonInput(KeyCode.Escape, mappedAction.Type, "pause_button");
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
}
