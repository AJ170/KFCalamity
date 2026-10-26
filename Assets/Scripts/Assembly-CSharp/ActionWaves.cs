using EventHelpers;
using Tutorial;

public class ActionWaves : TutorialSubStepBase
{
	private WaveManager waveManager;

	private GamePhaseManager gamePhaseManager;

	private void OnDestroy()
	{
		EventSystem.Instance.Unsubscribe(this);
	}

	public override void OnStart()
	{
		waveManager = TutorialManager.GameModeRef().WaveManagerRef();
		gamePhaseManager = TutorialManager.GameModeRef().GamePhaseManagerRef();
		EventSystem.Instance.Subscribe(EventId.AllWavesCleared, OnWavesCleared, this);
		waveManager.SetupTutorialWaves();
		gamePhaseManager.SetPhase(new Tutorial.WaveBreakGamePhase(TutorialManager.GameModeRef(), true));
	}

	private void OnWavesCleared(EventBase e)
	{
		SetComplete(true);
	}
}
