using System.Collections.Generic;
using PlayerInputHelpers;
using UnityEngine;

namespace Calamity.Input
{
	public class SoldierInputController : MonoBehaviour
	{
		public MovementMotor Motor;

		public float SyringeTargetThreshold = 0.5f;

		public float DropWeaponThreshold = 1.5f;

		private InputControl control;

		private int playerID;

		private List<object> movementLocks = new List<object>();

		private List<object> abilityLocks = new List<object>();

		private bool movementLocked;

		private bool abilitiesLocked;

		private PlayerWeaponController weaponController;

		private PlayerAbilityController abilityController;

		private AimController aimController;

		private DualInputAbilityAction syringeDualInputAction;

		private DualInputWeaponAction pistolDualInputAction;

		private DualInputWeaponAction primary1DualInputAction;

		private DualInputWeaponAction primary2DualInputAction;

		private DualInputCycleWeaponAction previousWeaponDualInputAction;

		private DualInputCycleWeaponAction nextWeaponDualInputAction;

		public InputControl Control
		{
			get
			{
				return control;
			}
		}

		public int PlayerID
		{
			get
			{
				return playerID;
			}
		}

		public bool IsMovementLocked
		{
			get
			{
				return movementLocked;
			}
		}

		public bool IsAbilitiesLocked
		{
			get
			{
				return abilitiesLocked;
			}
		}

		private void Start()
		{
			Soldier component = GetComponent<Soldier>();
			if (component != null)
			{
				playerID = component.PlayerInputID;
			}
			if (InputManager.Instance != null)
			{
				InputManager.Instance.RegisterSoldierController(this);
			}
			weaponController = GetComponent<PlayerWeaponController>();
			abilityController = GetComponent<PlayerAbilityController>();
			aimController = GetComponent<AimController>();
			syringeDualInputAction = new DualInputAbilityAction(this, abilityController, AbilitySlot.SyringeSelf, AbilitySlot.SyringeTarget, SyringeTargetThreshold);
			pistolDualInputAction = new DualInputWeaponAction(this, weaponController, WeaponAction.Equip, WeaponAction.Drop, DropWeaponThreshold, InventoryWeaponSlot.Pistol);
			primary1DualInputAction = new DualInputWeaponAction(this, weaponController, WeaponAction.Equip, WeaponAction.Drop, DropWeaponThreshold, InventoryWeaponSlot.Primary1);
			primary2DualInputAction = new DualInputWeaponAction(this, weaponController, WeaponAction.Equip, WeaponAction.Drop, DropWeaponThreshold, InventoryWeaponSlot.Primary2);
			previousWeaponDualInputAction = new DualInputCycleWeaponAction(this, weaponController, false, DropWeaponThreshold);
			nextWeaponDualInputAction = new DualInputCycleWeaponAction(this, weaponController, true, DropWeaponThreshold);
		}

		private void OnDestroy()
		{
			if (InputManager.Instance != null)
			{
				InputManager.Instance.UnregisterSoldierController(this);
			}
		}

		public void SetControl(InputControl control)
		{
			this.control = control;
		}

		private void Update()
		{
			if (control != null)
			{
				if (!movementLocked)
				{
					UpdateMovement();
				}
				UpdateEquip();
				if (!abilitiesLocked)
				{
					UpdateFire();
					UpdateReload();
					UpdateAbilities();
					UpdateAim();
					UpdateMisc();
				}
			}
		}

		public bool HasMovementLock(object source)
		{
			return movementLocks.Contains(source);
		}

		public bool HasAbilityLock(object source)
		{
			return abilityLocks.Contains(source);
		}

		public void SetLock(bool lockInput, object source)
		{
			SetLock(lockInput, lockInput, source);
		}

		public void SetLock(bool lockInput, bool stopMovement, object source)
		{
			SetMovementLock(lockInput, stopMovement, source);
			SetAbilityLock(lockInput, source);
		}

		public void SetMovementLock(bool lockInput, object source)
		{
			SetMovementLock(lockInput, lockInput, source);
		}

		public void SetMovementLock(bool lockInput, bool stopMovement, object source)
		{
			if (lockInput)
			{
				movementLocks.Add(source);
			}
			else
			{
				movementLocks.Remove(source);
			}
			movementLocked = movementLocks.Count > 0;
			if (movementLocked && stopMovement)
			{
				Motor.SetMovementDirection(Vector3.zero);
			}
			if (aimController != null)
			{
				aimController.LockInput(lockInput);
			}
		}

		public void SetAbilityLock(bool lockInput, object source)
		{
			if (lockInput)
			{
				abilityLocks.Add(source);
			}
			else
			{
				abilityLocks.Remove(source);
			}
			abilitiesLocked = abilityLocks.Count > 0;
			if (weaponController != null)
			{
				weaponController.SetLaserActive(!lockInput, source);
			}
		}

		private void UpdateMovement()
		{
			Vector3 vector = control.GetVector(InputAction.Movement);
			Motor.SetMovementDirection(vector.normalized);
		}

		private void UpdateRotation()
		{
			Vector3 vector = control.GetVector(InputAction.Rotation);
			Motor.SetFacingDirection(vector);
		}

