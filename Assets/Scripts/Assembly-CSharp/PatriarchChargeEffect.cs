using EffectHelpers;

public class PatriarchChargeEffect : EffectBaseTarget
{
	private AttributeModifier speedAttribute;

	private EffectResistance constraintResistance;

	private ZedAnimationController animationController;

	private float duration = 5f;

	private float speedMultiplier = 1.5f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = duration;
		maxRepeats = 1;
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
		constraintResistance = EffectResistanceController.CreateResistance(BuffClass.Constraint, gameObject);
		animationController = gameObject.GetComponent<ZedAnimationController>();
	}

	protected override void OnActivated()
	{
		speedAttribute.Alter(this, speedMultiplier);
		if (animationController != null)
		{
			animationController.TriggerAnimation(ZedAnimationTrigger.Charge);
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		speedAttribute.Restore(this);
		constraintResistance.Restore(this);
		if (animationController != null)
		{
			animationController.TriggerAnimation(ZedAnimationTrigger.Charge, false);
		}
	}
}
