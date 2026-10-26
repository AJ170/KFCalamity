using UnityEngine;

namespace Calamity.Input
{
	public class KeyInputWrapper : InputWrapper
	{
		private KeyCode key;

		private InputActionType type;

		public KeyInputWrapper(KeyCode key, InputActionType type)
		{
			this.key = key;
			this.type = type;
		}

		public override bool GetBool()
		{
			switch (type)
			{
			case InputActionType.Down:
				return UnityEngine.Input.GetKeyDown(key);
			case InputActionType.Up:
				return UnityEngine.Input.GetKeyUp(key);
			default:
				return UnityEngine.Input.GetKey(key);
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
