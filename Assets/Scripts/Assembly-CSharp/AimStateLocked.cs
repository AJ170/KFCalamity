using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class AimStateLocked : AimState
{
	private float searchAngle = 35f;

	private float searchRange = 20f;

	private float additionalMaintainAngle = 5f;

	private float additionalMaintainRange = 5f;

	private float targetSearchFrequency = 5f;

	private float targetSearchCounter = 1f;

	private float targetUpdateFrequency = 3f;

	private float targetUpdateCounter = 1f;

	private float heightUpdateFrequency = 4f;

	private float heightUpdateCounter = 1f;

	private float soldierSpeedModifier = 0.5f;

	private float soldierRotationModifier = 0.5f;

	private float slowUpdateCounter = 0.1f;

	private bool checkLineOfSight = true;

	private bool limitToTargetsOnScreen = true;

	private LineOfSightComponent lineOfSightComponent;

	private float inputThreshold = 0.4f;

	private bool inputAboveThreshold;

	private int targetLayerMask;

	private Transform weaponBone;

	private WeaponAimController weaponAimController;

	private AimBones targetAimBones;

	private Transform currentTargetAimBone;

	private float headAimCounter;

	private float slowCounter = 1f;

	private bool hasSlowed;

	private bool isAimingForHead;

	private bool isSlowed;

	private AimController controller;

	private AimFeedbackController feedbackController;

	private PlayerWeaponController weaponController;

	private GameObject currentTarget;

	private List<GameObject> oldTargets = new List<GameObject>();

	private AttributeModifier soldierSpeedAttribute;

	private AttributeModifier soldierRotationAttribute;

	private Camera mainCamera;

	public bool IsAimingForHead
	{
		get
		{
			return isAimingForHead;
		}
	}

	public GameObject Target
	{
		get
		{
			return currentTarget;
		}
	}

	public AimStateLocked(GameObject soldier, Transform spine, AimController controller, AimFeedbackController feedbackController)
		: base(AimStateType.Locked, soldier, spine)
	{
		this.controller = controller;
		this.feedbackController = feedbackController;
		targetLayerMask = 1 << LayerMask.NameToLayer("Enemy");
		weaponController = soldier.GetComponent<PlayerWeaponController>();
		WeaponMountController component = soldier.GetComponent<WeaponMountController>();
		if (component != null)
		{
			weaponBone = component.RightWeaponBone;
		}
		if (checkLineOfSight)
		{
			lineOfSightComponent = soldier.GetComponent<LineOfSightComponent>();
			if (lineOfSightComponent == null)
			{
				checkLineOfSight = false;
			}
		}
		mainCamera = Camera.main;
	}

	public override void OnActivated()
	{
		targetSearchCounter = 0f;
		feedbackController.Attach(AimFeedbackController.AimEffectType.Self);
		slowCounter = 1f;
		base.OnActivated();
	}

	public override void OnDeactivated()
	{
		ClearTarget();
		ClearOldTargets();
		SetModifiers(false);
		hasSlowed = false;
		feedbackController.Detach(AimFeedbackController.AimEffectType.Self);
		if (weaponController != null)
		{
			weaponController.OnHeadshotAim(false);
		}
	}

	public override void Update()
	{
		UpdateHeadAimCounter();
		UpdateSlowCounter();
		UpdateRotation();
		UpdateTargets();
		UpdateForcedSearch();
		UpdatePassiveSearch();
	}

	public override void LateUpdate()
	{
		UpdateVerticalRotation();
	}

	private void UpdateForcedSearch()
	{
		if (inputLocked)
		{
			return;
		}
		InputControl control = inputController.Control;
		if (control == null)
		{
			return;
		}
		if (control.Type == ControllerType.Keyboard)
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				SearchForNextTarget(control.GetVector(InputAction.Rotation).normalized);
				ResetTargetSearchCounter();
			}
			return;
		}
		Vector3 vector = control.GetVector(InputAction.Rotation);
		float magnitude = vector.magnitude;
		if (magnitude > inputThreshold)
		{
			if (!inputAboveThreshold)
			{
				SearchForNextTarget(vector.normalized);
				ResetTargetSearchCounter();
				inputAboveThreshold = true;
			}
		}
		else
		{
			inputAboveThreshold = false;
		}
	}

	private void UpdatePassiveSearch()
	{
		targetSearchCounter -= Time.deltaTime;
		if (targetSearchCounter <= 0f)
		{
			SearchForTarget(transform.forward);
			ResetTargetSearchCounter();
		}
	}

	private void ResetTargetSearchCounter()
	{
		targetSearchCounter = 1f / targetSearchFrequency;
	}

	private void UpdateRotation()
	{
		if (currentTarget != null)
		{
			UpdateTargetedHorizontalRotation();
		}
		else
		{
			UpdateFreeHorizontalRotation();
		}
		if (currentTarget != null)
		{
			UpdateTargetedVerticalRotationTarget();
		}
		else
		{
			UpdateFreeVerticalRotationTarget();
		}
	}

	private void UpdateTargetedHorizontalRotation()
	{
		Vector3 position = transform.position;
		Vector3 vector = ((!(currentTargetAimBone != null)) ? currentTarget.transform.position : currentTargetAimBone.position);
		Vector3 facingDirection = Vector3.Normalize(vector - position);
		facingDirection.y = 0f;
		motor.SetFacingDirection(facingDirection);
	}

	private void UpdateFreeHorizontalRotation()
	{
		if (!inputLocked && inputController.Control != null)
		{
			motor.SetFacingDirection(inputController.Control.GetVector(InputAction.Rotation));
		}
	}

	private void UpdateTargetedVerticalRotationTarget()
	{
		if (currentTargetAimBone != null)
		{
			spineRotationTarget = BoneRotationFromAimedTarget();
		}
		else
		{
			spineRotationTarget = BoneRotationFromTarget(currentTarget.transform.position);
		}
	}

	private void UpdateFreeVerticalRotationTarget()
	{
		heightUpdateCounter -= Time.deltaTime;
		if (heightUpdateCounter <= 0f)
		{
			spineRotationTarget = GetGroundAimRotation();
			heightUpdateCounter = 1f / heightUpdateFrequency;
		}
	}

	private void UpdateVerticalRotation()
	{
		if (spine != null)
		{
			spineRotation = Mathf.Lerp(spineRotation, spineRotationTarget + ((!(currentTarget == null)) ? 0f : 3.85f), 5f * Time.deltaTime);
			Vector3 localEulerAngles = spine.localEulerAngles;
			spine.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y, spineRotation);
		}
	}

	private void UpdateTargets()
	{
		targetUpdateCounter -= Time.deltaTime;
		if (targetUpdateCounter <= 0f)
		{
			UpdateActiveTarget();
			UpdateOldTargets();
			targetUpdateCounter = 1f / targetUpdateFrequency;
		}
	}

	private void UpdateActiveTarget()
	{
		if (currentTarget != null)
		{
			Vector3 position = transform.position;
			Vector3 position2 = currentTarget.transform.position;
			float range = searchRange + additionalMaintainRange;
			if (!PositionIsInRange(position, position2, range) || !IsPositionOnScreen(position2) || IsTargetStealthed(currentTarget) || (checkLineOfSight && !IsActiveTargetInLineOfSight()))
			{
				ClearTarget();
			}
		}
	}

	private void UpdateOldTargets()
	{
		if (oldTargets.Count <= 0)
		{
			return;
		}
		Vector3 position = transform.position;
		Vector3 forward = transform.forward;
		float num = searchAngle + additionalMaintainAngle;
		float range = searchRange + additionalMaintainRange;
		float cone = Mathf.Cos(num * ((float)Math.PI / 180f));
		for (int num2 = oldTargets.Count - 1; num2 >= 0; num2--)
		{
			GameObject gameObject = oldTargets[num2];
			if (gameObject != null)
			{
				Vector3 position2 = gameObject.transform.position;
				if (!PositionIsInRange(position, position2, range) || !PositionIsInArc(position, position2, forward, cone))
				{
					oldTargets.RemoveAt(num2);
				}
			}
			else
			{
				oldTargets.RemoveAt(num2);
			}
		}
	}

	private void SetTarget(GameObject target)
	{
		if (currentTarget != null && target != currentTarget)
		{
			CleanupTarget(currentTarget);
			AddToOldTargets(currentTarget);
		}
		currentTarget = target;
		SetupTarget(currentTarget);
	}

	private void SetupTarget(GameObject target)
	{
		targetAimBones = target.GetComponent<AimBones>();
		feedbackController.SetTarget(target, targetAimBones);
		AimAtBody();
	}

	private void CleanupTarget(GameObject target)
	{
		feedbackController.ClearTarget();
	}

	private void AddToOldTargets(GameObject target)
	{
		oldTargets.Add(target);
	}

	private void ClearTarget()
	{
		if (currentTarget != null)
		{
			CleanupTarget(currentTarget);
			AddToOldTargets(currentTarget);
			currentTarget = null;
		}
	}

	private void ClearOldTargets()
	{
		oldTargets.Clear();
	}

	private bool SearchForTarget(Vector3 direction)
	{
		GameObject[] targets = FindTargetsInRange(transform.position, searchRange, targetLayerMask);
		bool result = false;
		if (targets.Length > 0)
		{
			if (limitToTargetsOnScreen)
			{
				FilterTargetsOnScreen(ref targets);
			}
			if (checkLineOfSight)
			{
				FilterLineOfSightTargets(ref targets);
			}
			FilterStealthedTargets(ref targets);
			if (!ActiveTargetIsTargeted(ref targets))
			{
				result = TargetBestTarget(ref targets, direction);
			}
		}
		return result;
	}

	private void SearchForNextTarget(Vector3 direction)
	{
		GameObject[] targets = FindTargetsInRange(transform.position, searchRange, targetLayerMask);
		if (targets.Length > 0)
		{
			FilterOldTargets(ref targets);
			FilterActiveTarget(ref targets);
			if (limitToTargetsOnScreen)
			{
				FilterTargetsOnScreen(ref targets);
			}
			if (checkLineOfSight)
			{
				FilterLineOfSightTargets(ref targets);
			}
			FilterStealthedTargets(ref targets);
			if (!TargetBestTarget(ref targets, direction))
			{
				TargetOldestOldTarget();
				ClearOldTargets();
			}
		}
		else
		{
			ClearOldTargets();
		}
	}

	private void FilterOldTargets(ref GameObject[] targets)
	{
		for (int i = 0; i < targets.Length; i++)
		{
			if (targets[i] != null && IsOldTarget(targets[i]))
			{
				targets[i] = null;
			}
		}
	}

	private void FilterTargetsOnScreen(ref GameObject[] targets)
	{
		if (!(mainCamera != null))
		{
			return;
		}
		for (int i = 0; i < targets.Length; i++)
		{
			if (targets[i] != null && !IsTargetOnScreen(targets[i]))
			{
				targets[i] = null;
			}
		}
	}

	private void FilterLineOfSightTargets(ref GameObject[] targets)
	{
		for (int i = 0; i < targets.Length; i++)
		{
			if (targets[i] != null && !IsTargetInLineOfSight(targets[i]))
			{
				targets[i] = null;
			}
		}
	}

	private void FilterStealthedTargets(ref GameObject[] targets)
	{
		for (int i = 0; i < targets.Length; i++)
		{
			if (targets[i] != null && IsTargetStealthed(targets[i]))
			{
				targets[i] = null;
			}
		}
	}

	private void FilterActiveTarget(ref GameObject[] targets)
	{
		if (!(currentTarget != null))
		{
			return;
		}
		for (int i = 0; i < targets.Length; i++)
		{
			if (targets[i] == currentTarget)
			{
				targets[i] = null;
				break;
			}
		}
	}

	private bool TargetBestTarget(ref GameObject[] targets, Vector3 direction)
	{
		GameObject gameObject = null;
		gameObject = BestTargetInIncrementingAngles(ref targets, transform.position, direction, searchAngle);
		if (gameObject != null)
		{
			SetTarget(gameObject);
			return true;
		}
		return false;
	}

	private void TargetOldestOldTarget()
	{
		for (int i = 0; i < oldTargets.Count; i++)
		{
			GameObject gameObject = oldTargets[i];
			if (gameObject != null)
			{
				oldTargets.RemoveAt(i);
				SetTarget(gameObject);
				break;
			}
		}
	}

	private bool IsOldTarget(GameObject target)
	{
		bool result = false;
		foreach (GameObject oldTarget in oldTargets)
		{
			if (oldTarget != null && target == oldTarget)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private bool IsTargetOnScreen(GameObject target)
	{
		return IsPositionOnScreen(target.transform.position);
	}

	private bool IsPositionOnScreen(Vector3 position)
	{
		if (mainCamera != null)
		{
			Vector3 vector = mainCamera.WorldToScreenPoint(position);
			return !(vector.x < 0f) && !(vector.y < 0f) && !(vector.x > (float)Screen.width) && !(vector.y > (float)Screen.height);
		}
		return true;
	}

	private bool IsActiveTargetInLineOfSight()
	{
		return IsTargetInLineOfSight(currentTarget);
	}

	private bool IsTargetInLineOfSight(GameObject target)
	{
		if (lineOfSightComponent != null && target.GetComponent<LineOfSightTarget>() != null)
		{
			return lineOfSightComponent.CanSeeTarget(target);
		}
		return true;
	}

	private bool IsTargetStealthed(GameObject target)
	{
		StealthController component = target.GetComponent<StealthController>();
		return component != null && component.IsStealthed && !component.IsDetected;
	}

	private bool ActiveTargetIsTargeted(ref GameObject[] targets)
	{
		bool result = false;
		if (currentTarget != null)
		{
			for (int i = 0; i < targets.Length; i++)
			{
				if (targets[i] == currentTarget)
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	public void OnWeaponSwitch(GameObject weapon, bool active)
	{
		StopHeadAimCounter();
		weaponAimController = ((!(weapon != null)) ? null : weapon.GetComponent<WeaponAimController>());
		if (currentTarget != null)
		{
			AimAtBody();
		}
		if (active)
		{
			UpdateModifiers();
		}
	}

	public void OnAttack()
	{
		if (isAimingForHead)
		{
			isAimingForHead = false;
			if (CanAimForHead())
			{
				StartHeadAimCounter();
			}
			else
			{
				AimAtBody();
			}
		}
		else if (CanAimForHead())
		{
			StartHeadAimCounter();
		}
	}

	private void AimAtBody()
	{
		if (targetAimBones != null)
		{
			currentTargetAimBone = targetAimBones.Body;
		}
		isAimingForHead = false;
		if (CanAimForHead())
		{
			StartHeadAimCounter();
			feedbackController.Attach(AimFeedbackController.AimEffectType.BodyAim);
		}
		else
		{
			feedbackController.Attach(AimFeedbackController.AimEffectType.BodyStatic);
		}
		if (weaponController != null)
		{
			weaponController.OnHeadshotAim(false);
		}
	}

	private void AimAtHead()
	{
		if (targetAimBones != null)
		{
			if (targetAimBones.Head != null)
			{
				currentTargetAimBone = targetAimBones.Head;
				isAimingForHead = true;
				feedbackController.Attach(AimFeedbackController.AimEffectType.Head);
				if (weaponController != null)
				{
					weaponController.OnHeadshotAim(true);
				}
			}
			else
			{
				currentTargetAimBone = targetAimBones.Body;
				isAimingForHead = false;
				feedbackController.Attach(AimFeedbackController.AimEffectType.BodyStatic);
			}
		}
		else
		{
			isAimingForHead = false;
		}
	}

	private void StartHeadAimCounter()
	{
		if (weaponAimController != null)
		{
			headAimCounter = weaponAimController.HeadAimDelay;
		}
		else
		{
			headAimCounter = 1f;
		}
	}

	private void StopHeadAimCounter()
	{
		headAimCounter = 0f;
	}

	private void UpdateSlowCounter()
	{
		if (slowCounter > 0f && !hasSlowed)
		{
			slowCounter -= Time.deltaTime;
			if (slowCounter <= 0f)
			{
				hasSlowed = true;
				UpdateModifiers();
			}
		}
	}

	private void UpdateHeadAimCounter()
	{
		bool flag = false;
		if (headAimCounter > 0f)
		{
			headAimCounter -= Time.deltaTime;
			if (headAimCounter <= 0f)
			{
				AimAtHead();
				flag = true;
			}
		}
		if (!flag && currentTarget != null && isAimingForHead && weaponController.CanAttack())
		{
			weaponController.Attack();
		}
	}

	private float BoneRotationFromAimedTarget()
	{
		Vector3 forward = Vector3.Normalize(weaponBone.position - currentTargetAimBone.position);
		float num = Quaternion.LookRotation(forward).eulerAngles.x;
		if (num > 180f)
		{
			num -= 360f;
		}
		return num;
	}

	private bool CanAimForHead()
	{
		return weaponAimController != null && weaponAimController.CanAimForHead && targetAimBones != null && targetAimBones.Head != null;
	}

	private void SetModifiers(bool slowed)
	{
		if (slowed == isSlowed)
		{
			return;
		}
		if (slowed)
		{
			if (soldierSpeedAttribute == null || soldierRotationAttribute == null)
			{
				AttributeController component = soldier.GetComponent<AttributeController>();
				soldierSpeedAttribute = component.AttributeByName(AttributeName.Speed) as AttributeModifier;
				soldierRotationAttribute = component.AttributeByName(AttributeName.RotationSpeed) as AttributeModifier;
			}
			if (soldierSpeedAttribute != null)
			{
				soldierSpeedAttribute.Alter(this, soldierSpeedModifier);
			}
			if (soldierRotationAttribute != null)
			{
				soldierRotationAttribute.Alter(this, soldierRotationModifier);
			}
		}
		else
		{
			if (soldierSpeedAttribute != null)
			{
				soldierSpeedAttribute.Restore(this);
			}
			if (soldierRotationAttribute != null)
			{
				soldierRotationAttribute.Restore(this);
			}
		}
		controller.SetIronsight(slowed);
		isSlowed = slowed;
	}

	private void UpdateModifiers()
	{
		bool modifiers = weaponController != null && weaponController.ActiveWeapon != null && !weaponController.ActiveWeapon.PerkMatch(Perk.Berserker);
		SetModifiers(modifiers);
	}
}
