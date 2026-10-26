using UnityEngine;

public class ChargedProjectile : PhysicalProjectile
{
	public ParticleSystem SmallProjectileFX;

	public ParticleSystem LargeProjectileFX;

	public float LargeParticleThreshold = 0.5f;

	private float bonusCharge;

	public float BonusCharge
	{
		get
		{
			return bonusCharge;
		}
	}

	public void Fire(float bonus)
	{
		bonusCharge = bonus;
		SetFX(bonus);
		base.Fire();
	}

	private void SetFX(float bonus)
	{
		if (SmallProjectileFX != null && LargeProjectileFX != null)
		{
			if (bonus < LargeParticleThreshold)
			{
				SmallProjectileFX.Clear();
				SmallProjectileFX.gameObject.SetActive(true);
				LargeProjectileFX.gameObject.SetActive(false);
			}
			else
			{
				LargeProjectileFX.Clear();
				LargeProjectileFX.gameObject.SetActive(true);
				SmallProjectileFX.gameObject.SetActive(false);
			}
		}
		else if (SmallProjectileFX != null)
		{
			SmallProjectileFX.Clear();
			SmallProjectileFX.gameObject.SetActive(true);
		}
	}
}
