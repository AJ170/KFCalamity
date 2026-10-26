using EffectHelpers;

public class GorefastChargeEffect : EffectBaseTarget
{
	private AttributeModifier speedAttribute;

	private AttributeModifier dmgReceivedAttribute;

	private EffectResistance constraintResistance;

	private ZedAnimationController animationController;

	private float duration = 4f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = duration;
		maxRepeats = 1;
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
		dmgReceivedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageReceived, gameObject);
		constraintResistance = EffectResistanceController.CreateResistance(BuffClass.Constraint, gameObject);
		animationController = gameObject.GetComponent<ZedAnimationController>();
	}

	protected override void OnActivated()
	{
		speedAttribute.Alter(this, ScalingByGameSettings.GorefastChargeSpeedModifierByDifficuity());
		dmgReceivedAttribute.Alter(this, ScalingByGameSettings.GorefastChargeDamageModifierByDifficuity());
		if (animationController != null)
		{
			animationController.TriggerAnimation(ZedAnimationTrigger.Charge);
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		speedAttribute.Restore(this);
		dmgReceivedAttribute.Restore(this);
		constraintResistance.Restore(this);
		if (animationController != null)
		{
			animationController.TriggerAnimation(ZedAnimationTrigger.Charge, false);
		}
	}
}
