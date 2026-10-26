using EffectEvents;
using EventHelpers;

public class OnDeathSpeechTrigger : TriggerSpeechEffect
{
	public bool TriggerOnlyOnHeadshot;

	protected override bool ShouldPlay(TriggerBase e)
	{
		bool flag = base.ShouldPlay(e);
		if (flag && TriggerOnlyOnHeadshot && e.Id == TriggerId.BeingKilled)
		{
			BeingKilled beingKilled = (BeingKilled)e;
			flag = beingKilled.KilledByHeadshot;
		}
		return flag;
	}
}
