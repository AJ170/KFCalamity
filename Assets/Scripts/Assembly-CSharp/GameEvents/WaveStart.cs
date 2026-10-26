using EventHelpers;
using Spawning;

namespace GameEvents
{
	public class WaveStart : EventBase
	{
		private Wave wave;

		public Wave Wave
		{
			get
			{
				return wave;
			}
		}

		public WaveStart(Wave w)
			: base(EventId.WaveStart)
		{
			wave = w;
		}
	}
}
