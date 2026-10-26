using EventHelpers;

namespace GameEvents
{
	public class PlayerRemove : EventBase
	{
		private Player player;

		public Player Player
		{
			get
			{
				return player;
			}
		}

		public PlayerRemove(Player player)
			: base(EventId.PlayerRemove)
		{
			this.player = player;
		}
	}
}
