using UnityEngine;

public class DelayedWeaponSwitchAction : PlayerAction
{
	private InventoryWeaponSlot slot;

	private float delay;

	private PlayerWeaponController weaponController;

	private float counter;

	public DelayedWeaponSwitchAction(InventoryWeaponSlot slot, float delay, PlayerWeaponController weaponController)
		: base(ActionType.SwitchWeapon)
	{
		this.slot = slot;
		this.delay = delay;
		this.weaponController = weaponController;
	}

	public override void OnEnter(GameObject gameObject, PlayerActionController controller)
	{
		counter = delay;
		if (counter <= 0f)
		{
			OnTimerDone();
		}
		else
		{
			OnEnter();
		}
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
			weaponController.SetActiveWeapon(slot);
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
}
