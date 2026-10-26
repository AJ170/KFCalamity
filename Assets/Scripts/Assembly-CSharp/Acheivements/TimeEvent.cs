namespace Acheivements
{
	public class TimeEvent : EventBase
	{
		public int Time;

		public TimeEvent(int time)
			: base(EventId.Time)
		{
			Time = time;
		}
	}
}
