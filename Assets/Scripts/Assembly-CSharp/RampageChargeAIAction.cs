using System;
using AIActionTransitions;
using UnityEngine;

internal class RampageChargeAIAction : AIAction
{
	private Action<bool, bool> callback;

	private float baseChargeDuration = 1.1f;

	private float baseChargeLength = 12f;

	private float actualChargeDuration;

	private float actualChargeLength;

	private AIMovementMotor motor;

	private Transform transform;

	private Vector3 startPosition;

	private Vector3 targetPosition;

	private float timer;

	public RampageChargeAIAction(Action<bool, bool> callback)
	{
		this.callback = callback;
		actualChargeLength = baseChargeLength;
		actualChargeDuration = baseChargeDuration;
	}

	public override void OnStart()
	{
		motor = controller.GetComponent<AIMovementMotor>();
		if (motor == null)
		{
			Debug.LogError("RampageChargeAIAction:OnStart() Failed: No motor on GameObject");
			status = AIActionStatus.Failed;
			return;
		}
		transform = controller.transform;
		startPosition = transform.position;
		SetupTargetPosition();
		motor.Stop(true);
		motor.StartMotion();
		timer = actualChargeDuration;
		base.OnStart();
	}

	private void SetupTargetPosition()
	{
		AITargetHandler targetHandler = controller.TargetHandler;
		if (targetHandler.HasTarget)
		{
			Transform transform = targetHandler.Target.transform;
			Vector3 position = this.transform.position;
			targetPosition = GlobalHelpers.SampleNavMeshPosition(transform.position, 0.3f);
			Vector3 vector = Vector3.Normalize(targetPosition - position);
			targetPosition = position + vector * baseChargeLength;
			FireRaycast(position, vector);
			AdjustChargeInfoToValidPoint(position, targetPosition);
		}
		else
		{
			status = AIActionStatus.Failed;
		}
	}

	private void FireRaycast(Vector3 position, Vector3 direction)
	{
		LayerMask layerMask = (1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("CollisionBoxes solid")) | (1 << LayerMask.NameToLayer("CollisionBoxes semi-solid")) | (1 << LayerMask.NameToLayer("CollisionBoxes solid (no Line of sight)")) | (1 << LayerMask.NameToLayer("DestructableObject"));
		float num = 0.2f;
		position.y += num;
		RaycastHit hitInfo;
		Physics.Raycast(position, direction, out hitInfo, baseChargeLength, layerMask);
		Transform transform = hitInfo.transform;
		if (transform != null)
		{
			targetPosition = hitInfo.point;
			targetPosition.y -= num;
		}
	}

	private void AdjustChargeInfoToValidPoint(Vector3 start, Vector3 target)
	{
		Vector3 normalized = (target - start).normalized;
		float num = Vector3.Distance(start, target);
		if (!GlobalHelpers.ValidNavMeshPosition(target, 0.2f) && num > 0.25f)
		{
			target = start + normalized * (num * 0.9f);
			AdjustChargeInfoToValidPoint(start, target);
		}
		else
		{
			UpdateChargeInfo(target, num);
		}
	}

	private void UpdateChargeInfo(Vector3 position, float distance)
	{
		float num = baseChargeLength / baseChargeDuration;
		actualChargeLength = Mathf.Max(distance, 0f);
		actualChargeDuration = actualChargeLength / num;
		targetPosition = position;
	}

	public override Transition Update()
	{
		Transition transition = base.Update();
		if (transition == null && timer > 0f)
		{
			UpdateMotion();
			timer = Mathf.Max(timer - Time.deltaTime, 0f);
			if (timer <= 0f)
			{
				OnTimerEnd();
			}
		}
		return transition;
	}

	private void OnTimerEnd()
	{
		motor.EndMotion();
		motor.Resume();
		status = AIActionStatus.Complete;
	}

	private void UpdateMotion()
	{
		float t = (actualChargeDuration - timer) / actualChargeDuration;
		Vector3 forcedPosition = Vector3.Lerp(startPosition, targetPosition, t);
		motor.SetForcedPosition(forcedPosition);
	}

	public override void OnEnd()
	{
		bool flag = status == AIActionStatus.Complete;
		if (callback != null)
		{
			bool arg = actualChargeLength / baseChargeLength >= 0.95f;
			callback(flag, arg);
		}
		if (!flag)
		{
			Vector3 position = transform.position;
			if (!GlobalHelpers.ValidNavMeshPosition(position, 0.2f))
			{
				position = GlobalHelpers.SampleNavMeshPosition(position, 5f);
				transform.position = position;
				motor.Warp(position);
			}
		}
	}
}
