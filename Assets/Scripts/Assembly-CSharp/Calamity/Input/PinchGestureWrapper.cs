using UnityEngine;

namespace Calamity.Input
{
	public class PinchGestureWrapper : TouchGestureWrapper
	{
		private static TKPinchRecognizer pinchRecognizer;

		private float scale;

		private int pinchDirection;

		public PinchGestureWrapper(int pinchDirection)
		{
			if (pinchRecognizer == null)
			{
				pinchRecognizer = new TKPinchRecognizer();
				TouchKit.addGestureRecognizer(pinchRecognizer);
			}
			pinchRecognizer.gestureCompleteEvent += pinchRecognizer_gestureCompleteEvent;
			pinchRecognizer.gestureRecognizedEvent += pinchRecognizer_gestureRecognizedEvent;
			this.pinchDirection = pinchDirection;
			requiresUpdate = true;
		}

		public override void Update()
		{
			base.Update();
		}

		protected override void Reset()
		{
			scale = 0f;
		}

		private void pinchRecognizer_gestureRecognizedEvent(TKPinchRecognizer obj)
		{
			scale += obj.deltaScale;
		}

		private void pinchRecognizer_gestureCompleteEvent(TKPinchRecognizer obj)
		{
			if (Mathf.Sign(pinchDirection) == Mathf.Sign(scale))
			{
				OnRecognized();
			}
		}

		public override void Shutdown()
		{
			TouchKit.removeGestureRecognizer(pinchRecognizer);
		}
	}
}
