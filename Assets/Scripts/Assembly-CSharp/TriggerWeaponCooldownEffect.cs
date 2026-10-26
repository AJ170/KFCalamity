using EventHelpers;

public class TriggerWeaponCooldownEffect : EffectBaseListener
{
	public Weapon weapon;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && weapon != null)
		{
			weapon.TriggerRoundCooldown();
		}
	}
}
