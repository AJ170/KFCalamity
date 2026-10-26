using EffectEvents;
using EventHelpers;

public class TriggerInternalEventEffect : EffectBaseListener
{
	public TriggerId Event;

	public string Name;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && effectSystem != null && Event != 0)
		{
			TriggerBase @event = GetEvent(Event);
			if (@event != null)
			{
				effectSystem.TriggerInternal(@event);
			}
		}
	}

	private TriggerBase GetEvent(TriggerId id)
	{
		TriggerBase result = null;
		if (id == TriggerId.Attack)
		{
			result = new Attack(Name);
		}
		return result;
	}
}
