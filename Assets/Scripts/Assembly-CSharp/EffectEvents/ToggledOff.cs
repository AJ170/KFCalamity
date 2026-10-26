using EventHelpers;

namespace EffectEvents
{
	public class ToggledOff : TriggerBase
	{
		public ToggledOff()
			: base(TriggerId.ToggledOff)
		{
		}

		public ToggledOff(string customName)
			: base(TriggerId.ToggledOff, customName)
		{
		}
	}
}
