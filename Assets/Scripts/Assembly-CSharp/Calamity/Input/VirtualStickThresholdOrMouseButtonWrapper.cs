namespace Calamity.Input
{
	public class VirtualStickThresholdOrMouseButtonWrapper : HybridInputWrapper
	{
		public VirtualStickThresholdOrMouseButtonWrapper(string stickObjectName, string mouseButton, InputActionType inputType, string stickZone, float delay = 0f, bool sticky = false)
			: base(new VirtualStickThresholdWrapper(stickObjectName, stickZone, delay, sticky), new ButtonInputWrapper(mouseButton, inputType))
		{
		}

		public override bool GetBool()
		{
			if (StickTouchRecognizer.Instance.IsRightSideTouch)
			{
				return inputWrapper1.GetBool();
			}
			return (!inputWrapper1.GetBool()) ? inputWrapper2.GetBool() : inputWrapper1.GetBool();
		}
	}
}
