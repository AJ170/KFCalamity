using UnityEngine;

public class PlayerStats
{
	private Player player;

	private int kills;

	private int killStreak;

	private int longestKillStreak;

	private int money;

	private int damageDealt;

	private int healingDealt;

	private int deaths;

	public Player Player
	{
		get
		{
			return player;
		}
	}

	public int Kills
	{
		get
		{
			return kills;
		}
	}

	public int KillStreak
	{
		get
		{
			return killStreak;
		}
	}

	public int LongestKillStreak
	{
		get
		{
			return longestKillStreak;
		}
	}

	public int Money
	{
		get
		{
			return money;
		}
	}

	public int DamageDealt
	{
		get
		{
			return damageDealt;
		}
	}

	public int HealingDealt
	{
		get
		{
			return healingDealt;
		}
	}

	public int Deaths
	{
		get
		{
			return deaths;
		}
	}

	public PlayerStats(Player player)
	{
		this.player = player;
	}

	public void AddKill()
	{
		kills++;
		killStreak++;
		if (killStreak > longestKillStreak)
		{
			longestKillStreak = killStreak;
		}
	}

	public void AddMoney(int money)
	{
		this.money += money;
	}

	public int AddMoney(int money, float multiplier)
	{
		int num = (int)((float)money * multiplier);
		this.money += num;
		return num;
	}

	public void RemoveMoney(int money)
	{
		this.money = Mathf.Max(this.money - money, 0);
	}

	public bool KillStreakPenalty(bool protectionBuff, float multiplier)
	{
		if (killStreak > 0)
		{
			int num = killStreak;
			int a = killStreak / ((!protectionBuff) ? 2 : 4);
			a = Mathf.Max(a, (!protectionBuff) ? 10 : 5);
			a = (int)((float)a * multiplier);
			killStreak = Mathf.Max(killStreak - a, 0);
			return num != killStreak;
		}
		return false;
	}

	public void ResetKillStreak()
	{
		killStreak = 0;
	}

	public void AddDamageDealt(int amount)
	{
		damageDealt += amount;
	}

	public void AddHealingDealt(int amount)
	{
		healingDealt += amount;
	}

	public void AddDeath()
	{
		deaths++;
	}

	public void Reset()
	{
		kills = 0;
		killStreak = 0;
		longestKillStreak = 0;
		money = 0;
		damageDealt = 0;
		healingDealt = 0;
		deaths = 0;
	}

	public void DumpToConsole()
	{
		Debug.Log(string.Format("player{0}: kills {1} killstreak {2} damage dealt {3} healing dealt {4} deaths {5}", player.ID, kills, longestKillStreak, damageDealt, healingDealt, deaths));
	}
}
