using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class DamageDealt : TriggerBase
	{
		private GameObject victim;

		private int amount;

		public GameObject Victim
		{
			get
			{
				return victim;
			}
		}

		public int Amount
		{
			get
			{
				return amount;
			}
		}

		public DamageDealt(GameObject victim, int amount)
			: base(TriggerId.DamageDealt)
		{
			this.victim = victim;
			this.amount = amount;
		}
	}
}
