using UnityEngine.AI;
using System.Collections.Generic;
using EventHelpers;
using Tutorial;
using UnityEngine;

public class GameModeShootingRange : GameModeBase
{
	private EventSystem eventSystem;

	private GamePhaseManager gamePhaseManager;

	private WaveManager waveManager;

	private PlayerSpawnManager playerSpawnManager;

	private TutorialManager tutorialManager;

	public GameModeShootingRange(GameState gameState)
		: base(gameState)
	{
		eventSystem = gameState.Context.EventSystem;
		gamePhaseManager = new GamePhaseManager();
		waveManager = new WaveManager(eventSystem);
		playerSpawnManager = new PlayerSpawnManager(gameState);
		eventSystem.Subscribe(EventId.AllSoldiersKilled, OnGameEvent, this);
	}

	public override void Shutdown()
	{
		gamePhaseManager.Shutdown();
		waveManager.Shutdown();
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
		PickupManager.Instance.SetupStaticPickups();
		SetupPlayers();
	}

	public override void Update()
	{
		gamePhaseManager.Update();
	}

	private void OnGameEvent(EventBase e)
	{
		if (e.Id == EventId.AllSoldiersKilled)
		{
			gamePhaseManager.SetPhase(new Tutorial.GameOverGamePhase(gameState.Context, false));
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
		foreach (Player item in allPlayers)
		{
			if (!item.HasSoldier())
			{
				Vector3 source = instance2.RandomSoldierPosition();
				Vector3 position = RespawnPosition(source);
				playerSpawnManager.SpawnUnitForPlayerAtLocation(item, item.SelectedSoldier, position);
			}
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
