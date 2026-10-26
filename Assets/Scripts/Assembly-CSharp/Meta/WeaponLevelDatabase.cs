using UnityEngine;

namespace Meta
{
	public class WeaponLevelDatabase : LevelDatabase
	{
		public WeaponLevelDatabase()
		{
			maxLevel = 20;
			levels = new int[maxLevel];
			for (int i = 1; i <= maxLevel; i++)
			{
				levels[i - 1] = (int)((float)Mathf.Max(i * (i + 5) * 300 - 2000, 0) * 0.5f);
			}
		}
	}
}
