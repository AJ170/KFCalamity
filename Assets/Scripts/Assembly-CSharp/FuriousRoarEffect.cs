using EffectHelpers;

public class FuriousRoarEffect : EffectBaseTarget
{
	private AttributeModifier speedAttribute;

	private float speedMultiplier = 0.5f;

	private float duration = 2f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Debuff;
		buffClass = BuffClass.None;
		repeatInterval = duration;
		maxRepeats = 1;
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
	}

	protected override void OnActivated()
	{
		speedAttribute.Alter(this, speedMultiplier);
	}

	public override void Shutdown()
	{
		base.Shutdown();
		speedAttribute.Restore(this);
	}
}
