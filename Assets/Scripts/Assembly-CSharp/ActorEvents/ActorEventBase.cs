namespace ActorEvents
{
	public abstract class ActorEventBase
	{
		private ActorEventID id;

		public ActorEventID ID
		{
			get
			{
				return id;
			}
		}

		public ActorEventBase(ActorEventID id)
		{
			this.id = id;
		}
	}
}
