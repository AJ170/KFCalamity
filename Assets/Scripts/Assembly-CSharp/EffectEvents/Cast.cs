using EventHelpers;

namespace EffectEvents
{
	public class Cast : TriggerBase
	{
		public Cast()
			: base(TriggerId.Cast)
		{
		}

		public Cast(string customName)
			: base(TriggerId.Cast, customName)
		{
		}
	}
}
