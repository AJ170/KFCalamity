using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : WeaponController
{
	private InventoryWeaponSlot activeWeaponSlot = InventoryWeaponSlot.None;

	private Dictionary<InventoryWeaponSlot, StoredWeaponData> storedWeapons = new Dictionary<InventoryWeaponSlot, StoredWeaponData>();

	private PlayerAnimationController animationController;

	private PlayerActionController actionController;

	private AimController aimController;

	private PlayerGUILink GUILink;

	private SpeechController speechController;

	private SelectionCircle selectionCircle;

	public GameObject StartingMelee;

	public GameObject StartingPistol;

	public GameObject StartingPrimary1;

	public GameObject StartingPrimary2;

	public bool AutoEquipWeapon = true;

	private LaserController laserController;

	private KillStatController killStats;

	protected override void Start()
	{
		base.Start();
		laserController = new LaserController(base.gameObject);
		animationController = GetComponent<PlayerAnimationController>();
		actionController = GetComponent<PlayerActionController>();
		aimController = GetComponent<AimController>();
		GUILink = GetComponent<PlayerGUILink>();
		speechController = GetComponent<SpeechController>();
		selectionCircle = GetComponent<SelectionCircle>();
		killStats = GetComponent<KillStatController>();
		Soldier component = base.gameObject.GetComponent<Soldier>();
		if (component != null)
		{
			component.SetStartingWeapon(this, InventoryWeaponSlot.Melee);
			component.SetStartingWeapon(this, InventoryWeaponSlot.Pistol);
		}
		if (StartingMelee != null)
		{
			AddWeaponToSlot(StartingMelee, InventoryWeaponSlot.Melee);
			RefillTotalAmmunitionForSlot(InventoryWeaponSlot.Melee);
		}
		if (StartingPistol != null)
		{
			AddWeaponToSlot(StartingPistol, InventoryWeaponSlot.Pistol);
			RefillTotalAmmunitionForSlot(InventoryWeaponSlot.Pistol);
		}
		if (StartingPrimary1 != null)
		{
			AddWeaponToSlot(StartingPrimary1, InventoryWeaponSlot.Primary1);
			RefillTotalAmmunitionForSlot(InventoryWeaponSlot.Primary1);
		}
		if (StartingPrimary2 != null)
		{
			AddWeaponToSlot(StartingPrimary2, InventoryWeaponSlot.Primary2);
			RefillTotalAmmunitionForSlot(InventoryWeaponSlot.Primary2);
		}
		foreach (int value in Enum.GetValues(typeof(InventoryWeaponSlot)))
		{
			if (value != 0 && storedWeapons.ContainsKey((InventoryWeaponSlot)value))
			{
				SetActiveWeapon((InventoryWeaponSlot)value);
				break;
			}
		}
		if (activeWeapon == null && storedWeapons.ContainsKey(InventoryWeaponSlot.Melee))
		{
			SetActiveWeapon(InventoryWeaponSlot.Melee);
		}
	}

	private void OnDeath(Health health)
	{
		if (CanDropWeapon(activeWeaponSlot))
		{
			DropActiveWeapon();
		}
	}

	private void Update()
	{
		laserController.Update();
	}

	public void AddWeaponToSlot(GameObject weaponPrefab, InventoryWeaponSlot slot)
	{
		AddWeaponToSlot(weaponPrefab, slot, null);
	}

	public void AddWeaponToSlot(GameObject weaponPrefab, InventoryWeaponSlot slot, StoredWeaponData storedData)
	{
		if (weaponPrefab != null)
		{
			if (!IsSlotAvailable(slot))
			{
				Debug.LogError(string.Format("[PlayerWeaponController:AddWeaponToSlot] Failed to add weapon {0} to unit {1}: Slot {2} is full!", weaponPrefab.name, base.gameObject.name, slot.ToString()));
				return;
			}
			if (storedData != null)
			{
				storedData.Slot = slot;
			}
			else
			{
				storedData = new StoredWeaponData(weaponPrefab, slot);
			}
			storedWeapons.Add(slot, storedData);
			if (GUILink != null)
			{
				GUILink.AddWeapon(storedData);
			}
		}
		else
		{
			Debug.LogError(string.Format("[PlayerWeaponController:AddWeaponToSlot] Failed to add weapon to {0}, missing WeaponPrefab", base.gameObject.ToString()));
		}
	}

	public void RemoveWeapon(string weaponName)
	{
		if (activeWeaponGameObject != null && activeWeaponGameObject.name == weaponName)
		{
			Debug.LogWarning(string.Format("[PlayerWeaponController:RemoveWeapon] Removing active weapon {0}.", weaponName));
			DestroyActiveWeapon();
		}
		StoredWeaponData storedWeaponData = GetStoredWeaponData(weaponName);
		if (storedWeaponData != null)
		{
			storedWeapons.Remove(storedWeaponData.Slot);
			if (GUILink != null)
			{
				GUILink.RemoveWeapon(storedWeaponData.Slot);
			}
		}
	}

	public void RemoveWeapon(GameObject weaponPrefab)
	{
		StoredWeaponData storedWeaponData = GetStoredWeaponData(weaponPrefab);
		if (storedWeaponData == null)
		{
			Debug.LogError(string.Format("[PlayerWeaponController:RemoveWeapon] Unable to remove weapon {0}: Weapon not found.", weaponPrefab.name));
			return;
		}
		if (activeWeaponGameObject == weaponPrefab)
		{
			DestroyActiveWeapon();
		}
		storedWeapons.Remove(storedWeaponData.Slot);
		if (GUILink != null)
		{
			GUILink.RemoveWeapon(storedWeaponData.Slot);
		}
	}

	public void RemoveWeapon(InventoryWeaponSlot slot)
	{
		if (slot == InventoryWeaponSlot.None)
		{
			Debug.LogError("[PlayerWeaponController:RemoveWeapon] Cannot remove weapon from slot None");
			return;
		}
		if (activeWeaponSlot == slot)
		{
			DestroyActiveWeapon();
		}
		if (!storedWeapons.ContainsKey(slot))
		{
			Debug.LogError(string.Format("[PlayerWeaponController:RemoveWeapon] Unable to remove from slot {0}: Slot is empty.", slot.ToString()));
		}
		storedWeapons.Remove(slot);
		if (GUILink != null)
		{
			GUILink.RemoveWeapon(slot);
		}
	}

	public void SetActiveWeapon(string weaponName)
	{
		if (CanSwitchWeapon() && (!(activeWeaponGameObject != null) || !(activeWeaponGameObject.name == weaponName)))
		{
			StoredWeaponData storedWeaponData = GetStoredWeaponData(weaponName);
			if (storedWeaponData != null)
			{
				SetActiveWeapon(storedWeaponData);
			}
		}
	}

	public bool SetActiveWeapon(InventoryWeaponSlot slot)
	{
		if (!CanSwitchWeapon())
		{
			return false;
		}
		if (slot == InventoryWeaponSlot.None)
		{
			RemoveActiveWeapon();
		}
		else if (slot == activeWeaponSlot)
		{
			return false;
		}
		StoredWeaponData storedWeaponData = GetStoredWeaponData(slot);
		if (storedWeaponData == null)
		{
			return false;
		}
		SetActiveWeapon(storedWeaponData);
		return true;
	}

	public override bool SetActiveWeapon(GameObject weaponPrefab)
	{
		if (!CanSwitchWeapon())
		{
			return false;
		}
		if (activeWeaponGameObject != null && activeWeaponGameObject.name == weaponPrefab.name)
		{
			return false;
		}
		StoredWeaponData storedWeaponData = GetStoredWeaponData(weaponPrefab);
		if (storedWeaponData == null)
		{
			return false;
		}
		SetActiveWeapon(storedWeaponData);
		return true;
	}

	public bool SetActiveWeapon(StoredWeaponData storedData)
	{
		if (!CanSwitchWeapon())
		{
			return false;
		}
		if (actionController != null)
		{
			actionController.OnSwitchWeapon();
		}
		GameObject prefab = storedData.Prefab;
		base.SetActiveWeapon(prefab);
		if (activeWeapon != null)
		{
			activeWeaponSlot = storedData.Slot;
			activeWeapon.SetWeaponData(storedData);
			laserController.OnWeaponEquipped(activeWeaponGameObject);
			if (animationController != null)
			{
				string equipAnimation = activeWeapon.EquipAnimation;
				if (!string.IsNullOrEmpty(equipAnimation))
				{
					animationController.SetBool(equipAnimation, true);
				}
			}
			if (actionController != null)
			{
				SwitchWeaponAction action = new SwitchWeaponAction();
				actionController.AddAction(action);
			}
			if (GUILink != null)
			{
				GUILink.EquipWeapon(storedData.Slot, activeWeapon);
				if (killStats != null)
				{
					killStats.SetWeaponExp(storedData.Weapon.Name);
				}
			}
			if (aimController != null)
			{
				aimController.OnWeaponSwitch(prefab);
			}
			if (speechController != null)
			{
				speechController.OnWeaponSwitch(activeWeapon);
			}
			if (selectionCircle != null)
			{
				selectionCircle.OnWeaponSwitch(activeWeapon);
			}
			return true;
		}
		return false;
	}

	public void RemoveActiveWeapon()
	{
		if (activeWeapon != null)
		{
			StoreActiveWeaponData();
			DestroyActiveWeapon();
			if (GUILink != null)
			{
				GUILink.EquipWeapon(InventoryWeaponSlot.None, null);
			}
			if (aimController != null)
			{
				aimController.OnWeaponSwitch(null);
			}
		}
	}

	private bool CanSwitchWeapon()
	{
		bool result = true;
		if (actionController != null)
		{
			result = actionController.CanSwitchWeapon();
		}
		return result;
	}

	public void OnWeaponSwitchDone()
	{
		if (activeWeapon != null && activeWeapon.UseAmmunition)
		{
			TriggerReloadIfNeeded();
		}
	}

	public void AutoSwitchWeapon(bool canSwitchToActive = false)
	{
		Array values = Enum.GetValues(typeof(InventoryWeaponSlot));
		Array.Reverse(values);
		foreach (int item in values)
		{
			if ((canSwitchToActive || item != (int)activeWeaponSlot) && HasWeapon((InventoryWeaponSlot)item) && SwitchToSlotIfPossible((InventoryWeaponSlot)item))
			{
				break;
			}
		}
	}

	public void SwitchToNextWeapon()
	{
		InventoryWeaponSlot inventoryWeaponSlot = activeWeaponSlot;
		if (inventoryWeaponSlot != InventoryWeaponSlot.None)
		{
			while ((inventoryWeaponSlot = NextInventoryWeaponSlot(inventoryWeaponSlot)) != activeWeaponSlot && !SwitchToSlotIfPossible(inventoryWeaponSlot))
			{
			}
		}
	}

	// PS Vita port: D-pad left / right cycle the inventory slots (DualInputCycleWeaponAction); like SwitchToNextWeapon,
	// slots without a weapon or ammunition are skipped.
	public bool SwitchToAdjacentWeapon(bool forward)
	{
		if (activeWeaponSlot == InventoryWeaponSlot.None || !CanSwitchWeapon())
		{
			return false;
		}
		InventoryWeaponSlot inventoryWeaponSlot = activeWeaponSlot;
		while ((inventoryWeaponSlot = (forward ? NextInventoryWeaponSlot(inventoryWeaponSlot) : PreviousInventoryWeaponSlot(inventoryWeaponSlot))) != activeWeaponSlot)
		{
			if (SwitchToSlotIfPossible(inventoryWeaponSlot))
			{
				return true;
			}
		}
		return false;
	}

	public bool CanDropActiveWeapon()
	{
		return CanDropWeapon(activeWeaponSlot);
	}

	public void SwitchToPrimaryWeaponIfNeeded()
	{
		if (activeWeaponSlot != InventoryWeaponSlot.Primary1 && activeWeaponSlot != InventoryWeaponSlot.Primary2)
		{
			bool flag = false;
			if (!SwitchToSlotIfPossible(InventoryWeaponSlot.Primary2))
			{
				flag = SwitchToSlotIfPossible(InventoryWeaponSlot.Primary1);
			}
		}
	}

	private bool SwitchToSlotIfPossible(InventoryWeaponSlot slot)
	{
		if (HasWeapon(slot))
		{
			StoredWeaponData storedWeaponData = GetStoredWeaponData(slot);
			if (storedWeaponData != null && storedWeaponData.HasAmmunition() && actionController != null)
			{
				if (activeWeapon != null && activeWeapon.ActiveRoundCooldown > 0f)
				{
					DelayedWeaponSwitchAction action = new DelayedWeaponSwitchAction(slot, activeWeapon.ActiveRoundCooldown, this);
					actionController.AddAction(action);
				}
				else
				{
					SetActiveWeapon(slot);
				}
				return true;
			}
		}
		return false;
	}

	public void AutoSwitchIfHigherThanEquipped(InventoryWeaponSlot slot)
	{
		if (HigherThanEquipped(slot))
		{
			SetActiveWeapon(slot);
		}
	}

	private bool HigherThanEquipped(InventoryWeaponSlot slot)
	{
		if (slot == InventoryWeaponSlot.None)
		{
			return false;
		}
		if (activeWeaponSlot == InventoryWeaponSlot.None)
		{
			return true;
		}
		if (activeWeaponSlot != InventoryWeaponSlot.None && activeWeaponSlot == slot)
		{
			return false;
		}
		if ((slot == InventoryWeaponSlot.Primary1 || slot == InventoryWeaponSlot.Primary2) && (activeWeaponSlot == InventoryWeaponSlot.Primary1 || activeWeaponSlot == InventoryWeaponSlot.Primary2))
		{
			return true;
		}
		return slot > activeWeaponSlot;
	}

	private InventoryWeaponSlot NextInventoryWeaponSlot(InventoryWeaponSlot slot)
	{
		InventoryWeaponSlot result = InventoryWeaponSlot.None;
		switch (slot)
		{
		case InventoryWeaponSlot.Melee:
			result = InventoryWeaponSlot.Pistol;
			break;
		case InventoryWeaponSlot.Pistol:
			result = InventoryWeaponSlot.Primary1;
			break;
		case InventoryWeaponSlot.Primary1:
			result = InventoryWeaponSlot.Primary2;
			break;
		case InventoryWeaponSlot.Primary2:
			result = InventoryWeaponSlot.Melee;
			break;
		}
		return result;
	}

	private InventoryWeaponSlot PreviousInventoryWeaponSlot(InventoryWeaponSlot slot)
	{
		InventoryWeaponSlot result = InventoryWeaponSlot.None;
		switch (slot)
		{
		case InventoryWeaponSlot.Melee:
			result = InventoryWeaponSlot.Primary2;
			break;
		case InventoryWeaponSlot.Pistol:
			result = InventoryWeaponSlot.Melee;
			break;
		case InventoryWeaponSlot.Primary1:
			result = InventoryWeaponSlot.Pistol;
			break;
		case InventoryWeaponSlot.Primary2:
			result = InventoryWeaponSlot.Primary1;
			break;
		}
		return result;
	}

	protected override void DestroyActiveWeapon()
	{
		if (activeWeapon != null)
		{
			laserController.OnWeaponUnequipped();
			if (animationController != null)
			{
				string equipAnimation = activeWeapon.EquipAnimation;
				if (!string.IsNullOrEmpty(equipAnimation))
				{
					animationController.SetBool(equipAnimation, false);
				}
			}
		}
		base.DestroyActiveWeapon();
		activeWeaponSlot = InventoryWeaponSlot.None;
	}

	private void StoreActiveWeaponData()
	{
		if (activeWeapon != null)
		{
			activeWeapon.SaveWeaponData();
		}
	}

	private StoredWeaponData GetStoredWeaponData(GameObject prefab)
	{
		StoredWeaponData result = null;
		foreach (StoredWeaponData value in storedWeapons.Values)
		{
			if (value.Prefab == prefab)
			{
				result = value;
				break;
			}
		}
		return result;
	}

	private StoredWeaponData GetStoredWeaponData(string weaponName)
	{
		StoredWeaponData result = null;
		foreach (StoredWeaponData value in storedWeapons.Values)
		{
			if (value.Prefab.name == weaponName)
			{
				result = value;
				break;
			}
		}
		return result;
	}

	private StoredWeaponData GetStoredWeaponData(InventoryWeaponSlot slot)
	{
		StoredWeaponData result = null;
		if (storedWeapons.ContainsKey(slot))
		{
			result = storedWeapons[slot];
		}
		return result;
	}

	public bool IsSlotAvailable(InventoryWeaponSlot slot)
	{
		bool result = false;
		if (slot != InventoryWeaponSlot.None)
		{
			result = !storedWeapons.ContainsKey(slot);
		}
		return result;
	}

	public bool IsSlotAvailable(WeaponSlot slot)
	{
		bool result = false;
		switch (slot)
		{
		case WeaponSlot.Melee:
		case WeaponSlot.Pistol:
			result = !storedWeapons.ContainsKey(WeaponSlotToInventorySlot(slot));
			break;
		case WeaponSlot.Primary:
			result = !storedWeapons.ContainsKey(InventoryWeaponSlot.Primary1) || !storedWeapons.ContainsKey(InventoryWeaponSlot.Primary2);
			break;
		case WeaponSlot.Any:
			result = storedWeapons.Count < 4;
			break;
		}
		return result;
	}

	public InventoryWeaponSlot WeaponSlotToInventorySlot(WeaponSlot slot)
	{
		switch (slot)
		{
		case WeaponSlot.Melee:
			return InventoryWeaponSlot.Melee;
		case WeaponSlot.Pistol:
			return InventoryWeaponSlot.Pistol;
		case WeaponSlot.Primary:
			return InventoryWeaponSlot.Primary1;
		default:
			return InventoryWeaponSlot.None;
		}
	}

	private WeaponSlot InventorySlotToWeaponSlot(InventoryWeaponSlot slot)
	{
		switch (slot)
		{
		case InventoryWeaponSlot.Melee:
			return WeaponSlot.Melee;
		case InventoryWeaponSlot.Pistol:
			return WeaponSlot.Pistol;
		case InventoryWeaponSlot.Primary1:
		case InventoryWeaponSlot.Primary2:
			return WeaponSlot.Primary;
		default:
			return WeaponSlot.Any;
		}
	}

	public InventoryWeaponSlot GetInventorySlot(GameObject prefab)
	{
		StoredWeaponData storedWeaponData = GetStoredWeaponData(prefab);
		if (storedWeaponData != null)
		{
			return storedWeaponData.Slot;
		}
		return InventoryWeaponSlot.None;
	}

	public override bool CanAttack()
	{
		bool flag = base.CanAttack();
		if (flag && actionController != null)
		{
			flag = actionController.CanAttack();
		}
		if (flag && activeWeapon.RequireIronsight && aimController != null)
		{
			flag = aimController.IsAiming();
		}
		return flag;
	}

	public bool CanAttackSecondary()
	{
		bool flag = false;
		if (activeWeapon != null)
		{
			flag = activeWeapon.HasSecondaryModule() && activeWeapon.CanAttackSecondary();
		}
		if (flag && actionController != null)
		{
			flag = actionController.CanAttack();
		}
		return flag;
	}

	public override void Attack()
	{
		if (actionController != null)
		{
			actionController.OnAttack();
		}
		if (activeWeapon != null)
		{
			if (aimController != null && aimController.IsAiming())
			{
				activeWeapon.Attack(true, aimController.IsAimingAtHead(), aimController.AimTarget());
			}
			else
			{
				activeWeapon.Attack();
			}
		}
		OnAttack();
	}

	public void AttackSecondary()
	{
		if (actionController != null)
		{
			actionController.OnAttack();
		}
		if (activeWeapon != null && activeWeapon.HasSecondaryModule())
		{
			activeWeapon.AttackSecondary();
			SecondaryAttack secondaryAttackModule = activeWeapon.SecondaryAttackModule;
			if (secondaryAttackModule.UseAmmunition)
			{
				UpdateGUIAmmunitionForSlot(activeWeaponSlot);
				TriggerSecondaryReloadIfNeeded();
			}
			else if (secondaryAttackModule.UsePrimaryAmmunition)
			{
				TriggerReloadOrSwitchIfNeeded();
			}
		}
		if (aimController != null)
		{
			aimController.OnAttack();
		}
	}

	private void OnAttack()
	{
		if (aimController != null)
		{
			aimController.OnAttack();
		}
		if (activeWeapon != null && activeWeapon.UseAmmunition)
		{
			UpdateGUIAmmunitionForSlot(activeWeaponSlot);
			TriggerReloadOrSwitchIfNeeded();
		}
	}

	public bool CanReload()
	{
		bool flag = false;
		if (activeWeapon != null)
		{
			flag = activeWeapon.CanReload();
		}
		if (flag && actionController != null)
		{
			flag = actionController.CanReload();
		}
		return flag;
	}

	public bool CanReloadSecondary()
	{
		bool result = false;
		if (actionController != null)
		{
			result = actionController.CanReload();
		}
		return result;
	}

	public void Reload()
	{
		if (activeWeapon != null && actionController != null)
		{
			PlayerAction action = ((activeWeapon.WeaponReloadType != 0) ? ((PlayerAction)new ShellReloadAction(activeWeapon, OnReload)) : ((PlayerAction)new ReloadAction(activeWeapon, OnReload)));
			actionController.OnReload();
			actionController.AddAction(action);
		}
	}

	public void ReloadSecondary()
	{
		if (activeWeapon != null && activeWeapon.HasSecondaryModule() && actionController != null)
		{
			SecondaryReloadAction action = new SecondaryReloadAction(activeWeapon, OnReload);
			actionController.AddAction(action);
		}
	}

	public void TriggerReloadOrSwitchIfNeeded()
	{
		if (activeWeapon != null && !activeWeapon.HasAmmunitionLoaded())
		{
			if (CanReload())
			{
				Reload();
			}
			else if (ActiveWeaponOutOfAmmunition() && CanSwitchWeapon())
			{
				AutoSwitchWeapon();
				SpeechManager.PlaySpeech(SpeechManager.SpeechLine.OutOfAmmo, base.gameObject);
			}
		}
	}

	public bool TriggerReloadIfNeeded(bool checkRoundCooldown = false)
	{
		if (activeWeapon != null && (!checkRoundCooldown || activeWeapon.IsRoundCooldownReady()) && activeWeapon.NeedsReload() && CanReload())
		{
			Reload();
			return true;
		}
		return false;
	}

	public bool TriggerSecondaryReloadIfNeeded()
	{
		if (activeWeapon != null && activeWeapon.HasSecondaryModule())
		{
			SecondaryAttack secondaryAttackModule = activeWeapon.SecondaryAttackModule;
			if (secondaryAttackModule.CanReload() && secondaryAttackModule.NeedsReload() && CanReloadSecondary())
			{
				ReloadSecondary();
				return true;
			}
		}
		return false;
	}

	private void OnReload()
	{
		if (activeWeapon != null)
		{
			UpdateGUIAmmunitionForSlot(activeWeaponSlot);
		}
	}

	public void RefillMagazineForSlot(InventoryWeaponSlot slot, int magCount = 1)
	{
		if (storedWeapons.ContainsKey(slot))
		{
			StoredWeaponData storedWeaponData = storedWeapons[slot];
			if (storedWeaponData.Weapon.UseAmmunition)
			{
				storedWeaponData.RefillMagazine(magCount);
				if (slot == activeWeaponSlot)
				{
					activeWeapon.LoadWeaponData();
				}
				UpdateGUIAmmunitionForSlot(slot);
			}
		}
		else
		{
			Debug.LogWarning(string.Format("[PlayerWeaponController:RefillMagazineForSlot] Failed to refill ammunition for slot {0}: Slot is empty.", slot.ToString()));
		}
	}

	public void RefillTotalAmmunitionForSlot(InventoryWeaponSlot slot)
	{
		if (storedWeapons.ContainsKey(slot))
		{
			StoredWeaponData storedWeaponData = storedWeapons[slot];
			if (storedWeaponData.Weapon.UseAmmunition)
			{
				storedWeaponData.RefillTotal();
				if (slot == activeWeaponSlot)
				{
					activeWeapon.LoadWeaponData();
				}
				UpdateGUIAmmunitionForSlot(slot);
			}
		}
		else
		{
			Debug.LogWarning(string.Format("[PlayerWeaponController:RefillTotalAmmunitionForSlot] Failed to refill ammunition for slot {0}: Slot is empty.", slot.ToString()));
		}
	}

	public void RefillSecondaryAmmunitionForSlot(InventoryWeaponSlot slot, int magCount = 1)
	{
		if (storedWeapons.ContainsKey(slot))
		{
			StoredWeaponData storedWeaponData = storedWeapons[slot];
			if (storedWeaponData.Weapon.HasSecondaryModule() && storedWeaponData.Weapon.SecondaryAttackModule.UseAmmunition)
			{
				storedWeaponData.RefillSecondary(magCount);
				if (slot == activeWeaponSlot)
				{
					activeWeapon.LoadWeaponData();
				}
				UpdateGUIAmmunitionForSlot(slot);
			}
		}
		else
		{
			Debug.LogWarning(string.Format("[PlayerWeaponController:RefillMagazineForSlot] Failed to refill ammunition for slot {0}: Slot is empty.", slot.ToString()));
		}
	}

	public void RefillAmmunitionBoxForSlot(InventoryWeaponSlot slot)
	{
		if (storedWeapons.ContainsKey(slot))
		{
			StoredWeaponData storedWeaponData = storedWeapons[slot];
			if (storedWeaponData.Weapon.UseAmmunition)
			{
				storedWeaponData.RefillAmmunitionBox();
				if (slot == activeWeaponSlot)
				{
					activeWeapon.LoadWeaponData();
				}
				UpdateGUIAmmunitionForSlot(slot);
			}
		}
		else
		{
			Debug.LogWarning(string.Format("[PlayerWeaponController:RefillAmmunitionBoxForSlot] Failed to refill ammunition for slot {0}: Slot is empty.", slot.ToString()));
		}
	}

	public bool AmmunitionInSlotFull(InventoryWeaponSlot slot)
	{
		bool result = true;
		if (storedWeapons.ContainsKey(slot))
		{
			StoredWeaponData storedWeaponData = storedWeapons[slot];
			result = storedWeaponData.AmmunitionFull();
		}
		return result;
	}

	public bool SecondaryAmmunitionInSlotFull(InventoryWeaponSlot slot)
	{
		bool result = true;
		if (storedWeapons.ContainsKey(slot))
		{
			StoredWeaponData storedWeaponData = storedWeapons[slot];
			result = storedWeaponData.SecondaryAmmunitionFull();
		}
		return result;
	}

	public void UpdateGUIAmmunition()
	{
		UpdateGUIAmmunitionForSlot(activeWeaponSlot);
	}

	private void UpdateGUIAmmunitionForSlot(InventoryWeaponSlot slot)
	{
		if (GUILink != null)
		{
			GUILink.UpdateAmmunitionForSlot(slot);
		}
	}

	public bool ActiveWeaponOutOfAmmunition()
	{
		bool result = false;
		if (activeWeapon != null && activeWeapon.UseAmmunition && !activeWeapon.HasAmmunitionLoaded() && !activeWeapon.HasSpareAmmunition())
		{
			result = true;
		}
		return result;
	}

	public bool HasWeapon(GameObject prefab)
	{
		bool result = false;
		foreach (StoredWeaponData value in storedWeapons.Values)
		{
			if (value.Prefab == prefab)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public bool HasWeapon(string weaponName)
	{
		bool result = false;
		foreach (StoredWeaponData value in storedWeapons.Values)
		{
			if (value.Weapon.name == weaponName)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public bool HasWeapon(InventoryWeaponSlot slot)
	{
		return !IsSlotAvailable(slot);
	}

	public bool ShouldAutoEquipWeapon()
	{
		bool result = false;
		if (AutoEquipWeapon && (activeWeapon == null || activeWeaponSlot == InventoryWeaponSlot.Melee || ActiveWeaponOutOfAmmunition()))
		{
			result = true;
		}
		return result;
	}

	public bool IsActiveWeapon(InventoryWeaponSlot slot)
	{
		return activeWeaponSlot == slot;
	}

	public bool CanDropWeapon(InventoryWeaponSlot slot)
	{
		if (slot == InventoryWeaponSlot.Melee || slot == InventoryWeaponSlot.None)
		{
			return false;
		}
		bool flag = false;
		StoredWeaponData storedWeaponData = GetStoredWeaponData(slot);
		flag = storedWeaponData != null && storedWeaponData.Weapon.CanDrop;
		if (flag && actionController != null)
		{
			flag = actionController.CanDropWeapon();
		}
		return flag;
	}

	public void DropActiveWeapon()
	{
		if (activeWeapon != null)
		{
			DropWeaponBySlot(activeWeaponSlot);
		}
	}

	public void DropWeaponBySlot(InventoryWeaponSlot slot)
	{
		PickupManager instance = PickupManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		StoredWeaponData storedWeaponData = GetStoredWeaponData(slot);
		if (storedWeaponData == null)
		{
			return;
		}
		WeaponPickup weaponPickup = instance.CreateWeaponPickup(storedWeaponData.Weapon.PickupPrefab, base.transform.root.position);
		if (weaponPickup != null)
		{
			AudioManager.PlayClipAtPoint("weapon_drop", base.transform.root.position + Vector3.up, 1f, Time.timeScale);
			storedWeaponData.Slot = InventoryWeaponSlot.None;
			weaponPickup.LastOwner = base.gameObject;
			weaponPickup.WeaponData = storedWeaponData;
			weaponPickup.Rotate(base.transform.rotation);
			bool flag = AutoEquipWeapon && slot == activeWeaponSlot;
			RemoveWeapon(slot);
			if (flag)
			{
				AutoSwitchWeapon();
			}
			weaponPickup.FallInArc = true;
			instance.ForceWeaponCollisionCheck(base.transform);
		}
	}

	private void SetLaserActive(bool active)
	{
		laserController.ForceLasersActive(active);
	}

	public void SetLaserActive(bool active, object source)
	{
		laserController.SetLaserActive(active, source);
	}

	public void OnHeadshotAim(bool aiming)
	{
		laserController.OnHeadshotAim(aiming);
	}

	public void RefillAmmunition()
	{
		foreach (StoredWeaponData value in storedWeapons.Values)
		{
			if (value.Slot != activeWeaponSlot)
			{
				Weapon component = value.Prefab.GetComponent<Weapon>();
				if (component != null)
				{
					value.SpareAmmunition = component.MaxTotalAmmunition;
				}
			}
			if (activeWeapon != null)
			{
				activeWeapon.RefillSpareAmmunition();
				UpdateGUIAmmunitionForSlot(value.Slot);
			}
		}
	}
}
