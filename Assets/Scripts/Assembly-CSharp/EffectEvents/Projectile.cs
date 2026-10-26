using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class Projectile : TriggerBase
	{
		private Transform bone;

		private Vector3 direction;

		private Vector3 target;

		private Collider collider;

		public Transform Bone
		{
			get
			{
				return bone;
			}
		}

		public Vector3 Direction
		{
			get
			{
				return direction;
			}
		}

		public Vector3 Target
		{
			get
			{
				return target;
			}
		}

		public Collider Collider
		{
			get
			{
				return collider;
			}
		}

		public Projectile(Transform bone)
			: this(bone, bone.forward)
		{
		}

		public Projectile(Transform bone, Vector3 direction)
			: this(bone, direction, bone.position + direction * 10f)
		{
		}

		public Projectile(Transform bone, Vector3 direction, Vector3 target, Collider collider = null)
			: base(TriggerId.Projectile)
		{
			this.bone = bone;
			this.direction = direction;
			this.target = target;
			this.collider = collider;
		}
	}
}
