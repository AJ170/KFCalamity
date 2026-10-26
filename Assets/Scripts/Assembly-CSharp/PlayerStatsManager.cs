using System.Collections.Generic;
using EventHelpers;
using GameEvents;

public class PlayerStatsManager
{
	private static PlayerStatsManager instance;

	private EventSystem eventSystem;

	private List<PlayerStats> playerStats = new List<PlayerStats>();

	public static PlayerStatsManager Instance
	{
		get
		{
			return instance;
		}
	}

	public int NumPlayerStats
	{
		get
		{
			return playerStats.Count;
		}
	}

	public PlayerStatsManager(EventSystem eventSystem)
	{
		instance = this;
		this.eventSystem = eventSystem;
		RegisterEvents();
	}

	public void RegisterEvents()
	{
		eventSystem.Subscribe(EventId.PlayerAdd, OnGameEvents, this);
		eventSystem.Subscribe(EventId.PlayerRemove, OnGameEvents, this);
	}

	private void OnGameEvents(EventBase e)
	{
		if (e.Id == EventId.PlayerAdd)
		{
			AddStats(((PlayerAdd)e).Player);
		}
		else if (e.Id == EventId.PlayerRemove)
		{
			RemoveStats(((PlayerRemove)e).Player);
		}
	}

	private void AddStats(Player player)
	{
		if (player != null)
		{
			playerStats.Add(new PlayerStats(player));
		}
	}

	private void RemoveStats(Player player)
	{
		if (player != null)
		{
			playerStats.Remove(StatsByPlayer(player));
		}
	}

	public void ResetAllStats()
	{
		foreach (PlayerStats playerStat in playerStats)
		{
			playerStat.Reset();
		}
	}

	public void Clear()
	{
		playerStats.Clear();
	}

	public List<PlayerStats> GetAllStats()
	{
		return playerStats;
	}

	public PlayerStats StatsByPlayer(Player player)
	{
		foreach (PlayerStats playerStat in playerStats)
		{
			if (playerStat.Player == player)
			{
				return playerStat;
			}
		}
		return null;
	}

	public PlayerStats StatsByPlayerId(int playerId)
	{
		foreach (PlayerStats playerStat in playerStats)
		{
			if (playerStat.Player.ID == playerId)
			{
				return playerStat;
			}
		}
		return null;
	}

	public void DumpToConsole()
	{
		foreach (PlayerStats playerStat in playerStats)
		{
			playerStat.DumpToConsole();
		}
	}
}
