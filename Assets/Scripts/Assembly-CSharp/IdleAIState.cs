using UnityEngine;

public class IdleAIState : AIState
{
	private float targetSearchFrequency = 1f;

	private float targetSearchCountdown;

	public IdleAIState(AIController controller)
		: base("Idle", controller)
	{
	}

	public override void OnEnter()
	{
		float num = 1f / targetSearchFrequency;
		float num2 = num / 2f;
		targetSearchCountdown = num + Random.Range(0f - num2, num2);
	}

	public override void OnUpdate()
	{
		targetSearchCountdown -= Time.deltaTime;
		if (targetSearchCountdown <= 0f)
		{
			SearchForTarget();
			targetSearchCountdown = 1f / targetSearchFrequency;
		}
	}

	public override bool OnAIEvent(AIEvent triggeredEvent)
	{
		bool flag = base.OnAIEvent(triggeredEvent);
		if (!flag)
		{
			if (triggeredEvent.ID == AIEventID.UnitDeath)
			{
				status = AIStateStatus.Completed;
				flag = true;
			}
			else if (triggeredEvent.ID == AIEventID.TargetGained || triggeredEvent.ID == AIEventID.TargetChanged)
			{
				controller.StateManager.SetActiveState(new CombatAIState(controller));
				status = AIStateStatus.Completed;
				flag = true;
			}
		}
		return flag;
	}

	private void SearchForTarget()
	{
		controller.TargetHandler.SearchForTarget();
	}
}
