using EffectHelpers;
using UnityEngine;

public class SuperResistanceBonusEffect : EffectBaseTarget
{
	private AttributeModifier damageReceived;

	private GameObject bonusParticle;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = 1f;
		maxRepeats = 30;
		damageReceived = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageReceived, gameObject);
	}

	protected override void OnActivated()
	{
		damageReceived.Alter(this, 0.5f);
		if (ParticleManager.Instance != null)
		{
			bonusParticle = ParticleManager.Instance.CreateParticle("FxBoostShield", Vector3.zero, Quaternion.identity);
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
		damageReceived.Restore(this);
	}
}
