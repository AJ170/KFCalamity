using UnityEngine;

public class CombatAIState : AIState
{
	private AITargetHandler targetHandler;

	private CombatAIAction attackAction;

	public CombatAIState(AIController controller)
		: base("Combat", controller)
	{
	}

	public override void OnEnter()
	{
		targetHandler = controller.TargetHandler;
		if (targetHandler.HasTarget)
		{
			SetupActions();
			return;
		}
		Debug.LogError("CombatAIState has failed: no target");
		status = AIStateStatus.Failed;
	}

	public override bool OnAIEvent(AIEvent triggeredEvent)
	{
		bool flag = base.OnAIEvent(triggeredEvent);
		if (!flag && triggeredEvent.ID != AIEventID.UnitDeath && (triggeredEvent.ID == AIEventID.TargetDied || triggeredEvent.ID == AIEventID.TargetLost))
		{
			GameObject gameObject = controller.TargetHandler.SearchForTarget();
			if (gameObject == null)
			{
				actionController.StopAll();
				status = AIStateStatus.Completed;
			}
			flag = true;
		}
		return flag;
	}

	private void SetupActions()
	{
		attackAction = null;
		if (controller.gameObject.name == "Patriarch")
		{
			attackAction = new PatriarchCombatAIAction();
		}
		else if (controller.gameObject.name == "Scrake")
		{
			attackAction = new ScrakeCombatAIAction();
		}
		else
		{
			attackAction = new CombatAIAction();
		}
		actionController.Primary.Push(attackAction);
	}
}
