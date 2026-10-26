using EventHelpers;
using UnityEngine;

public class TriggerPlayerActionEffect : EffectBaseListener
{
	public PlayerAction.ActionType ActionType = PlayerAction.ActionType.Dodge;

	private PlayerActionController actionController;

	private void OnMounted(GameObject parent)
	{
		actionController = parent.GetComponent<PlayerActionController>();
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && actionController != null)
		{
			PlayerAction action = GetAction();
			if (action != null)
			{
				TriggerOnAction();
				actionController.AddAction(action);
			}
		}
	}

	private PlayerAction GetAction()
	{
		PlayerAction result = null;
		switch (ActionType)
		{
		case PlayerAction.ActionType.Dodge:
			result = new DodgeAction();
			break;
		case PlayerAction.ActionType.Rampage:
			result = new RampageAction();
			break;
		}
		return result;
	}

	private void TriggerOnAction()
	{
		PlayerAction.ActionType actionType = ActionType;
		if (actionType == PlayerAction.ActionType.Dodge || actionType == PlayerAction.ActionType.Rampage)
		{
			actionController.OnDodge();
		}
	}
}
