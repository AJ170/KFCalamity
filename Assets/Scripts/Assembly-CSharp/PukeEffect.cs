using UnityEngine;

public class PukeEffect : DamageOverTimeEffect
{
	private GameObject pukeParticle;

	private AttributeModifier healingAttribute;

	private float healingMultiplier = 0.25f;

	protected override void Awake()
	{
		base.Awake();
		repeatInterval = 0.5f;
		maxRepeats = 10;
		damageType = DamageType.Bile;
		damagePerTick = 3;
		healingAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.HealingReceived, gameObject);
	}

	protected override void OnActivated()
	{
		if (ParticleManager.Instance != null)
		{
			pukeParticle = ParticleManager.Instance.CreateParticle("FxBloatPuked", Vector3.zero, Quaternion.identity);
			if (pukeParticle != null)
			{
				pukeParticle.transform.parent = gameObject.transform;
				pukeParticle.transform.localPosition = new Vector3(0f, 1.8f, 0f);
				pukeParticle.GetComponent<ParticleSystem>().Play();
			}
		}
		healingAttribute.Alter(this, healingMultiplier);
	}

	public override void Shutdown()
	{
		base.Shutdown();
		healingAttribute.Restore(this);
		if (pukeParticle != null)
		{
			Object.Destroy(pukeParticle);
		}
	}
}
