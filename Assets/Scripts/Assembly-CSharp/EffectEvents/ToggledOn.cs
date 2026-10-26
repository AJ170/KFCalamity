using EventHelpers;

namespace EffectEvents
{
	public class ToggledOn : TriggerBase
	{
		public ToggledOn()
			: base(TriggerId.ToggledOn)
		{
		}

		public ToggledOn(string customName)
			: base(TriggerId.ToggledOn, customName)
		{
		}
	}
}
