using EffectHelpers;
using UnityEngine;

public class MigraneEffect : EffectBaseTarget
{
	private GameObject screamParticle;

	private AttributeModifier damageAttribute;

	private AttributeModifier speedAttribute;

	private float damageMultiplier = 0.5f;

	private float speedMultiplier = 0.5f;

	protected override void Awake()
	{
		base.Awake();
		repeatInterval = 2f;
		maxRepeats = 1;
		effectType = EffectType.Continuous;
		buffType = BuffType.Debuff;
		buffClass = BuffClass.Constraint;
		damageAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageDealt, gameObject);
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
	}

	protected override void OnActivated()
	{
		if (ParticleManager.Instance != null)
		{
			screamParticle = ParticleManager.Instance.CreateParticle("FxSirenScreamRecieve", Vector3.zero, Quaternion.identity);
			if (screamParticle != null)
			{
				screamParticle.transform.parent = gameObject.transform;
				screamParticle.transform.localPosition = new Vector3(0f, 1.8f, 0f);
				screamParticle.GetComponent<ParticleSystem>().Play();
			}
		}
		damageAttribute.Alter(this, damageMultiplier);
		speedAttribute.Alter(this, speedMultiplier);
	}

	public override void Shutdown()
	{
		base.Shutdown();
		damageAttribute.Restore(this);
		speedAttribute.Restore(this);
		if (screamParticle != null)
		{
			Object.Destroy(screamParticle);
		}
	}
}
