using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class BulletImpact : TriggerBase
	{
		public Vector3 Position;

		public Vector3 FromDirection;

		public PhysicMaterial Material;

		public BulletImpact(Vector3 position, Vector3 fromDirection, PhysicMaterial material)
			: base(TriggerId.BulletImpact)
		{
			Position = position;
			FromDirection = fromDirection;
			Material = material;
		}
	}
}
