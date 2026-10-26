using EffectEvents;
using EventHelpers;

public class TriggerFakeDeathEffect : EffectBaseListener
{
	protected override void Execute(TriggerBase e)
	{
		if (NoCustomEvents() || MatchCustomEvent(e.CustomName))
		{
			effectSystem.TriggerInternal(new BeingKilled(base.transform.root.gameObject));
			effectSystem.TriggerInternal(new Death());
		}
	}
}
