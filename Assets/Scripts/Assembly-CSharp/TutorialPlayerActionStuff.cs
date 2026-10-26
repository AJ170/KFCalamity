using UnityEngine;

public class TutorialPlayerActionStuff : PlayerAction
{
	private TutorialPlayerActionBase baseAction;

	private TutorialActionType actionType;

	public TutorialPlayerActionStuff(TutorialPlayerActionBase ba, TutorialActionType actionType)
		: base(ActionType.Dodge)
	{
		baseAction = ba;
		this.actionType = actionType;
		state = ActionState.Active;
	}

	public override void OnDodge()
	{
		if (actionType == TutorialActionType.Dodge)
		{
			TutorialPlayerActionDodge tutorialPlayerActionDodge = baseAction as TutorialPlayerActionDodge;
			tutorialPlayerActionDodge.SetComplete();
			state = ActionState.Completed;
		}
	}

	public override void OnAim()
	{
	}

	public override void OnUseAbility(Ability ability, AbilitySlot slot)
	{
		if (slot == AbilitySlot.Grenade)
		{
			Debug.Log("GRENADE!");
		}
	}

	public override void OnDropWeapon()
	{
		if (actionType == TutorialActionType.DropWeapon)
		{
			state = ActionState.Completed;
		}
	}
}
