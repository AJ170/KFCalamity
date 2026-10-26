namespace Spawning
{
	public class QueuedRelocation
	{
		public SpawnedUnit SpawnedObject;

		public float Range;

		public QueuedRelocation(SpawnedUnit spawnedObject, float range)
		{
			SpawnedObject = spawnedObject;
			Range = range;
		}
	}
}
