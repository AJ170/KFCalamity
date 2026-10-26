using EventHelpers;

namespace EffectEvents
{
	public class TimerStart : TriggerBase
	{
		public TimerStart(string customName)
			: base(TriggerId.TimerStart, customName)
		{
		}
	}
}
