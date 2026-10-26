using System.Collections.Generic;
using ActorEvents;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
	private List<PlayerAction> activeActions = new List<PlayerAction>();

	private ActorEventController eventController;

	private void Start()
	{
		eventController = GetComponent<ActorEventController>();
	}

	private void Update()
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			PlayerAction playerAction = activeActions[num];
			if (playerAction.IsDone)
			{
				RemoveAction(playerAction);
			}
			else
			{
				playerAction.Update();
				if (playerAction.IsDone)
				{
					RemoveAction(playerAction);
				}
			}
		}
	}

	public void AddAction(PlayerAction action)
	{
		action.OnEnter(base.gameObject, this);
		activeActions.Add(action);
	}

	private void RemoveAction(PlayerAction action)
	{
		if (action.Aborted)
		{
			action.OnAborted();
		}
		action.OnExit();
		activeActions.Remove(action);
	}

	public void OnAttack()
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone)
			{
				activeActions[num].OnAttack();
			}
		}
	}

	public void OnUseAbility(Ability ability, AbilitySlot slot)
	{
		if (eventController != null)
		{
			eventController.Trigger(new UseAbility(ability));
		}
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone)
			{
				activeActions[num].OnUseAbility(ability, slot);
			}
		}
	}

	public void OnReload()
	{
		if (eventController != null)
		{
			eventController.Trigger(new ReloadWeapon());
		}
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone)
			{
				activeActions[num].OnReload();
			}
		}
	}

	public void OnSwitchWeapon()
	{
		if (eventController != null)
		{
			eventController.Trigger(new SwitchWeapon());
		}
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone)
			{
				activeActions[num].OnSwitchWeapon();
			}
		}
	}

	public void OnDropWeapon()
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone)
			{
				activeActions[num].OnDropWeapon();
			}
		}
	}

	private void OnDeath(Health health)
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone)
			{
				activeActions[num].OnDeath();
			}
		}
	}

	public void OnAim()
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone)
			{
				activeActions[num].OnAim();
			}
		}
	}

	public void OnDodge()
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone)
			{
				activeActions[num].OnDodge();
			}
		}
	}

	public virtual bool CanAttack()
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone && !activeActions[num].CanAttack())
			{
				return false;
			}
		}
		return true;
	}

	public virtual bool CanUseAbility()
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone && !activeActions[num].CanUseAbility())
			{
				return false;
			}
		}
		return true;
	}

	public virtual bool CanMove()
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone && !activeActions[num].CanMove())
			{
				return false;
			}
		}
		return true;
	}

	public virtual bool CanRotate()
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone && !activeActions[num].CanRotate())
			{
				return false;
			}
		}
		return true;
	}

	public virtual bool CanReload()
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone && !activeActions[num].CanReload())
			{
				return false;
			}
		}
		return true;
	}

	public virtual bool CanSwitchWeapon()
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone && !activeActions[num].CanSwitchWeapon())
			{
				return false;
			}
		}
		return true;
	}

	public virtual bool CanDropWeapon()
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone && !activeActions[num].CanDropWeapon())
			{
				return false;
			}
		}
		return true;
	}

	public virtual bool CanAim()
	{
		for (int num = activeActions.Count - 1; num >= 0; num--)
		{
			if (!activeActions[num].IsDone && !activeActions[num].CanAim())
			{
				return false;
			}
		}
		return true;
	}
}
