using EffectHelpers;
using UnityEngine;

public class PatriarchLastStandShieldEffect : EffectBaseTarget
{
	private Health health;

	private GameObject immuneParticle;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = 6f;
		maxRepeats = 1;
		health = gameObject.GetComponentInRoot<Health>();
	}

	protected override void OnActivated()
	{
		if (ParticleManager.Instance != null)
		{
			immuneParticle = ParticleManager.Instance.CreateParticle("FxLastStandShield", Vector3.zero, Quaternion.identity);
			if (immuneParticle != null)
			{
				immuneParticle.transform.parent = gameObject.transform;
				immuneParticle.transform.localPosition = new Vector3(0f, 0.5f, 0f);
				immuneParticle.GetComponent<ParticleSystem>().Play();
			}
		}
		if (health != null)
		{
			health.Invulnerable = true;
		}
	}

	protected override void OnRefreshed(EffectBaseTarget newEffect)
	{
		if (immuneParticle != null)
		{
			immuneParticle.GetComponent<ParticleSystem>().Stop();
			immuneParticle.GetComponent<ParticleSystem>().Play();
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (immuneParticle != null)
		{
			Object.Destroy(immuneParticle);
		}
		if (health != null)
		{
			health.Invulnerable = false;
		}
	}
}
