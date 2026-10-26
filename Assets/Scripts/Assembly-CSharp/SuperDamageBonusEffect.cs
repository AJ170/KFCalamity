using EffectHelpers;
using UnityEngine;

public class SuperDamageBonusEffect : EffectBaseTarget
{
	private AttributeModifier damageDealt;

	private AttributeModifier force;

	private GameObject bonusParticle;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = 1f;
		maxRepeats = 30;
		damageDealt = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageDealt, gameObject);
		force = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Force, gameObject);
	}

	protected override void OnActivated()
	{
		damageDealt.Alter(this, 2f);
		force.Alter(this, 2f);
		if (ParticleManager.Instance != null)
		{
			bonusParticle = ParticleManager.Instance.CreateParticle("FxBoostDamage", Vector3.zero, Quaternion.identity);
			if (bonusParticle != null)
			{
				bonusParticle.transform.parent = gameObject.transform;
				bonusParticle.transform.localPosition = Vector3.zero;
				bonusParticle.GetComponent<ParticleSystem>().Play();
			}
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (bonusParticle != null)
		{
			Object.Destroy(bonusParticle);
		}
		damageDealt.Restore(this);
		force.Restore(this);
	}
}
