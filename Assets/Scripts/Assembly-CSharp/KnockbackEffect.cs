using EffectHelpers;
using UnityEngine;

public class KnockbackEffect : EffectBaseTarget
{
	private AIController aiController;

	private EffectResistance constraint;

	protected float duration = 0.03f;

	protected float knockbackForce = 10f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Debuff;
		buffClass = BuffClass.Constraint;
		repeatInterval = duration;
		maxRepeats = 1;
		aiController = gameObject.GetComponent<AIController>();
		constraint = EffectResistanceController.CreateResistance(BuffClass.Constraint, gameObject);
	}

	protected override void OnActivated()
	{
		if (aiController != null)
		{
			Vector3 normalized = (gameObject.transform.position - base.Source.transform.position).normalized;
			aiController.TriggerAIEvent(new KnockbackAIEvent(normalized, knockbackForce * constraint.Multiplier, duration));
		}
	}
}
