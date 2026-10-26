using UnityEngine;

public class WeaponDisplay
{
	private class Inventory
	{
		public string Name;

		public InventoryWeaponSlot WeaponSlot;

		public AmmoDisplayType AmmoType;

		public bool HasAmmo;

		public string WeaponTexture;

		public WeaponSlotState WeaponState;
	}

	private class ActiveWeapon
	{
		public InventoryWeaponSlot WeaponSlot;

		public AmmoDisplayType AmmoType;

		public int CurrentAmmo;

		public int TotalAmmo;
	}

	private enum WeaponSlotState
	{
		Passive = 1,
		Selected = 2,
		Empty = 3,
		None = 4
	}

	private ActiveWeapon EquippedWeapon;

	private Inventory[] WeaponInventory;

	private GUIStyle weaponStatusStyle;

	private GUIStyle weaponStyle;

	private GUIStyle textStyle;

	private GUISkin weaponSkins;

	private Rect weaponRect;

	private Rect weaponAmmoRect;

	private Rect bgRect;

	private string weaponAmmoText;

	private Matrix4x4 guiMatrixBackup;

	public WeaponDisplay(GUISkin skin)
	{
		WeaponInventory = new Inventory[4];
		WeaponInventory[0] = new Inventory();
		WeaponInventory[0].WeaponSlot = InventoryWeaponSlot.Melee;
		WeaponInventory[0].WeaponState = WeaponSlotState.None;
		WeaponInventory[1] = new Inventory();
		WeaponInventory[1].WeaponSlot = InventoryWeaponSlot.Pistol;
		WeaponInventory[1].WeaponState = WeaponSlotState.None;
		WeaponInventory[2] = new Inventory();
		WeaponInventory[2].WeaponSlot = InventoryWeaponSlot.Primary1;
		WeaponInventory[2].WeaponState = WeaponSlotState.None;
		WeaponInventory[3] = new Inventory();
		WeaponInventory[3].WeaponSlot = InventoryWeaponSlot.Primary2;
		WeaponInventory[3].WeaponState = WeaponSlotState.None;
		weaponSkins = skin;
		weaponStatusStyle = skin.GetStyle("weaponBG");
		textStyle = skin.label;
	}

	public void SetSizeBasedOnResolution(Rect parentRect)
	{
		bgRect = ScaledRect.getRect(65f, 22f, weaponStatusStyle.fixedWidth, weaponStatusStyle.fixedHeight, Alignment.TopLeft);
		textStyle.fontSize = ScaledFont.getFontSize(14);
		weaponAmmoRect = ScaledRect.getRect(50f, 110f, 300f, 80f, Alignment.TopLeft);
	}

	public void Display()
	{
		GUI.DrawTexture(bgRect, weaponStatusStyle.normal.background);
		if (weaponStyle != null)
		{
			GUI.DrawTexture(bgRect, weaponStyle.normal.background);
		}
		DisplayWeaponAmmo();
		DisplayWeaponSlots();
	}

	private void DisplayWeaponSlots()
	{
		guiMatrixBackup = GUI.matrix;
		Vector2 pivotPoint = new Vector2(Mathf.Ceil(bgRect.x + bgRect.width / 2f), Mathf.Ceil(bgRect.y + bgRect.height / 2f));
		for (int i = 0; i < 4; i++)
		{
			GUIUtility.RotateAroundPivot((float)i * 90f, pivotPoint);
			if (WeaponInventory[i].WeaponState != WeaponSlotState.None)
			{
				if (WeaponInventory[i].HasAmmo)
				{
					GUI.DrawTexture(bgRect, getSlotStateTexture(WeaponInventory[i].WeaponState));
				}
				else
				{
					GUI.DrawTexture(bgRect, weaponStatusStyle.hover.background);
				}
			}
			GUI.matrix = guiMatrixBackup;
		}
	}

	private void DisplayWeaponAmmo()
	{
		GUI.Label(weaponAmmoRect, weaponAmmoText, textStyle);
	}

	private void SetAmmoText()
	{
		if (EquippedWeapon != null)
		{
			if (EquippedWeapon.AmmoType == AmmoDisplayType.Magazine)
			{
				weaponAmmoText = EquippedWeapon.CurrentAmmo + " | " + EquippedWeapon.TotalAmmo;
			}
			else if (EquippedWeapon.AmmoType == AmmoDisplayType.Shells)
			{
				weaponAmmoText = EquippedWeapon.CurrentAmmo + " | " + EquippedWeapon.TotalAmmo;
			}
			else if (EquippedWeapon.AmmoType == AmmoDisplayType.Total)
			{
				weaponAmmoText = EquippedWeapon.CurrentAmmo.ToString();
			}
			else
			{
				weaponAmmoText = string.Empty;
			}
		}
	}

