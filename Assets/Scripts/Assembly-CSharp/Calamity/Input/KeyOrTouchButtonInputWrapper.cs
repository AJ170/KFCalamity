using UnityEngine;

namespace Calamity.Input
{
	public class KeyOrTouchButtonInputWrapper : HybridInputWrapper
	{
		private TouchHandler touch;

		public KeyOrTouchButtonInputWrapper(KeyCode key, InputActionType type, string touchButtonName, bool touchButtonIsToggle = false, float touchPressDelay = 0f, bool canHoldDown = false)
			: base(new KeyInputWrapper(key, type), new TouchButtonWrapper(touchButtonName, touchButtonIsToggle, touchPressDelay, canHoldDown))
		{
			touch = TouchHandler._instance;
		}
	}
}
