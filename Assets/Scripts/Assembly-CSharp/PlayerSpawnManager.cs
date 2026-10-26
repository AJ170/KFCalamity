using System.Collections.Generic;
using GameEvents;
using UnityEngine;

public class PlayerSpawnManager
{
	private List<PlayerSpawnPoint> spawnPoints = new List<PlayerSpawnPoint>();

	private Game.Context context;

	private GameUnitManager gameUnitManager;

	public PlayerSpawnManager(GameState gameState)
	{
		context = gameState.Context;
		gameUnitManager = context.GameUnitManager;
		SetupSpawnPoints();
	}

	private void SetupSpawnPoints()
	{
		spawnPoints.Clear();
		GameObject[] array = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			PlayerSpawnPoint component = gameObject.GetComponent<PlayerSpawnPoint>();
			if (component != null)
			{
				spawnPoints.Add(component);
			}
			else
			{
				Debug.LogWarning("[PlayerSpawnManager:SetupSpawnPoints] Located spawn point with missing script on object " + gameObject.name, gameObject);
			}
		}
		if (spawnPoints.Count == 0)
		{
			Debug.LogWarning("[PlayerSpawnManager:SetupSpawnPoints] Unable to locate any player spawn points!");
		}
	}

	public List<GameObject> SpawnUnitsForPlayers(List<Player> players)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Player player in players)
		{
			if (!player.HasSoldier())
			{
				GameObject item = SpawnUnitForPlayer(player);
				list.Add(item);
			}
		}
		return list;
	}

	public GameObject SpawnUnitForPlayer(Player player)
	{
		string selectedSoldier = player.SelectedSoldier;
		Transform spawnLocationByID = GetSpawnLocationByID(player.ID);
		if (spawnLocationByID != null)
		{
			return SpawnUnitForPlayerAtTransform(player, selectedSoldier, spawnLocationByID);
		}
		return SpawnUnitForPlayerAtLocation(player, selectedSoldier, Vector3.zero, Quaternion.identity);
	}

	public GameObject SpawnUnitForPlayerAtTransform(Player player, string soldier, Transform transform)
	{
		return SpawnUnitForPlayerAtLocation(player, soldier, transform.position, transform.rotation);
	}

	public GameObject SpawnUnitForPlayerAtLocation(Player player, string soldier, Vector3 position)
	{
		return SpawnUnitForPlayerAtLocation(player, soldier, position, Quaternion.identity);
	}

	public GameObject SpawnUnitForPlayerAtLocation(Player player, string soldier, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = gameUnitManager.CreateUnit(soldier, position, rotation);
		SetupPlayerUnit(gameObject, player);
		context.EventSystem.Trigger(new CreateSoldier(gameObject, player));
		return gameObject;
	}

	public Transform GetSpawnLocationByID(int id)
	{
		Transform transform = null;
		if (spawnPoints.Count > 0)
		{
			foreach (PlayerSpawnPoint spawnPoint in spawnPoints)
			{
				if (spawnPoint.PlayerID == id)
				{
					transform = spawnPoint.transform;
					break;
				}
			}
			if (transform == null)
			{
				if (id != -1)
				{
					transform = GetSpawnLocationByID(-1);
				}
				if (transform == null)
				{
					Debug.LogWarning(string.Format("[PlayerSpawnManager:GetSpawnLocationByID] Failed to locate spawn point for player {0} - Using other available spawn point.", id));
					transform = spawnPoints[0].transform;
				}
			}
		}
		else
		{
			Debug.LogWarning("[PlayerSpawnManager:GetSpawnLocationByID] No spawn points available for players!");
		}
		return transform;
	}

	private void SetupPlayerUnit(GameObject spawnedUnit, Player player)
	{
		PlayerGUILink component = spawnedUnit.GetComponent<PlayerGUILink>();
		if (component != null && player.PlayerGUI != null)
		{
			component.SetGUI(player.PlayerGUI);
		}
		Soldier component2 = spawnedUnit.GetComponent<Soldier>();
		if (component2 != null)
		{
			component2.Setup(player.ID, player.InputID);
		}
		player.SetSoldier(spawnedUnit);
	}
}
