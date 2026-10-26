using System.Collections.Generic;
using UnityEngine;

public class PlayerGUI
{
	public PlayerHudBase Hud;

	public GameObject HudObject;

	public PlayerShopWindow Shop;

	public UnitBar UnitBar;

	public GameObject UnitBarObject;

	private Player player;

	private GameObject soldier;

	private PlayerStats stats;

	private GuiInGameManager guiManager;

	private HUDText combatText;

	private PlayerWeaponController inventory;

	private Color unablePickupColor;

	private bool isDead;

	private Dictionary<InventoryWeaponSlot, StoredWeaponData> weapons = new Dictionary<InventoryWeaponSlot, StoredWeaponData>();

	public Dictionary<InventoryWeaponSlot, StoredWeaponData> Weapons
	{
		get
		{
			return weapons;
		}
	}

	public Player Player
	{
		get
		{
			return player;
		}
	}

	public GameObject Soldier
	{
		get
		{
			return soldier;
		}
	}

	public PlayerWeaponController Inventory
	{
		get
		{
			return inventory;
		}
	}

	public PlayerStats Stats
	{
		get
		{
			return stats;
		}
	}

	public GuiInGameManager GuiManager
	{
		get
		{
			return guiManager;
		}
	}

	public PlayerGUI(Player player, GuiInGameManager guiManager)
	{
		this.guiManager = guiManager;
		this.player = player;
		stats = PlayerStatsManager.Instance.StatsByPlayer(player);
		unablePickupColor = new Color(0.75f, 0.1f, 0.1f);
	}

	public void SetSizeBasedOnResolution()
	{
	}

	public void SwitchGui(PlayerGUI otherGui)
	{
		weapons = otherGui.weapons;
		isDead = otherGui.isDead;
		soldier = otherGui.soldier;
		stats = otherGui.stats;
		inventory = otherGui.inventory;
		if (UnitBar.CombatText != null)
		{
			combatText = UnitBar.CombatText.GetComponent<HUDText>();
		}
		UnitBar.gameObject.SetActive(true);
		UnitBar.SetTarget(soldier);
		UnitBar.Show();
		UnitBar.OnHealthUpdate(soldier.GetComponent<Health>());
		UnitBar.OnArmorUpdate(soldier.GetComponent<Health>());
		UnitBar.BarArmor.CalculateBar = true;
		Hud.RegisterHudHealthListener(soldier);
		Hud.SyncMoneyDisplay();
	}

	public int Money()
	{
		return stats.Money;
	}

	public void OnSoldierSpawn(GameObject soldier)
	{
		isDead = false;
		this.soldier = soldier;
		inventory = soldier.GetComponent<PlayerWeaponController>();
		if (UnitBar.CombatText != null)
		{
			combatText = UnitBar.CombatText.GetComponent<HUDText>();
		}
		UnitBar.gameObject.SetActive(true);
		UnitBar.SetTarget(soldier);
		UnitBar.Show();
		Hud.RegisterHudHealthListener(soldier);
		Hud.OnRespawn();
		Hud.SyncMoneyDisplay();
	}

	public void OnSoldierDespawn()
	{
		isDead = true;
		weapons.Clear();
		Hud.OnSoldierDespawn();
		Hud.RemoveHudHealthListener();
		Shop.ClearInventory();
		if (UnitBar != null)
		{
			UnitBar.Reset();
			UnitBar.gameObject.SetActive(false);
		}
		combatText = null;
	}

	public int GetID()
	{
		if (player != null)
		{
			return player.ID;
		}
		return 0;
	}

	public void DisablePlayerGUI()
	{
		Hud.DisablePlayerGUI();
	}

	public void ActivatePlayerGUI()
	{
		Hud.ActivatePlayerGUI();
	}

	public void AddAbility(AbilitySlot slot, string image, int charges)
	{
		Hud.AddAbility(slot, image, charges);
	}

	public void RemoveAbility(AbilitySlot slot)
	{
		Hud.RemoveAbility(slot);
	}

	public void SetAbilityOnCooldown(AbilitySlot slot, float time)
	{
		Hud.SetAbilityOnCooldown(slot, time);
	}

	public void StopAbilityCooldown(AbilitySlot slot)
	{
		Hud.StopAbilityCooldown(slot);
	}

	public void SetAbilityCharges(AbilitySlot slot, int charges)
	{
		Hud.SetAbilityCharges(slot, charges);
	}

