using UnityEngine.AI;
using System;
using AIActionTransitions;
using UnityEngine;

internal class LeapAIAction : AIAction
{
	private enum LeapState
	{
		Inactive = 0,
		ChargeUp = 1,
		Leap = 2
	}

	private float leapDuration = 0.4f;

	private float targetOffset = -1.4f;

	private float freeJumpLength = 5f;

	private float speed;

	private float height = 1f;

	private AIMovementMotor motor;

	private ZedAnimationController animationController;

	private Transform transform;

	private LeapState state;

	private Vector3 startPosition;

	private Vector3 targetPosition;

	private float counter;

	public override void OnStart()
	{
		transform = controller.transform;
		SetupTargetPosition();
		startPosition = transform.position;
		motor = controller.GetComponent<AIMovementMotor>();
		if (motor != null)
		{
			motor.Stop(true);
			motor.StartMotion();
			Vector3 rotationDirection = Vector3.Normalize(targetPosition - transform.position);
			rotationDirection.y = 0f;
			motor.SetRotationDirection(rotationDirection);
			counter = leapDuration;
			state = LeapState.Leap;
			base.OnStart();
		}
		else
		{
			Debug.LogWarning("LeapAIAction failed: No motor on GameObject");
			status = AIActionStatus.Failed;
		}
	}

	private void SetupTargetPosition()
	{
		AITargetHandler targetHandler = controller.TargetHandler;
		if (targetHandler.HasTarget)
		{
			Transform transform = targetHandler.Target.transform;
			targetPosition = transform.position;
			Vector3 vector = Vector3.Normalize(this.transform.position - targetPosition);
			targetPosition -= vector * targetOffset;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(targetPosition, out hit, 1f, -1))
			{
				targetPosition = hit.position;
			}
		}
		else
		{
			targetPosition = this.transform.position + this.transform.forward * freeJumpLength;
		}
	}

	public override Transition Update()
	{
		counter = Mathf.Max(counter - Time.deltaTime, 0f);
		if (counter <= 0f)
		{
			OnTimerEnd();
		}
		else if (state == LeapState.Leap)
		{
			UpdateLeapPosition();
		}
		return base.Update();
	}

	private void OnTimerEnd()
	{
		if (state == LeapState.ChargeUp)
		{
			counter = leapDuration;
			state = LeapState.Leap;
			return;
		}
		transform.position = targetPosition;
		motor.EndMotion();
		motor.Resume();
		motor.Warp(targetPosition);
		status = AIActionStatus.Complete;
	}

	private void UpdateLeapPosition()
	{
		float num = (leapDuration - counter) / leapDuration;
		Vector3 forcedPosition = Vector3.Lerp(startPosition, targetPosition, num);
		forcedPosition.y += height * Mathf.Sin((float)Math.PI * num);
		motor.SetForcedPosition(forcedPosition);
	}
}
