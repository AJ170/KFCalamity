using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class BeingKilled : TriggerBase
	{
		private GameObject attacker;

		private bool headshot;

		private EffectSystem effectSystem;

		public GameObject Attacker
		{
			get
			{
				return attacker;
			}
		}

		public bool KilledByHeadshot
		{
			get
			{
				return headshot;
			}
		}

		public EffectSystem SystemEffect
		{
			get
			{
				return effectSystem;
			}
		}

		public BeingKilled(GameObject attacker)
			: base(TriggerId.BeingKilled)
		{
			this.attacker = attacker;
			headshot = false;
		}

		public BeingKilled(GameObject attacker, bool headshot)
			: base(TriggerId.BeingKilled)
		{
			this.attacker = attacker;
			this.headshot = headshot;
		}

		public BeingKilled(GameObject attacker, bool headshot, EffectSystem effectSystem)
			: base(TriggerId.BeingKilled)
		{
			this.attacker = attacker;
			this.headshot = headshot;
			this.effectSystem = effectSystem;
		}
	}
}
