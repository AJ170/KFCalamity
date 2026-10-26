using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour
{
	public Transform SpineTransform;

	public AimState.AimStateType StartingState;

	private AimState activeState;

	private Dictionary<AimState.AimStateType, AimState> states = new Dictionary<AimState.AimStateType, AimState>();

	private AimStateLocked lockedAimState;

	public GameObject AimEffect;

	public GameObject TargetBodyStaticEffect;

	public GameObject TargetBodyAimEffect;

	public GameObject TargetHeadAimEffect;

	public AudioClip AimAudioClip;

	public AudioClip HeadAimAudioClip;

	public float Volume = 1f;

	private AimFeedbackController feedbackController;

	private PlayerActionController actionController;

	private PlayerAnimationController animationController;

	private bool ironsightActive;

	private bool forceIronsight;

	private void Start()
	{
		SetupFeedback();
		SetupStates();
		SetActiveState(StartingState);
		actionController = GetComponent<PlayerActionController>();
		animationController = GetComponent<PlayerAnimationController>();
	}

	private void SetupFeedback()
	{
		feedbackController = new AimFeedbackController(base.gameObject, AimEffect, TargetBodyStaticEffect, TargetBodyAimEffect, TargetHeadAimEffect, AimAudioClip, HeadAimAudioClip, Volume);
	}

	private void SetupStates()
	{
		states.Add(AimState.AimStateType.Free, new AimStateFree(base.gameObject, SpineTransform));
		lockedAimState = new AimStateLocked(base.gameObject, SpineTransform, this, feedbackController);
		states.Add(AimState.AimStateType.Locked, lockedAimState);
		states.Add(AimState.AimStateType.Friendly, new AimStateFriendly(base.gameObject, SpineTransform, this));
		PlayerWeaponController component = base.gameObject.GetComponent<PlayerWeaponController>();
		if (component.ActiveWeaponGameObject != null)
		{
			OnWeaponSwitch(component.ActiveWeaponGameObject);
		}
	}

	private void Update()
	{
		if (activeState != null)
		{
			activeState.Update();
		}
	}

	private void LateUpdate()
	{
		if (activeState != null)
		{
			activeState.LateUpdate();
		}
	}

	private void OnDestroy()
	{
		foreach (AimState value in states.Values)
		{
			value.Shutdown();
		}
		feedbackController.Shutdown();
	}

	public void ToggleAimState()
	{
		AimState.AimStateType aimStateType = AimState.AimStateType.Free;
		if (activeState != null)
		{
			aimStateType = activeState.Type;
		}
		switch (aimStateType)
		{
		case AimState.AimStateType.Free:
			SetActiveState(AimState.AimStateType.Locked);
			break;
		case AimState.AimStateType.Locked:
			SetActiveState(AimState.AimStateType.Free);
			break;
		}
	}

	public void SetActiveState(AimState.AimStateType type)
	{
		if (states.ContainsKey(type))
		{
			AimState aimState = states[type];
			SetActiveState(aimState);
		}
		else
		{
			Debug.Log(string.Format("[AimController:SetActiveState] No AimState of type {0} initialized.", type.ToString()));
		}
	}

	private void SetActiveState(AimState state)
	{
		if (state != activeState)
		{
			if (activeState != null)
			{
				activeState.OnDeactivated();
			}
			if (actionController != null && state.Type == AimState.AimStateType.Locked)
			{
				actionController.OnAim();
			}
			activeState = state;
			activeState.OnActivated();
		}
	}

	public bool ActivateFriendlyAim(float searchAngle, float searchRange, float fireAngle, FriendlyAimType aimType, FriendlyAimTargetEvaluator evaluator, AimCallback callback)
	{
		if (states.ContainsKey(AimState.AimStateType.Friendly))
		{
			AimStateFriendly aimStateFriendly = (AimStateFriendly)states[AimState.AimStateType.Friendly];
			bool flag = aimStateFriendly.Activate(searchAngle, searchRange, fireAngle, aimType, evaluator, callback);
			if (flag)
			{
				SetActiveState(aimStateFriendly);
			}
			return flag;
		}
		callback(null);
		return false;
	}

	public bool IsAiming()
	{
		bool result = false;
		if (activeState != null && activeState.Type == AimState.AimStateType.Locked)
		{
			result = true;
		}
		return result;
	}

	public bool IsAimingAtHead()
	{
		bool result = false;
		if (activeState != null && activeState.Type == AimState.AimStateType.Locked)
		{
			result = lockedAimState.IsAimingForHead;
		}
		return result;
	}

	public GameObject AimTarget()
	{
		if (activeState != null && activeState.Type == AimState.AimStateType.Locked)
		{
			return lockedAimState.Target;
		}
		return null;
	}

	public bool CanAim()
	{
		bool result = true;
		if ((activeState != null && activeState.Type == AimState.AimStateType.Friendly) || (actionController != null && !actionController.CanAim()))
		{
			result = false;
		}
		return result;
	}

	public void LockInput(bool inputLocked)
	{
		foreach (AimState value in states.Values)
		{
			value.LockInput(inputLocked);
		}
	}

	public void OnWeaponSwitch(GameObject weapon)
	{
		if (lockedAimState != null)
		{
			lockedAimState.OnWeaponSwitch(weapon, activeState == lockedAimState);
		}
	}

	public void OnAttack()
	{
		if (lockedAimState != null)
		{
			lockedAimState.OnAttack();
		}
	}

	public void SetIronsight(bool active)
	{
		if (active != ironsightActive)
		{
			ironsightActive = active;
			UpdateIronsight();
		}
		else
		{
			ironsightActive = active;
		}
	}

	public void SetForceIronsight(bool active)
	{
		if (active != forceIronsight)
		{
			forceIronsight = active;
			UpdateIronsight();
		}
		else
		{
			forceIronsight = active;
		}
	}

	private void UpdateIronsight()
	{
		bool value = forceIronsight || ironsightActive;
		if (animationController != null)
		{
			animationController.SetBool("Ironsight", value);
		}
	}
}
