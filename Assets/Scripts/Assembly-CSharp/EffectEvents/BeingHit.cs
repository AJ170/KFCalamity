using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class BeingHit : TriggerBase
	{
		private GameObject attacker;

		private Transform transform;

		private Vector3 fromDirection;

		public GameObject Attacker
		{
			get
			{
				return attacker;
			}
		}

		public Transform HitTransform
		{
			get
			{
				return transform;
			}
		}

		public Vector3 FromDirection
		{
			get
			{
				return fromDirection;
			}
		}

		public BeingHit(GameObject attacker, Transform transform)
			: base(TriggerId.BeingHit)
		{
			this.attacker = attacker;
			this.transform = transform;
		}

		public BeingHit(GameObject attacker, Transform transform, Vector3 fromDirection)
			: base(TriggerId.BeingHit)
		{
			this.attacker = attacker;
			this.transform = transform;
			this.fromDirection = fromDirection;
		}
	}
}
