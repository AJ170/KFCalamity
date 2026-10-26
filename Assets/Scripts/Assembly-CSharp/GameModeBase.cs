public class GameModeBase
{
	protected GameState gameState;

	public GameModeBase(GameState gameState)
	{
		this.gameState = gameState;
	}

	public virtual GameState GameStateRef()
	{
		return null;
	}

	public virtual GamePhaseManager GamePhaseManagerRef()
	{
		return null;
	}

	public virtual WaveManager WaveManagerRef()
	{
		return null;
	}

	public virtual PlayerSpawnManager PlayerSpawnManagerRef()
	{
		return null;
	}

	public virtual void Shutdown()
	{
	}

	public virtual void Start()
	{
	}

	public virtual void Stop()
	{
	}

	public virtual void Reset()
	{
	}

	public virtual void Update()
	{
	}

	public virtual bool NeedRespawn()
	{
		return false;
	}

	public virtual void RespawnPlayers()
	{
	}
}
