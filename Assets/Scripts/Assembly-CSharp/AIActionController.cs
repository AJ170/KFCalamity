public class AIActionController
{
	private AIController controller;

	private AIActionStack primaryAction;

	private AIActionStack secondaryAction;

	public AIActionStack Primary
	{
		get
		{
			return primaryAction;
		}
	}

	public AIActionStack Secondary
	{
		get
		{
			return secondaryAction;
		}
	}

	public AIActionController(AIController controller)
	{
		this.controller = controller;
		Setup();
	}

	private void Setup()
	{
		primaryAction = new AIActionStack(controller, this);
		secondaryAction = new AIActionStack(controller, this);
	}

	public void Update()
	{
		primaryAction.Update();
		secondaryAction.Update();
	}

	public void StopAll()
	{
		Shutdown();
	}

	public void Shutdown()
	{
		primaryAction.Shutdown();
		secondaryAction.Shutdown();
	}

	public bool OnAIEvent(AIEvent triggeredEvent)
	{
		bool flag = primaryAction.OnAIEvent(triggeredEvent);
		if (!flag)
		{
			flag = secondaryAction.OnAIEvent(triggeredEvent);
		}
		return flag;
	}
}
