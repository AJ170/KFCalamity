using AIActionTransitions;
using UnityEngine;

internal class WaitAIAction : AIAction
{
	private float duration;

	private float counter;

	private AIMovementMotor motor;

	private bool movingOnStart;

	public WaitAIAction()
		: this(-1f)
	{
	}

	public WaitAIAction(float duration)
	{
		this.duration = duration;
	}

	public override void OnStart()
	{
		motor = controller.GetComponent<AIMovementMotor>();
		if (motor != null)
		{
			movingOnStart = motor.Moving;
			if (movingOnStart)
			{
				motor.Stop();
			}
		}
		counter = duration;
		base.OnStart();
	}

	public override Transition Update()
	{
		counter -= Time.deltaTime;
		if (counter <= 0f)
		{
			status = AIActionStatus.Complete;
		}
		return base.Update();
	}

	public override void OnResumed()
	{
		if (motor != null)
		{
			movingOnStart = motor.Moving;
			if (movingOnStart)
			{
				motor.Stop();
			}
		}
		base.OnResumed();
	}

	public override void OnSuspended()
	{
		if (motor != null && movingOnStart)
		{
			motor.Resume();
		}
		base.OnSuspended();
	}

	public override void OnEnd()
	{
		if (motor != null && movingOnStart)
		{
			motor.Resume();
		}
	}

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		if (triggeredEvent.ID == AIEventID.UnitDeath)
		{
			return new Transition(TransitionType.Done);
		}
		return null;
	}
}
