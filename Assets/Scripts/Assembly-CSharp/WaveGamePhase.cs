using EventHelpers;
using GameEvents;
using UnityEngine;

public class WaveGamePhase : GamePhaseBase
{
	protected GameModeBase gameMode;

	protected WaveManager waveManager;

	protected GuiInGameManager guiManager;

	protected EventSystem eventSystem;

	public WaveGamePhase(GameModeBase gameMode)
		: base(gameMode.GamePhaseManagerRef())
	{
		this.gameMode = gameMode;
		waveManager = gameMode.WaveManagerRef();
		Game.Context context = gameMode.GameStateRef().Context;
		guiManager = context.GUIManager;
		eventSystem = context.EventSystem;
		eventSystem.Subscribe(EventId.WaveUnitsKilled, OnWaveUnitsKilled, this);
	}

	public override void Shutdown()
	{
		eventSystem.Unsubscribe(this);
	}

	public override void OnEnter()
	{
		waveManager.NextWave();
		eventSystem.Trigger(new WaveStart(waveManager.CurrentWave()));
		SetWaveInfo();
		DisplayWaveMessage();
	}

	protected virtual void SetWaveInfo()
	{
		int wave = waveManager.CurrentWaveIndex() + 1;
		if (guiManager != null)
		{
			guiManager.SetWave(wave, waveManager.NumWaves());
			guiManager.SetEnemiesLeft(waveManager.CurrentWave().TotalEnemies);
			guiManager.OnWaveSpawn();
		}
	}

	protected virtual void DisplayWaveMessage()
	{
		int num = waveManager.CurrentWaveIndex() + 1;
		if (guiManager != null)
		{
			guiManager.DisplayMessage(Localizer.GetString("gui_message_wave") + " " + num, 3f, MessageBoard.FontSize.Large);
		}
	}

	private void OnWaveUnitsKilled(EventBase e)
	{
		if (waveManager.AtLastWave())
		{
			eventSystem.Trigger(new AllWavesCleared());
		}
		else
		{
			GotoWaveBreakPhase();
		}
	}

	public virtual void GotoWaveBreakPhase()
	{
		Debug.LogWarning("WaveBaseGamePhase:GotoWaveBreakPhase() Implement this method");
	}

	public override void OnExit()
	{
		eventSystem.Trigger(new WaveEnd(waveManager.CurrentWave()));
		eventSystem.Unsubscribe(EventId.WaveUnitsKilled, this);
	}
}
