public abstract class AIEvent
{
	private AIEventID id;

	public AIEventID ID
	{
		get
		{
			return id;
		}
	}

	protected AIEvent(AIEventID id)
	{
		this.id = id;
	}
}
