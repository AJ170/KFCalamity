using System;
using UnityEngine;

public class AimStateFriendly : AimState
{
	private float searchAngle = 30f;

	private float searchRange = 15f;

	private float fireAngle = 3f;

	private float verticalLerpSpeed = 5f;

	private float verticalLerpValue;

	private float verticalRotationOffset = 3.85f;

	private float verticalRotationTarget;

	private AimCallback callback;

	private int targetLayerMask;

	private GameObject activeTarget;

	private AimController aimController;

	public AimStateFriendly(GameObject soldier, Transform spine, AimController aimController)
		: base(AimStateType.Friendly, soldier, spine)
	{
		this.aimController = aimController;
		targetLayerMask = 1 << LayerMask.NameToLayer("Player");
	}

	public bool Activate(float searchAngle, float searchRange, FriendlyAimType aimType, AimCallback callback)
	{
		return Activate(searchAngle, searchRange, 3f, aimType, FriendlyAimTargetEvaluator.None, callback);
	}

	public bool Activate(float searchAngle, float searchRange, float fireAngle, FriendlyAimType aimType, FriendlyAimTargetEvaluator evaluator, AimCallback callback)
	{
		this.searchAngle = searchAngle;
		this.searchRange = searchRange;
		this.fireAngle = fireAngle;
		this.callback = callback;
		SearchForTarget(evaluator);
		if (aimType == FriendlyAimType.Instant || activeTarget == null || TargetInFireArc())
		{
			callback(activeTarget);
			return false;
		}
		return true;
	}

	public override void Update()
	{
		if (activeTarget != null)
		{
			UpdateRotation();
			CheckTarget();
		}
		else
		{
			callback(null);
			aimController.SetActiveState(AimStateType.Free);
		}
	}

	public override void LateUpdate()
	{
		UpdateVerticalRotation();
	}

	private void UpdateRotation()
	{
		UpdateHorizontalRotation();
		UpdateVerticalRotationTarget();
	}

	private void UpdateHorizontalRotation()
	{
		Vector3 position = transform.position;
		Vector3 position2 = activeTarget.transform.position;
		Vector3 facingDirection = Vector3.Normalize(position2 - position);
		motor.SetFacingDirection(facingDirection);
	}

	private void UpdateVerticalRotationTarget()
	{
		verticalRotationTarget = BoneRotationFromTarget(activeTarget.transform.position);
	}

	private void CheckTarget()
	{
		if (TargetInFireArc())
		{
			callback(activeTarget);
			aimController.SetActiveState(AimStateType.Free);
		}
	}

	public void SearchForTarget(FriendlyAimTargetEvaluator evaluator)
	{
		GameObject[] targets = FindTargetsInDirection(transform.position, transform.forward, searchAngle, searchRange, targetLayerMask);
		EvaluateTargets(evaluator, ref targets);
		TargetBestTarget(ref targets);
	}

	private void Reset()
	{
		activeTarget = null;
		callback = null;
	}

	private void EvaluateTargets(FriendlyAimTargetEvaluator evaluator, ref GameObject[] targets)
	{
		if (evaluator == FriendlyAimTargetEvaluator.None)
		{
			return;
		}
		for (int i = 0; i < targets.Length; i++)
		{
			if (targets[i] != null && evaluator == FriendlyAimTargetEvaluator.NotFullHealth)
			{
				bool flag = false;
				Health component = targets[i].GetComponent<Health>();
				if (component != null)
				{
					flag = component.CurrentHealth != component.MaxHealth;
				}
				if (!flag)
				{
					targets[i] = null;
				}
			}
		}
	}

	private void TargetBestTarget(ref GameObject[] targets)
	{
		GameObject gameObject = null;
		if (targets.Length > 0)
		{
			float num = 0f;
			Vector3 forward = transform.forward;
			Vector3 position = transform.position;
			float num2 = 0f;
			GameObject gameObject2 = null;
			for (int i = 0; i < targets.Length; i++)
			{
				gameObject2 = targets[i];
				if (gameObject2 != null && gameObject2 != soldier)
				{
					Vector3 rhs = Vector3.Normalize(gameObject2.transform.position - position);
					num2 = Vector3.Dot(forward, rhs);
					if (num2 > num)
					{
						gameObject = gameObject2;
						num = num2;
					}
				}
			}
		}
		activeTarget = gameObject;
	}

	private bool TargetInFireArc()
	{
		bool result = false;
		if (activeTarget != null)
		{
			float cone = Mathf.Cos(fireAngle * ((float)Math.PI / 180f));
			result = PositionIsInArc(transform.position, activeTarget.transform.position, transform.forward, cone);
		}
		return result;
	}

	private void UpdateVerticalRotation()
	{
		if (spine != null)
		{
			verticalLerpValue = Mathf.Lerp(verticalLerpValue, verticalRotationTarget + verticalRotationOffset, verticalLerpSpeed * Time.deltaTime);
			Vector3 localEulerAngles = spine.localEulerAngles;
			spine.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y, verticalLerpValue);
		}
	}
}
