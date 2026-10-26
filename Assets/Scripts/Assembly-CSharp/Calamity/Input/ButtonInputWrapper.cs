using UnityEngine;

namespace Calamity.Input
{
	public class ButtonInputWrapper : InputWrapper
	{
		private string button;

		private InputActionType type;

		public ButtonInputWrapper(string button, InputActionType type)
		{
			this.button = button;
			this.type = type;
		}

		public override bool GetBool()
		{
			switch (type)
			{
			case InputActionType.Down:
				return UnityEngine.Input.GetButtonDown(button);
			case InputActionType.Up:
				return UnityEngine.Input.GetButtonUp(button);
			default:
				return UnityEngine.Input.GetButton(button);
			}
		}

		public override int GetInt()
		{
			return GetBool() ? 1 : 0;
		}

		public override float GetFloat()
		{
			return (!GetBool()) ? 0f : 1f;
		}
	}
}
