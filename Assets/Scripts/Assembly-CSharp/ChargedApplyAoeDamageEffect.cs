using EffectEvents;

public class ChargedApplyAoeDamageEffect : ApplyAoeDamageEffect
{
	public float ChargeDamageScaling = 2f;

	private ChargedProjectile chargedProjectile;

	protected override void Start()
	{
		base.Start();
		FindProjectile();
	}

	private void FindProjectile()
	{
		if (base.transform.parent != null)
		{
			chargedProjectile = base.transform.parent.GetComponent<ChargedProjectile>();
		}
	}

	protected override float GetDamageMultiplier(HitAOE hitAoe)
	{
		float num = base.GetDamageMultiplier(hitAoe);
		if (chargedProjectile != null)
		{
			float num2 = 1f + chargedProjectile.BonusCharge * ChargeDamageScaling;
			num *= num2;
		}
		return num;
	}
}
