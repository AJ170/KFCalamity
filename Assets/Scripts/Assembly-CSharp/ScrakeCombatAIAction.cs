using AIActionTransitions;

internal class ScrakeCombatAIAction : CombatAIAction
{
	private int attackCounter;

	private AIAbility impaleAbility;

	public override void OnStart()
	{
		base.OnStart();
		if (status == AIActionStatus.Active && abilityController != null)
		{
			impaleAbility = abilityController.AbilityByName("ScrakeImpale") as AIAbility;
		}
	}

	protected override void UpdateAttack(ref Transition transition)
	{
		if (transition == null && targetTransform != null && weaponController.CanAttack() && CheckInRange())
		{
			attackCounter++;
			if (attackCounter >= 3 && impaleAbility != null && impaleAbility.CanCast())
			{
				transition = new Transition(TransitionType.SuspendFor, new UseAbilityAIAction(impaleAbility));
				attackCounter = 0;
			}
			if (transition == null)
			{
				transition = new Transition(TransitionType.SuspendFor, new AttackAIAction());
			}
		}
	}
}
