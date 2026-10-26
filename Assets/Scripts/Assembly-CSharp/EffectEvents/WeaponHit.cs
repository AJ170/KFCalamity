using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class WeaponHit : TriggerBase
	{
		private Transform transform;

		public Transform HitTransform
		{
			get
			{
				return transform;
			}
		}

		public WeaponHit(Transform transform)
			: base(TriggerId.WeaponHit)
		{
			this.transform = transform;
		}
	}
}
