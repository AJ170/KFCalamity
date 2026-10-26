using System.Collections.Generic;
using UnityEngine;

public class PlayerHudBase : MonoBehaviour
{
	public enum WeaponSlotState
	{
		Passive = 1,
		Selected = 2,
		Empty = 3,
		None = 4
	}

	public class InventoryWeapon
	{
		public AmmoDisplayType AmmoType;

		public bool HasAmmo;

		public bool UseAmmunition;

		public WeaponSlotState WeaponState;

		public UISprite SlotSprite;

		public Weapon Weapon;

		public StoredWeaponData Data;
	}

	private PlayerGUI playerGui;

	private UIAnchor anchor;

	protected Weapon equippedWeapon;

	protected InventoryWeaponSlot equippedSlot = InventoryWeaponSlot.None;

	protected int ID;

	protected Dictionary<InventoryWeaponSlot, InventoryWeapon> inventoryWeapons = new Dictionary<InventoryWeaponSlot, InventoryWeapon>();

	protected Dictionary<AbilitySlot, AbilityButtonNormal> abilities = new Dictionary<AbilitySlot, AbilityButtonNormal>();

	public GameInfo GameStatistics { get; set; }

	public PlayerGUI PlayerGui
	{
		get
		{
			return playerGui;
		}
		set
		{
			playerGui = value;
		}
	}

	public virtual KillStreak GetKillStreakCounter()
	{
		return null;
	}

	public virtual AbilityButtonNormal GetAbilityButton(AbilitySlot slot)
	{
		return null;
	}

	protected virtual void OnSyncingGui(PlayerHudBase otherHud)
	{
	}

	public virtual void ToggleAllTweeens(bool toggle)
	{
	}

	public void SyncGui(PlayerHudBase otherHud)
	{
		otherHud.RemoveHudHealthListener();
		inventoryWeapons = otherHud.inventoryWeapons;
		equippedWeapon = otherHud.equippedWeapon;
		equippedSlot = otherHud.equippedSlot;
		ID = otherHud.ID;
		OnSyncingGui(otherHud);
		UpdateAmmunitionBySlot(InventoryWeaponSlot.Melee);
		UpdateAmmunitionBySlot(InventoryWeaponSlot.Pistol);
		UpdateAmmunitionBySlot(InventoryWeaponSlot.Primary1);
		UpdateAmmunitionBySlot(InventoryWeaponSlot.Primary2);
		if (otherHud.equippedSlot != InventoryWeaponSlot.None)
		{
			EquipWeapon(otherHud.equippedSlot, otherHud.equippedWeapon);
		}
		AbilityButtonNormal abilityButton = otherHud.GetAbilityButton(AbilitySlot.Dodge);
		AddAbility(AbilitySlot.Dodge, abilityButton.IconName, abilityButton.AbilityCharges);
		if (abilityButton.CurrentCooldown > 0f)
		{
			abilities[AbilitySlot.Dodge].SetCooldown(abilityButton.CurrentCooldown);
		}
		AbilityButtonNormal abilityButton2 = otherHud.GetAbilityButton(AbilitySlot.Grenade);
		AddAbility(AbilitySlot.Grenade, abilityButton2.IconName, abilityButton2.AbilityCharges);
		AbilityButtonNormal abilityButton3 = otherHud.GetAbilityButton(AbilitySlot.SyringeSelf);
		AddAbility(AbilitySlot.SyringeSelf, abilityButton3.IconName, abilityButton3.AbilityCharges);
		AbilityButtonNormal abilityButton4 = otherHud.GetAbilityButton(AbilitySlot.Perk);
		AddAbility(AbilitySlot.Perk, abilityButton4.IconName, abilityButton4.AbilityCharges);
		if (abilityButton4.CurrentCooldown > 0f)
		{
			abilities[AbilitySlot.Perk].SetCooldown(abilityButton4.CurrentCooldown);
		}
	}

	public virtual void Start()
	{
	}

	public virtual void OnDestroy()
	{
	}

	protected void SetupWeaponSlot(InventoryWeaponSlot slot, UISprite weaponSlotSprite)
	{
		if (!inventoryWeapons.ContainsKey(slot))
		{
			InventoryWeapon inventoryWeapon = new InventoryWeapon();
			inventoryWeapon.WeaponState = WeaponSlotState.None;
			inventoryWeapon.SlotSprite = weaponSlotSprite;
			inventoryWeapon.AmmoType = AmmoDisplayType.None;
			inventoryWeapon.HasAmmo = true;
			inventoryWeapons.Add(slot, inventoryWeapon);
			SetWeaponSlotTexture(slot);
		}
	}

