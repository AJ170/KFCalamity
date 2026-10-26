using EffectEvents;
using EffectHelpers;
using UnityEngine;

public class HealOverTimeEffect : EffectBaseTarget
{
	protected int healthPerTick = 5;

	public Health healthComponent;

	private AttributeModifier healingDealt;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = 0.5f;
		maxRepeats = 10;
		healthComponent = gameObject.GetComponentInRoot<Health>();
		CreateHealingModifier();
		if (healthComponent == null)
		{
			Debug.LogError(string.Format("HealOverTimeEffect:Awake() Missing Health component in {0}", gameObject.transform.root.name));
		}
	}

	protected override void OnRefreshed(EffectBaseTarget newEffect)
	{
		CreateHealingModifier();
	}

	protected override void OnUpdate()
	{
		if (healthComponent != null)
		{
			int num = healthComponent.RestoreHealth(healthPerTick, healingDealt.Multiplier);
			if (num > 0)
			{
				GameObject healer = base.Source;
				GameObject target = gameObject;
				TriggerDelegateHealingEvents(healer, target, num);
			}
		}
	}

	private void CreateHealingModifier()
	{
		healingDealt = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.HealingDealt, base.Source);
	}

	private void TriggerDelegateHealingEvents(GameObject healer, GameObject target, int amount)
	{
		if (healer != null)
		{
			EffectEventDelegator component = healer.GetComponent<EffectEventDelegator>();
			if (component != null)
			{
				component.ListenTrigger(new HealingDealt(target, amount));
			}
		}
		base.Owner.TriggerRemote(target, new HealingReceived(healer, amount));
	}
}
