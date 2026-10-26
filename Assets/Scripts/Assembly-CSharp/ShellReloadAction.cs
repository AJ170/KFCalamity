using System;
using UnityEngine;

public class ShellReloadAction : PlayerAction
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

	private float postReloadDelay = 0.1f;

	private float softAbortDelay = 0.075f;

	private ReloadState reloadState;

	private float counter;

	private float reloadTime;

	private PlayerAnimationController animationController;

	private PlayerWeaponController weaponController;

	private AimController aimController;

	private Action callback;

	private PlayerGUILink guiLink;

	private WeaponReloadAudio reloadAudio;

	private bool reloadedAtLeastOnce;

	public ShellReloadAction(Weapon weapon)
		: base(ActionType.ReloadShell)
	{
		this.weapon = weapon;
	}

	public ShellReloadAction(Weapon weapon, Action callback)
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
		reloadAudio = weapon.GetComponentInChildren<WeaponReloadAudio>();
		reloadTime = weapon.ReloadTime;
		if (reloadTime > 0f)
		{
			float activeRoundCooldown = weapon.ActiveRoundCooldown;
			reloadState = ReloadState.PreReloadDelay;
			if (activeRoundCooldown > 0f)
			{
				counter = activeRoundCooldown;
			}
			else
			{
				OnTimerDone();
			}
			StartReloadBar(reloadTime);
			if (aimController != null && aimController.IsAiming())
			{
				aimController.SetActiveState(AimState.AimStateType.Free);
			}
			SetLaser(false);
			if (weapon.MagazineSize > 1 && UnityEngine.Random.value < 0.2f)
			{
				SpeechManager.PlaySpeech(SpeechManager.SpeechLine.Reload, gameObject);
			}
			OnEnter();
		}
		else
		{
			Debug.LogWarning(string.Format("[ReloadAction:OnEnter] Invalid reload time ({0}) for weapon {1}.", reloadTime, weapon.Name));
			weapon.ReloadMagazine();
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

	public override void OnExit()
	{
		if (reloadAudio != null)
		{
			reloadAudio.Reset();
		}
		ResetAnimation();
		SetLaser(true);
	}

	public override void OnAborted()
	{
		ResetAnimation();
		if (reloadAudio != null)
		{
			reloadAudio.Abort();
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
			weapon.ReloadAmmunition(1);
			reloadedAtLeastOnce = true;
			if (callback != null)
			{
				callback();
			}
			if (weapon.CanReload())
			{
				counter = reloadTime;
				if (reloadAudio != null)
				{
					reloadAudio.Execute();
				}
				if (weapon.LoadedAmmunitionDeficit() == 1)
				{
					StartReloadBar(reloadTime + postReloadDelay);
				}
				else
				{
					StartReloadBar(reloadTime);
				}
			}
			else
			{
				ResetAnimation();
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
		}
		else
		{
			state = ActionState.Completed;
		}
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
			animationController.SetBool(PlayerAnimationTrigger.Reload, true);
			animationController.ClearAttackTrigger();
			if (reloadAudio != null)
			{
				reloadAudio.Execute();
			}
		}
	}

	private void ResetAnimation()
	{
		if (animationController != null)
		{
			animationController.SetBool(PlayerAnimationTrigger.Reload, false);
		}
	}

	private void StartReloadBar(float duration)
	{
		if (guiLink != null)
		{
			guiLink.StartReloadBar(duration);
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

	public override bool CanReload()
	{
		return false;
	}

	public override bool CanAttack()
	{
		SoftAbort();
		return false;
	}

	public override bool CanAim()
	{
		return reloadedAtLeastOnce;
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

	public override void OnAim()
	{
		Abort();
	}
}
