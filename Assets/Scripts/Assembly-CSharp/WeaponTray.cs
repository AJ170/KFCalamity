using UnityEngine;

public class WeaponTray : MonoBehaviour
{
	public UISprite MeleeSlot;

	public UISprite PistolSlot;

	public UISprite PrimarySlot1;

	public UISprite PrimarySlot2;

	private void Start()
	{
		MeleeSlot.spriteName = string.Empty;
		PistolSlot.spriteName = string.Empty;
		PrimarySlot1.spriteName = string.Empty;
		PrimarySlot2.spriteName = string.Empty;
	}

	private void Update()
	{
	}

	private UISprite GetSpriteFromSlot(InventoryWeaponSlot slot)
	{
		object result;
		switch (slot)
		{
		case InventoryWeaponSlot.Melee:
			result = MeleeSlot;
			break;
		case InventoryWeaponSlot.Pistol:
			result = PistolSlot;
			break;
		case InventoryWeaponSlot.Primary1:
			result = PrimarySlot1;
			break;
		case InventoryWeaponSlot.Primary2:
			result = PrimarySlot2;
			break;
		default:
			result = null;
			break;
		}
		return (UISprite)result;
	}

	public void EquipWeapon(InventoryWeaponSlot slot, Weapon newEquippedWeapon)
	{
	}
}
