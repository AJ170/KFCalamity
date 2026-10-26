using EventHelpers;
using UnityEngine;

public class TutorialPlayerActionHeal : TutorialPlayerActionBase
{
	private PlayerAbilityController abilityController;

	private Ability syringeSelfAbility;

	private Health healthComponent;

	public TutorialPlayerActionHeal(GameObject player)
		: base(player)
	{
	}

	public override void OnShutdown()
	{
		syringeSelfAbility.EventSystem.Unsubscribe(TriggerId.Cast, this);
	}

	public override void OnStartEvent()
	{
		abilityController = Player.GetComponent<PlayerAbilityController>();
		healthComponent = Player.GetComponent<Health>();
		syringeSelfAbility = abilityController.AbilityBySlot(AbilitySlot.SyringeSelf);
		syringeSelfAbility.EventSystem.Subscribe(TriggerId.Cast, OnHealSelf, this);
	}

	public override void Update()
	{
		if ((float)healthComponent.CurrentHealth / (float)healthComponent.MaxHealth > 0.9f)
		{
			OnComplete();
		}
	}

	private void OnHealSelf(TriggerBase e)
	{
		OnComplete();
	}
}
