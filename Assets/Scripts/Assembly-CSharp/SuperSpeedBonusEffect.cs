using EffectHelpers;
using UnityEngine;

public class SuperSpeedBonusEffect : EffectBaseTarget
{
	private AttributeModifier speedAttribute;

	private GameObject bonusParticle;

	private PlayerAnimationController animationController;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = 1f;
		maxRepeats = 30;
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
		animationController = gameObject.GetComponent<PlayerAnimationController>();
	}

	protected override void OnActivated()
	{
		speedAttribute.Alter(this, 1.5f);
		if (ParticleManager.Instance != null)
		{
			bonusParticle = ParticleManager.Instance.CreateParticle("FxHurry", Vector3.zero, Quaternion.identity);
			if (bonusParticle != null)
			{
				bonusParticle.transform.parent = gameObject.transform;
				bonusParticle.transform.localPosition = Vector3.zero;
				bonusParticle.GetComponent<ParticleSystem>().Play();
			}
		}
		if (animationController != null)
		{
			animationController.TriggerAnimation(PlayerAnimationTrigger.Fast);
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (bonusParticle != null)
		{
			Object.Destroy(bonusParticle);
		}
		if (animationController != null)
		{
			animationController.TriggerAnimation(PlayerAnimationTrigger.Fast, false);
		}
		speedAttribute.Restore(this);
	}
}
