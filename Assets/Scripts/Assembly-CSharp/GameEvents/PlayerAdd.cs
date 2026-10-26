using EventHelpers;

namespace GameEvents
{
	public class PlayerAdd : EventBase
	{
		private Player player;

		public Player Player
		{
			get
			{
				return player;
			}
		}

		public PlayerAdd(Player player)
			: base(EventId.PlayerAdd)
		{
			this.player = player;
		}
	}
}