	protected virtual void OnNoWeapon(InventoryWeaponSlot slot, InventoryWeapon weapon)
	{
	}

	protected virtual void OnWeaponSlotStateChanged(WeaponSlotState slotState, InventoryWeaponSlot slot, InventoryWeapon weapon)
	{
	}

	protected void SetWeaponSlotTexture(InventoryWeaponSlot slot)
	{
		InventoryWeapon inventoryWeapon = inventoryWeapons[slot];
		if (inventoryWeapon.Data != null)
		{
			if (inventoryWeapon.SlotSprite != null)
			{
				inventoryWeapon.SlotSprite.gameObject.SetActive(true);
				inventoryWeapon.SlotSprite.enabled = true;
			}
			OnWeaponSlotStateChanged(inventoryWeapon.WeaponState, slot, inventoryWeapon);
		}
		else
		{
			OnNoWeapon(slot, inventoryWeapon);
		}
		if (inventoryWeapon.SlotSprite != null)
		{
			inventoryWeapon.SlotSprite.MakePixelPerfect();
		}
	}

	public virtual void RegisterHudHealthListener(GameObject soldier)
	{
	}

	public virtual void RemoveHudHealthListener()
	{
	}

	public virtual void OnDamageUpdate(Health UnitHealthComponent, int i, int k, float f)
	{
	}

	public virtual void OnScaleTweenEnd()
	{
	}

	public virtual void OnAlphaTweenEnd()
	{
	}

	public virtual void OnWeaponSwapFlashEneded()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void TweenIn()
	{
	}

	public virtual void TweenOut()
	{
	}

	public virtual void OnTriggerEnter(Collider collider)
	{
	}

	public virtual void OnTriggerExit(Collider collider)
	{
	}

	public virtual void CheckForNullColliders()
	{
	}

	public virtual void ToggleWeaponTray()
	{
	}

	public virtual void OnPauseMenuChanged(bool isOpen)
	{
	}

	public void Initialize(GuiInGameManager manager, PlayerGUI pGui, int playerNumber, Vector2 pixelOffset, Camera camera)
	{
		playerGui = pGui;
		ID = playerNumber + 1;
		anchor = GetComponent<UIAnchor>();
		if (anchor != null)
		{
			anchor.relativeOffset.Set(pixelOffset.x, pixelOffset.y);
			anchor.uiCamera = camera;
		}
		UIAnchor[] componentsInChildren = GetComponentsInChildren<UIAnchor>();
		UIAnchor[] array = componentsInChildren;
		foreach (UIAnchor uIAnchor in array)
		{
			uIAnchor.uiCamera = camera;
		}
		OnInitialize(manager, pGui, playerNumber, pixelOffset, camera);
	}

	public virtual void OnInitialize(GuiInGameManager manager, PlayerGUI pGui, int playerNumber, Vector2 pixelOffset, Camera camera)
	{
	}

	public virtual void OnRespawn()
	{
	}

	public virtual void DisablePlayerGUI()
	{
	}

	public virtual void ActivatePlayerGUI()
	{
	}

	public virtual void AddAbility(AbilitySlot slot, string image, int charges)
	{
	}

	public virtual void RemoveAbility(AbilitySlot slot)
	{
		if (abilities.ContainsKey(slot))
		{
			abilities[slot].StopCooldown();
			abilities.Remove(slot);
		}
	}

	public virtual void SetAbilityOnCooldown(AbilitySlot slot, float time)
	{
		if (abilities[slot] != null)
		{
			abilities[slot].SetCooldown(time);
		}
	}

	public virtual void StopAbilityCooldown(AbilitySlot slot)
	{
		if (abilities.ContainsKey(slot) && abilities[slot] != null)
		{
			abilities[slot].StopCooldown();
		}
	}

	public virtual void SetAbilityCharges(AbilitySlot slot, int charges)
	{
		if (abilities[slot] != null)
		{
			abilities[slot].SetCharges(charges);
		}
	}

	public virtual void StartReload(float duration)
	{
	}

	public virtual void StopReload()
	{
	}

	protected virtual void SetAmmoText()
	{
	}

