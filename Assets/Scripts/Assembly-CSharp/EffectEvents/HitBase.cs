using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class HitBase : TriggerBase
	{
		private Transform transform;

		private bool headshot;

		public Transform HitTransform
		{
			get
			{
				return transform;
			}
		}

		public bool Headshot
		{
			get
			{
				return headshot;
			}
		}

		public HitBase(TriggerId trigger, Transform transform, bool headshot = false)
			: base(trigger)
		{
			this.transform = transform;
			this.headshot = headshot;
		}

		public HitBase(TriggerId trigger, string customName, Transform transform, bool headshot = false)
			: base(trigger, customName)
		{
			this.transform = transform;
			this.headshot = headshot;
		}
	}
}
