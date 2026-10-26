using UnityEngine;

public class UseAbilityAction : PlayerAction
{
	private enum UseAbilityState
	{
		Inactive = 0,
		ChargeUp = 1,
		WindDown = 2,
		Completed = 3
	}

	private Ability ability;

	private GameObject gameObject;

	private AbilityController abilityController;

	private PlayerWeaponController weaponController;

	private UseAbilityState abilityState;

	private float counter;

	public UseAbilityAction(Ability ability)
		: base(ActionType.UseAbility)
	{
		this.ability = ability;
	}

	public override void OnEnter(GameObject gameObject, PlayerActionController controller)
	{
		this.gameObject = gameObject;
		abilityController = ability.AbilityController();
		weaponController = gameObject.GetComponent<PlayerWeaponController>();
		if (ability.ChargeUp > 0f)
		{
			StartChargeUp();
		}
		else
		{
			Cast();
		}
		SetLaser(false);
		OnEnter();
	}

	public override void Update()
	{
		if (!(counter > 0f))
		{
			return;
		}
		counter -= Time.deltaTime;
		if (counter <= 0f)
		{
			if (abilityState == UseAbilityState.ChargeUp)
			{
				Cast();
			}
			else if (abilityState == UseAbilityState.WindDown)
			{
				OnCompleted();
			}
		}
	}

	private void StartChargeUp()
	{
		ability.OnChargeUp();
		counter = ability.ChargeUp;
		abilityState = UseAbilityState.ChargeUp;
	}

	private void Cast()
	{
		ability.Cast();
		OnCastCompleted();
		if (ability.WindDown > 0f)
		{
			StartWindDown();
		}
		else
		{
			OnCompleted();
		}
	}

	private void OnCastCompleted()
	{
		if (abilityController != null)
		{
			abilityController.OnCastCompleted(ability);
		}
	}

	private void StartWindDown()
	{
		counter = ability.WindDown;
		abilityState = UseAbilityState.WindDown;
	}

	private void OnCompleted()
	{
		state = ActionState.Completed;
		if (gameObject != null)
		{
			PlayerWeaponController component = gameObject.GetComponent<PlayerWeaponController>();
			if (component != null)
			{
				component.TriggerReloadIfNeeded();
			}
		}
	}

	public override void OnExit()
	{
		SetLaser(true);
	}

	public override void OnAborted()
	{
		state = ActionState.Aborted;
	}

	private void Abort()
	{
		state = ActionState.Aborted;
	}

	private void SetLaser(bool active)
	{
		if (weaponController != null)
		{
			weaponController.SetLaserActive(active, this);
		}
	}

	public override bool CanAttack()
	{
		return false;
	}

	public override bool CanUseAbility()
	{
		return false;
	}

	public override bool CanReload()
	{
		return false;
	}

	public override bool CanSwitchWeapon()
	{
		return false;
	}

	public override bool CanDropWeapon()
	{
		return false;
	}

	public override void OnDeath()
	{
		Abort();
	}
}
