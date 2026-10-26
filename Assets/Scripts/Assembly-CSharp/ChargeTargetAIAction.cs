using System;
using AIActionTransitions;
using UnityEngine;

internal class ChargeTargetAIAction : FollowTargetAIAction
{
	private Action callback;

	private float pathCheckDelay = 0.5f;

	private float counter;

	public ChargeTargetAIAction(Action callback)
	{
		this.callback = callback;
		allowZombieLikeMovement = false;
	}

	public override void OnStart()
	{
		if (callback == null)
		{
			status = AIActionStatus.Failed;
		}
		else
		{
			base.OnStart();
		}
	}

	public override Transition Update()
	{
		if (counter < pathCheckDelay)
		{
			counter += Time.deltaTime;
			if (counter >= pathCheckDelay)
			{
				if (!motor.Agent.hasPath)
				{
					status = AIActionStatus.Complete;
				}
				else if (motor.Agent.pathEndPosition != targetPosition)
				{
					motor.Agent.Stop(true);
					motor.Agent.ResetPath();
					motor.Agent.Resume();
					ActivatePath();
				}
			}
		}
		return base.Update();
	}

	public override void OnEnd()
	{
		if (callback != null)
		{
			callback();
		}
	}

	protected override void OnTargetReached(ref Transition transition)
	{
		transition = new Transition(TransitionType.Done);
		status = AIActionStatus.Complete;
	}
}
