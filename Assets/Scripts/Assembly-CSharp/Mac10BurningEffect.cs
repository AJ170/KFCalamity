using EffectHelpers;

public class Mac10BurningEffect : BurningEffect
{
	protected override void Awake()
	{
		base.Awake();
		buffClass = BuffClass.Fire;
		repeatInterval = 0.5f;
		maxRepeats = 8;
		damagePerTick = 5;
		damageType = DamageType.Fire;
	}
}
