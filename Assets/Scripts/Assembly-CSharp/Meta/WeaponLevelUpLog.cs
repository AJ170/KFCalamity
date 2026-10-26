namespace Meta
{
	public struct WeaponLevelUpLog
	{
		public string weaponName;

		public int level;

		public WeaponLevelUpLog(string weaponName, int level)
		{
			this.weaponName = weaponName;
			this.level = level;
		}
	}
}