	public void UpdateEquippedWeaponAmmo(int currentAmmo, int totalAmmo)
	{
		if (EquippedWeapon != null)
		{
			EquippedWeapon.CurrentAmmo = currentAmmo;
			EquippedWeapon.TotalAmmo = totalAmmo;
			SetAmmoText();
		}
	}

	public void UpdateAmmoStatusBySlot(InventoryWeaponSlot slot, bool hasAmmo)
	{
		Inventory inventory = FindInventoryWeaponBySlot(slot);
		inventory.HasAmmo = hasAmmo;
	}

	public void AddInventoryWeapon(InventoryWeaponSlot slot, Weapon newWeapon, StoredWeaponData storedWeapons)
	{
		Inventory inventory = FindInventoryWeaponBySlot(slot);
		inventory.AmmoType = newWeapon.AmmunitionDisplay;
		inventory.HasAmmo = true;
		inventory.WeaponState = WeaponSlotState.Passive;
		inventory.Name = newWeapon.Name;
		inventory.WeaponTexture = newWeapon.Image;
	}

	public void RemoveInventoryWeapon(InventoryWeaponSlot slot)
	{
		if (EquippedWeapon != null && EquippedWeapon.WeaponSlot == slot)
		{
			EquippedWeapon.WeaponSlot = InventoryWeaponSlot.None;
			EquippedWeapon.AmmoType = AmmoDisplayType.None;
			EquippedWeapon.CurrentAmmo = 0;
			EquippedWeapon.TotalAmmo = 0;
			SetAmmoText();
			SetActiveWeaponTexture(null);
		}
		Inventory inventory = FindInventoryWeaponBySlot(slot);
		inventory.Name = string.Empty;
		inventory.HasAmmo = false;
		inventory.WeaponState = WeaponSlotState.None;
		inventory.WeaponTexture = string.Empty;
		inventory.AmmoType = AmmoDisplayType.None;
	}

	public void OnWeaponEquip(InventoryWeaponSlot slot, Weapon equippedWeapon)
	{
		if (EquippedWeapon == null)
		{
			EquippedWeapon = new ActiveWeapon();
		}
		else
		{
			Inventory inventory = FindInventoryWeaponBySlot(EquippedWeapon.WeaponSlot);
			if (inventory != null)
			{
				inventory.WeaponState = WeaponSlotState.Passive;
			}
		}
		Inventory inventory2 = FindInventoryWeaponBySlot(slot);
		inventory2.WeaponState = WeaponSlotState.Selected;
		EquippedWeapon.WeaponSlot = inventory2.WeaponSlot;
		EquippedWeapon.AmmoType = inventory2.AmmoType;
		EquippedWeapon.TotalAmmo = equippedWeapon.SpareAmmunition;
		EquippedWeapon.CurrentAmmo = equippedWeapon.LoadedAmmunition;
		SetAmmoText();
		SetActiveWeaponTexture(inventory2.WeaponTexture);
		for (int i = 0; i < 4 && WeaponInventory[i].WeaponSlot != inventory2.WeaponSlot; i++)
		{
		}
	}

	private Inventory FindInventoryWeaponBySlot(InventoryWeaponSlot slot)
	{
		Inventory[] weaponInventory = WeaponInventory;
		foreach (Inventory inventory in weaponInventory)
		{
			if (inventory.WeaponSlot == slot)
			{
				return inventory;
			}
		}
		Debug.Log("<WeaponDisplay> Cannot find weapon in GUI: weapon inventory");
		return null;
	}

	private void SetActiveWeaponTexture(string textureStyle)
	{
		if (string.IsNullOrEmpty(textureStyle))
		{
			weaponStyle = null;
		}
		else
		{
			weaponStyle = weaponSkins.GetStyle(textureStyle);
		}
	}

	private Texture getSlotStateTexture(WeaponSlotState state)
	{
		switch (state)
		{
		case WeaponSlotState.Selected:
			return weaponStatusStyle.focused.background;
		case WeaponSlotState.Passive:
			return weaponStatusStyle.active.background;
		case WeaponSlotState.Empty:
			return weaponStatusStyle.hover.background;
		default:
			return weaponStatusStyle.onNormal.background;
		}
	}
}
