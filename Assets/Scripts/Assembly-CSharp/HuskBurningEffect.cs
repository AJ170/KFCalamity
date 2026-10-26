using EffectHelpers;
using UnityEngine;

public class HuskBurningEffect : DamageOverTimeEffect
{
	private GameObject burnParticle;

	protected override void Awake()
	{
		base.Awake();
		buffClass = BuffClass.Fire;
		repeatInterval = 1f;
		maxRepeats = 5;
		damagePerTick = 10;
		damageType = DamageType.Fire;
		ignoreArmor = true;
	}

	protected override void OnActivated()
	{
		if (ParticleManager.Instance != null)
		{
			burnParticle = ParticleManager.Instance.CreateParticle("FxOnFireSmall", Vector3.zero, Quaternion.identity);
			if (burnParticle != null)
			{
				burnParticle.transform.parent = gameObject.transform;
				burnParticle.transform.localPosition = new Vector3(0f, 1.5f, 0f);
				burnParticle.GetComponent<ParticleSystem>().Play();
			}
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (burnParticle != null)
		{
			Object.Destroy(burnParticle);
		}
	}
}
