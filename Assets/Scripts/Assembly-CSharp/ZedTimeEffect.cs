using EventHelpers;

public class ZedTimeEffect : EffectBaseListener
{
	public float Duration = 6f;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			TimeScaleManager.TriggerZedTime(Duration);
		}
	}
}
