using EventHelpers;

namespace EffectEvents
{
	public class Death : TriggerBase
	{
		private bool headshot;

		public bool Headshot
		{
			get
			{
				return headshot;
			}
		}

		public Death(bool headshot = false)
			: base(TriggerId.Death)
		{
			this.headshot = headshot;
		}

		public Death(string customName, bool headshot = false)
			: base(TriggerId.Death, customName)
		{
			this.headshot = headshot;
		}
	}
}
