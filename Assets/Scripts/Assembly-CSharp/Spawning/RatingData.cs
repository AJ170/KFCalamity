using System.Collections.Generic;
using UnityEngine;

namespace Spawning
{
	public class RatingData
	{
		public Vector3 Position;

		public Vector3 Direction;

		public float MinRange = -1f;

		public float MaxRange = -1f;

		public List<ZedType> Squad;

		public SpawnPointRatingCallback Callback;

		public RatingMethod Method;

		public RatingData(List<ZedType> squad, SpawnPointRatingCallback callback)
		{
			Squad = squad;
			Callback = callback;
		}

		public RatingData(float maxRange, List<ZedType> squad, SpawnPointRatingCallback callback)
		{
			MaxRange = maxRange;
			Squad = squad;
			Callback = callback;
		}

		public RatingData(float minRange, float maxRange, List<ZedType> squad, SpawnPointRatingCallback callback)
		{
			MinRange = minRange;
			MaxRange = maxRange;
			Squad = squad;
			Callback = callback;
		}
	}
}
