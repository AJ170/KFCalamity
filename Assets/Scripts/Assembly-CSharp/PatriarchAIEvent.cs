public class PatriarchAIEvent : AIEvent
{
	public enum EventType
	{
		Charge = 0,
		HealMove = 1
	}

	private EventType type;

	public EventType Type
	{
		get
		{
			return type;
		}
	}

	public PatriarchAIEvent(EventType type)
		: base(AIEventID.Patriarch)
	{
		this.type = type;
	}
}
