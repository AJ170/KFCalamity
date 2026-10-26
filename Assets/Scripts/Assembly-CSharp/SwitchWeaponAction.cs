using UnityEngine;

public class SwitchWeaponAction : PlayerAction
{
	private float duration = 0.7f;

	private float counter;

	private PlayerWeaponController weaponController;

	public SwitchWeaponAction()
		: base(ActionType.SwitchWeapon)
	{
	}

	public override void OnEnter(GameObject gameObject, PlayerActionController controller)
	{
		weaponController = gameObject.GetComponent<PlayerWeaponController>();
		SetLaser(false);
		counter = duration;
		OnEnter();
	}

	public override void Update()
	{
		if (counter > 0f)
		{
			counter -= Time.deltaTime;
			if (counter <= 0f)
			{
				OnTimerDone();
			}
		}
	}

	private void OnTimerDone()
	{
		state = ActionState.Completed;
		if (weaponController != null)
		{
			weaponController.OnWeaponSwitchDone();
		}
	}

	public override void OnExit()
	{
		SetLaser(true);
	}

	private void SetLaser(bool active)
	{
		if (weaponController != null)
		{
			weaponController.SetLaserActive(active, this);
		}
	}

	private void Abort()
	{
		state = ActionState.Aborted;
	}

	public override bool CanAttack()
	{
		return false;
	}

	public override bool CanReload()
	{
		return false;
	}

	public override bool CanDropWeapon()
	{
		return false;
	}

	public override void OnSwitchWeapon()
	{
		Abort();
	}

	public override void OnUseAbility(Ability ability, AbilitySlot slot)
	{
	}
}
