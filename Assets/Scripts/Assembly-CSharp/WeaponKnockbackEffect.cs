public class WeaponKnockbackEffect : KnockbackEffect
{
	protected override void Awake()
	{
		duration = 0.1f;
		knockbackForce = 10f;
		base.Awake();
	}
}
