using UnityEngine;

public class ReloadGameState : StateBase
{
	private Game.Context context;

	private GameSettings gameSettings;

	private CommandQueue commandQueue;

	public ReloadGameState(Game.Context context, GameSettings gameSettings)
	{
		this.context = context;
		this.gameSettings = gameSettings;
		commandQueue = new CommandQueue();
	}

	public override void OnActive()
	{
		context.LevelManager.LoadLevelAsync("reset", OnResetSceneLoaded);
	}

	private void OnResetSceneLoaded(int asyncId)
	{
		commandQueue.Add(GotoNewGameState, 1f);
		Resources.UnloadUnusedAssets();
	}

	private void GotoNewGameState()
	{
		StateManager stateManager = context.StateManager;
		stateManager.AddState("GameState", new GameState(context, gameSettings));
		stateManager.ChangeState("GameState");
	}

	public override void OnInactive()
	{
	}

	public override void Update()
	{
		commandQueue.Update();
	}
}
