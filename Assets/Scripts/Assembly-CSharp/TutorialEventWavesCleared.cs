using EventHelpers;
using UnityEngine;

public class TutorialEventWavesCleared : TutorialEventBase, IMissionCounter
{
	public GameObject ExitArrows;

	private WaveManager waveManager;

	private bool needsUpdate;

	private bool clearedMessageTriggered;

	public bool NeedsUpdate
	{
		get
		{
			return needsUpdate;
		}
		set
		{
			needsUpdate = value;
		}
	}

	public bool ShowCounter
	{
		get
		{
			return true;
		}
	}

	public QuestEntry.EntryType DisplayType
	{
		get
		{
			return QuestEntry.EntryType.Counter;
		}
	}

	private void OnDestroy()
	{
		EventSystem.Instance.Unsubscribe(this);
	}

	public override void OnStart()
	{
		waveManager = TutorialManager.GameModeRef().WaveManagerRef();
		EventSystem.Instance.Subscribe(EventId.AllWavesCleared, OnWavesCleared, this);
		EventSystem.Instance.Subscribe(EventId.WaveUnitsKilled, OnWaveUnitsKilled, this);
	}

	private void OnWavesCleared(EventBase e)
	{
		ExitArrows.SetActive(true);
		SetComplete(true);
	}

	private void OnWaveUnitsKilled(EventBase e)
	{
		NeedsUpdate = true;
		if (!clearedMessageTriggered && waveManager.CurrentWaveIndex() == 0)
		{
			TutorialManager.RunSpeech(Localizer.GetString("tut_commander_waves_cleared"), 0f, null);
			TutorialSpeechManager.Play(TutorialSpeechLine.WavesCleared);
			clearedMessageTriggered = true;
		}
	}

	public int GetCurrentCount()
	{
		return waveManager.CurrentWaveIndex() + 1;
	}

	public int GetMaxCount()
	{
		return waveManager.NumWaves();
	}

	bool IMissionCounter.IsComplete
	{
		get
		{
			return base.IsComplete;
		}
	}
}
