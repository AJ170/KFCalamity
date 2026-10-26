using UnityEngine;

public class ReloadMainMenu : StateBase
{
	private Game.Context context;

	private GameSettings gameSettings;

	private CommandQueue commandQueue;

	private bool goToLevelSelect;

	public ReloadMainMenu(Game.Context context, GameSettings gameSettings, bool levelSelect = false)
	{
		this.context = context;
		this.gameSettings = gameSettings;
		commandQueue = new CommandQueue();
		goToLevelSelect = levelSelect;
	}

	public override void OnActive()
	{
		context.LevelManager.LoadLevelAsync("reset", OnResetSceneLoaded);
	}

	private void OnResetSceneLoaded(int asyncId)
	{
		commandQueue.Add(GotoNewMainMenu, 1f);
		Resources.UnloadUnusedAssets();
	}

	private void GotoNewMainMenu()
	{
		StateManager stateManager = context.StateManager;
		MainMenu mainMenu = new MainMenu(context, true, goToLevelSelect);
		mainMenu.LastGameSettings = gameSettings;
		stateManager.AddState("MainMenu", mainMenu);
		stateManager.ChangeState("MainMenu");
	}

	public override void OnInactive()
	{
	}

	public override void Update()
	{
		commandQueue.Update();
	}
}
