using UnityEngine;

namespace Meta
{
	public class PerkLevelDatabase : LevelDatabase
	{
		public PerkLevelDatabase()
		{
			maxLevel = 20;
			levels = new int[maxLevel];
			for (int i = 1; i <= maxLevel; i++)
			{
				levels[i - 1] = Mathf.Max(i * (i + 5) * 300 - 2000, 0);
			}
		}
	}
}
