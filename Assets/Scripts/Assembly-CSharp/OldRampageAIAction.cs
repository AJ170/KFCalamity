using AIActionTransitions;
using UnityEngine;

internal class OldRampageAIAction : AIAction
{
	private enum RampageState
	{
		Inactive = 0,
		ChargeUp = 1,
		Charge = 2
	}

	private const float pi = 3.14159f;

	private float chargeUpDuration = 0.3f;

	private float baseChargeDuration = 1.4f;

	private float actualChargeDuration;

	private float baseChargeLength = 12f;

	private float actualChargeLength;

	private float collisionRangeOffset = 1.5f;

	private AIMovementMotor motor;

	private ZedAnimationController animationController;

	private AIAbilityController abilityController;

	private Transform transform;

	private RampageState state;

	private Vector3 startPosition;

	private Vector3 targetPosition;

	private float counter;

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		Transition transition = null;
		if (triggeredEvent.ID == AIEventID.TargetLost || triggeredEvent.ID == AIEventID.TargetDied)
		{
			OnTimerEnd(ref transition);
		}
		return transition;
	}

	public override void OnStart()
	{
		transform = controller.transform;
		SetupTargetPosition();
		startPosition = transform.position;
		motor = controller.GetComponent<AIMovementMotor>();
		if (motor != null)
		{
			motor.Stop(true);
			motor.StartMotion();
			Vector3 rotationDirection = Vector3.Normalize(targetPosition - transform.position);
			rotationDirection.y = 0f;
			motor.SetRotationDirection(rotationDirection);
			abilityController = controller.GetComponent<AIAbilityController>();
			counter = chargeUpDuration;
			state = RampageState.ChargeUp;
			animationController = controller.GetComponent<ZedAnimationController>();
			base.OnStart();
		}
		else
		{
			Debug.LogWarning("ChargeAIAction failed: No motor on GameObject");
			status = AIActionStatus.Failed;
		}
	}

	private void SetupTargetPosition()
	{
		AITargetHandler targetHandler = controller.TargetHandler;
		actualChargeLength = baseChargeLength;
		actualChargeDuration = baseChargeDuration;
		if (targetHandler.HasTarget)
		{
			Transform transform = targetHandler.Target.transform;
			targetPosition = transform.position;
			Vector3 vector = Vector3.Normalize(targetPosition - this.transform.position);
			targetPosition = this.transform.position + vector * actualChargeLength;
			FireRaycast(this.transform.position, vector);
		}
		else
		{
			targetPosition = this.transform.position + this.transform.forward * actualChargeLength;
		}
	}

	protected void FireRaycast(Vector3 position, Vector3 direction)
	{
		LayerMask layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("CollisionBoxes solid")) | (1 << LayerMask.NameToLayer("CollisionBoxes semi-solid")) | (1 << LayerMask.NameToLayer("CollisionBoxes solid (no Line of sight)"));
		RaycastHit hitInfo;
		Physics.Raycast(position, direction, out hitInfo, baseChargeLength, layerMask);
		Transform transform = hitInfo.transform;
		if (transform != null)
		{
			float num = baseChargeLength / baseChargeDuration;
			actualChargeLength = Mathf.Max(hitInfo.distance - collisionRangeOffset, 0f);
			actualChargeDuration = actualChargeLength / num;
			targetPosition = hitInfo.point;
		}
	}

	public override Transition Update()
	{
		counter = Mathf.Max(counter - Time.deltaTime, 0f);
		Transition transition = base.Update();
		if (transition == null)
		{
			if (counter <= 0f)
			{
				OnTimerEnd(ref transition);
			}
			else if (state == RampageState.Charge)
			{
				UpdateChargePosition();
			}
		}
		return transition;
	}

	private void OnTimerEnd(ref Transition transition)
	{
		if (state == RampageState.ChargeUp)
		{
			counter = actualChargeDuration;
			Debug.Log("start charge, duration " + counter + " time " + Time.time);
			state = RampageState.Charge;
			return;
		}
		Debug.Log("charge over " + Time.time);
		motor.EndMotion();
		motor.Resume();
		status = AIActionStatus.Complete;
		if (animationController != null)
		{
			animationController.TriggerAnimation(ZedAnimationTrigger.Rampage, false);
		}
		if (!(actualChargeLength < baseChargeLength))
		{
			return;
		}
		UseAbilityAIAction useAbilityAIAction = null;
		if (abilityController != null)
		{
			Ability ability = abilityController.AbilityByName("FleshpoundEnrageAbility");
			if (ability != null)
			{
				useAbilityAIAction = new UseAbilityAIAction(ability as AIAbility);
			}
		}
		if (useAbilityAIAction != null)
		{
			transition = new Transition(TransitionType.ChangeTo, useAbilityAIAction);
		}
	}

	private void UpdateChargePosition()
	{
		float t = (actualChargeDuration - counter) / actualChargeDuration;
		Vector3 forcedPosition = Vector3.Lerp(startPosition, targetPosition, t);
		motor.SetForcedPosition(forcedPosition);
	}
}
