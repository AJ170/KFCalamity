using Calamity.Input;
using EffectHelpers;
using UnityEngine;

public class RampageAction : PlayerAction
{
	private enum InternalState
	{
		Inactive = 0,
		Dodging = 1,
		PostDodgeDelay = 2
	}

	private float duration = 0.5f;

	private float speedMultiplier = 3f;

	private float postDodgeDelay = 0.1f;

	private GameObject gameObject;

	private MovementMotor motor;

	private PlayerAnimationController animationController;

	private SoldierInputController inputController;

	private PlayerWeaponController weaponController;

	private AimController aimController;

	private AttributeModifier speedAttribute;

	private EffectResistance constraintResistance;

	private EffectResistance fireResistance;

	private InternalState dodgeState;

	private float counter;

	private int oldLayer;

	private int layer;

	public RampageAction()
		: base(ActionType.Rampage)
	{
	}

	public override void OnEnter(GameObject gameObject, PlayerActionController controller)
	{
		this.gameObject = gameObject;
		animationController = gameObject.GetComponent<PlayerAnimationController>();
		inputController = gameObject.GetComponent<SoldierInputController>();
		weaponController = gameObject.GetComponent<PlayerWeaponController>();
		motor = gameObject.GetComponent<MovementMotor>();
		aimController = gameObject.GetComponent<AimController>();
		if (motor == null)
		{
			state = ActionState.Aborted;
			return;
		}
		Vector3 movementDirection = motor.MovementDirection;
		Vector3 vector = ((!(movementDirection != Vector3.zero)) ? gameObject.transform.forward : movementDirection.normalized);
		motor.SetMovementDirection(vector);
		motor.SetFacingDirection(Vector3.zero);
		motor.SetForcedGravity(-9.81f);
		gameObject.transform.rotation = Quaternion.LookRotation(vector);
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
		speedAttribute.Alter(this, speedMultiplier);
		constraintResistance = EffectResistanceController.CreateResistance(BuffClass.Constraint, gameObject);
		constraintResistance.SetTemporaryImmunity(this);
		fireResistance = EffectResistanceController.CreateResistance(BuffClass.Fire, gameObject);
		fireResistance.SetTemporaryImmunity(this);
		if (animationController != null)
		{
			animationController.SetBool(PlayerAnimationTrigger.Rampage, true);
		}
		if (inputController != null)
		{
			inputController.SetLock(true, false, this);
		}
		if (aimController != null && aimController.IsAiming())
		{
			aimController.SetActiveState(AimState.AimStateType.Free);
		}
		SetLaser(false);
		counter = duration;
		oldLayer = gameObject.layer;
		gameObject.layer = LayerMask.NameToLayer("PlayerDodge");
		dodgeState = InternalState.Dodging;
		OnEnter();
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
		if (dodgeState == InternalState.Dodging && postDodgeDelay > 0f)
		{
			if (inputController != null)
			{
				inputController.SetMovementLock(false, this);
			}
			speedAttribute.Restore(this);
			constraintResistance.Restore(this);
			fireResistance.Restore(this);
			gameObject.layer = oldLayer;
			if (animationController != null)
			{
				animationController.SetBool(PlayerAnimationTrigger.Rampage, false);
			}
			counter = postDodgeDelay;
			dodgeState = InternalState.PostDodgeDelay;
		}
		else
		{
			state = ActionState.Completed;
			Reset();
			OnCompleted();
		}
	}

	public override void OnAborted()
	{
		Reset();
	}

	private void Reset()
	{
		motor.SetMovementDirection(Vector3.zero);
		motor.SetForcedGravity(0f);
		if (inputController != null)
		{
			inputController.SetLock(false, this);
		}
		speedAttribute.Restore(this);
		constraintResistance.Restore(this);
		fireResistance.Restore(this);
		gameObject.layer = oldLayer;
		SetLaser(true);
		if (animationController != null)
		{
			animationController.SetBool(PlayerAnimationTrigger.Rampage, false);
		}
	}

	private void OnCompleted()
	{
		if (gameObject != null)
		{
			PlayerWeaponController component = gameObject.GetComponent<PlayerWeaponController>();
			if (component != null)
			{
				component.TriggerReloadIfNeeded();
			}
		}
	}

	private void SetLaser(bool active)
	{
		if (weaponController != null)
		{
			weaponController.SetLaserActive(active, this);
		}
	}

	public override bool CanAim()
	{
		return false;
	}
}
