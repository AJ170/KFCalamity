using AIActionTransitions;
using UnityEngine;

internal class HoldGroundAIAction : AIAction
{
	private float duration;

	private float counter;

	private AIMovementMotor motor;

	private bool movingOnStart;

	public HoldGroundAIAction()
		: this(-1f)
	{
	}

	public HoldGroundAIAction(float duration)
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
			counter = duration;
			base.OnStart();
		}
		else
		{
			Debug.LogWarning("MoveToAIAction failed: No motor on GameObject");
			status = AIActionStatus.Failed;
		}
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
		movingOnStart = motor.Moving;
		if (movingOnStart)
		{
			motor.Stop();
		}
		base.OnResumed();
	}

	public override void OnSuspended()
	{
		if (movingOnStart)
		{
			motor.Resume();
		}
		base.OnSuspended();
	}

	public override void OnEnd()
	{
		if (movingOnStart)
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

	public void ExtendDuration(float time)
	{
		counter += time;
	}
}
