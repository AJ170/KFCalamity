namespace Calamity.Input
{
	public class TouchGestureWrapper : InputWrapper
	{
		private bool recognized;

		private bool thisFrame;

		protected bool Recognized
		{
			get
			{
				return recognized;
			}
		}

		protected virtual void Reset()
		{
		}

		public override void Update()
		{
			if (recognized && !thisFrame)
			{
				recognized = false;
				Reset();
			}
			thisFrame = false;
		}

		protected void OnRecognized()
		{
			recognized = true;
			thisFrame = true;
		}

		public override bool GetBool()
		{
			return recognized;
		}
	}
}
