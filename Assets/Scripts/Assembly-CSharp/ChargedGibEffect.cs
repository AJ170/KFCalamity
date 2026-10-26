using EventHelpers;
using Ragdoll;

public class ChargedGibEffect : GibEffect
{
	private ChargedProjectile chargedProjectile;

	public float ChargeThreshold = 0.5f;

	public float GreaterThreshold = 0.9f;

	protected override void Start()
	{
		base.Start();
		FindProjectile();
	}

	protected override void Execute(TriggerBase e)
	{
		if (chargedProjectile != null)
		{
			if (chargedProjectile.BonusCharge > GreaterThreshold)
			{
				Type = GibType.Greater;
			}
			else
			{
				Type = GibType.Lesser;
			}
		}
		base.Execute(e);
	}

	private void FindProjectile()
	{
		if (base.transform.parent != null)
		{
			chargedProjectile = base.transform.parent.GetComponent<ChargedProjectile>();
		}
	}

	protected override bool ShouldTrigger(TriggerBase e)
	{
		if (chargedProjectile != null)
		{
			float bonusCharge = chargedProjectile.BonusCharge;
			if (bonusCharge > ChargeThreshold)
			{
				return base.ShouldTrigger(e);
			}
		}
		return false;
	}
}
