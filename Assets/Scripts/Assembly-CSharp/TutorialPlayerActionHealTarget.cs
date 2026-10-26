using EventHelpers;
using UnityEngine;

public class TutorialPlayerActionHealTarget : TutorialPlayerActionBase
{
	private PlayerAbilityController abilityController;

	private Ability syringeTargetAbility;

	private Ability syringeSelfAbility;

	private Health healthComponent;

	public TutorialPlayerActionHealTarget(GameObject player)
		: base(player)
	{
	}

	public void SetComplete()
	{
		OnComplete();
	}

	private void OnFriendlyAimFailed(TriggerBase e)
	{
		if (!base.IsComplete)
		{
			OnFailed();
		}
	}

	private void OnFriendlyAim(TriggerBase e)
	{
		OnComplete();
	}

	private void OnHealSelf(TriggerBase e)
	{
		if (!base.IsComplete)
		{
			OnFailed();
		}
	}

	public override void OnShutdown()
	{
		syringeTargetAbility.EventSystem.Unsubscribe(TriggerId.FriendlyAim, this);
		syringeTargetAbility.EventSystem.Unsubscribe(TriggerId.FriendlyAimFailed, this);
		syringeSelfAbility.EventSystem.Unsubscribe(TriggerId.Cast, this);
	}

	public override void OnStartEvent()
	{
		abilityController = Player.GetComponent<PlayerAbilityController>();
		syringeTargetAbility = abilityController.AbilityBySlot(AbilitySlot.SyringeTarget);
		syringeSelfAbility = abilityController.AbilityBySlot(AbilitySlot.SyringeSelf);
		syringeTargetAbility.EventSystem.Subscribe(TriggerId.FriendlyAimFailed, OnFriendlyAimFailed, this);
		syringeTargetAbility.EventSystem.Subscribe(TriggerId.FriendlyAim, OnFriendlyAim, this);
		syringeSelfAbility.EventSystem.Subscribe(TriggerId.Cast, OnHealSelf, this);
		GameObject gameObject = GameObject.Find("TutorialSupportSpecialist2HealTarget");
		healthComponent = gameObject.GetComponent<Health>();
	}

	public override void OnEndEvent()
	{
		syringeTargetAbility.EventSystem.Unsubscribe(TriggerId.FriendlyAimFailed, this);
		syringeTargetAbility.EventSystem.Unsubscribe(TriggerId.FriendlyAim, this);
		syringeSelfAbility.EventSystem.Unsubscribe(TriggerId.Cast, this);
	}

	public override void Update()
	{
		if ((float)healthComponent.CurrentHealth / (float)healthComponent.MaxHealth > 0.9f)
		{
			OnComplete();
		}
	}
}
