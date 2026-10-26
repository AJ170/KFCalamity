using UnityEngine;

public class StartUpState : StateBase
{
	private Game.Context context;

	private GameObject GUIManager;

	private int menuAsyncId;

	private MainMenu mainMenuState;

	public StartUpState(Game.Context gameContext)
	{
		context = gameContext;
		mainMenuState = new MainMenu(gameContext, false);
	}

	public override void OnActive()
	{
		if (DeviceSwitchController.CurrentDeviceMode != DeviceMode.Touch)
		{
			CursorManager.SetCursorByType(CursorType.Menu);
		}
		context.LevelManager.LoadLevelAsync("startup", OnStartUpSceneLoaded);
		menuAsyncId = context.LevelManager.LoadLevelAsync("menu", mainMenuState.OnMenuSceneLoaded, false);
	}

	private void OnStartUpSceneLoaded(int asyncId)
	{
		GUIManager = GameObject.Find("GuiStartUpManager");
		GUIManager.GetComponent<GuiStartUpManager>().Setup(this);
	}

	public override void Update()
	{
	}

	public void OnMenuStart()
	{
		context.LevelManager.ActivateLoadedScene(menuAsyncId);
		StateManager stateManager = context.StateManager;
		stateManager.AddState("MainMenu", mainMenuState);
		stateManager.ChangeState("MainMenu");
	}
}
