using System.Collections.Generic;
using UnityEngine;

public class WeaponWidget : MonoBehaviour
{
	private const string SPRITE_WEAPON_SLOT_PASSIVE = "hud_swipe_dot_unselected";

	private const string SPRITE_WEAPON_SLOT_NO_AMMO = "hud_swipe_dot_no_ammo";

	private const string SPRITE_WEAPON_SLOT_ACTIVE = "hud_swipe_dot_selected";

	public const string SLOT_TEXTURE_PREFIX_ACTIVE = "_white";

	public const string SLOT_TEXTURE_PREFIX_PASSIVE = "_gray";

	public const string SLOT_TEXTURE_PREFIX_EMPTY = "_red";

	public WeaponList WeaponList;

	private WeaponButton[] weaponButtons;

	private Stack<UISprite> unusedListItems = new Stack<UISprite>();

	private List<UISprite> usedListItems = new List<UISprite>();

	private Dictionary<InventoryWeaponSlot, PlayerHudBase.InventoryWeapon> inventoryWeapons;

	private InventoryWeaponSlot equippedSlot;

	private PlayerWeaponController playerWeaponController;

	public void Initialize(Dictionary<InventoryWeaponSlot, PlayerHudBase.InventoryWeapon> inventoryWeapons, WeaponButton[] weaponButtons)
	{
		this.weaponButtons = weaponButtons;
		this.inventoryWeapons = inventoryWeapons;
		WeaponList.SetInventoryWeapons(inventoryWeapons);
	}

	public void OnWeaponAdded(InventoryWeaponSlot slot, Weapon newWeapon)
	{
		WeaponButton weaponButton = GetWeaponButton(slot);
		AddToList(slot, newWeapon);
	}

	public void OnWeaponRemoved(InventoryWeaponSlot slot)
	{
		WeaponList.RemoveWeapon(slot);
	}

	public void UpdateAmmo()
	{
		WeaponList.UpdateAmmo();
	}

	public void OnWeaponEquipped(InventoryWeaponSlot slot, Weapon newEquippedWeapon)
	{
		equippedSlot = slot;
		WeaponList.OnWeaponEquipped(slot);
	}

	public void OnUpdateAmmunitionBySlot(InventoryWeaponSlot slot)
	{
		WeaponButton weaponButton = GetWeaponButton(slot);
		PlayerHudBase.InventoryWeapon inventoryWeapon = inventoryWeapons[slot];
		weaponButton.IconSprite.alpha = 1f;
		if (slot == equippedSlot)
		{
			WeaponList.UpdateAmmo();
		}
	}

	public void OnNoWeapon(InventoryWeaponSlot slot)
	{
		WeaponButton weaponButton = GetWeaponButton(slot);
		weaponButton.IconSprite.spriteName = "hud_swipe_dot_unselected";
		weaponButton.IconSprite.alpha = 0.3f;
	}

	public void OnWeaponSlotStateChanged(PlayerHudBase.WeaponSlotState slotState, InventoryWeaponSlot slot, PlayerHudBase.InventoryWeapon weapon)
	{
		WeaponButton weaponButton = weaponButtons[(int)slot];
		PlayerHudBase.InventoryWeapon inventoryWeapon = inventoryWeapons[slot];
		if (slotState == PlayerHudBase.WeaponSlotState.Selected)
		{
			weaponButton.IconSprite.spriteName = "hud_swipe_dot_selected";
		}
		else if (slotState == PlayerHudBase.WeaponSlotState.Passive && inventoryWeapon.HasAmmo)
		{
			weaponButton.IconSprite.spriteName = "hud_swipe_dot_unselected";
		}
		else if (slotState == PlayerHudBase.WeaponSlotState.Passive && inventoryWeapon.UseAmmunition && !inventoryWeapon.HasAmmo)
		{
			weaponButton.IconSprite.spriteName = "hud_swipe_dot_no_ammo";
		}
		else if (slotState == PlayerHudBase.WeaponSlotState.None)
		{
			OnNoWeapon(slot);
		}
		if (slotState != PlayerHudBase.WeaponSlotState.None)
		{
			weaponButton.IconSprite.alpha = 1f;
		}
	}

	private WeaponButton GetWeaponButton(InventoryWeaponSlot slot)
	{
		return weaponButtons[(int)slot];
	}

	private void AddToList(InventoryWeaponSlot slot, Weapon newWeapon)
	{
		string spriteName = newWeapon.TouchImage + "_white";
		WeaponList.AddItem(spriteName, newWeapon, slot);
	}

	private void RemoveFromList(InventoryWeaponSlot slot)
	{
		WeaponList.RemoveWeapon(slot);
	}

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
