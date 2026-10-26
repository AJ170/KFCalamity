using EventHelpers;

namespace GameEvents
{
	public class PerkLevelUp : EventBase
	{
		private Perk perk;

		private int level;

		public Perk Perk
		{
			get
			{
				return perk;
			}
		}

		public int Level
		{
			get
			{
				return level;
			}
		}

		public PerkLevelUp(Perk perk, int level)
			: base(EventId.PerkLevelUp)
		{
			this.perk = perk;
			this.level = level;
		}
	}
}
