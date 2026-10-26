namespace Meta
{
	public abstract class LevelDatabase
	{
		protected int maxLevel = 20;

		protected int[] levels;

		public int MaxLevel
		{
			get
			{
				return maxLevel;
			}
		}

		public int GetLevelExperience(int level)
		{
			if (level > 0 && level <= maxLevel)
			{
				return levels[level - 1];
			}
			return 0;
		}

		public int GetLevelByExperience(int totalExperience)
		{
			int num = levels[maxLevel - 1];
			if (totalExperience >= num)
			{
				return maxLevel;
			}
			int num2 = 0;
			int num3 = 0;
			for (int i = 1; i <= maxLevel; i++)
			{
				num2 = GetLevelExperience(i);
				num3 = GetLevelExperience(i + 1);
				if (num2 <= totalExperience && totalExperience < num3)
				{
					return i;
				}
			}
			return 0;
		}
	}
}
