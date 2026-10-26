using EventHelpers;

public class CustomListenerSampleEffect : EffectBaseListener
{
	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && e.Id != TriggerId.Hit)
		{
		}
	}
}
