using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class DamageReceived : TriggerBase
	{
		private GameObject attacker;

		private int amount;

		public GameObject Attacker
		{
			get
			{
				return attacker;
			}
		}

		public int Amount
		{
			get
			{
				return amount;
			}
		}

		public DamageReceived(GameObject attacker, int amount)
			: base(TriggerId.DamageReceived)
		{
			this.attacker = attacker;
			this.amount = amount;
		}
	}
}
