using Calamity.Input;

namespace PlayerInputHelpers
{
	// PS Vita port: D-pad left / right. A tap switches to the previous / next weapon, holding past the threshold drops
	// the weapon in hand (on the console layout each weapon slot's own button does that).
	public class DualInputCycleWeaponAction : DualInputActionBase
	{
		private PlayerWeaponController weaponController;

		private bool forward;

		public DualInputCycleWeaponAction(SoldierInputController playerInputController, PlayerWeaponController weaponController, bool forward, float inputThreshold)
			: base(inputThreshold, playerInputController)
		{
			this.weaponController = weaponController;
			this.forward = forward;
		}

		protected override bool ThredholdAction()
		{
			if (weaponController != null && weaponController.CanDropActiveWeapon())
			{
				weaponController.DropActiveWeapon();
				return true;
			}
			return false;
		}

		protected override bool NormalAction()
		{
			if (weaponController != null)
			{
				return weaponController.SwitchToAdjacentWeapon(forward);
			}
			return false;
		}
	}
}
