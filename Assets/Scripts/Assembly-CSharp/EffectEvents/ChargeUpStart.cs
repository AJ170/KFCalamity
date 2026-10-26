using EventHelpers;

namespace EffectEvents
{
	public class ChargeUpStart : TriggerBase
	{
		private float chargeUpTime;

		public float ChargeUpTime
		{
			get
			{
				return chargeUpTime;
			}
		}

		public ChargeUpStart()
			: base(TriggerId.ChargeUpStart)
		{
		}

		public ChargeUpStart(float chargeUpTime)
			: base(TriggerId.ChargeUpStart)
		{
			this.chargeUpTime = chargeUpTime;
		}
	}
}
