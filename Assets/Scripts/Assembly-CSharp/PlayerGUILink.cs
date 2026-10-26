using UnityEngine;

public class PlayerGUILink : MonoBehaviour
{
	private PlayerGUI playerGUI;

	private bool isQuitting;

	private void Start()
	{
	}

	public void SwitchGui(PlayerGUI playerGUI)
	{
		this.playerGUI = playerGUI;
	}

	public void SetGUI(PlayerGUI playerGUI)
	{
		this.playerGUI = playerGUI;
		playerGUI.OnSoldierSpawn(base.gameObject);
	}

	public void OnDestroy()
	{
		if (playerGUI != null && !isQuitting)
		{
			playerGUI.OnSoldierDespawn();
		}
	}

	private void OnApplicationQuit()
	{
		isQuitting = true;
	}

	private void OnDeath(Health health)
	{
		if (playerGUI != null)
		{
			playerGUI.OnSoldierDespawn();
		}
	}

	public void AddWeapon(StoredWeaponData weaponData)
	{
		if (playerGUI != null)
		{
			playerGUI.AddInventoryWeapon(weaponData.Slot, weaponData.Weapon, weaponData);
		}
	}

	public void RemoveWeapon(InventoryWeaponSlot slot)
	{
		if (playerGUI != null)
		{
			playerGUI.RemoveInventoryWeapon(slot);
		}
	}

	public void EquipWeapon(InventoryWeaponSlot slot, Weapon equippedWeapon)
	{
		if (playerGUI != null)
		{
			playerGUI.EquipWeapon(slot, equippedWeapon);
		}
	}

	public void UpdateAmmunitionForSlot(InventoryWeaponSlot slot)
	{
		if (playerGUI != null)
		{
			playerGUI.UpdateAmmunitionBySlot(slot);
		}
	}

	public void StartReloadBar(float duration)
	{
		if (playerGUI != null)
		{
			playerGUI.StartReload(duration);
		}
	}

	public void StopReloadBar()
	{
		if (playerGUI != null)
		{
			playerGUI.StopReload();
		}
	}

	public void AddAbility(Ability ability, AbilitySlot slot)
	{
		if (playerGUI != null && ability != null)
		{
			playerGUI.AddAbility(slot, ability.Image, (!ability.UseCharges) ? (-1) : ability.ChargesLeft());
		}
	}

	public void RemoveAbility(AbilitySlot slot)
	{
		if (playerGUI != null)
		{
			playerGUI.RemoveAbility(slot);
		}
	}

	public void SetAbilityCooldown(Ability ability, AbilitySlot slot)
	{
		if (playerGUI != null && ability != null && ability.Cooldown > 0f)
		{
			playerGUI.SetAbilityOnCooldown(slot, ability.Cooldown);
		}
	}

	public void SetAbilityCooldownOver(AbilitySlot slot)
	{
		if (playerGUI != null)
		{
			playerGUI.StopAbilityCooldown(slot);
		}
	}

	public void SetAbilityCharges(Ability ability, AbilitySlot slot)
	{
		if (playerGUI != null && ability != null)
		{
			playerGUI.SetAbilityCharges(slot, ability.ChargesLeft());
		}
	}

	public void SetKillStreak(int killCount, float moneyMultiplier)
	{
		if (playerGUI != null)
		{
			playerGUI.SetKillStreak(killCount, moneyMultiplier);
		}
	}

	public void ResetKillStreak()
	{
		if (playerGUI != null)
		{
			playerGUI.ResetKillStreak();
		}
	}

	public void OnMoneyGained(int amount)
	{
		if (playerGUI != null)
		{
			playerGUI.OnMoneyGained(amount);
		}
	}

	public void OnMoneyGained(int baseAmount, int amount, float moneyMultiplier)
	{
		if (playerGUI != null)
		{
			playerGUI.OnMoneyGained(baseAmount, amount, moneyMultiplier);
		}
	}

	public void OnMoneyChange()
	{
		if (playerGUI != null)
		{
			playerGUI.OnMoneyChange();
		}
	}

	public void SetWeaponExperience(int exp, int maxExp, int weaponLvl)
	{
		playerGUI.SetWeaponExperience(exp, maxExp, weaponLvl);
	}

	public void OnPickup(PickupType type)
	{
		if (playerGUI != null)
		{
			playerGUI.OnPickup(type);
		}
	}

	public void OnWeaponPickup(Weapon weapon)
	{
		if (playerGUI != null && weapon != null)
		{
			playerGUI.OnPickup(PickupType.Weapon, weapon);
		}
	}

	public void OnBonusPickup(PickupBonusType bonusType)
	{
		if (playerGUI != null && bonusType != 0)
		{
			playerGUI.OnPickup(PickupType.Bonus, bonusType);
		}
	}

	public void OnPickupFailed(PickupType type)
	{
		if (playerGUI != null)
		{
			playerGUI.OnPickupUnabled(type);
		}
	}
}
