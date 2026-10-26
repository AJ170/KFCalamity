using UnityEngine;

public class MainMenu : StateBase
{
	private Game.Context context;

	private GameObject guiManagerObject;

	private GuiMenuManager guiMenuManager;

	private GameSettings lastGameSettings;

	private bool loadMenuScene;

	private bool goToLevelSelect;

	public GameSettings LastGameSettings
	{
		get
		{
			return lastGameSettings;
		}
		set
		{
			lastGameSettings = value;
		}
	}

	public MainMenu(Game.Context gameContext, bool loadMenuScene, bool levelSelect = false)
	{
		context = gameContext;
		this.loadMenuScene = loadMenuScene;
		goToLevelSelect = levelSelect;
	}

	public override void OnActive()
	{
		context.PlayerManager.RemoveAllPlayers();
		if (loadMenuScene)
		{
			context.LevelManager.LoadLevelAsync("menu", OnMenuSceneLoaded);
		}
	}

	public void OnMenuSceneLoaded(int asyncId)
	{
		guiManagerObject = GameObject.Find("GuiMenuManager");
		guiMenuManager = guiManagerObject.GetComponent<GuiMenuManager>();
		guiMenuManager.Setup(this, context.PlayerManager, lastGameSettings);
		if (goToLevelSelect)
		{
			guiMenuManager.LoadState(MenuStates.GameSetup, true);
		}
	}

	public override void Update()
	{
	}

	public void OnGameStart(GameSettings gameSettings)
	{
		StateManager stateManager = context.StateManager;
		stateManager.AddState("GameState", new GameState(context, gameSettings));
		stateManager.ChangeState("GameState");
	}
}
