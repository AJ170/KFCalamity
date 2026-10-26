using EffectHelpers;

public class BerserkerRamageEffect : EffectBaseTarget
{
	private AttributeModifier killstreakPenalty;

	private float duration = 6f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = duration;
		maxRepeats = 1;
		killstreakPenalty = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.KillstreakPenalty, gameObject);
	}

	protected override void OnActivated()
	{
		killstreakPenalty.Alter(this, 0f);
	}

	public override void Shutdown()
	{
		killstreakPenalty.Restore(this);
	}
}
