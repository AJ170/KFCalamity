using AIActionTransitions;
using UnityEngine;

internal class KnockbackAIAction : AIAction
{
	private AIMovementMotor motor;

	private Rigidbody body;

	private bool lastKinematic;

	private KnockbackAIEvent knockbackEvent;

	private float timer;

	public KnockbackAIAction(KnockbackAIEvent knockbackEvent)
	{
		this.knockbackEvent = knockbackEvent;
	}

	public override void OnStart()
	{
		base.OnStart();
		motor = controller.GetComponent<AIMovementMotor>();
		if (motor == null)
		{
			Debug.LogError("RampageChargeAIAction:OnStart() Failed: No motor on GameObject");
			status = AIActionStatus.Failed;
		}
		else if (knockbackEvent != null)
		{
			StopMotor();
			Knockback();
		}
		else
		{
			Debug.LogError("RampageChargeAIAction:KnockbackAIAction() Failed: No knockback-event");
			status = AIActionStatus.Failed;
		}
	}

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		if (triggeredEvent.ID == AIEventID.Knockback && status == AIActionStatus.Active)
		{
			StackKnockback(triggeredEvent as KnockbackAIEvent);
		}
		return null;
	}

	public override Transition Update()
	{
		Transition transition = base.Update();
		if (transition == null && timer > 0f)
		{
			timer = Mathf.Max(timer - Time.deltaTime, 0f);
			if (timer == 0f)
			{
				status = AIActionStatus.Complete;
			}
		}
		return transition;
	}

	private void StopMotor()
	{
		motor.Stop(true);
	}

	private void ResumeMotor()
	{
		motor.Resume();
	}

	private void Knockback()
	{
		body = controller.GetComponent<Rigidbody>();
		if (body != null)
		{
			lastKinematic = body.isKinematic;
			body.isKinematic = false;
			body.velocity = knockbackEvent.Direction * knockbackEvent.Force;
			timer = knockbackEvent.Duration;
		}
	}

	private void StackKnockback(KnockbackAIEvent knockbackEvent)
	{
		if (body != null)
		{
			body.velocity += knockbackEvent.Direction * knockbackEvent.Force;
			timer = (timer + knockbackEvent.Duration) * 0.5f;
		}
	}

	public override void OnEnd()
	{
		ResumeMotor();
		if (body != null)
		{
			body.isKinematic = lastKinematic;
		}
	}
}
