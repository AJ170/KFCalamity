using EventHelpers;

namespace GameEvents
{
	public class PauseToggled : EventBase
	{
		private bool paused;

		public bool Paused
		{
			get
			{
				return paused;
			}
		}

		public PauseToggled(bool paused)
			: base(EventId.PauseToggled)
		{
			this.paused = paused;
		}
	}
}
