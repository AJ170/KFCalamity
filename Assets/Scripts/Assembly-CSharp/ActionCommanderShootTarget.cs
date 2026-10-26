using EffectEvents;
using UnityEngine;

public class ActionCommanderShootTarget : TutorialSubStepBase
{
	public GameObject Commander;

	public GameObject Target;

	private PlayerWeaponController commanderWeaponController;

	private Health targetHealth;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTargetDeath(Health health)
	{
		SetComplete(true);
	}

	public override void OnStart()
	{
		EffectEventDelegator component = Target.GetComponent<EffectEventDelegator>();
		component.TriggerInternal(new Death());
		Health component2 = Target.GetComponent<Health>();
		component2.OnDeathSignal.AddReceiver(OnTargetDeath);
		AttributeModifier attributeModifier = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageDealt, Commander);
		component2.TakeDamage(component2.CurrentHealth, DamageType.Explosion, attributeModifier.Multiplier, true);
	}
}
