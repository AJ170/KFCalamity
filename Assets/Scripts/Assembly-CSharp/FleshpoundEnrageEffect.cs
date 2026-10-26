using EffectHelpers;
using UnityEngine;

public class FleshpoundEnrageEffect : EffectBaseTarget
{
	private AITargetHandler targetHandler;

	private MaterialSelector lightMaterialSelector;

	private AttributeModifier speedAttribute;

	private AttributeModifier dmgDealtAttribute;

	private EffectResistance constraintResistance;

	private GameObject enrageParticle;

	private float speedMultiplier = 1.8f;

	private float dmgDealtMultiplier = 1.3f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = 1f;
		maxRepeats = 10;
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
		dmgDealtAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageDealt, gameObject);
		constraintResistance = EffectResistanceController.CreateResistance(BuffClass.Constraint, gameObject);
	}

	protected override void OnActivated()
	{
		if (ParticleManager.Instance != null)
		{
			enrageParticle = ParticleManager.Instance.CreateParticle("FxFleshpoundEnrage", Vector3.zero, Quaternion.identity);
			if (enrageParticle != null)
			{
				enrageParticle.transform.parent = gameObject.transform;
				enrageParticle.transform.localPosition = new Vector3(0f, 1.5f, 0f);
				enrageParticle.GetComponent<ParticleSystem>().Play();
			}
		}
		speedAttribute.Alter(this, speedMultiplier);
		dmgDealtAttribute.Alter(this, dmgDealtMultiplier);
		constraintResistance.SetTemporaryImmunity(this);
		targetHandler = gameObject.GetComponent<AITargetHandler>();
		if (targetHandler != null)
		{
			targetHandler.FixateTargets = true;
		}
		lightMaterialSelector = gameObject.GetComponent<MaterialSelector>();
		if (lightMaterialSelector != null)
		{
			lightMaterialSelector.SetMaterial(1);
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (enrageParticle != null)
		{
			Object.Destroy(enrageParticle);
		}
		speedAttribute.Restore(this);
		dmgDealtAttribute.Restore(this);
		constraintResistance.Restore(this);
		if (targetHandler != null)
		{
			targetHandler.FixateTargets = false;
		}
		if (lightMaterialSelector != null)
		{
			lightMaterialSelector.SetMaterial(0);
		}
	}
}