	public virtual void EquipWeapon(InventoryWeaponSlot slot, Weapon newEquippedWeapon)
	{
		if (equippedSlot != InventoryWeaponSlot.None)
		{
			if (inventoryWeapons[equippedSlot].HasAmmo)
			{
				inventoryWeapons[equippedSlot].WeaponState = WeaponSlotState.Passive;
				inventoryWeapons[equippedSlot].HasAmmo = true;
			}
			else
			{
				inventoryWeapons[equippedSlot].WeaponState = WeaponSlotState.Passive;
				inventoryWeapons[equippedSlot].HasAmmo = false;
			}
			SetWeaponSlotTexture(equippedSlot);
		}
		if (!inventoryWeapons.ContainsKey(slot))
		{
			Debug.Log("NO SLOT: " + slot);
		}
		if (newEquippedWeapon == null)
		{
			Debug.Log("WEAPON IS NULL " + slot);
		}
		inventoryWeapons[slot].AmmoType = newEquippedWeapon.AmmunitionDisplay;
		if (newEquippedWeapon.TotalAmmunition > 0 || newEquippedWeapon.Type == WeaponType.Melee)
		{
			inventoryWeapons[slot].HasAmmo = true;
		}
		else
		{
			inventoryWeapons[slot].HasAmmo = false;
		}
		inventoryWeapons[slot].UseAmmunition = newEquippedWeapon.UseAmmunition;
		inventoryWeapons[slot].WeaponState = WeaponSlotState.Selected;
		equippedWeapon = newEquippedWeapon;
		equippedSlot = slot;
		SetWeaponSlotTexture(slot);
		SetAmmoText();
	}

	public virtual void UpdateAmmunitionBySlot(InventoryWeaponSlot slot)
	{
		if (inventoryWeapons[slot].UseAmmunition)
		{
			if (inventoryWeapons[slot].WeaponState == WeaponSlotState.Selected)
			{
				SetAmmoText();
			}
			if (inventoryWeapons[slot].Data.LoadedAmmunition + inventoryWeapons[slot].Data.SpareAmmunition > 0)
			{
				inventoryWeapons[slot].HasAmmo = true;
			}
			else
			{
				inventoryWeapons[slot].HasAmmo = false;
			}
		}
		SetWeaponSlotTexture(slot);
	}

	public virtual void AddInventoryWeapon(InventoryWeaponSlot slot, Weapon newWeapon, StoredWeaponData storedWeapons)
	{
		inventoryWeapons[slot].WeaponState = WeaponSlotState.Passive;
		inventoryWeapons[slot].Data = storedWeapons;
		inventoryWeapons[slot].Weapon = newWeapon;
		inventoryWeapons[slot].HasAmmo = true;
		inventoryWeapons[slot].UseAmmunition = newWeapon.UseAmmunition;
		SetWeaponSlotTexture(slot);
	}

	protected virtual void OnEquippedWeaponRemoved(InventoryWeaponSlot slot)
	{
	}

	public virtual void RemoveInventoryWeapon(InventoryWeaponSlot slot)
	{
		if (inventoryWeapons.ContainsKey(slot))
		{
			bool flag = false;
			if (equippedWeapon != null && inventoryWeapons[slot].Weapon != null && inventoryWeapons[slot].Weapon.Name == equippedWeapon.Name)
			{
				flag = true;
			}
			inventoryWeapons[slot].WeaponState = WeaponSlotState.None;
			inventoryWeapons[slot].Data = null;
			inventoryWeapons[slot].Weapon = null;
			inventoryWeapons[slot].HasAmmo = false;
			inventoryWeapons[slot].UseAmmunition = false;
			if (flag)
			{
				OnEquippedWeaponRemoved(slot);
				equippedWeapon = null;
				equippedSlot = InventoryWeaponSlot.None;
				SetAmmoText();
			}
			SetWeaponSlotTexture(slot);
		}
		else
		{
			Debug.Log("[PlayerHud]<RemoveInventoryWeapon> - inventoryWeapons does not contain: " + slot);
		}
	}

	public virtual void AddMoney(int baseAmount, int money)
	{
	}

	public virtual void AddMoney(int baseAmount, int money, float multiplier)
	{
	}

	public virtual void SyncMoneyDisplay()
	{
	}

	public virtual void SetKillStreak(int killCount, float moneyMultiplier)
	{
	}

	public virtual string GetCurrentHexCodeColor()
	{
		return string.Empty;
	}

	public virtual void ResetKillStreak()
	{
	}

	public virtual void OnSoldierDespawn()
	{
	}

	public int GetID()
	{
		return ID;
	}

	public virtual WeaponTrayDrag GetWeaponTray()
	{
		return null;
	}

	public virtual void SetWeaponExperience(int exp, int maxExp, int weaponLvl)
	{
	}
}
