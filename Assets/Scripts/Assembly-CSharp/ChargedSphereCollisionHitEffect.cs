public class ChargedSphereCollisionHitEffect : SphereCollisionHitEffect
{
	private ChargedProjectile chargedProjectile;

	public float ChargeRadiusBonus = 1f;

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

	protected override float GetMultiplier()
	{
		float num = base.GetMultiplier();
		if (chargedProjectile != null)
		{
			float num2 = 1f + chargedProjectile.BonusCharge * ChargeRadiusBonus;
			num *= num2;
		}
		return num;
	}
}
