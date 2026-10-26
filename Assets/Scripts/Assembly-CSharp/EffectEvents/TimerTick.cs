using EventHelpers;

namespace EffectEvents
{
	public class TimerTick : TriggerBase
	{
		public TimerTick(string customName)
			: base(TriggerId.TimerTick, customName)
		{
		}
	}
}
