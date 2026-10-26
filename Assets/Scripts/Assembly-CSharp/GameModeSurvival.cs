using UnityEngine.AI;
using System.Collections.Generic;
using EventHelpers;
using Survival;
using UnityEngine;

public class GameModeSurvival : GameModeBase
{
	private EventSystem eventSystem;

	private GamePhaseManager gamePhaseManager;

	private WaveManager waveManager;

	private PlayerSpawnManager playerSpawnManager;

	private WeaponDropManager weaponDropManager;

	public GameModeSurvival(GameState gameState)
		: base(gameState)
	{
		eventSystem = gameState.Context.EventSystem;
		gamePhaseManager = new GamePhaseManager();
		GameSettings gameSettings = gameState.GameSettings;
		waveManager = new WaveManager(eventSystem);
		waveManager.SetupTemplateWaves(gameSettings.GameMode);
		waveManager.SetupSurvivalWaves(gameSettings.GameLength);
		playerSpawnManager = new PlayerSpawnManager(gameState);
		weaponDropManager = new WeaponDropManager();
		eventSystem.Subscribe(EventId.AllSoldiersKilled, OnGameEvent, this);
		eventSystem.Subscribe(EventId.AllWavesCleared, OnGameEvent, this);
	}

	public override void Shutdown()
	{
		gamePhaseManager.Shutdown();
		waveManager.Shutdown();
		weaponDropManager.Shutdown();
		eventSystem.Unsubscribe(this);
	}

	public override GameState GameStateRef()
	{
		return gameState;
	}

	public override GamePhaseManager GamePhaseManagerRef()
	{
		return gamePhaseManager;
	}

	public override WaveManager WaveManagerRef()
	{
		return waveManager;
	}

	public override PlayerSpawnManager PlayerSpawnManagerRef()
	{
		return playerSpawnManager;
	}

	public override void Start()
	{
		SetupPlayers();
		gamePhaseManager.SetPhase(new Survival.WaveBreakGamePhase(this));
	}

	public override void Update()
	{
		gamePhaseManager.Update();
	}

	private void OnGameEvent(EventBase e)
	{
		if (e.Id == EventId.AllSoldiersKilled || e.Id == EventId.AllWavesCleared)
		{
			bool victorious = e.Id == EventId.AllWavesCleared;
			gamePhaseManager.SetPhase(new Survival.GameOverGamePhase(gameState.Context, victorious));
		}
	}

	private void SetupPlayers()
	{
		PlayerManager instance = PlayerManager.Instance;
		List<Player> allPlayers = instance.GetAllPlayers();
		foreach (Player item in allPlayers)
		{
			item.CreateIngameGUI(gameState.Context.GUIManager);
		}
		playerSpawnManager.SpawnUnitsForPlayers(allPlayers);
	}

	public override void RespawnPlayers()
	{
		if (!NeedRespawn())
		{
			return;
		}
		PlayerManager instance = PlayerManager.Instance;
		SoldierManager instance2 = SoldierManager.Instance;
		List<Player> allPlayers = instance.GetAllPlayers();
		SpeechManager.PlaySpeech(SpeechManager.SpeechLine.PlayerRespawn);
		foreach (Player item in allPlayers)
		{
			if (item.HasSoldier())
			{
				continue;
			}
			Vector3 source = instance2.RandomSoldierPosition();
			Vector3 vector = RespawnPosition(source);
			if (vector == Vector3.zero)
			{
				Transform spawnLocationByID = playerSpawnManager.GetSpawnLocationByID(item.ID);
				if (spawnLocationByID != null)
				{
					vector = spawnLocationByID.position;
				}
			}
			playerSpawnManager.SpawnUnitForPlayerAtLocation(item, item.SelectedSoldier, vector);
		}
	}

	private Vector3 RespawnPosition(Vector3 source)
	{
		Vector3 result = source;
		List<Vector3> list = GlobalHelpers.CircleVectors(source, 5, 2f);
		foreach (Vector3 item in list)
		{
			NavMeshHit hit;
			NavMesh.SamplePosition(item, out hit, 0.5f, 1);
			if (hit.hit)
			{
				result = hit.position;
				break;
			}
		}
		return result;
	}

	public override bool NeedRespawn()
	{
		return PlayerManager.Instance.NumberOfPlayers != SoldierManager.Instance.NumAliveSoldiers;
	}
}
