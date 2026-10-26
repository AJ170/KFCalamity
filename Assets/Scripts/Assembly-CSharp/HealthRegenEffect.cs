using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class HealthRegenEffect : EffectBaseTarget
{
	private Health healthComponent;

	private int healthRegen = 3;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Infinite;
		buffType = BuffType.None;
		buffClass = BuffClass.None;
		repeatInterval = 1f;
		maxRepeats = 3;
		receiveTriggers = true;
		healthComponent = gameObject.GetComponentInRoot<Health>();
		if (healthComponent == null)
		{
			Debug.LogError(string.Format("HealthRegenEffect:Awake() Missing Health component in {0}", gameObject.transform.root.name));
		}
	}

	public override bool Evaluator()
	{
		return !TimeToDelete();
	}

	protected override void OnActivated()
	{
		healthComponent.CurrentHealthRegen = healthRegen;
	}

	protected override void OnUpdate()
	{
		if (TimeToDelete())
		{
			deleted = true;
		}
	}

	public override void OnReceivedTrigger(TriggerBase e)
	{
		if (e.Id == TriggerId.DamageReceived)
		{
			deleted = true;
		}
	}

	private bool TimeToDelete()
	{
		if (healthComponent != null && !healthComponent.Dead)
		{
			float num = healthComponent.CurrentHealth;
			float num2 = healthComponent.MaxHealth;
			float num3 = num / num2;
			if (num3 >= 0.25f)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	public override void Shutdown()
	{
		base.Shutdown();
		healthComponent.RestoreHealthRegen();
	}
}
