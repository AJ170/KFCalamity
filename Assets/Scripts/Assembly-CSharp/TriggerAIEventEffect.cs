using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class TriggerAIEventEffect : EffectBaseListenerWithTarget
{
	public enum TriggeredAIEvent
	{
		PatriarchCharge = 0,
		PatriarchHeal = 1,
		Rampage = 2,
		Impale = 3,
		Leap = 4,
		IncomingMissile = 5
	}

	public TriggeredAIEvent Event;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		GameObject gameObject = FindTargetObject(e);
		if (!(gameObject != null))
		{
			return;
		}
		AIController component = gameObject.GetComponent<AIController>();
		if (component != null)
		{
			AIEvent @event = GetEvent();
			if (@event != null)
			{
				component.TriggerAIEvent(@event);
			}
		}
	}

	private AIEvent GetEvent()
	{
		AIEvent result = null;
		switch (Event)
		{
		case TriggeredAIEvent.PatriarchCharge:
			result = new PatriarchAIEvent(PatriarchAIEvent.EventType.Charge);
			break;
		case TriggeredAIEvent.PatriarchHeal:
			result = new PatriarchAIEvent(PatriarchAIEvent.EventType.HealMove);
			break;
		case TriggeredAIEvent.Rampage:
			result = new RampageAIEvent();
			break;
		case TriggeredAIEvent.Impale:
			result = new ImpaleAIEvent();
			break;
		case TriggeredAIEvent.Leap:
			result = new MotionAIEvent(new AILeapMotion());
			break;
		case TriggeredAIEvent.IncomingMissile:
			result = new IncomingProjectileAIEvent();
			break;
		}
		return result;
	}
}
