using EventHelpers;

namespace EffectEvents
{
	public class ChargeUpRelease : TriggerBase
	{
		public float Bonus;

		public ChargeUpRelease()
			: base(TriggerId.ChargeUpRelease)
		{
		}

		public ChargeUpRelease(float bonus)
			: base(TriggerId.ChargeUpRelease)
		{
			Bonus = bonus;
		}
	}
}
