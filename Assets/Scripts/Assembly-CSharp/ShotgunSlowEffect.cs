using EffectHelpers;
using UnityEngine;

public class ShotgunSlowEffect : EffectBaseTarget
{
	private AttributeModifier speedAttribute;

	private GameObject slowParticle;

	private ZedAnimationController animationController;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Debuff;
		buffClass = BuffClass.Constraint;
		repeatInterval = 0.1f;
		maxRepeats = 7;
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
		animationController = gameObject.GetComponent<ZedAnimationController>();
	}

	protected override void OnActivated()
	{
		if (ParticleManager.Instance != null)
		{
			slowParticle = ParticleManager.Instance.CreateParticle("FxSlow", Vector3.zero, Quaternion.identity);
			if (slowParticle != null)
			{
				slowParticle.transform.parent = gameObject.transform;
				slowParticle.transform.localPosition = Vector3.zero;
				slowParticle.GetComponent<ParticleSystem>().Play();
			}
		}
		if (animationController != null)
		{
			animationController.TriggerAnimation(ZedAnimationTrigger.Slow);
		}
		speedAttribute.Alter(this, 0.5f);
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (slowParticle != null)
		{
			Object.Destroy(slowParticle);
		}
		if (animationController != null)
		{
			animationController.TriggerAnimation(ZedAnimationTrigger.Slow, false);
		}
		speedAttribute.Restore(this);
	}
}
