using EventHelpers;

namespace EffectEvents
{
	public class TimerEnd : TriggerBase
	{
		public TimerEnd()
			: base(TriggerId.TimerEnd)
		{
		}

		public TimerEnd(string customName)
			: base(TriggerId.TimerEnd, customName)
		{
		}
	}
}
