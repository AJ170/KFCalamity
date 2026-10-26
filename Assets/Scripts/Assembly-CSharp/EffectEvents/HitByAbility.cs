using EventHelpers;

namespace EffectEvents
{
	public class HitByAbility : TriggerBase
	{
		public HitByAbility()
			: base(TriggerId.HitByAbility)
		{
		}

		public HitByAbility(string customName)
			: base(TriggerId.HitByAbility, customName)
		{
		}
	}
}
