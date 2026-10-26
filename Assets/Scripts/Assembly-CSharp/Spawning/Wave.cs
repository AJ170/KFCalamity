using System.Collections.Generic;
using UnityEngine;

namespace Spawning
{
	public class Wave
	{
		private List<int> squads;

		private int totalEnemies;

		private int maxEnemiesAtOnce;

		private bool bossWave;

		public List<int> Squads
		{
			get
			{
				return squads;
			}
		}

		public int TotalEnemies
		{
			get
			{
				return totalEnemies;
			}
		}

		public int MaxEnemiesAtOnce
		{
			get
			{
				return maxEnemiesAtOnce;
			}
		}

		public bool BossWave
		{
			get
			{
				return bossWave;
			}
		}

		public Wave(List<int> squads, int totalEnemies, int maxEnemiesAtOnce, bool bossWave = false)
		{
			this.squads = squads;
			this.totalEnemies = totalEnemies;
			this.maxEnemiesAtOnce = maxEnemiesAtOnce;
			this.bossWave = bossWave;
		}

		public void ScaleEnemiesAtOnce(float scale)
		{
			maxEnemiesAtOnce = (int)((float)maxEnemiesAtOnce * scale);
			maxEnemiesAtOnce = Mathf.Max(maxEnemiesAtOnce, 1);
		}

		public void ScaleTotalEnemies(float scale)
		{
			totalEnemies = (int)((float)totalEnemies * scale);
			totalEnemies = Mathf.Max(totalEnemies, 1);
		}
	}
}
