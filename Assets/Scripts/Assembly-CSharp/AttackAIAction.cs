using AIActionTransitions;
using EffectEvents;
using UnityEngine;

internal class AttackAIAction : AIAction
{
	private WeaponController weaponController;

	private bool firstUpdate = true;

	private float counter;

	public override void OnStart()
	{
		weaponController = controller.GetComponent<WeaponController>();
		if (weaponController == null)
		{
			Debug.LogError("AttackAIAction failed to locate WeaponController on unit " + controller.name);
			status = AIActionStatus.Failed;
			return;
		}
		Weapon activeWeapon = weaponController.ActiveWeapon;
		if (activeWeapon != null)
		{
			counter = activeWeapon.RoundCooldown;
		}
		controller.TriggerGlobalEvent(new AIAttack());
		base.OnStart();
	}

	public override Transition Update()
	{
		if (firstUpdate && weaponController.CanAttack())
		{
			weaponController.Attack();
		}
		counter -= Time.deltaTime;
		if (counter <= 0f)
		{
			status = AIActionStatus.Complete;
		}
		return base.Update();
	}

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		if (triggeredEvent.ID == AIEventID.TargetChanged || triggeredEvent.ID == AIEventID.TargetDied || triggeredEvent.ID == AIEventID.TargetLost || triggeredEvent.ID == AIEventID.UnitDeath)
		{
			return new Transition(TransitionType.Done);
		}
		return null;
	}
}
