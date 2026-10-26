public class AIStateManager
{
	private AIState activeState;

	private AIController controller;

	public AIState ActiveState
	{
		get
		{
			return activeState;
		}
	}

	public AIStateManager(AIController controller)
	{
		this.controller = controller;
		SetDefaultState();
	}

	public void Update()
	{
		if (activeState != null)
		{
			AIStateStatus status = activeState.Status;
			if (status == AIStateStatus.Failed || status == AIStateStatus.Completed)
			{
				SetDefaultState();
			}
			else
			{
				activeState.Update();
			}
		}
	}

	public void SetActiveState(AIState state)
	{
		if (activeState != null)
		{
			activeState.OnExit();
		}
		activeState = state;
		activeState.SetController(controller);
		activeState.OnEnter();
	}

	public bool OnAIEvent(AIEvent triggeredEvent)
	{
		if (activeState != null)
		{
			return activeState.OnAIEvent(triggeredEvent);
		}
		return false;
	}

	private void SetDefaultState()
	{
		SetActiveState(new IdleAIState(controller));
	}
}
