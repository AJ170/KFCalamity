namespace Spawning
{
	public class WaveSquad
	{
		private ZedType[] units;

		private bool spawnAsWhole;

		private bool discardOverflow;

		public ZedType[] Units
		{
			get
			{
				return units;
			}
		}

		public bool SpawnAsWhole
		{
			get
			{
				return spawnAsWhole;
			}
		}

		public bool DiscardOverflow
		{
			get
			{
				return discardOverflow;
			}
		}

		public WaveSquad(ZedType[] u)
		{
			units = u;
		}

		public WaveSquad(ZedType[] units, bool spawnAsWhole, bool discardOverflow = false)
		{
			this.units = units;
			this.spawnAsWhole = spawnAsWhole;
			this.discardOverflow = discardOverflow;
		}

		public WaveSquad Clone()
		{
			ZedType[] array = new ZedType[units.Length];
			units.CopyTo(array, 0);
			return new WaveSquad(array, spawnAsWhole, discardOverflow);
		}

		public QueuedSquad GetQueueClone()
		{
			ZedType[] array = new ZedType[units.Length];
			units.CopyTo(array, 0);
			return new QueuedSquad(array, spawnAsWhole, discardOverflow);
		}
	}
}
