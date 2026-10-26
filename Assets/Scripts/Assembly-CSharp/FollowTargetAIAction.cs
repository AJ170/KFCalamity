using AIActionTransitions;
using UnityEngine;

internal class FollowTargetAIAction : MoveToAIAction
{
	private float targetMoveThresholdForUpdate = 0.1f;

	private float maxThreshold = 3f;

	private AITargetHandler targetHandler;

	protected Transform targetTransform;

	private Vector3 lastTargetPosition;

	protected bool allowZombieLikeMovement = true;

	private PathTarget? zombieTarget;

	public FollowTargetAIAction()
	{
		updateFrequency = 4f;
	}

	public override void OnStart()
	{
		targetHandler = controller.TargetHandler;
		if (targetHandler.HasTarget)
		{
			UpdateTarget();
			UpdatePosition();
			SetCounter();
			base.OnStart();
		}
		else
		{
			Debug.LogError("FollowTargetAIAction failed: No target");
			status = AIActionStatus.Failed;
		}
	}

	protected override void OnUpdateCounter(ref Transition transition)
	{
		base.OnUpdateCounter(ref transition);
		if (transition == null && ShouldUpdatePath())
		{
			UpdatePosition();
			ActivatePath();
		}
	}

	protected override bool HaveReachedTarget()
	{
		if (targetTransform == null)
		{
			return true;
		}
		Vector3 position = transform.position;
		Vector3 position2 = targetTransform.position;
		float num = Vector3.Distance(position, position2);
		return num <= stoppingDistance;
	}

	protected override void OnTargetReached(ref Transition transition)
	{
		if (targetTransform != null)
		{
			transition = new Transition(TransitionType.SuspendFor, new FaceTargetAIAction(FaceTargetAIAction.ExpirationMode.MaxRange, stoppingDistance + 0.1f));
		}
		else
		{
			base.OnTargetReached(ref transition);
		}
	}

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		Transition transition = base.OnAIEvent(triggeredEvent);
		if (transition == null)
		{
			if (triggeredEvent.ID == AIEventID.TargetLost)
			{
				status = AIActionStatus.Complete;
			}
			else if (triggeredEvent.ID == AIEventID.TargetChanged)
			{
				UpdateTarget();
				if (status == AIActionStatus.Active)
				{
					UpdatePosition();
					ActivatePath();
					SetCounter();
				}
			}
		}
		return transition;
	}

	private bool ShouldUpdatePath()
	{
		Vector3 position = controller.transform.position;
		Vector3 position2 = targetTransform.position;
		float num = 0f;
		float num2 = 0f;
		if (ZombieLikeMovement() && zombieTarget.HasValue)
		{
			num = Mathf.Max(Vector3.Distance(position, lastTargetPosition) - motor.StoppingDistance, 0f);
			if (num < 0.6f)
			{
				return true;
			}
			num2 = Mathf.Max(Vector3.Distance(position2, lastTargetPosition) - zombieTarget.Value.DistanceOffset, 0f);
		}
		else
		{
			num = Mathf.Max(Vector3.Distance(position, position2) - motor.StoppingDistance, 0f);
			num2 = Vector3.Distance(position2, lastTargetPosition);
		}
		float num3 = Mathf.Min(Mathf.Pow(num, 2f) * targetMoveThresholdForUpdate, maxThreshold);
		if (num2 >= num3)
		{
			return true;
		}
		return false;
	}

	private void UpdateTarget()
	{
		if (targetHandler.HasTarget)
		{
			GameObject target = targetHandler.Target;
			targetTransform = target.transform;
		}
		else
		{
			status = AIActionStatus.Complete;
		}
	}

	private void UpdatePosition()
	{
		targetPosition = targetTransform.position;
		if (ZombieLikeMovement())
		{
			SetZombieTarget();
		}
		lastTargetPosition = targetPosition;
	}

	private bool ZombieLikeMovement()
	{
		bool flag = false;
		if (motor != null)
		{
			flag = allowZombieLikeMovement && motor.ZombieLike;
			if (flag)
			{
				float num = Vector3.Distance(controller.transform.position, targetTransform.position) - motor.StoppingDistance;
				if (num < motor.DistanceBeforeZombie)
				{
					flag = false;
				}
			}
		}
		return flag;
	}

	private void SetZombieTarget()
	{
		PathingTargets component = targetTransform.GetComponent<PathingTargets>();
		if (component != null)
		{
			zombieTarget = component.GetSmartZedTarget(controller.transform.position);
			if (zombieTarget.HasValue)
			{
				targetPosition = zombieTarget.Value.Position;
			}
		}
	}

	public override void OnResumed()
	{
		UpdateTarget();
		if (status != AIActionStatus.Complete)
		{
			UpdatePosition();
			base.OnResumed();
		}
	}
}
