public abstract class AIState
{
	protected AIActionController actionController;

	protected AIController controller;

	protected string name;

	protected AIStateStatus status;

	public string Name
	{
		get
		{
			return name;
		}
	}

	public AIStateStatus Status
	{
		get
		{
			return status;
		}
	}

	public AIActionController ActionController
	{
		get
		{
			return actionController;
		}
	}

	public AIState(string name, AIController controller)
	{
		this.name = name;
		this.controller = controller;
		Setup();
	}

	private void Setup()
	{
		actionController = new AIActionController(controller);
	}

	public void Update()
	{
		actionController.Update();
		OnUpdate();
	}

	public virtual void OnEnter()
	{
	}

	public virtual void OnUpdate()
	{
	}

	public virtual void OnExit()
	{
		actionController.Shutdown();
	}

	public virtual bool OnAIEvent(AIEvent triggeredEvent)
	{
		return actionController.OnAIEvent(triggeredEvent);
	}

	public void SetController(AIController controller)
	{
		this.controller = controller;
	}

	public bool FailedOrFinished()
	{
		return status == AIStateStatus.Failed || status == AIStateStatus.Completed;
	}
}
