public class GamePhaseBase
{
	protected GamePhaseManager gamePhaseManager;

	public GamePhaseBase(GamePhaseManager gamePhaseManager)
	{
		this.gamePhaseManager = gamePhaseManager;
	}

	public virtual void Shutdown()
	{
	}

	public virtual void OnEnter()
	{
	}

	public virtual void OnExit()
	{
	}

	public virtual void Update()
	{
	}
}
