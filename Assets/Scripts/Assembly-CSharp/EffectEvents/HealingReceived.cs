using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class HealingReceived : TriggerBase
	{
		private GameObject healer;

		private int amount;

		public GameObject Healer
		{
			get
			{
				return healer;
			}
		}

		public int Amount
		{
			get
			{
				return amount;
			}
		}

		public HealingReceived(GameObject healer, int amount)
			: base(TriggerId.HealingReceived)
		{
			this.healer = healer;
			this.amount = amount;
		}
	}
}
