using EventHelpers;
using Spawning;

namespace GameEvents
{
	public class WaveCreate : EventBase
	{
		private Wave wave;

		public Wave Wave
		{
			get
			{
				return wave;
			}
		}

		public WaveCreate(Wave w)
			: base(EventId.WaveCreate)
		{
			wave = w;
		}
	}
}
