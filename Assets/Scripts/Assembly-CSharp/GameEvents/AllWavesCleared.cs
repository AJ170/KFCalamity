using EventHelpers;

namespace GameEvents
{
	public class AllWavesCleared : EventBase
	{
		public AllWavesCleared()
			: base(EventId.AllWavesCleared)
		{
		}
	}
}
