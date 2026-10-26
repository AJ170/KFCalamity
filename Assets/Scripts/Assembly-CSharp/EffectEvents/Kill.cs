using EventHelpers;
using UnityEngine;

namespace EffectEvents
{
	public class Kill : TriggerBase
	{
		private GameObject victim;

		private bool headshot;

		public GameObject Victim
		{
			get
			{
				return victim;
			}
		}

		public bool Headshot
		{
			get
			{
				return headshot;
			}
		}

		public Kill(GameObject victim)
			: base(TriggerId.Kill)
		{
			this.victim = victim;
			headshot = false;
		}

		public Kill(GameObject victim, bool headshot)
			: base(TriggerId.Kill)
		{
			this.victim = victim;
			this.headshot = headshot;
		}
	}
}
