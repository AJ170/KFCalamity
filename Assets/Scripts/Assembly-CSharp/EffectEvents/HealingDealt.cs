using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class HealingDealt : TriggerBase
	{
		private GameObject target;

		private int amount;

		public GameObject Target
		{
			get
			{
				return target;
			}
		}

		public int Amount
		{
			get
			{
				return amount;
			}
		}

		public HealingDealt(GameObject target, int amount)
			: base(TriggerId.HealingDealt)
		{
			this.target = target;
			this.amount = amount;
		}
	}
}
