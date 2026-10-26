using Tutorial;

public class ActionStartWaves : TutorialSubStepBase
{
	private WaveManager waveManager;

	private GamePhaseManager gamePhaseManager;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void OnStart()
	{
		waveManager = TutorialManager.GameModeRef().WaveManagerRef();
		gamePhaseManager = TutorialManager.GameModeRef().GamePhaseManagerRef();
		waveManager.SetupTutorialWaves();
		gamePhaseManager.SetPhase(new Tutorial.WaveBreakGamePhase(TutorialManager.GameModeRef(), true));
		SetComplete(true);
	}
}
