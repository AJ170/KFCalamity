using UnityEngine;

public class GameModeManager
{
	private GameState gameState;

	private GameModeBase gameMode;

	private float startTime;

	private float elapsedTime;

	public GameModeBase GameMode
	{
		get
		{
			return gameMode;
		}
	}

	public float ElapsedTime
	{
		get
		{
			return elapsedTime;
		}
	}

	public GameModeManager(GameState game)
	{
		gameState = game;
	}

	public void Shutdown()
	{
		if (gameMode != null)
		{
			gameMode.Shutdown();
		}
	}

	public void SetGameMode(GameModeSetting gameMode)
	{
		switch (gameMode)
		{
		case GameModeSetting.Survival:
			this.gameMode = new GameModeSurvival(gameState);
			break;
		case GameModeSetting.Tutorial:
			this.gameMode = new GameModeTutorial(gameState);
			break;
		case GameModeSetting.ShootingRange:
			this.gameMode = new GameModeShootingRange(gameState);
			break;
		case GameModeSetting.None:
			Debug.LogError("GameModeManager:SetGameMode() Failed");
			break;
		}
	}

	public void Start()
	{
		if (gameMode != null)
		{
			gameMode.Start();
			startTime = Time.time;
		}
	}

	public void Stop()
	{
		if (gameMode != null)
		{
			gameMode.Stop();
		}
	}

	public void Reset()
	{
		if (gameMode != null)
		{
			gameMode.Reset();
		}
	}

	public void Update()
	{
		if (gameMode != null)
		{
			elapsedTime = Time.time - startTime;
			gameMode.Update();
		}
	}
}
