using AIActionTransitions;
using UnityEngine;

internal class CombatAIAction : AIAction
{
	protected AITargetHandler targetHandler;

	protected WeaponController weaponController;

	protected AIAbilityController abilityController;

	private AIMovementMotor motor;

	protected Transform targetTransform;

	protected float attackRange = 2f;

	private float updateFrequency = 10f;

	private float updateDelay;

	private float lastUpdate;

	private float targetSearchFrequency = 1f;

	private float targetSearchDelay;

	private float lastTargetSearch;

	private bool usingAbilities;

	public CombatAIAction()
	{
		updateDelay = 1f / updateFrequency;
		targetSearchDelay = 1f / targetSearchFrequency;
	}

	public override void OnStart()
	{
		weaponController = controller.GetComponent<WeaponController>();
		if (weaponController == null)
		{
			Debug.LogError("CombatAIAction failed to locate WeaponController on unit " + controller.name);
			status = AIActionStatus.Failed;
			return;
		}
		abilityController = controller.GetComponent<AIAbilityController>();
		if (abilityController != null)
		{
			SetupAbilities();
		}
		else
		{
			usingAbilities = false;
		}
		targetHandler = controller.TargetHandler;
		GameObject target = targetHandler.Target;
		if (target != null)
		{
			targetTransform = target.transform;
			motor = controller.GetComponent<AIMovementMotor>();
			StartMovingToTarget();
			base.OnStart();
		}
		else
		{
			Debug.LogError("CombatAIAction failed: no target");
			status = AIActionStatus.Failed;
		}
	}

	public override Transition Update()
	{
		Transition transition = base.Update();
		if (transition == null)
		{
			if (lastTargetSearch + targetSearchDelay <= Time.timeSinceLevelLoad)
			{
				SearchForTarget();
				lastTargetSearch = Time.timeSinceLevelLoad;
			}
			if (lastUpdate + updateDelay <= Time.timeSinceLevelLoad)
			{
				UpdateAbilityUsage(ref transition);
				UpdateAttack(ref transition);
				if (transition == null)
				{
					lastUpdate = Time.timeSinceLevelLoad;
				}
			}
		}
		return transition;
	}

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		Transition result = null;
		if (triggeredEvent.ID == AIEventID.UnitDeath)
		{
			if (status == AIActionStatus.Active)
			{
				result = new Transition(TransitionType.Done);
			}
			else
			{
				status = AIActionStatus.Complete;
			}
		}
		else if (triggeredEvent.ID == AIEventID.TargetGained)
		{
			TargetGainedAIEvent targetGainedAIEvent = (TargetGainedAIEvent)triggeredEvent;
			targetTransform = targetGainedAIEvent.Target.transform;
			StartMovingToTarget();
		}
		else if (triggeredEvent.ID == AIEventID.TargetChanged)
		{
			TargetChangedAIEvent targetChangedAIEvent = (TargetChangedAIEvent)triggeredEvent;
			targetTransform = targetChangedAIEvent.Target.transform;
		}
		else if (triggeredEvent.ID == AIEventID.TargetLost || triggeredEvent.ID == AIEventID.TargetDied)
		{
			targetTransform = null;
		}
		else if (triggeredEvent.ID == AIEventID.Motion)
		{
			if (status == AIActionStatus.Active)
			{
				MotionAIEvent motionAIEvent = triggeredEvent as MotionAIEvent;
				if (motionAIEvent != null)
				{
					result = new Transition(TransitionType.SuspendFor, new MotionAIAction(motionAIEvent.Agent, motionAIEvent.Link, motionAIEvent.Motion));
				}
			}
		}
		else if (triggeredEvent.ID == AIEventID.Stun && status == AIActionStatus.Active)
		{
			StunAIEvent stunAIEvent = triggeredEvent as StunAIEvent;
			result = new Transition(TransitionType.SuspendFor, new StunAIAction(stunAIEvent.Duration));
		}
		return result;
	}

	private void SetupAbilities()
	{
		usingAbilities = abilityController.AnyCastableAbility();
	}

	protected bool CheckInRange()
	{
		Vector3 position = controller.transform.position;
		Vector3 position2 = targetTransform.position;
		float num = Vector3.Distance(position, position2);
		return num < attackRange;
	}

	private void UpdateAbilityUsage(ref Transition transition)
	{
		if (transition == null && usingAbilities)
		{
			AIAbility aIAbility = abilityController.CheckForCastableAbility();
			if (aIAbility != null)
			{
				transition = new Transition(TransitionType.SuspendFor, new UseAbilityAIAction(aIAbility));
			}
		}
	}

	protected virtual void UpdateAttack(ref Transition transition)
	{
		if (transition == null && targetTransform != null && weaponController.CanAttack() && CheckInRange())
		{
			transition = new Transition(TransitionType.SuspendFor, new AttackAIAction());
		}
	}

	private void StartMovingToTarget()
	{
		float num = 0f;
		if (motor != null)
		{
			num = motor.Speed;
		}
		if (num > 0f)
		{
			actionController.Secondary.Push(new FollowTargetAIAction());
		}
		else
		{
			actionController.Secondary.Push(new FaceTargetAIAction());
		}
	}

	private void SearchForTarget()
	{
		targetHandler.SearchForTarget();
	}
}
