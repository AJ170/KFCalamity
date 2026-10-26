using EffectEvents;
using EventHelpers;

public class TriggerCastOnDeath : EffectBaseListener
{
	public bool TriggerOnHeadshot = true;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && e.Id == TriggerId.Death)
		{
			Death death = (Death)e;
			bool headshot = death.Headshot;
			if (!headshot || headshot == TriggerOnHeadshot)
			{
				effectSystem.TriggerInternal(new Cast(base.name));
			}
		}
	}
}
