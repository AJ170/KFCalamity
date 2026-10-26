using UnityEngine.AI;
using AIActionTransitions;
using UnityEngine;

public class JumpAIAction : AIAction
{
	private const float pi = 3.14159f;

	private float duration = 1f;

	private float height = 0.5f;

	private Vector3 startPosition;

	private Vector3 endPosition;

	private NavMeshAgent agent;

	private ZedAnimationController animationController;

	private AIMovementMotor motor;

	private bool movingOnStart;

	private float counter;

	public JumpAIAction(NavMeshAgent agent, OffMeshLinkData linkData)
	{
		this.agent = agent;
		endPosition = linkData.endPos;
	}

	public JumpAIAction(NavMeshAgent agent, OffMeshLinkData linkData, AIJumpMotion jumpMotion)
	{
		this.agent = agent;
		endPosition = linkData.endPos;
		height = jumpMotion.JumpHeight;
		duration = jumpMotion.Duration;
	}

	public override void OnStart()
	{
		motor = controller.GetComponent<AIMovementMotor>();
		if (motor != null)
		{
			movingOnStart = motor.Moving;
			if (movingOnStart)
			{
				motor.Stop(true);
			}
			motor.StartMotion();
			startPosition = controller.transform.position;
			Vector3 rotationDirection = Vector3.Normalize(endPosition - startPosition);
			rotationDirection.y = 0f;
			motor.SetRotationDirection(rotationDirection);
			animationController = controller.GetComponent<ZedAnimationController>();
			if (animationController != null)
			{
				animationController.TriggerAnimation(ZedAnimationTrigger.Jump);
			}
			base.OnStart();
		}
		else
		{
			Debug.LogWarning("JumpAIAction failed: No motor on GameObject");
			status = AIActionStatus.Failed;
		}
	}

	public override Transition Update()
	{
		counter = Mathf.Min(counter + Time.deltaTime, duration);
		UpdatePosition();
		if (counter == duration)
		{
			OnCompleted();
		}
		return base.Update();
	}

	private void UpdatePosition()
	{
		float num = counter / duration;
		Vector3 forcedPosition = Vector3.Lerp(startPosition, endPosition, num);
		forcedPosition.y += height * Mathf.Sin(3.14159f * num);
		motor.SetForcedPosition(forcedPosition);
	}

	private void OnCompleted()
	{
		agent.CompleteOffMeshLink();
		agent.Resume();
		motor.EndMotion();
		status = AIActionStatus.Complete;
	}

	public override void OnEnd()
	{
		if (movingOnStart)
		{
			motor.Resume();
		}
	}

	public override void OnSuspended()
	{
		OnCompleted();
	}
}
