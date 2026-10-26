namespace Meta
{
	public struct PerkLevelUpLog
	{
		public Perk perk;

		public int level;

		public PerkLevelUpLog(Perk perk, int level)
		{
			this.perk = perk;
			this.level = level;
		}
	}
}
