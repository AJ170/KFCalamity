using UnityEngine;

namespace Calamity.Input
{
	public class DoubleTapOrKeyWrapper : HybridInputWrapper
	{
		public DoubleTapOrKeyWrapper(string doubleTapObjectName, KeyCode key, InputActionType type)
			: base(new DoubleTapInputWrapper(doubleTapObjectName), new KeyInputWrapper(key, type))
		{
		}
	}
}
