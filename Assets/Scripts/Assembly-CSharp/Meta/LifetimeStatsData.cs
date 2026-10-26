using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta
{
	public class LifetimeStatsData : MetaData
	{
		private uint kills;

		private uint deaths;

		private uint cashEarned;

		private uint syringesUsed;

		private uint damageDone;

		private uint easyGamesCompleted;

		private uint normalGamesCompleted;

		private uint hardGamesCompleted;

		private uint hellGamesCompleted;

		private uint clotsKilled;

		private uint gorefastsKilled;

		private uint crawlerKilled;

		private uint stalkersKilled;

		private uint sirensKilled;

		private uint bloatsKilled;

		private uint husksKilled;

		private uint scrakesKilled;

		private uint fleshpoundsKilled;

		private uint patriarchsKilled;

		public uint Kills
		{
			get
			{
				return kills;
			}
		}

		public uint Deaths
		{
			get
			{
				return deaths;
			}
		}

		public uint CashEarned
		{
			get
			{
				return cashEarned;
			}
		}

		public uint SyringesUsed
		{
			get
			{
				return syringesUsed;
			}
		}

		public uint DamageDone
		{
			get
			{
				return damageDone;
			}
		}

		public uint EasyGamesCompleted
		{
			get
			{
				return easyGamesCompleted;
			}
		}

		public uint NormalGamesCompleted
		{
			get
			{
				return normalGamesCompleted;
			}
		}

		public uint HardGamesCompleted
		{
			get
			{
				return hardGamesCompleted;
			}
		}

		public uint HellGamesCompleted
		{
			get
			{
				return hellGamesCompleted;
			}
		}

		public uint ClotsKilled
		{
			get
			{
				return clotsKilled;
			}
		}

		public uint GorefastsKilled
		{
			get
			{
				return gorefastsKilled;
			}
		}

		public uint CrawlersKilled
		{
			get
			{
				return crawlerKilled;
			}
		}

		public uint StalkersKilled
		{
			get
			{
				return stalkersKilled;
			}
		}

		public uint SirensKilled
		{
			get
			{
				return sirensKilled;
			}
		}

		public uint BloatsKilled
		{
			get
			{
				return bloatsKilled;
			}
		}

		public uint HusksKilled
		{
			get
			{
				return husksKilled;
			}
		}

		public uint ScrakesKilled
		{
			get
			{
				return scrakesKilled;
			}
		}

		public uint FleshpoundsKilled
		{
			get
			{
				return fleshpoundsKilled;
			}
		}

		public uint PatriarchsKilled
		{
			get
			{
				return patriarchsKilled;
			}
		}

		public LifetimeStatsData()
		{
			hasNewData = true;
		}

		public override string FileName()
		{
			return "LifetimeStats";
		}

		public override List<string> Serialize()
		{
			List<string> list = new List<string>();
			list.Add(SerializeData("kills", kills));
			list.Add(SerializeData("deaths", deaths));
			list.Add(SerializeData("cashEarned", cashEarned));
			list.Add(SerializeData("syringesUsed", syringesUsed));
			list.Add(SerializeData("damageDone", damageDone));
			list.Add(SerializeData("easyGamesCompleted", easyGamesCompleted));
			list.Add(SerializeData("normalGamesCompleted", normalGamesCompleted));
			list.Add(SerializeData("hardGamesCompleted", hardGamesCompleted));
			list.Add(SerializeData("hellGamesCompleted", hellGamesCompleted));
			list.Add(SerializeData("clotsKilled", clotsKilled));
			list.Add(SerializeData("gorefastsKilled", gorefastsKilled));
			list.Add(SerializeData("crawlerKilled", crawlerKilled));
			list.Add(SerializeData("stalkersKilled", stalkersKilled));
			list.Add(SerializeData("sirensKilled", sirensKilled));
			list.Add(SerializeData("bloatsKilled", bloatsKilled));
			list.Add(SerializeData("husksKilled", husksKilled));
			list.Add(SerializeData("scrakesKilled", scrakesKilled));
			list.Add(SerializeData("fleshpoundsKilled", fleshpoundsKilled));
			list.Add(SerializeData("patriarchsKilled", patriarchsKilled));
			return list;
		}

		protected override void OnDeserializeLine(string name, string value)
		{
			switch (name)
			{
			case "kills":
				kills = Convert.ToUInt32(value);
				break;
			case "deaths":
				deaths = Convert.ToUInt32(value);
				break;
			case "cashEarned":
				cashEarned = Convert.ToUInt32(value);
				break;
			case "syringesUsed":
				syringesUsed = Convert.ToUInt32(value);
				break;
			case "damageDone":
				damageDone = Convert.ToUInt32(value);
				break;
			case "easyGamesCompleted":
				easyGamesCompleted = Convert.ToUInt32(value);
				break;
			case "normalGamesCompleted":
				normalGamesCompleted = Convert.ToUInt32(value);
				break;
			case "hardGamesCompleted":
				hardGamesCompleted = Convert.ToUInt32(value);
				break;
			case "hellGamesCompleted":
				hellGamesCompleted = Convert.ToUInt32(value);
				break;
			case "clotsKilled":
				clotsKilled = Convert.ToUInt32(value);
				break;
			case "gorefastsKilled":
				gorefastsKilled = Convert.ToUInt32(value);
				break;
			case "crawlerKilled":
				crawlerKilled = Convert.ToUInt32(value);
				break;
			case "stalkersKilled":
				stalkersKilled = Convert.ToUInt32(value);
				break;
			case "sirensKilled":
				sirensKilled = Convert.ToUInt32(value);
				break;
			case "bloatsKilled":
				bloatsKilled = Convert.ToUInt32(value);
				break;
			case "husksKilled":
				husksKilled = Convert.ToUInt32(value);
				break;
			case "scrakesKilled":
				scrakesKilled = Convert.ToUInt32(value);
				break;
			case "fleshpoundsKilled":
				fleshpoundsKilled = Convert.ToUInt32(value);
				break;
			case "patriarchsKilled":
				patriarchsKilled = Convert.ToUInt32(value);
				break;
			}
		}

		public void UpdateStat(LifetimeStat stat, uint amount)
		{
			switch (stat)
			{
			case LifetimeStat.Kills:
				kills += amount;
				break;
			case LifetimeStat.Deaths:
				deaths += amount;
				break;
			case LifetimeStat.CashEarned:
				cashEarned += amount;
				break;
			case LifetimeStat.SyringesUsed:
				syringesUsed += amount;
				break;
			case LifetimeStat.DamageDone:
				damageDone += amount;
				break;
			case LifetimeStat.EasyGamesCompleted:
				easyGamesCompleted += amount;
				break;
			case LifetimeStat.NormalGamesCompleted:
				normalGamesCompleted += amount;
				break;
			case LifetimeStat.HardGamesCompleted:
				hardGamesCompleted += amount;
				break;
			case LifetimeStat.HellGamesCompleted:
				hellGamesCompleted += amount;
				break;
			case LifetimeStat.ClotsKilled:
				clotsKilled += amount;
				break;
			case LifetimeStat.GorefastsKilled:
				gorefastsKilled += amount;
				break;
			case LifetimeStat.CrawlersKilled:
				crawlerKilled += amount;
				break;
			case LifetimeStat.StalkersKilled:
				stalkersKilled += amount;
				break;
			case LifetimeStat.SirensKilled:
				sirensKilled += amount;
				break;
			case LifetimeStat.BloatsKilled:
				bloatsKilled += amount;
				break;
			case LifetimeStat.HusksKilled:
				husksKilled += amount;
				break;
			case LifetimeStat.ScrakesKilled:
				scrakesKilled += amount;
				break;
			case LifetimeStat.FleshpoundsKilled:
				fleshpoundsKilled += amount;
				break;
			case LifetimeStat.PatriarchsKilled:
				patriarchsKilled += amount;
				break;
			}
			hasNewData = true;
		}

		public override void DebugPrint()
		{
			Debug.Log("kills " + kills);
			Debug.Log("deaths " + deaths);
			Debug.Log("cashEarned " + cashEarned);
			Debug.Log("syringesUsed " + syringesUsed);
			Debug.Log("damageDone " + damageDone);
			Debug.Log("easyGamesCompleted " + easyGamesCompleted);
			Debug.Log("normalGamesCompleted " + normalGamesCompleted);
			Debug.Log("hardGamesCompleted " + hardGamesCompleted);
			Debug.Log("hellGamesCompleted " + hellGamesCompleted);
			Debug.Log("clotsKilled " + clotsKilled);
			Debug.Log("gorefastsKilled " + gorefastsKilled);
			Debug.Log("crawlerKilled " + crawlerKilled);
			Debug.Log("stalkersKilled " + stalkersKilled);
			Debug.Log("sirensKilled " + sirensKilled);
			Debug.Log("bloatsKilled " + bloatsKilled);
			Debug.Log("husksKilled " + husksKilled);
			Debug.Log("scrakesKilled " + scrakesKilled);
			Debug.Log("fleshpoundsKilled " + fleshpoundsKilled);
			Debug.Log("patriarchsKilled " + patriarchsKilled);
		}
	}
}
