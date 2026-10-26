using EventHelpers;

namespace GameEvents
{
	public class WeaponLevelUp : EventBase
	{
		private Weapon weapon;

		private int level;

		public Weapon Weapon
		{
			get
			{
				return weapon;
			}
		}

		public int Level
		{
			get
			{
				return level;
			}
		}

		public WeaponLevelUp(Weapon weapon, int level)
			: base(EventId.WeaponLevelUp)
		{
			this.weapon = weapon;
			this.level = level;
		}
	}
}
