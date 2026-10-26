using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class Affect : TriggerBase
	{
		private Transform transform;

		public Transform AffectTransform
		{
			get
			{
				return transform;
			}
		}

		public Affect(Transform transform)
			: base(TriggerId.Affect)
		{
			this.transform = transform;
		}

		public Affect(string customName, Transform transform)
			: base(TriggerId.Affect, customName)
		{
			this.transform = transform;
		}
	}
}
