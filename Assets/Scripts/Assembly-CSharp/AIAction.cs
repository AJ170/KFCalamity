using AIActionTransitions;

public class AIAction
{
	protected AIController controller;

	protected AIActionController actionController;

	protected AIActionType type;

	protected AIActionStatus status;

	public AIActionType Type
	{
		get
		{
			return type;
		}
	}

	public AIActionStatus Status
	{
		get
		{
			return status;
		}
	}

	public virtual void OnStart()
	{
		if (status != AIActionStatus.Complete)
		{
			status = AIActionStatus.Active;
		}
	}

	public virtual Transition Update()
	{
		if (IsDone())
		{
			return new Transition(TransitionType.Done);
		}
		return null;
	}

	public virtual void OnSuspended()
	{
		if (status == AIActionStatus.Active)
		{
			status = AIActionStatus.Suspended;
		}
	}

	public virtual void OnResumed()
	{
		if (status == AIActionStatus.Suspended)
		{
			status = AIActionStatus.Active;
		}
	}

	public virtual void OnEnd()
	{
		status = AIActionStatus.Complete;
	}

	public virtual void Abort()
	{
		status = AIActionStatus.Complete;
	}

	public virtual Transition OnAIEvent(AIEvent triggeredEvent)
	{
		return null;
	}

	public void SetController(AIController controller, AIActionController actionController)
	{
		this.controller = controller;
		this.actionController = actionController;
	}

	public bool IsDone()
	{
		return status == AIActionStatus.Complete || status == AIActionStatus.Failed;
	}
}
