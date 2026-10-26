using EventHelpers;

public class RestoreSecondaryAmmoEffect : EffectBaseListener
{
	public SecondaryAttack SecondaryAttack;

	public int Amount = 1;

	protected override void Execute(TriggerBase e)
	{
		if ((NoCustomEvents() || MatchCustomEvent(e.CustomName)) && SecondaryAttack != null)
		{
			SecondaryAttack.FillAmmunition(Amount);
		}
	}
}
