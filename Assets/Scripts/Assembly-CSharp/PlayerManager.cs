using System.Collections.Generic;
using GameEvents;
using UnityEngine;

public class PlayerManager
{
	private static PlayerManager instance;

	private EventSystem eventSystem;

	public int MaxPlayers = 1;

	private List<Player> players = new List<Player>();

	public static PlayerManager Instance
	{
		get
		{
			return instance;
		}
	}

	public int NumberOfPlayers
	{
		get
		{
			return players.Count;
		}
	}

	public PlayerManager(EventSystem eventSystem)
	{
		instance = this;
		this.eventSystem = eventSystem;
	}

	public void Update()
	{
		foreach (Player player in players)
		{
			player.Update();
		}
	}

	public Player AddPlayer(int playerID)
	{
		return AddPlayer(playerID, playerID);
	}

	public Player AddPlayer(int playerID, int inputID)
	{
		Player player = new Player(playerID, inputID);
		players.Add(player);
		eventSystem.Trigger(new PlayerAdd(player));
		return player;
	}

	public List<Player> GetAllPlayers()
	{
		return players;
	}

	public bool DoesPlayerWithIDExist(int id)
	{
		bool result = false;
		foreach (Player player in players)
		{
			if (player.ID == id)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public Player GetPlayerByID(int id)
	{
		Player player = null;
		foreach (Player player2 in players)
		{
			if (player2.ID == id)
			{
				player = player2;
				break;
			}
		}
		if (player == null)
		{
			Debug.LogError("[PlayerManager:GetPlayerByID] Failed to locate player with id " + id);
		}
		return player;
	}

	public void RemovePlayer(int playerID)
	{
		Player playerByID = GetPlayerByID(playerID);
		if (playerByID != null)
		{
			eventSystem.Trigger(new PlayerRemove(playerByID));
			playerByID.Shutdown();
			players.Remove(playerByID);
		}
		else
		{
			Debug.LogError(string.Format("[PlayerManager:RemovePlayer] Failed to remove player with ID {0}: Player not found.", playerID));
		}
	}

	public void RemoveAllPlayers()
	{
		for (int num = players.Count - 1; num >= 0; num--)
		{
			RemovePlayer(players[num].ID);
		}
	}

	public List<GameObject> GetAllSoldiers()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Player player in players)
		{
			if (player.HasSoldier())
			{
				list.Add(player.GetSoldier());
			}
		}
		return list;
	}

	public int PlayerCount()
	{
		return players.Count;
	}
}
