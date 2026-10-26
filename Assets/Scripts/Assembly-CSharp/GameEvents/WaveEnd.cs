using EventHelpers;
using Spawning;

namespace GameEvents
{
	public class WaveEnd : EventBase
	{
		private Wave wave;

		public Wave Wave
		{
			get
			{
				return wave;
			}
		}

		public WaveEnd(Wave w)
			: base(EventId.WaveEnd)
		{
			wave = w;
		}
	}
}
