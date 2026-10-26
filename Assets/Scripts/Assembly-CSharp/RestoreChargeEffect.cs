using EventHelpers;

public class RestoreChargeEffect : EffectBaseListener
{
	public Ability Ability;

	public int Count = 1;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && Ability != null)
		{
			Ability.RefillCharges(Count);
		}
	}
}
