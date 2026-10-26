using EventHelpers;

namespace EffectEvents
{
	public class ChargeUpMax : TriggerBase
	{
		private bool success = true;

		public bool Success
		{
			get
			{
				return success;
			}
		}

		public ChargeUpMax()
			: base(TriggerId.ChargeUpMax)
		{
		}

		public ChargeUpMax(bool success)
			: base(TriggerId.ChargeUpMax)
		{
			this.success = success;
		}
	}
}
