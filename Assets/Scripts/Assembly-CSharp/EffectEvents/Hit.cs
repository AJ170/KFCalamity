using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class Hit : HitBase
	{
		public Hit(Transform transform, bool headshot = false)
			: base(TriggerId.Hit, transform, headshot)
		{
		}

		public Hit(string customName, Transform transform, bool headshot = false)
			: base(TriggerId.Hit, customName, transform, headshot)
		{
		}
	}
}
