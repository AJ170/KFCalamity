using UnityEngine;

namespace Calamity.Input
{
	public class AnyKeyInputWrapper : InputWrapper
	{
		private InputActionType type;

		private bool prevDown;

		private bool isDown;

		public AnyKeyInputWrapper(InputActionType type)
		{
			this.type = type;
			requiresUpdate = true;
			prevDown = false;
			isDown = false;
		}

		public override void Update()
		{
			prevDown = isDown;
			isDown = UnityEngine.Input.anyKey;
		}

		public override int GetInt()
		{
			return GetBool() ? 1 : 0;
		}

		public override float GetFloat()
		{
			return (!GetBool()) ? 0f : 1f;
		}

		public override bool GetBool()
		{
			switch (type)
			{
			case InputActionType.Down:
				return isDown && !prevDown;
			case InputActionType.Up:
				return !isDown && prevDown;
			default:
				return isDown;
			}
		}
	}
}
