using UnityEngine;

namespace Calamity.Input
{
	public class VirtualStickOrMouseRotationWrapper : HybridInputWrapper
	{
		private bool wasRightSideTouch;

		private Vector3 mouse = Vector3.zero;

		public VirtualStickOrMouseRotationWrapper(string stickObjectName)
			: base(new VirtualStickWrapper(stickObjectName), new MouseRotationWrapper())
		{
		}

		public override Vector3 GetVector()
		{
			Vector3 result = Vector3.zero;
			if (StickTouchRecognizer.Instance.IsRightSideTouch)
			{
				result = inputWrapper1.GetVector();
			}
			else
			{
				if (wasRightSideTouch)
				{
					mouse = UnityEngine.Input.mousePosition;
				}
				if (mouse != Vector3.zero && (UnityEngine.Input.mousePosition - mouse).magnitude > 5f)
				{
					mouse = Vector3.zero;
				}
				if (mouse == Vector3.zero)
				{
					result = inputWrapper2.GetVector();
				}
			}
			wasRightSideTouch = StickTouchRecognizer.Instance.IsRightSideTouch;
			return result;
		}
	}
}
