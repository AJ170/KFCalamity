using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class FriendlyAim : TriggerBase
	{
		public GameObject Target;

		public FriendlyAim(GameObject target)
			: base(TriggerId.FriendlyAim)
		{
			Target = target;
		}
	}
}
