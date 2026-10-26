public class GamePhaseManager
{
	private GamePhaseBase gamePhase;

	public void Shutdown()
	{
		if (gamePhase != null)
		{
			gamePhase.Shutdown();
		}
	}

	public void SetPhase(GamePhaseBase phase)
	{
		if (gamePhase != null)
		{
			gamePhase.OnExit();
		}
		gamePhase = phase;
		gamePhase.OnEnter();
	}

	public void Update()
	{
		if (gamePhase != null)
		{
			gamePhase.Update();
		}
	}
}
