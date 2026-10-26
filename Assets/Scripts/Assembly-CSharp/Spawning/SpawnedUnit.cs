using UnityEngine;

namespace Spawning
{
	public class SpawnedUnit
	{
		public GameObject GameObject;

		public ZedType Type;

		public float AliveSince;

		public float TimeAlive
		{
			get
			{
				return Time.time - AliveSince;
			}
		}

		public SpawnedUnit(GameObject gameObject, ZedType type)
		{
			GameObject = gameObject;
			Type = type;
			AliveSince = Time.time;
		}
	}
}
