using UnityEngine;

public class AmmoPickup : Pickup
{
	private void Awake()
	{
		type = PickupType.Ammo;
	}

	protected override void OnRespawn()
	{
		ShowPickupObject();
	}

	protected override void OnCollidePlayer(Collider collider)
	{
		GameObject gameObject = collider.gameObject;
		PlayerWeaponController playerWeaponController = WeaponController(gameObject);
		PlayerAbilityController controller = AbilityController(gameObject);
		bool flag = AnyWeaponNeedRefill(playerWeaponController);
		bool flag2 = GrenadeNeedRefill(controller);
		if (flag || flag2)
		{
			PlayPickupSound();
			PlayPickupParticle();
			if (flag)
			{
				RefillWeapons(playerWeaponController);
			}
			if (flag2)
			{
				RefillGrenades(controller);
			}
			PlayerGUILink playerGUILink = GUILinkByObject(gameObject);
			if (playerGUILink != null)
			{
				playerGUILink.OnPickup(base.Type);
			}
			playerWeaponController.TriggerReloadIfNeeded();
			if (Respawn())
			{
				HidePickupObject();
			}
			else
			{
				ForceDelete();
			}
		}
		else
		{
			OnFailedPickup(gameObject);
		}
	}

	private PlayerWeaponController WeaponController(GameObject go)
	{
		return go.GetComponent<PlayerWeaponController>();
	}

	private PlayerAbilityController AbilityController(GameObject go)
	{
		return go.GetComponent<PlayerAbilityController>();
	}

	private bool AnyWeaponNeedRefill(PlayerWeaponController controller)
	{
		if (controller != null)
		{
			return !controller.AmmunitionInSlotFull(InventoryWeaponSlot.Pistol) || !controller.AmmunitionInSlotFull(InventoryWeaponSlot.Primary1) || !controller.AmmunitionInSlotFull(InventoryWeaponSlot.Primary2);
		}
		return false;
	}

	private bool GrenadeNeedRefill(PlayerAbilityController controller)
	{
		if (controller != null)
		{
			Ability ability = controller.AbilityBySlot(AbilitySlot.Grenade);
			if (ability != null)
			{
				return !ability.AllChargesLeft();
			}
		}
		return false;
	}

	private void RefillWeapons(PlayerWeaponController controller)
	{
		if (controller != null)
		{
			if (controller.HasWeapon(InventoryWeaponSlot.Pistol))
			{
				controller.RefillAmmunitionBoxForSlot(InventoryWeaponSlot.Pistol);
			}
			if (controller.HasWeapon(InventoryWeaponSlot.Primary1))
			{
				controller.RefillAmmunitionBoxForSlot(InventoryWeaponSlot.Primary1);
			}
			if (controller.HasWeapon(InventoryWeaponSlot.Primary2))
			{
				controller.RefillAmmunitionBoxForSlot(InventoryWeaponSlot.Primary2);
			}
		}
	}

	private void RefillGrenades(PlayerAbilityController controller)
	{
		if (controller != null)
		{
			Ability ability = controller.AbilityBySlot(AbilitySlot.Grenade);
			if (ability != null)
			{
				ability.RefillCharges(1);
			}
		}
	}
}
