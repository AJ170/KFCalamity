using EventHelpers;

namespace GameEvents
{
	public class WaveUnitsKilled : EventBase
	{
		public WaveUnitsKilled()
			: base(EventId.WaveUnitsKilled)
		{
		}
	}
}
