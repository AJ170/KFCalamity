using UnityEngine.AI;
using AIActionTransitions;
using UnityEngine;

internal class ImpaleAIAction : AIAction
{
	private enum InternalState
	{
		Inactive = 0,
		PreStepDelay = 1,
		Step = 2,
		PostStepDelay = 3,
		Impale = 4,
		Complete = 5
	}

	private float preStepDelayLength = 0.47f;

	private float stepLength = 0.33f;

	private float postStepDelayLength = 0.1f;

	private float impaleLength = 2.5f;

	private float stepDistance = 0.9f;

	private Vector3 startPosition;

	private Vector3 targetPosition;

	private bool hitSomeone;

	private InternalState impaleState;

	private float counter;

	private AIAction secondaryAction;

	private AIMovementMotor motor;

	public override void OnStart()
	{
		counter = preStepDelayLength;
		impaleState = InternalState.PreStepDelay;
		base.OnStart();
	}

	public override Transition Update()
	{
		if (impaleState == InternalState.Step)
		{
			UpdateStepPosition();
		}
		counter -= Time.deltaTime;
		if (counter <= 0f)
		{
			OnCounterFinished();
		}
		return base.Update();
	}

	private void OnCounterFinished()
	{
		if (impaleState == InternalState.PreStepDelay)
		{
			StartMovement();
			counter = stepLength;
			impaleState = InternalState.Step;
		}
		else if (impaleState == InternalState.Step)
		{
			StopMovement();
			if (hitSomeone)
			{
				impaleState = InternalState.Impale;
				counter = impaleLength;
			}
			else
			{
				impaleState = InternalState.PostStepDelay;
				counter = postStepDelayLength;
			}
		}
		else
		{
			status = AIActionStatus.Complete;
		}
	}

	public override void OnEnd()
	{
		if (secondaryAction != null && !secondaryAction.IsDone())
		{
			secondaryAction.Abort();
		}
	}

	private void StartMovement()
	{
		motor = controller.GetComponent<AIMovementMotor>();
		if (motor != null)
		{
			Transform transform = controller.transform;
			startPosition = transform.position;
			targetPosition = startPosition + transform.forward * stepDistance;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(targetPosition, out hit, 0.3f, -1))
			{
				targetPosition = hit.position;
			}
			motor.Stop();
			motor.StartMotion();
		}
	}

	private void StopMovement()
	{
		secondaryAction = new HoldGroundAIAction((!hitSomeone) ? postStepDelayLength : impaleLength);
		actionController.Secondary.Push(secondaryAction);
		if (motor != null)
		{
			motor.EndMotion();
			motor.Resume();
		}
	}

	private void UpdateStepPosition()
	{
		if (motor != null)
		{
			float t = (stepLength - counter) / stepLength;
			Vector3 forcedPosition = Vector3.Lerp(startPosition, targetPosition, t);
			motor.SetForcedPosition(forcedPosition);
		}
	}

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		if (triggeredEvent.ID == AIEventID.Impale)
		{
			hitSomeone = true;
			if (impaleState == InternalState.PostStepDelay)
			{
				if (secondaryAction != null)
				{
					HoldGroundAIAction holdGroundAIAction = secondaryAction as HoldGroundAIAction;
					if (holdGroundAIAction != null)
					{
						holdGroundAIAction.ExtendDuration(impaleLength);
					}
				}
				impaleState = InternalState.Impale;
				counter = impaleLength;
			}
		}
		return null;
	}
}
