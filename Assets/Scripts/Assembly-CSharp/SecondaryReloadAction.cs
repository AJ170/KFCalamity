using System;
using UnityEngine;

public class SecondaryReloadAction : PlayerAction
{
	private enum ReloadState
	{
		Inactive = 0,
		PreReloadDelay = 1,
		Reloading = 2,
		PostReloadDelay = 3,
		SoftAbort = 4
	}

	private Weapon weapon;

	private SecondaryAttack secondaryAttack;

	private float preReloadDelay = 0.1f;

	private float postReloadDelay = 0.1f;

	private float softAbortDelay = 0.075f;

	private ReloadState reloadState;

	private float reloadTime;

	private float counter;

	private PlayerAnimationController animationController;

	private PlayerWeaponController weaponController;

	private AimController aimController;

	private PlayerGUILink guiLink;

	private Action callback;

	private WeaponSecondaryReloadAudio SecondaryReloadAudio;

	public SecondaryReloadAction(Weapon weapon)
		: base(ActionType.ReloadSecondary)
	{
		this.weapon = weapon;
	}

	public SecondaryReloadAction(Weapon weapon, Action callback)
		: this(weapon)
	{
		this.callback = callback;
	}

	public override void OnEnter(GameObject gameObject, PlayerActionController controller)
	{
		guiLink = gameObject.GetComponent<PlayerGUILink>();
		animationController = gameObject.GetComponent<PlayerAnimationController>();
		weaponController = gameObject.GetComponent<PlayerWeaponController>();
		aimController = gameObject.GetComponent<AimController>();
		SecondaryReloadAudio = weapon.GetComponentInChildren<WeaponSecondaryReloadAudio>();
		if (weapon.HasSecondaryModule())
		{
			secondaryAttack = weapon.SecondaryAttackModule;
			reloadTime = secondaryAttack.ReloadTime;
			reloadState = ReloadState.PreReloadDelay;
			if (preReloadDelay > 0f)
			{
				counter = preReloadDelay;
			}
			else
			{
				OnTimerDone();
			}
			if (guiLink != null)
			{
				float duration = reloadTime + preReloadDelay + postReloadDelay;
				guiLink.StartReloadBar(duration);
			}
			if (aimController != null && aimController.IsAiming())
			{
				aimController.SetActiveState(AimState.AimStateType.Free);
			}
			SetLaser(false);
			OnEnter();
		}
		else
		{
			Abort();
		}
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

	public override void OnAborted()
	{
		ResetAnimation();
		if (SecondaryReloadAudio != null)
		{
			SecondaryReloadAudio.Abort();
		}
		if (guiLink != null)
		{
			guiLink.StopReloadBar();
		}
		SetLaser(true);
	}

	private void OnTimerDone()
	{
		if (reloadState == ReloadState.PreReloadDelay)
		{
			counter = reloadTime;
			reloadState = ReloadState.Reloading;
			TriggerAnimation();
		}
		else if (reloadState == ReloadState.Reloading)
		{
			secondaryAttack.Reload();
			if (callback != null)
			{
				callback();
			}
			if (postReloadDelay > 0f)
			{
				counter = postReloadDelay;
				reloadState = ReloadState.PostReloadDelay;
			}
			else
			{
				state = ActionState.Completed;
			}
		}
		else
		{
			state = ActionState.Completed;
		}
	}

	public override void OnExit()
	{
		ResetAnimation();
		if (SecondaryReloadAudio != null)
		{
			SecondaryReloadAudio.Reset();
		}
		SetLaser(true);
	}

	private void SetLaser(bool active)
	{
		if (weaponController != null)
		{
			weaponController.SetLaserActive(active, this);
		}
	}

	private void TriggerAnimation()
	{
		if (animationController != null)
		{
			animationController.SetBool(PlayerAnimationTrigger.ReloadSecondary, true);
			animationController.ClearAttackTrigger();
			if (SecondaryReloadAudio != null)
			{
				SecondaryReloadAudio.Execute();
			}
		}
	}

	private void ResetAnimation()
	{
		if (animationController != null)
		{
			animationController.SetBool(PlayerAnimationTrigger.ReloadSecondary, false);
		}
	}

	private void Abort()
	{
		state = ActionState.Aborted;
		OnAborted();
	}

	private void SoftAbort()
	{
		if (reloadState != ReloadState.SoftAbort)
		{
			OnAborted();
			counter = softAbortDelay;
			reloadState = ReloadState.SoftAbort;
		}
	}

	public override bool CanAttack()
	{
		SoftAbort();
		return false;
	}

	public override bool CanReload()
	{
		return false;
	}

	public override bool CanAim()
	{
		return false;
	}

	public override void OnAttack()
	{
		Abort();
	}

	public override void OnSwitchWeapon()
	{
		Abort();
	}

	public override void OnUseAbility(Ability ability, AbilitySlot slot)
	{
		if (slot != AbilitySlot.Dodge)
		{
			Abort();
		}
	}

	public override void OnDropWeapon()
	{
		Abort();
	}
}
