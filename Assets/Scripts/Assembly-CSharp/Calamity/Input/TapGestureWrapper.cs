namespace Calamity.Input
{
	public class TapGestureWrapper : TouchGestureWrapper
	{
		private static TKTapRecognizer tapRecognizer;

		public TapGestureWrapper()
		{
			if (tapRecognizer == null)
			{
				tapRecognizer = new TKTapRecognizer();
				TouchKit.addGestureRecognizer(tapRecognizer);
			}
			tapRecognizer.gestureRecognizedEvent += tapRecognizer_gestureRecognizedEvent;
			requiresUpdate = true;
		}

		private void tapRecognizer_gestureRecognizedEvent(TKTapRecognizer obj)
		{
			OnRecognized();
		}

		public override void Update()
		{
			base.Update();
		}

		public override void Shutdown()
		{
			TouchKit.removeGestureRecognizer(tapRecognizer);
		}
	}
}
