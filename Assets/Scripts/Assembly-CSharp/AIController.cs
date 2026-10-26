using EventHelpers;
using UnityEngine;

public class AIController : MonoBehaviour
{
	private AIStateManager stateManager;

	private AITargetHandler targetHandler;

	private EffectEventDelegator eventDelegator;

	public AIStateManager StateManager
	{
		get
		{
			return stateManager;
		}
	}

	public AITargetHandler TargetHandler
	{
		get
		{
			return targetHandler;
		}
	}

	private void Start()
	{
		stateManager = new AIStateManager(this);
		targetHandler = GetComponent<AITargetHandler>();
		eventDelegator = GetComponent<EffectEventDelegator>();
	}

	private void Update()
	{
		stateManager.Update();
	}

	public void TriggerAIEvent(AIEvent triggeredEvent)
	{
		stateManager.OnAIEvent(triggeredEvent);
	}

	public void TriggerGlobalEvent(TriggerBase e)
	{
		if (eventDelegator != null)
		{
			eventDelegator.TriggerInternal(e);
		}
	}

	private void OnDeath(Health health)
	{
		if (base.enabled)
		{
			targetHandler.ClearTarget();
			TriggerAIEvent(new UnitDeathAIEvent());
			stateManager.SetActiveState(new DeadAIState());
		}
	}
}
