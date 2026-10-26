using Meta;
using UnityEngine;

public class LifetimeStatsHandler : MonoBehaviour
{
	private static MetaManager metaManager;

	private void Awake()
	{
		metaManager = MetaManager.Instance;
	}

	private void OnDestroy()
	{
		metaManager = null;
	}

	public static void AddKill()
	{
		if (metaManager != null)
		{
			metaManager.UpdateStat(LifetimeStat.Kills);
		}
	}

	public static void AddDeath()
	{
		if (metaManager != null)
		{
			metaManager.UpdateStat(LifetimeStat.Deaths);
		}
	}

	public static void AddCashEarned(int cash)
	{
		if (metaManager != null)
		{
			metaManager.UpdateStat(LifetimeStat.CashEarned, cash);
		}
	}

	public static void AddSyringeUsed()
	{
		if (metaManager != null)
		{
			metaManager.UpdateStat(LifetimeStat.SyringesUsed);
		}
	}

	public static void AddDamageDone(int damage)
	{
		if (metaManager != null)
		{
			metaManager.UpdateStat(LifetimeStat.DamageDone, damage);
		}
	}

	public static void AddLevelCompleted(DifficultySetting difficulty)
	{
		if (metaManager != null)
		{
			LifetimeStat stat = LifetimeStat.None;
			switch (difficulty)
			{
			case DifficultySetting.Beginner:
				stat = LifetimeStat.EasyGamesCompleted;
				break;
			case DifficultySetting.Normal:
				stat = LifetimeStat.NormalGamesCompleted;
				break;
			case DifficultySetting.Hard:
				stat = LifetimeStat.HardGamesCompleted;
				break;
			case DifficultySetting.Hell:
				stat = LifetimeStat.HellGamesCompleted;
				break;
			}
			metaManager.UpdateStat(stat);
		}
	}

	public static void AddZedKill(Zed zed)
	{
		if (!(metaManager != null) || !(zed != null))
		{
			return;
		}
		ZedType type = zed.Type;
		if (type != (ZedType)(-1))
		{
			LifetimeStat stat = LifetimeStat.None;
			switch (type)
			{
			case ZedType.Clot:
				stat = LifetimeStat.ClotsKilled;
				break;
			case ZedType.Gorefast:
				stat = LifetimeStat.GorefastsKilled;
				break;
			case ZedType.Bloat:
				stat = LifetimeStat.BloatsKilled;
				break;
			case ZedType.Crawler:
				stat = LifetimeStat.CrawlersKilled;
				break;
			case ZedType.Siren:
				stat = LifetimeStat.SirensKilled;
				break;
			case ZedType.Stalker:
				stat = LifetimeStat.StalkersKilled;
				break;
			case ZedType.Husk:
				stat = LifetimeStat.HusksKilled;
				break;
			case ZedType.Scrake:
				stat = LifetimeStat.ScrakesKilled;
				break;
			case ZedType.Fleshpound:
				stat = LifetimeStat.FleshpoundsKilled;
				break;
			case ZedType.Patriarch:
				stat = LifetimeStat.PatriarchsKilled;
				break;
			}
			metaManager.UpdateStat(stat);
		}
	}
}
