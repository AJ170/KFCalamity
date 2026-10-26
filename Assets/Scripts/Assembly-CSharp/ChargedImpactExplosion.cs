using EffectEvents;
using EventHelpers;
using UnityEngine;

public class ChargedImpactExplosion : EffectBaseListener
{
	public GameObject NoChargeFX;

	public GameObject LightChargeFX;

	public GameObject HeavyChargeFX;

	public float HeavyThreshold = 0.5f;

	private ChargedProjectile chargedProjectile;

	protected override void Start()
	{
		base.Start();
		FindProjectile();
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			Vector3 position;
			if (e.Id == TriggerId.Impact)
			{
				Impact impact = (Impact)e;
				position = impact.Position;
			}
			else
			{
				position = base.transform.position;
			}
			GameObject fX = GetFX();
			if (fX != null)
			{
				SpawnParticle(fX, position);
			}
		}
	}

	private void SpawnParticle(GameObject prefab, Vector3 position)
	{
		GameObject effect = EffectPool.GetEffect(prefab);
		if (effect != null)
		{
			effect.SetActive(true);
			effect.transform.position = position;
			ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
			if (particleSystem != null)
			{
				particleSystem.Clear();
				particleSystem.Play();
			}
		}
	}

	private GameObject GetFX()
	{
		GameObject result = null;
		float num = 0f;
		if (chargedProjectile != null)
		{
			num = chargedProjectile.BonusCharge;
		}
		if (num > HeavyThreshold && HeavyChargeFX != null)
		{
			result = HeavyChargeFX;
		}
		else if (num > 0f && LightChargeFX != null)
		{
			result = LightChargeFX;
		}
		else if (NoChargeFX != null)
		{
			result = NoChargeFX;
		}
		return result;
	}

	private void FindProjectile()
	{
		if (base.transform.parent != null)
		{
			chargedProjectile = base.transform.parent.GetComponent<ChargedProjectile>();
		}
	}
}
