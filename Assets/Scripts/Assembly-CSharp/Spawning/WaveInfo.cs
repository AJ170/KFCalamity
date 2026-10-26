namespace Spawning
{
	public class WaveInfo
	{
		private Wave wave;

		private int unitsKilled;

		private int unitsSpawned;

		private int unitsLeftToSpawn;

		private int totalUnitsInWave;

		private int maxUnitsAtOnce;

		public Wave Wave
		{
			get
			{
				return wave;
			}
		}

		public int UnitsKilled
		{
			get
			{
				return unitsKilled;
			}
			set
			{
				unitsKilled = value;
			}
		}

		public int UnitsSpawned
		{
			get
			{
				return unitsSpawned;
			}
			set
			{
				unitsSpawned = value;
			}
		}

		public int UnitsLeftToSpawn
		{
			get
			{
				return unitsLeftToSpawn;
			}
			set
			{
				unitsLeftToSpawn = value;
			}
		}

		public int TotalUnitsInWave
		{
			get
			{
				return totalUnitsInWave;
			}
		}

		public int MaxUnitsAtOnce
		{
			get
			{
				return maxUnitsAtOnce;
			}
			set
			{
				maxUnitsAtOnce = value;
			}
		}

		public WaveInfo(Wave wave)
		{
			this.wave = wave;
			unitsLeftToSpawn = wave.TotalEnemies;
			totalUnitsInWave = wave.TotalEnemies;
			maxUnitsAtOnce = wave.MaxEnemiesAtOnce;
		}
	}
}