	public void EquipWeapon(InventoryWeaponSlot slot, Weapon equippedWeapon)
	{
		Hud.EquipWeapon(slot, equippedWeapon);
		Shop.SetEquipedWeaponReference(equippedWeapon);
		if (Weapons != null && Weapons[slot] != null)
		{
			SetAmmoStatus(Weapons[slot].HasAmmunition());
		}
	}

	public void UpdateAmmunitionBySlot(InventoryWeaponSlot slot)
	{
		Hud.UpdateAmmunitionBySlot(slot);
		if (Weapons != null && Weapons[slot] != null)
		{
			SetAmmoStatus(Weapons[slot].HasAmmunition());
		}
	}

	public void AddInventoryWeapon(InventoryWeaponSlot slot, Weapon newWeapon, StoredWeaponData storedWeapons)
	{
		weapons.Add(slot, storedWeapons);
		Hud.AddInventoryWeapon(slot, newWeapon, storedWeapons);
		Shop.AddInventoryWeapon(slot, newWeapon, storedWeapons);
	}

	public void RemoveInventoryWeapon(InventoryWeaponSlot slot)
	{
		weapons.Remove(slot);
		Hud.RemoveInventoryWeapon(slot);
		Shop.RemoveInventoryWeapon(slot);
	}

	public void StartReload(float time)
	{
		UnitBar.StartReload(time);
		Hud.StartReload(time);
	}

	public void StopReload()
	{
		UnitBar.StopReload();
		Hud.StopReload();
	}

	public void HideUnitBar()
	{
		UnitBar.Hide();
	}

	public void ShowUnitBar()
	{
		if (!isDead)
		{
			UnitBar.Show();
		}
	}

	public void SetKillStreak(int killCount, float moneyMultiplier)
	{
		Hud.SetKillStreak(killCount, moneyMultiplier);
	}

	public void ResetKillStreak()
	{
		Hud.ResetKillStreak();
	}

	public void OnMoneyChange()
	{
		Hud.SyncMoneyDisplay();
	}

	public void OnMoneyGained(int value)
	{
		Hud.AddMoney(value, value);
	}

	public void OnMoneyGained(int baseAmount, int value, float moneyMultiplier)
	{
		if (moneyMultiplier <= 1f)
		{
			Hud.AddMoney(baseAmount, value);
		}
		else
		{
			Hud.AddMoney(baseAmount, value, moneyMultiplier);
		}
	}

	public void SetWeaponExperience(int exp, int maxExp, int weaponLvl)
	{
		Hud.SetWeaponExperience(exp, maxExp, weaponLvl);
	}

	public void SetAmmoStatus(bool isVisible)
	{
		UnitBar.SetAmmoStatus(isVisible);
	}

	public void OnPickup(PickupType type)
	{
		if (combatText != null)
		{
			switch (type)
			{
			case PickupType.Ammo:
				combatText.Add(Localizer.GetString("gui_pickup_ammo"), Color.yellow, 0.4f);
				break;
			case PickupType.Syringe:
				combatText.Add(Localizer.GetString("gui_pickup_medkit"), Color.yellow, 0.4f);
				break;
			}
		}
	}

	public void OnPickup(PickupType type, Weapon weapon)
	{
		if (combatText != null && type == PickupType.Weapon && weapon != null)
		{
			combatText.Add(Localizer.GetString("gui_pickup") + " " + weapon.Name, Color.yellow, 1f);
		}
	}

	public void OnPickup(PickupType type, PickupBonusType bonusType)
	{
		if (combatText != null && type == PickupType.Bonus)
		{
			switch (bonusType)
			{
			case PickupBonusType.Damage:
				combatText.Add(Localizer.GetString("gui_pickup_boost_damage"), Color.yellow, 0.4f);
				break;
			case PickupBonusType.Shield:
				combatText.Add(Localizer.GetString("gui_pickup_boost_shield"), Color.yellow, 0.4f);
				break;
			}
		}
	}

	public void OnPickupUnabled(PickupType type)
	{
		if (combatText != null)
		{
			switch (type)
			{
			case PickupType.Ammo:
				combatText.Add(Localizer.GetString("gui_pickup_ammo_full"), unablePickupColor, 4f);
				break;
			case PickupType.Syringe:
				combatText.Add(Localizer.GetString("gui_pickup_medkit_full"), unablePickupColor, 4f);
				break;
			case PickupType.Weapon:
				combatText.Add(Localizer.GetString("gui_pickup_slot_full", true), unablePickupColor, 4f);
				break;
			case PickupType.Armor:
				combatText.Add(Localizer.GetString("gui_pickup_armor_full"), unablePickupColor, 4f);
				break;
			}
		}
	}
}
