using AIActionTransitions;
using UnityEngine;

internal class MoveToAIAction : AIAction
{
	protected float stoppingDistance;

	protected Transform transform;

	protected Vector3 targetPosition;

	protected AIMovementMotor motor;

	protected float updateFrequency = 3f;

	private float updateCounter;

	protected MoveToAIAction()
	{
	}

	public MoveToAIAction(Vector3 targetPosition)
	{
		this.targetPosition = targetPosition;
	}

	public override void OnStart()
	{
		transform = controller.transform;
		motor = controller.GetComponent<AIMovementMotor>();
		if (motor != null)
		{
			stoppingDistance = motor.StoppingDistance;
			ActivatePath();
			base.OnStart();
		}
		else
		{
			Debug.LogError("MoveToAIAction failed: No motor on GameObject");
			status = AIActionStatus.Failed;
		}
	}

	public override Transition Update()
	{
		Transition transition = base.Update();
		if (transition == null)
		{
			updateCounter -= Time.deltaTime;
			if (updateCounter <= 0f)
			{
				OnUpdateCounter(ref transition);
				SetCounter();
			}
		}
		return transition;
	}

	protected virtual void OnUpdateCounter(ref Transition transition)
	{
		if (HaveReachedTarget())
		{
			OnTargetReached(ref transition);
		}
	}

	protected virtual void OnTargetReached(ref Transition transition)
	{
		transition = new Transition(TransitionType.Done);
	}

	protected virtual bool HaveReachedTarget()
	{
		Vector3 position = transform.position;
		float num = Vector3.Distance(position, targetPosition);
		return num <= stoppingDistance;
	}

	protected virtual void ActivatePath()
	{
		motor.MoveToTargetPosition(targetPosition);
	}

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		Transition result = null;
		if (triggeredEvent.ID == AIEventID.UnitDeath)
		{
			result = new Transition(TransitionType.Done);
		}
		else if (triggeredEvent.ID == AIEventID.Relocated)
		{
			ActivatePath();
		}
		else if (triggeredEvent.ID == AIEventID.Knockback && status == AIActionStatus.Active)
		{
			KnockbackAIEvent knockbackEvent = (KnockbackAIEvent)triggeredEvent;
			return new Transition(TransitionType.SuspendFor, new KnockbackAIAction(knockbackEvent));
		}
		return result;
	}

	protected void SetCounter()
	{
		updateCounter = 1f / updateFrequency;
	}

	public override void OnSuspended()
	{
		if (motor != null)
		{
			motor.Stop();
		}
		base.OnSuspended();
	}

	public override void OnResumed()
	{
		ActivatePath();
		SetCounter();
		base.OnResumed();
	}
}
