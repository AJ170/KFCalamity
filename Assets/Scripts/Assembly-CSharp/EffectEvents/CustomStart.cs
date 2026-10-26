using EventHelpers;

namespace EffectEvents
{
	public class CustomStart : TriggerBase
	{
		public CustomStart()
			: base(TriggerId.CustomStart)
		{
		}

		public CustomStart(string customName)
			: base(TriggerId.CustomStart, customName)
		{
		}
	}
}
