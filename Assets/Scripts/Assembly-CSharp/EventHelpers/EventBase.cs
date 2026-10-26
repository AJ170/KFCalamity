namespace EventHelpers
{
	public class EventBase
	{
		private EventId id;

		public EventId Id
		{
			get
			{
				return id;
			}
		}

		public EventBase(EventId eventId)
		{
			id = eventId;
		}
	}
}
