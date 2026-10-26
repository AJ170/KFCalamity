public class ChargedApplyForceEffect : ApplyForceEffect
{
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

	protected override float GetMultiplier()
	{
		float num = base.GetMultiplier();
		if (chargedProjectile != null)
		{
			float num2 = 1f + chargedProjectile.BonusCharge * 2f;
			num *= num2;
		}
		return num;
	}
}
