using AIActionTransitions;
using UnityEngine;

public class StunAIAction : AIAction
{
	private float duration;

	public StunAIAction(float duration)
	{
		this.duration = duration;
		if (duration <= 0f)
		{
			status = AIActionStatus.Failed;
		}
	}

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		if (triggeredEvent.ID == AIEventID.Stun && status == AIActionStatus.Active)
		{
			StunAIEvent stunAIEvent = triggeredEvent as StunAIEvent;
			float num = stunAIEvent.Duration;
			if (num > 0f && num > duration)
			{
				duration = num;
			}
		}
		return null;
	}

	public override Transition Update()
	{
		if (status == AIActionStatus.Active && duration > 0f)
		{
			duration = Mathf.Max(duration - Time.deltaTime, 0f);
			if (duration == 0f)
			{
				status = AIActionStatus.Complete;
			}
		}
		return base.Update();
	}
}
