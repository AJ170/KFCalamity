using UnityEngine;

public abstract class PlayerAction
{
	public enum ActionType
	{
		Dead = 0,
		SpecialFire = 1,
		UseAbility = 2,
		ReloadNormal = 3,
		ReloadShell = 4,
		ReloadSecondary = 5,
		SwitchWeapon = 6,
		FriendlyAim = 7,
		Dodge = 8,
		Rampage = 9,
		MeleeDash = 10
	}

	protected enum ActionState
	{
		Inactive = 0,
		Active = 1,
		Completed = 2,
		Aborted = 3
	}

	protected ActionType type;

	protected ActionState state;

	public ActionType Type
	{
		get
		{
			return type;
		}
	}

	public bool Active
	{
		get
		{
			return state == ActionState.Active;
		}
	}

	public bool Completed
	{
		get
		{
			return state == ActionState.Completed;
		}
	}

	public bool Aborted
	{
		get
		{
			return state == ActionState.Aborted;
		}
	}

	public bool IsDone
	{
		get
		{
			return state == ActionState.Completed || state == ActionState.Aborted;
		}
	}

	public PlayerAction(ActionType type)
	{
		this.type = type;
	}

	protected void OnEnter()
	{
		state = ActionState.Active;
	}

	public virtual void OnEnter(GameObject gameObject, PlayerActionController controller)
	{
		state = ActionState.Active;
	}

	public virtual void Update()
	{
	}

	public virtual void OnExit()
	{
	}

	public virtual void OnAborted()
	{
		state = ActionState.Aborted;
	}

	public virtual bool CanAttack()
	{
		return true;
	}

	public virtual bool CanUseAbility()
	{
		return true;
	}

	public virtual bool CanMove()
	{
		return true;
	}

	public virtual bool CanRotate()
	{
		return true;
	}

	public virtual bool CanReload()
	{
		return true;
	}

	public virtual bool CanSwitchWeapon()
	{
		return true;
	}

	public virtual bool CanDropWeapon()
	{
		return true;
	}

	public virtual bool CanAim()
	{
		return true;
	}

	public virtual void OnAttack()
	{
	}

	public virtual void OnUseAbility(Ability ability, AbilitySlot slot)
	{
	}

	public virtual void OnReload()
	{
	}

	public virtual void OnSwitchWeapon()
	{
	}

	public virtual void OnDropWeapon()
	{
	}

	public virtual void OnDeath()
	{
	}

	public virtual void OnAim()
	{
	}

	public virtual void OnDodge()
	{
	}
}
