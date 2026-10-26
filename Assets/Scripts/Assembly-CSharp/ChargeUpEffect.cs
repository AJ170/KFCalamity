using ActorEvents;
using EffectEvents;
using EventHelpers;
using UnityEngine;

public class ChargeUpEffect : EffectBaseListener
{
	public int MaxCharges = 10;

	public float ChargesPerSecond = 3.333f;

	public float ChargeUpTick = 0.25f;

	public bool ConsumeAmmunitionPerCharge;

	public bool TriggerCooldownOnRelease = true;

	public float Cooldown = 1f;

	public AudioSource AudioSource;

	private bool isActive;

	private float tickCounter;

	private float chargeCounter;

	private int charges;

	private WeaponController weaponController;

	private Weapon weapon;

	private void OnMounted(GameObject parent)
	{
		weaponController = parent.GetComponent<WeaponController>();
		RegisterActorEventCallbacks(parent);
		if (ConsumeAmmunitionPerCharge || TriggerCooldownOnRelease)
		{
			FindWeapon(parent);
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			if (!isActive && (!ConsumeAmmunitionPerCharge || (weapon != null && weapon.LoadedAmmunition > 0)))
			{
				isActive = true;
				chargeCounter = 1f / ChargesPerSecond;
				effectSystem.TriggerInternal(new ChargeUpStart());
			}
			if (isActive)
			{
				tickCounter = ChargeUpTick;
			}
		}
	}

	private void Update()
	{
		if (!isActive)
		{
			return;
		}
		if (charges < MaxCharges && (!ConsumeAmmunitionPerCharge || (weapon != null && weapon.LoadedAmmunition > 0)))
		{
			chargeCounter -= Time.deltaTime;
			if (chargeCounter <= 0f)
			{
				charges++;
				chargeCounter += 1f / ChargesPerSecond;
				if (ConsumeAmmunitionPerCharge)
				{
					weapon.ConsumeAmmunition(1);
				}
				effectSystem.TriggerInternal(new ChargeUpTick(charges, charges == MaxCharges));
				if (charges == MaxCharges)
				{
					effectSystem.TriggerInternal(new ChargeUpMax());
				}
			}
		}
		tickCounter -= Time.deltaTime;
		if (tickCounter <= 0f)
		{
			TriggerEvent();
			if (ConsumeAmmunitionPerCharge && charges == 0 && weapon != null)
			{
				weapon.ConsumeAmmunition(1);
			}
			if (TriggerCooldownOnRelease && weapon != null)
			{
				weapon.TriggerRoundCooldown(Cooldown);
			}
			Reset();
		}
	}

	private void TriggerEvent()
	{
		effectSystem.TriggerInternal(new ChargeUpRelease((float)charges / (float)MaxCharges));
	}

	private void FindWeapon(GameObject parent)
	{
		if (weaponController != null)
		{
			weapon = weaponController.ActiveWeapon;
		}
	}

	private void RegisterActorEventCallbacks(GameObject parent)
	{
		ActorEventController component = parent.GetComponent<ActorEventController>();
		if (component != null)
		{
			component.Register(ActorEventID.UseAbility, OnCancelEvent);
			component.Register(ActorEventID.SwitchWeapon, OnCancelEvent);
		}
	}

	private void OnCancelEvent(ActorEventBase e)
	{
		if (!isActive)
		{
			return;
		}
		bool flag = true;
		if (e.ID == ActorEventID.UseAbility)
		{
			UseAbility useAbility = e as UseAbility;
			Ability ability = useAbility.Ability;
			if (ability.Cooldown == 0f && ability.WindDown == 0f)
			{
				flag = false;
			}
		}
		if (!flag)
		{
			return;
		}
		if (ConsumeAmmunitionPerCharge && charges > 0 && weaponController != null && weapon != null)
		{
			weapon.RestoreAmmunition(charges);
			PlayerWeaponController playerWeaponController = weaponController as PlayerWeaponController;
			if (playerWeaponController != null)
			{
				playerWeaponController.UpdateGUIAmmunition();
			}
		}
		if (AudioSource != null)
		{
			AudioSource.Stop();
		}
		effectSystem.TriggerInternal(new ChargeUpCancel());
		Reset();
	}

	private void Reset()
	{
		charges = 0;
		tickCounter = 0f;
		chargeCounter = 0f;
		isActive = false;
	}
}
