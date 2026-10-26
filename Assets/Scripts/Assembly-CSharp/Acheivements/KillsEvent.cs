namespace Acheivements
{
	public class KillsEvent : EventBase
	{
		public int Kills;

		public KillsEvent(int kills)
			: base(EventId.Kills)
		{
			Kills = kills;
		}
	}
}
