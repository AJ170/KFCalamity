using EventHelpers;

namespace EffectEvents
{
	public class ChargeUpTick : TriggerBase
	{
		public int Charges;

		public bool Full;

		public ChargeUpTick(int charges, bool full = false)
			: base(TriggerId.ChargeUpTick)
		{
			Charges = charges;
			Full = full;
		}
	}
}
