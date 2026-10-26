using EffectEvents;
using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class HurryEffect : EffectBaseTarget
{
	private AttributeModifier speedAttribute;

	private EffectResistance slowResistance;

	private GameObject hurryParticle;

	private PlayerAnimationController animationController;

	private float speedMultiplier = 1.3f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = 1f;
		maxRepeats = 10;
		receiveTriggers = true;
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
		slowResistance = EffectResistanceController.CreateResistance(BuffClass.Constraint, gameObject);
		animationController = gameObject.GetComponent<PlayerAnimationController>();
	}

	protected override void OnActivated()
	{
		if (ParticleManager.Instance != null)
		{
			hurryParticle = ParticleManager.Instance.CreateParticle("FxHurry", Vector3.zero, Quaternion.identity);
			if (hurryParticle != null)
			{
				hurryParticle.transform.parent = gameObject.transform;
				hurryParticle.transform.localPosition = new Vector3(0f, 0.1f, 0f);
				hurryParticle.transform.localRotation = Quaternion.Euler(Vector3.zero);
				hurryParticle.GetComponent<ParticleSystem>().Play();
			}
		}
		if (animationController != null)
		{
			animationController.TriggerAnimation(PlayerAnimationTrigger.Fast);
		}
		slowResistance.SetTemporaryImmunity(this);
		speedAttribute.Alter(this, speedMultiplier);
	}

	protected override void OnRefreshed(EffectBaseTarget newEffect)
	{
		if (hurryParticle != null)
		{
			hurryParticle.GetComponent<ParticleSystem>().Stop();
			hurryParticle.GetComponent<ParticleSystem>().Play();
		}
	}

	public override void OnReceivedTrigger(TriggerBase e)
	{
		if (e.Id != TriggerId.Hit && e.Id != TriggerId.HitAOE)
		{
			return;
		}
		HitBase hitBase = (HitBase)e;
		Transform hitTransform = hitBase.HitTransform;
		if (hitTransform != null)
		{
			EffectController component = hitTransform.GetComponent<EffectController>();
			if (component != null)
			{
				component.CreateEffect("WeaponKnockbackEffect", gameObject, base.Creator);
			}
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (hurryParticle != null)
		{
			Object.Destroy(hurryParticle);
		}
		if (animationController != null)
		{
			animationController.TriggerAnimation(PlayerAnimationTrigger.Fast, false);
		}
		speedAttribute.Restore(this);
		slowResistance.Restore(this);
	}
}
