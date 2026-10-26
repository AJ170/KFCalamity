using UnityEngine;

public class FriendlyAimAction : PlayerAction
{
	private float searchAngle = 30f;

	private float searchRange = 15f;

	private float fireAngle = 3f;

	private FriendlyAimType aimType;

	private bool requireTarget;

	private FriendlyAimTargetEvaluator evaluator;

	private AimCallback callback;

	private AimController aimController;

	public FriendlyAimAction(float searchAngle, float searchRange, float fireAngle, bool requireTarget, FriendlyAimType aimType, FriendlyAimTargetEvaluator evaluator, AimCallback callback)
		: base(ActionType.FriendlyAim)
	{
		this.searchAngle = searchAngle;
		this.searchRange = searchRange;
		this.fireAngle = fireAngle;
		this.aimType = aimType;
		this.requireTarget = requireTarget;
		this.evaluator = evaluator;
		this.callback = callback;
	}

	public override void OnEnter(GameObject gameObject, PlayerActionController controller)
	{
		aimController = gameObject.GetComponentInChildren<AimController>();
		bool flag = false;
		if (aimController != null)
		{
			flag = aimController.ActivateFriendlyAim(searchAngle, searchRange, fireAngle, aimType, evaluator, OnAimTarget);
		}
		if (flag)
		{
			OnEnter();
		}
		else
		{
			state = ActionState.Completed;
		}
	}

	private void OnAimTarget(GameObject target)
	{
		if (target != null || !requireTarget)
		{
			callback(target);
		}
		state = ActionState.Completed;
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
		state = ActionState.Aborted;
	}
}