		private void UpdateFire()
		{
			if (control.GetBool(InputAction.Fire))
			{
				if (weaponController.CanAttack())
				{
					weaponController.Attack();
				}
				else
				{
					weaponController.TriggerReloadIfNeeded(true);
				}
			}
			else if (control.GetBool(InputAction.FireSecondary))
			{
				if (weaponController.CanAttackSecondary())
				{
					weaponController.AttackSecondary();
				}
				else
				{
					weaponController.TriggerSecondaryReloadIfNeeded();
				}
			}
		}

		private void UpdateEquip()
		{
			if (DeviceSwitchController.CurrentDeviceMode != DeviceMode.Touch && !movementLocked)
			{
				pistolDualInputAction.ProcessInput(control.GetBool(InputAction.InventoryPistol));
				primary1DualInputAction.ProcessInput(control.GetBool(InputAction.InventoryPrimary1));
				primary2DualInputAction.ProcessInput(control.GetBool(InputAction.InventoryPrimary2));
				previousWeaponDualInputAction.ProcessInput(control.GetBool(InputAction.PreviousWeapon));
				nextWeaponDualInputAction.ProcessInput(control.GetBool(InputAction.NextWeapon));
			}
			if (!movementLocked && control.GetBool(InputAction.InventoryMelee) && !weaponController.SetActiveWeapon(InventoryWeaponSlot.Melee))
			{
				PlayErrorSound();
			}
			if (DeviceSwitchController.CurrentDeviceMode != DeviceMode.Touch)
			{
				return;
			}
			if (control.GetBool(InputAction.DropPistolWeapon))
			{
				if (weaponController.CanDropWeapon(InventoryWeaponSlot.Pistol))
				{
					weaponController.DropWeaponBySlot(InventoryWeaponSlot.Pistol);
				}
			}
			else if (control.GetBool(InputAction.DropPrimary1Weapon))
			{
				if (weaponController.CanDropWeapon(InventoryWeaponSlot.Primary1))
				{
					weaponController.DropWeaponBySlot(InventoryWeaponSlot.Primary1);
				}
			}
			else if (control.GetBool(InputAction.DropPrimary2Weapon) && weaponController.CanDropWeapon(InventoryWeaponSlot.Primary2))
			{
				weaponController.DropWeaponBySlot(InventoryWeaponSlot.Primary2);
			}
			if (control.GetBool(InputAction.InventoryPistol) && !weaponController.SetActiveWeapon(InventoryWeaponSlot.Pistol))
			{
				PlayErrorSound();
			}
			if (control.GetBool(InputAction.InventoryPrimary1) && !weaponController.SetActiveWeapon(InventoryWeaponSlot.Primary1))
			{
				PlayErrorSound();
			}
			if (control.GetBool(InputAction.InventoryPrimary2) && !weaponController.SetActiveWeapon(InventoryWeaponSlot.Primary2))
			{
				PlayErrorSound();
			}
		}

		private void UpdateReload()
		{
			if (control.GetBool(InputAction.Reload) && weaponController.CanReload())
			{
				weaponController.Reload();
			}
		}

		private void UpdateAbilities()
		{
			if (control.GetBool(InputAction.Ability))
			{
				if (abilityController.CanCast(AbilitySlot.Perk))
				{
					abilityController.Cast(AbilitySlot.Perk);
					return;
				}
				PlayErrorSound();
			}
			if (control.GetBool(InputAction.Grenade))
			{
				if (abilityController.CanCast(AbilitySlot.Grenade))
				{
					abilityController.Cast(AbilitySlot.Grenade);
					return;
				}
				PlayErrorSound();
			}
			syringeDualInputAction.ProcessInput(control.GetBool(InputAction.Syringe));
			if (control.GetBool(InputAction.Dodge))
			{
				if (abilityController.CanCast(AbilitySlot.Dodge))
				{
					abilityController.Cast(AbilitySlot.Dodge);
				}
				else
				{
					PlayErrorSound();
				}
			}
		}

		private void UpdateAim()
		{
			if (!(aimController != null))
			{
				return;
			}
			bool @bool = control.GetBool(InputAction.Aim);
			bool flag = aimController.IsAiming();
			if (aimController.CanAim())
			{
				if (@bool && !flag)
				{
					aimController.SetActiveState(AimState.AimStateType.Locked);
				}
				else if (!@bool && flag)
				{
					aimController.SetActiveState(AimState.AimStateType.Free);
				}
			}
		}

		private void UpdateMisc()
		{
			if (!control.GetBool(InputAction.ThrowMoney))
			{
				return;
			}
			PickupManager instance = PickupManager.Instance;
			if (instance != null && instance.CanCreateMoneyPickup())
			{
				Soldier component = GetComponent<Soldier>();
				if (component != null && component.CanDropMoney())
				{
					component.DropMoney();
				}
			}
		}

		public void PlayErrorSound()
		{
			if (GuiInGameManager.Instance != null)
			{
				GuiInGameManager.Instance.PlaySound(GuiInGameManager.Sound.ShopPurchaseUnable);
			}
		}
	}
}
