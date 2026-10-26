using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class HitAOE : HitBase
	{
		private float aoeRadius;

		private float distanceFromImpact;

		public float AOERadius
		{
			get
			{
				return aoeRadius;
			}
		}

		public float DistanceFromImpact
		{
			get
			{
				return distanceFromImpact;
			}
		}

		public HitAOE(Transform transform, float aoeRadius, float distanceFromImpact)
			: base(TriggerId.HitAOE, transform)
		{
			this.aoeRadius = aoeRadius;
			this.distanceFromImpact = distanceFromImpact;
		}

		public HitAOE(string customName, Transform transform, float aoeRadius, float distanceFromImpact)
			: base(TriggerId.HitAOE, customName, transform)
		{
			this.aoeRadius = aoeRadius;
			this.distanceFromImpact = distanceFromImpact;
		}
	}
}
