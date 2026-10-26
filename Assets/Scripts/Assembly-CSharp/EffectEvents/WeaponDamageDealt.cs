using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class WeaponDamageDealt : TriggerBase
	{
		private GameObject victim;

		public GameObject Victim
		{
			get
			{
				return victim;
			}
		}

		public WeaponDamageDealt(GameObject victim)
			: base(TriggerId.WeaponDamageDealt)
		{
			this.victim = victim;
		}
	}
}
