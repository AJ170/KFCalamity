namespace Acheivements
{
	public class HealingEvent : EventBase
	{
		public int Healing;

		public HealingEvent(int healing)
			: base(EventId.Healing)
		{
			Healing = healing;
		}
	}
}
