using EventHelpers;
using Tutorial;
using UnityEngine;

public class ActionNewWave : MonoBehaviour
{
	public int NewWaveTimer = 5;

	public LayerMask AllowedObjects;

	public GameObject ExitArrows;

	private float timer;

	private bool hasSpawned;

	private bool initialized;

	private WaveManager waveManager;

	private GamePhaseManager gamePhaseManager;

	private GuiInGameManager guiManager;

	private TutorialManager tutorialManager;

	private ActionOpenDoors openDoor;

	private ActionCloseDoors closeDoor;

	private ActionToggleParticleEffects toggleParticles;

	private bool canSpawn = true;

	private void Update()
	{
		if (timer > 0f)
		{
			int num = Mathf.CeilToInt(timer);
			timer = Mathf.Max(0f, timer - Time.deltaTime);
			int num2 = Mathf.CeilToInt(timer);
			if (num > num2)
			{
				guiManager.DisplayMessage(Localizer.GetString("gui_message_wave_in") + " " + num2, 10f);
			}
			if (timer == 0f)
			{
				SpawnNewWave();
				timer = -1f;
			}
		}
	}

	private void OnWavesCleared(EventBase e)
	{
		openDoor.SetComplete(false);
		openDoor.OnStart();
		toggleParticles.OnStart();
		tutorialManager.RunSpeech(Localizer.GetString("tut_commander_new_waves_done"), 0f, null);
		TutorialSpeechManager.Play(TutorialSpeechLine.WavesMoreDone);
		ExitArrows.SetActive(true);
		canSpawn = true;
	}

	private void Setup()
	{
		if (!initialized)
		{
			tutorialManager = base.gameObject.GetComponentInRoot<TutorialManager>();
			waveManager = tutorialManager.GameModeRef().WaveManagerRef();
			gamePhaseManager = tutorialManager.GameModeRef().GamePhaseManagerRef();
			guiManager = tutorialManager.GuiManager;
			initialized = true;
			TutorialSubStepBase[] components = GetComponents<TutorialSubStepBase>();
			TutorialSubStepBase[] array = components;
			foreach (TutorialSubStepBase tutorialSubStepBase in array)
			{
				tutorialSubStepBase.enabled = true;
				tutorialSubStepBase.SetTutorialManager(tutorialManager);
				tutorialSubStepBase.SetPlayer(tutorialManager.Soldier);
			}
			openDoor = GetComponent<ActionOpenDoors>();
			closeDoor = GetComponent<ActionCloseDoors>();
			toggleParticles = GetComponent<ActionToggleParticleEffects>();
			EventSystem.Instance.Subscribe(EventId.AllWavesCleared, OnWavesCleared, this);
		}
	}

	private void OnDestroy()
	{
		EventSystem.Instance.Unsubscribe(this);
	}

	private void OnTriggerEnter(Collider collider)
	{
		if ((AllowedObjects.value & (1 << collider.gameObject.layer)) != 0 && canSpawn)
		{
			Setup();
			timer = NewWaveTimer;
			hasSpawned = false;
			guiManager.DisplayMessage(Localizer.GetString("gui_message_wave_in") + " " + Mathf.CeilToInt(timer), 10f);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if ((AllowedObjects.value & (1 << collider.gameObject.layer)) != 0)
		{
			timer = -1f;
			if (!hasSpawned)
			{
				guiManager.DisplayMessage(Localizer.GetString("gui_message_abort"), 2f);
			}
		}
	}

	private void SpawnNewWave()
	{
		waveManager.SetupTutorialWaves();
		gamePhaseManager.SetPhase(new Tutorial.WaveBreakGamePhase(tutorialManager.GameModeRef(), false));
		hasSpawned = true;
		guiManager.DisplayMessage(Localizer.GetString("gui_message_start_round"), 3f);
		closeDoor.SetComplete(false);
		closeDoor.OnStart();
		toggleParticles.OnStart();
		ExitArrows.SetActive(false);
		canSpawn = false;
	}
}
