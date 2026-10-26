using Calamity.Input;
using Tutorial;

public class ActionExitTutorial : TutorialSubStepBase
{
	private GamePhaseManager gamePhaseManager;

	public override void OnStart()
	{
		TutorialManager.ToggleQuestLog(false);
		TutorialManager.ToggleTutorialMessage(false);
		Game.Context context = TutorialManager.GameModeRef().GameStateRef().Context;
		gamePhaseManager = TutorialManager.GameModeRef().GamePhaseManagerRef();
		SoldierInputController component = Player.GetComponent<SoldierInputController>();
		component.SetLock(true, this);
		gamePhaseManager.SetPhase(new Tutorial.GameOverGamePhase(context, true));
	}
}
