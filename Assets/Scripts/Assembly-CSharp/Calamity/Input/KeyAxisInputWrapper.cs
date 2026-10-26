using UnityEngine;

namespace Calamity.Input
{
	public class KeyAxisInputWrapper : InputWrapper
	{
		private KeyCode keyA;

		private KeyCode keyB;

		private InputActionType type;

		public KeyAxisInputWrapper(KeyCode keyA, KeyCode keyB, InputActionType type)
		{
			this.keyA = keyA;
			this.keyB = keyB;
			this.type = type;
		}

		public override bool GetBool()
		{
			return GetValue(keyA) || GetValue(keyB);
		}

		public override int GetInt()
		{
			if (GetValue(keyA))
			{
				return -1;
			}
			if (GetValue(keyB))
			{
				return 1;
			}
			return 0;
		}

		public override float GetFloat()
		{
			return GetInt();
		}

		private bool GetValue(KeyCode key)
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
	}
}
