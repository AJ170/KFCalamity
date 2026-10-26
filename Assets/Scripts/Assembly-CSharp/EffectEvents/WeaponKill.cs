using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class WeaponKill : TriggerBase
	{
		private GameObject victim;

		public GameObject Victim
		{
			get
			{
				return victim;
			}
		}

		public WeaponKill(GameObject victim)
			: base(TriggerId.WeaponKill)
		{
			this.victim = victim;
		}
	}
}
