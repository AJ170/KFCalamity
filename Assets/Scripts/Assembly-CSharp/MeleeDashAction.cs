using System;
using Calamity.Input;
using EffectHelpers;
using UnityEngine;

public class MeleeDashAction : PlayerAction
{
	private enum InternalState
	{
		Inactive = 0,
		Dashing = 1,
		StartedAttack = 2,
		Finished = 3
	}

	private GameObject gameObject;

	private Transform transform;

	private MovementMotor motor;

	private PlayerAnimationController animationController;

	private SoldierInputController inputController;

	private AimController aimController;

	private AttributeModifier speedAttribute;

	private EffectResistance constraintResistance;

	private EffectResistance fireResistance;

	private InternalState dashState;

	private float maxDashDistance = 1f;

	private float weaponMaxRange = 1f;

	private float searchAngle = 45f;

	private float speedMultiplier = 2f;

	private float targetDistance = 1f;

	private float losCheckHeightOffset = 0.5f;

	private LayerMask losLayer;

	private GameObject target;

	private Transform targetTransform;

	private Vector3 direction;

	private Vector3 targetPosition;

	private float remainingDistance;

	private Action attackTrigger;

	private float TotalRange
	{
		get
		{
			return weaponMaxRange + maxDashDistance;
		}
	}

	public MeleeDashAction(Action attackTrigger, float maxDashDistance, float weaponMaxRange, float searchAngle, float targetDistance, float speedMultiplier)
		: base(ActionType.MeleeDash)
	{
		this.attackTrigger = attackTrigger;
		this.maxDashDistance = maxDashDistance;
		this.weaponMaxRange = weaponMaxRange;
		this.searchAngle = searchAngle;
		this.targetDistance = targetDistance;
		this.speedMultiplier = speedMultiplier;
		losLayer = 1 << LayerMask.NameToLayer("Ground") << LayerMask.NameToLayer("CollisionBoxes solid") << LayerMask.NameToLayer("CollisionBoxes semi-solid") << LayerMask.NameToLayer("CollisionBoxes solid (no Line of sight)");
	}

	public override void OnEnter(GameObject gameObject, PlayerActionController controller)
	{
		this.gameObject = gameObject;
		transform = gameObject.transform;
		animationController = gameObject.GetComponent<PlayerAnimationController>();
		inputController = gameObject.GetComponent<SoldierInputController>();
		motor = gameObject.GetComponent<MovementMotor>();
		aimController = gameObject.GetComponent<AimController>();
		if (motor == null)
		{
			TriggerAttack();
			state = ActionState.Aborted;
			return;
		}
		target = GetTarget();
		if (ShouldDash())
		{
			SetupTarget();
			if (direction == Vector3.zero)
			{
				TriggerAttack();
				state = ActionState.Completed;
				return;
			}
			StartDash();
			if (animationController != null)
			{
				animationController.SetBool(PlayerAnimationTrigger.Running, true);
			}
			dashState = InternalState.Dashing;
			state = ActionState.Active;
		}
		else
		{
			TriggerAttack();
			state = ActionState.Completed;
		}
	}

	public override void Update()
	{
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		remainingDistance -= motor.CurrentMovementSpeed() * Time.deltaTime;
		if (dashState == InternalState.Dashing && remainingDistance < weaponMaxRange)
		{
			TriggerAttack();
			targetPosition = GetTargetPosition();
			dashState = InternalState.StartedAttack;
		}
		if (remainingDistance <= 0f)
		{
			state = ActionState.Completed;
		}
		else if (dashState != InternalState.StartedAttack)
		{
		}
	}

	private bool ShouldDash()
	{
		if (target != null)
		{
			float num = Vector3.Distance(transform.position, target.transform.position);
			if (num > weaponMaxRange && num < TotalRange)
			{
				return true;
			}
		}
		return false;
	}

	public override void OnExit()
	{
		Reset();
	}

	private void StartDash()
	{
		SetMovement();
		SetAttributes();
		LockInput(true);
	}

	private void SetMovement()
	{
		motor.SetMovementDirection(direction);
		motor.SetFacingDirection(direction);
	}

	private void SetAttributes()
	{
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
		if (aimController != null && aimController.IsAiming())
		{
			speedMultiplier *= 2f;
		}
		speedAttribute.Alter(this, speedMultiplier);
		constraintResistance = EffectResistanceController.CreateResistance(BuffClass.Constraint, gameObject);
		constraintResistance.SetTemporaryImmunity(this);
	}

	private void LockInput(bool lockInput)
	{
		if (inputController != null)
		{
			if (lockInput)
			{
				inputController.SetLock(true, false, this);
			}
			else
			{
				inputController.SetLock(false, this);
			}
		}
	}

	private void TriggerAttack()
	{
		if (attackTrigger != null)
		{
			attackTrigger();
		}
	}

	private void SetupTarget()
	{
		targetTransform = target.transform;
		targetPosition = GetTargetPosition();
	}

	private GameObject GetTarget()
	{
		GameObject gameObject = null;
		gameObject = GetTargetFromAim();
		if (gameObject == null)
		{
		}
		return gameObject;
	}

	private GameObject GetTargetFromAim()
	{
		if (aimController != null && aimController.IsAiming())
		{
			return aimController.AimTarget();
		}
		return null;
	}

	private GameObject SearchForTarget()
	{
		GameObject result = null;
		if (gameObject != null)
		{
			GameObject[] targets;
			int num = TargetsInRange(out targets);
			if (num > 0)
			{
				result = BestTarget(ref targets);
			}
		}
		return result;
	}

	private int TargetsInRange(out GameObject[] targets)
	{
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Enemy");
		Collider[] array = Physics.OverlapSphere(transform.position, TotalRange, layerMask);
		targets = new GameObject[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject != gameObject)
			{
				targets[i] = array[i].gameObject;
			}
		}
		return array.Length;
	}

	private GameObject BestTarget(ref GameObject[] targets)
	{
		GameObject result = null;
		if (targets.Length > 0)
		{
			Vector3 position = this.transform.position;
			Vector3 forward = this.transform.forward;
			float num = Mathf.Cos(searchAngle * ((float)Math.PI / 180f));
			float num2 = num;
			for (int i = 0; i < targets.Length; i++)
			{
				GameObject gameObject = targets[i];
				if (targets != null)
				{
					Transform transform = gameObject.transform;
					float num3 = Vector3.Dot(forward, (transform.position - position).normalized);
					if (num3 > num2 && InLineOfSight(transform))
					{
						result = gameObject;
						num2 = num3;
					}
				}
			}
		}
		return result;
	}

	private bool InLineOfSight(Transform target)
	{
		Vector3 vector = Vector3.up * losCheckHeightOffset;
		return !Physics.Linecast(transform.position + vector, target.position + vector, losLayer);
	}

	private Vector3 GetTargetPosition()
	{
		if (targetTransform != null)
		{
			Vector3 position = targetTransform.position;
			Vector3 position2 = transform.position;
			direction = Vector3.Normalize(position - position2);
			Vector3 vector = position - direction * targetDistance;
			remainingDistance = Vector3.Distance(position2, vector);
			return vector;
		}
		return targetPosition;
	}

	private void Reset()
	{
		if (speedAttribute != null)
		{
			speedAttribute.Restore(this);
		}
		if (constraintResistance != null)
		{
			constraintResistance.Restore(this);
		}
		LockInput(false);
	}
}
