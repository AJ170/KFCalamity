public class StunAIEvent : AIEvent
{
	private float duration;

	public float Duration
	{
		get
		{
			return duration;
		}
	}

	public StunAIEvent(float duration)
		: base(AIEventID.Stun)
	{
		this.duration = duration;
	}
}
