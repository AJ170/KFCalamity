using EffectHelpers;

public class PatriarchStealthChargeEffect : EffectBaseTarget
{
	private AttributeModifier speedAttribute;

	private EffectResistance constraintResistance;

	private ZedAnimationController animationController;

	private StealthController stealthController;

	private float duration = 20f;

	private float speedMultiplier = 1.5f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = 0.5f;
		maxRepeats = (int)(duration / repeatInterval);
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
		constraintResistance = EffectResistanceController.CreateResistance(BuffClass.Constraint, gameObject);
		animationController = gameObject.GetComponent<ZedAnimationController>();
		stealthController = gameObject.GetComponent<StealthController>();
	}

	protected override void OnActivated()
	{
		speedAttribute.Alter(this, speedMultiplier);
		if (animationController != null)
		{
			animationController.TriggerAnimation(ZedAnimationTrigger.Charge);
		}
	}

	protected override void OnUpdate()
	{
		if (stealthController != null && !stealthController.IsStealthed)
		{
			deleted = true;
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
		if (stealthController != null && stealthController.IsStealthed)
		{
			stealthController.Unstealth();
		}
	}
}
