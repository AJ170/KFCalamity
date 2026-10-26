using UnityEngine;

public class GameState : StateBase
{
	private static GameState instance;

	private Game.Context context;

	private GameSettings gameSettings;

	private ScalingByGameSettings scalingByGameSettings;

	private GameModeManager gameModeManager;

	private BuildRuntimeSettings buildSettings;

	public static GameState Instance
	{
		get
		{
			return instance;
		}
	}

	public Game.Context Context
	{
		get
		{
			return context;
		}
	}

	public GameModeManager GameModeManager
	{
		get
		{
			return gameModeManager;
		}
	}

	public GameSettings GameSettings
	{
		get
		{
			return gameSettings;
		}
	}

	public GameState(Game.Context context, GameSettings gameSettings)
	{
		this.context = context;
		this.gameSettings = gameSettings;
		scalingByGameSettings = new ScalingByGameSettings(gameSettings, context.EventSystem);
		gameModeManager = new GameModeManager(this);
		buildSettings = BuildRuntimeSettings.Instance;
	}

	public override void Shutdown()
	{
		scalingByGameSettings.Shutdown();
		gameModeManager.Shutdown();
		MetaManager.Instance.StopAutoSaveStats();
	}

	public override void OnActive()
	{
		instance = this;
		if (DeviceSwitchController.CurrentDeviceMode != DeviceMode.Touch)
		{
			CursorManager.SetCursorByType(CursorType.InGame);
		}
		context.LevelManager.LoadLevelAsync("load", OnLoadSceneLoaded);
	}

	private void OnLoadSceneLoaded(int asyncId)
	{
		context.LevelManager.LoadLevelAsync("game", OnGameSceneLoaded);
	}

	private void OnGameSceneLoaded(int asyncId)
	{
		GameObject gameObject = GameObject.Find("GameUnitManager");
		GameObject gameObject2 = GameObject.Find("GuiIngameManager");
		if (gameObject != null)
		{
			context.GameUnitManager = gameObject.GetComponent<GameUnitManager>();
			context.GameUnitManager.SetContext(context);
		}
		else
		{
			Debug.LogError("[GameState:OnGameSceneLoaded] Failed to locate GameUnitManager");
		}
		if (gameObject2 != null)
		{
			context.GUIManager = gameObject2.GetComponent<GuiInGameManager>();
		}
		else
		{
			Debug.LogError("[GameState:OnGameSceneLoaded] Failed to locate GuiIngameManager");
		}
		GA.API.Design.NewEvent("Level Played:" + gameSettings.LevelName);
		context.LevelManager.LoadLevelAsync(gameSettings.LevelName, OnLevelLoaded);
	}

	public void OnLevelLoaded(int asyncId)
	{
		gameModeManager.SetGameMode(gameSettings.GameMode);
		gameModeManager.Start();
		MetaManager.Instance.StartAutoSaveStats();
	}

	public override void OnInactive()
	{
		context.Game.ResetSystems();
		MetaManager.Instance.StopAutoSaveStats();
		context.GameUnitManager = null;
		context.GUIManager = null;
		instance = null;
	}

	public override void Update()
	{
		gameModeManager.Update();
	}

	public void ReloadGameState()
	{
		StateManager stateManager = context.StateManager;
		stateManager.AddState("ReloadGameState", new ReloadGameState(context, gameSettings));
		stateManager.ChangeState("ReloadGameState");
	}

	public void ReloadMainMenu(bool levelSelect = false)
	{
		StateManager stateManager = context.StateManager;
		stateManager.AddState("ReloadMainMenu", new ReloadMainMenu(context, gameSettings, levelSelect));
		stateManager.ChangeState("ReloadMainMenu");
	}
}
