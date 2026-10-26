using EventHelpers;

public class ListenerSampleEffect : EffectBaseListener
{
	protected override void Execute(TriggerBase e)
	{
		if (e.Id != TriggerId.Hit)
		{
		}
	}
}
