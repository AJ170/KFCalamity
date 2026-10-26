namespace Calamity.Input
{
	public class SwipeGestureWrapper : TouchGestureWrapper
	{
		private TKSwipeDirection swipeDirectionToDetect;

		private TKSwipeRecognizer swipeRecognizer;

		private int invert;

		private TKSwipeDirection lastSwipeDirection;

		public SwipeGestureWrapper(TKSwipeDirection swipeDirection = TKSwipeDirection.All, bool invert = false)
		{
			swipeDirectionToDetect = swipeDirection;
			if (swipeRecognizer == null)
			{
				swipeRecognizer = new TKSwipeRecognizer(20f, 40f);
				TouchKit.addGestureRecognizer(swipeRecognizer);
			}
			requiresUpdate = true;
			this.invert = ((!invert) ? 1 : (-1));
			swipeRecognizer.gestureRecognizedEvent += swipeRecognizer_gestureRecognizedEvent;
		}

		public override int GetInt()
		{
			if (base.Recognized)
			{
				if (swipeDirectionToDetect == TKSwipeDirection.Horizontal)
				{
					return ((lastSwipeDirection != TKSwipeDirection.Left) ? 1 : (-1)) * invert;
				}
				if (swipeDirectionToDetect == TKSwipeDirection.Vertical)
				{
					return ((lastSwipeDirection == TKSwipeDirection.Up) ? 1 : (-1)) * invert;
				}
				return ((swipeDirectionToDetect == lastSwipeDirection) ? 1 : 0) * invert;
			}
			return 0;
		}

		public override bool GetBool()
		{
			return GetInt() != 0;
		}

		private void swipeRecognizer_gestureRecognizedEvent(TKSwipeRecognizer obj)
		{
			TKSwipeDirection completedSwipeDirection = obj.completedSwipeDirection;
			bool flag = false;
			if (completedSwipeDirection == swipeDirectionToDetect)
			{
				flag = true;
			}
			else if (swipeDirectionToDetect == TKSwipeDirection.Horizontal && (completedSwipeDirection == TKSwipeDirection.Left || completedSwipeDirection == TKSwipeDirection.Right))
			{
				flag = true;
			}
			else if (swipeDirectionToDetect == TKSwipeDirection.Vertical && (completedSwipeDirection == TKSwipeDirection.Up || completedSwipeDirection == TKSwipeDirection.Down))
			{
				flag = true;
			}
			else if (swipeDirectionToDetect == TKSwipeDirection.All)
			{
				flag = true;
			}
			if (flag)
			{
				OnRecognized();
				lastSwipeDirection = obj.completedSwipeDirection;
			}
		}

		public override void Shutdown()
		{
			TouchKit.removeGestureRecognizer(swipeRecognizer);
		}
	}
}
