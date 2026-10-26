using AIActionTransitions;
using UnityEngine;

public class FaceTargetAIAction : AIAction
{
	public enum ExpirationMode
	{
		None = 0,
		Timer = 1,
		MinRange = 2,
		MaxRange = 3
	}

	private ExpirationMode expiration = ExpirationMode.Timer;

	private float expirationValue;

	private AITargetHandler targetHandler;

	private Transform targetTransform;

	protected AIMovementMotor motor;

	private bool movingOnStart;

	private float updateFrequency = 10f;

	private float counter;

	public FaceTargetAIAction(ExpirationMode expiration, float expirationValue)
	{
		this.expiration = expiration;
		this.expirationValue = expirationValue;
	}

	public FaceTargetAIAction()
	{
		expiration = ExpirationMode.None;
	}

	public override void OnStart()
	{
		motor = controller.GetComponent<AIMovementMotor>();
		if (motor != null)
		{
			movingOnStart = motor.Moving;
			motor.Stop(true);
			targetHandler = controller.TargetHandler;
			if (targetHandler.HasTarget)
			{
				targetTransform = targetHandler.Target.transform;
				base.OnStart();
			}
			else
			{
				MonoBehaviour.print("FaceTargetAIAction failed: No target");
				status = AIActionStatus.Failed;
			}
		}
		else
		{
			Debug.LogWarning("FaceTargetAIAction failed: No motor on GameObject");
			status = AIActionStatus.Failed;
		}
	}

	public override Transition Update()
	{
		CheckTarget();
		if (!IsDone())
		{
			if (expiration == ExpirationMode.Timer)
			{
				expirationValue -= Time.deltaTime;
				if (expirationValue <= 0f)
				{
					status = AIActionStatus.Complete;
				}
			}
			counter -= Time.deltaTime;
			if (counter <= 0f)
			{
				UpdateRotation();
				CheckRangeExpiration();
				counter = 1f / updateFrequency;
			}
		}
		return base.Update();
	}

	private void CheckTarget()
	{
		if (targetTransform == null)
		{
			status = AIActionStatus.Complete;
		}
	}

	private void CheckRangeExpiration()
	{
		if (expiration == ExpirationMode.MinRange)
		{
			Vector3 position = controller.transform.position;
			Vector3 position2 = targetTransform.position;
			if (Vector3.Distance(position, position2) < expirationValue)
			{
				status = AIActionStatus.Complete;
			}
		}
		else if (expiration == ExpirationMode.MaxRange)
		{
			Vector3 position3 = controller.transform.position;
			Vector3 position4 = targetTransform.position;
			if (Vector3.Distance(position3, position4) > expirationValue)
			{
				status = AIActionStatus.Complete;
			}
		}
	}

	private void UpdateRotation()
	{
		Vector3 position = controller.transform.position;
		Vector3 position2 = targetTransform.position;
		Vector3 rotationDirection = Vector3.Normalize(position2 - position);
		rotationDirection.y = 0f;
		motor.SetRotationDirection(rotationDirection);
	}

	public override void OnResumed()
	{
		movingOnStart = motor.Moving;
		if (movingOnStart)
		{
			motor.Stop();
		}
		status = AIActionStatus.Active;
	}

	public override void OnSuspended()
	{
		if (movingOnStart)
		{
			motor.Resume();
		}
		status = AIActionStatus.Suspended;
	}

	public override void OnEnd()
	{
		motor.Resume();
	}

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		if (triggeredEvent.ID == AIEventID.UnitDeath || triggeredEvent.ID == AIEventID.TargetLost || triggeredEvent.ID == AIEventID.TargetDied)
		{
			return new Transition(TransitionType.Done);
		}
		if (triggeredEvent.ID == AIEventID.TargetChanged)
		{
			TargetChangedAIEvent targetChangedAIEvent = (TargetChangedAIEvent)triggeredEvent;
			targetTransform = targetChangedAIEvent.Target.transform;
		}
		else if (triggeredEvent.ID == AIEventID.Knockback && status == AIActionStatus.Active)
		{
			motor.EndRotation();
			KnockbackAIEvent knockbackEvent = (KnockbackAIEvent)triggeredEvent;
			return new Transition(TransitionType.SuspendFor, new KnockbackAIAction(knockbackEvent));
		}
		return null;
	}
}
