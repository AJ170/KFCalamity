using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class Detected : TriggerBase
	{
		public GameObject Target;

		public Detected(GameObject target)
			: base(TriggerId.Detected)
		{
			Target = target;
		}
	}
}
