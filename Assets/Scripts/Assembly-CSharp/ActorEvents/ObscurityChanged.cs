namespace ActorEvents
{
	public class ObscurityChanged : ActorEventBase
	{
		private bool obscured;

		public bool Obscured
		{
			get
			{
				return obscured;
			}
		}

		public ObscurityChanged(bool obscured)
			: base(ActorEventID.ObscurityChanged)
		{
			this.obscured = obscured;
		}
	}
}
