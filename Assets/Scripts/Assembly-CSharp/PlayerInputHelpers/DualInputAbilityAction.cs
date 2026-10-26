using Calamity.Input;

namespace PlayerInputHelpers
{
	public class DualInputAbilityAction : DualInputActionBase
	{
		private PlayerAbilityController abilityController;

		private AbilitySlot normalAbilitySlot;

		private AbilitySlot thresholdAbilitySlot;

		public DualInputAbilityAction(SoldierInputController playerInputController, PlayerAbilityController abilityController, AbilitySlot normalAbilitySlot, AbilitySlot thresholdAbilitySlot, float inputThreshold)
			: base(inputThreshold, playerInputController)
		{
			this.abilityController = abilityController;
			this.normalAbilitySlot = normalAbilitySlot;
			this.thresholdAbilitySlot = thresholdAbilitySlot;
		}

		protected override void ThresholdInputStart(float threshold)
		{
			abilityController.OnInputChargeUpStart(thresholdAbilitySlot, threshold);
		}

		protected override void ThresholdInputRelease()
		{
			abilityController.OnInputChargeUpRelease(thresholdAbilitySlot);
		}

		protected override bool ThresholdEvaluator()
		{
			if (thresholdAbilitySlot == AbilitySlot.SyringeTarget)
			{
				return abilityController.HasChargesLeft(AbilitySlot.SyringeSelf);
			}
			return true;
		}

		protected override bool ThredholdAction()
		{
			if (abilityController != null)
			{
				bool flag = CastAbility(thresholdAbilitySlot);
				abilityController.OnInputChargeUpMax(thresholdAbilitySlot, flag);
				return flag;
			}
			return false;
		}

		protected override bool NormalAction()
		{
			if (abilityController != null)
			{
				return CastAbility(normalAbilitySlot);
			}
			return false;
		}

		private bool CastAbility(AbilitySlot slot)
		{
			if (slot != 0 && CanCast(slot))
			{
				abilityController.Cast(slot);
				return true;
			}
			return false;
		}

		private bool CanCast(AbilitySlot slot)
		{
			bool flag = abilityController.CanCast(slot);
			if (flag && slot == AbilitySlot.SyringeTarget)
			{
				flag = abilityController.CanCast(AbilitySlot.SyringeSelf, true);
			}
			return flag;
		}
	}
}
