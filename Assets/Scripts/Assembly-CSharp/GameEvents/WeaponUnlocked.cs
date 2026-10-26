using EventHelpers;

namespace GameEvents
{
	public class WeaponUnlocked : EventBase
	{
		private Weapon weapon;

		public Weapon Weapon
		{
			get
			{
				return weapon;
			}
		}

		public WeaponUnlocked(Weapon weapon)
			: base(EventId.WeaponUnlocked)
		{
			this.weapon = weapon;
		}
	}
}
