using Calamity.Input;

namespace PlayerInputHelpers
{
	public class DualInputWeaponAction : DualInputActionBase
	{
		private PlayerWeaponController weaponController;

		private WeaponAction normalWeaponAction;

		private WeaponAction thresholdWeaponAction;

		private InventoryWeaponSlot slot;

		public DualInputWeaponAction(SoldierInputController playerInputController, PlayerWeaponController weaponController, WeaponAction normalWeaponAction, WeaponAction thresholdWeaponAction, float inputThreshold, InventoryWeaponSlot slot = InventoryWeaponSlot.None)
			: base(inputThreshold, playerInputController)
		{
			this.weaponController = weaponController;
			this.normalWeaponAction = normalWeaponAction;
			this.thresholdWeaponAction = thresholdWeaponAction;
			this.slot = slot;
		}

		protected override bool ThredholdAction()
		{
			if (weaponController != null)
			{
				return ExcecuteWeaponAction(thresholdWeaponAction);
			}
			return false;
		}

		protected override bool NormalAction()
		{
			if (weaponController != null)
			{
				return ExcecuteWeaponAction(normalWeaponAction);
			}
			return false;
		}

		private bool ExcecuteWeaponAction(WeaponAction action)
		{
			switch (action)
			{
			case WeaponAction.Reload:
				if (weaponController.CanReload())
				{
					weaponController.Reload();
					return true;
				}
				return weaponController.TriggerSecondaryReloadIfNeeded();
			case WeaponAction.Drop:
				if (weaponController.CanDropWeapon(slot))
				{
					if (slot != InventoryWeaponSlot.None)
					{
						weaponController.DropWeaponBySlot(slot);
					}
					else
					{
						weaponController.DropActiveWeapon();
					}
					return true;
				}
				break;
			case WeaponAction.Equip:
				if (slot != InventoryWeaponSlot.None)
				{
					return weaponController.SetActiveWeapon(slot);
				}
				return false;
			}
			return false;
		}
	}
}
