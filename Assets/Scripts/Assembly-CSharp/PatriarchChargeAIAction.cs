using AIActionTransitions;
using UnityEngine;

internal class PatriarchChargeAIAction : AIAction
{
	private float maxDuration = 20f;

	private FollowTargetAIAction followTargetAction;

	private float counter;

	public override void OnStart()
	{
		GameObject target = controller.TargetHandler.Target;
		if (target == null)
		{
			Debug.LogError("PatriarchChargeAIAction failed: no target");
			status = AIActionStatus.Failed;
		}
		else
		{
			StartMovingToTarget();
			counter = maxDuration;
			base.OnStart();
		}
	}

	public override Transition Update()
	{
		if (counter > 0f)
		{
			counter -= Time.deltaTime;
			if (counter <= 0f)
			{
				status = AIActionStatus.Complete;
			}
		}
		return base.Update();
	}

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		Transition result = null;
		if (triggeredEvent.ID == AIEventID.UnitDeath || triggeredEvent.ID == AIEventID.TargetLost || triggeredEvent.ID == AIEventID.TargetDied)
		{
			result = new Transition(TransitionType.Done);
		}
		return result;
	}

	private void StartMovingToTarget()
	{
		followTargetAction = new ChargeTargetAIAction(OnChargeDone);
		actionController.Secondary.Push(followTargetAction);
	}

	private void OnChargeDone()
	{
		status = AIActionStatus.Complete;
	}

	public override void OnEnd()
	{
		if (followTargetAction != null && !followTargetAction.IsDone())
		{
			followTargetAction.Abort();
		}
	}
}
