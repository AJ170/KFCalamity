public class GameOverGamePhase : GamePhaseBase
{
	protected Game.Context context;

	protected bool victorious;

	protected CommandQueue commandQueue;

	public GameOverGamePhase(Game.Context context, bool victorious)
		: base(null)
	{
		this.context = context;
		this.victorious = victorious;
		commandQueue = new CommandQueue();
	}

	public override void Update()
	{
		commandQueue.Update();
	}

	protected virtual void DisplayEndScreen()
	{
		if (context.GUIManager != null)
		{
			EndScreenType endType = (victorious ? EndScreenType.SurvivalWin : EndScreenType.SurvivalLose);
			context.GUIManager.DisplayEndScreen(endType);
		}
		else
		{
			GameState.Instance.ReloadMainMenu();
		}
	}

	private void SetPlayerInputLock(bool lockInput)
	{
		if (SoldierManager.Instance != null)
		{
			SoldierManager.Instance.SetLockedInput(lockInput, this);
		}
	}

	protected void LockPlayerInput()
	{
		SetPlayerInputLock(true);
	}
}
