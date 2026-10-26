using System.Collections.Generic;
using EventHelpers;
using UnityEngine;

public class WeaponDropManager
{
	private class WeaponData
	{
		private Weapon weapon;

		private float weight;

		public Weapon Weapon
		{
			get
			{
				return weapon;
			}
		}

		public float Weight
		{
			get
			{
				return weight;
			}
			set
			{
				weight = value;
			}
		}

		public WeaponData(Weapon weapon)
		{
			this.weapon = weapon;
			weight = weapon.DropWeight;
		}
	}

	public static WeaponDropManager instance;

	private List<WeaponData> droppableWeapons = new List<WeaponData>();

	private List<WeaponData> allowedWeapons = new List<WeaponData>();

	private bool allWeaponsUnlocked;

	private bool guaranteeBloatDrop = true;

	public static WeaponDropManager Instance
	{
		get
		{
			return instance;
		}
	}

	public WeaponDropManager()
	{
		instance = this;
		EventSystem.Instance.Subscribe(EventId.WeaponUnlocked, OnWeaponUnlocked, this);
		SetupWeaponDrops();
	}

	public void Shutdown()
	{
		EventSystem.Instance.Unsubscribe(this);
		instance = null;
	}

	private void SetupWeaponDrops()
	{
		OuyaPurchaseManager ouyaPurchaseManager = OuyaPurchaseManager.Instance;
		WeaponFactory weaponFactory = WeaponFactory.Instance;
		List<Weapon> list = weaponFactory.WeaponTemplates();
		foreach (Weapon item in list)
		{
			if (item != null && item.CanDrop && (!SpecialWeapon(item.ShopDisplayMode) || !(ouyaPurchaseManager != null) || ouyaPurchaseManager.GamePurchased) && item.DropWeight != 0f)
			{
				droppableWeapons.Add(new WeaponData(item));
			}
		}
		UpdateWeaponData();
	}

	private void OnWeaponUnlocked(EventBase e)
	{
		UpdateWeaponData();
	}

	private void UpdateWeaponData()
	{
		MetaManager metaManager = MetaManager.Instance;
		if (!(metaManager != null))
		{
			return;
		}
		allWeaponsUnlocked = true;
		foreach (WeaponData droppableWeapon in droppableWeapons)
		{
			if (!metaManager.IsWeaponUnlocked(droppableWeapon.Weapon.Name))
			{
				droppableWeapon.Weight = droppableWeapon.Weapon.DropWeight * 2f;
				if (!SpecialWeapon(droppableWeapon.Weapon.ShopDisplayMode))
				{
					allWeaponsUnlocked = false;
				}
			}
		}
	}

	public bool DropWeapon(Zed zed, out string weaponName)
	{
		weaponName = string.Empty;
		PickupManager pickupManager = PickupManager.Instance;
		if (zed != null && pickupManager != null && ShouldDropWeapon(zed))
		{
			Weapon weapon = null;
			weapon = ((!allWeaponsUnlocked && zed.Type == ZedType.Patriarch) ? WeaponToDropBoss() : WeaponToDrop(zed.Type));
			if (weapon != null)
			{
				weaponName = weapon.Name;
				Transform transform = zed.transform;
				WeaponPickup weaponPickup = pickupManager.CreateWeaponPickup(weapon.PickupPrefab, transform.root.position);
				if (weaponPickup != null)
				{
					Quaternion rotation = default(Quaternion);
					rotation.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
					weaponPickup.Rotate(rotation);
					if (zed.Type == ZedType.Bloat)
					{
						guaranteeBloatDrop = false;
					}
					return true;
				}
			}
			return false;
		}
		return false;
	}

	private bool ShouldDropWeapon(Zed zed)
	{
		if (zed != null && zed.WeaponDropChance > 0f)
		{
			if (zed.Type == ZedType.Bloat && guaranteeBloatDrop)
			{
				return true;
			}
			return Random01() <= zed.WeaponDropChance;
		}
		return false;
	}

	private Weapon WeaponToDrop(ZedType zedType)
	{
		int num = WaveManager.Instance.CurrentWaveIndex() + 1;
		GameLengthSetting gameLength = GameState.Instance.GameSettings.GameLength;
		float num2 = 0f;
		allowedWeapons.Clear();
		droppableWeapons.QuickShuffle();
		foreach (WeaponData droppableWeapon in droppableWeapons)
		{
			if (num >= droppableWeapon.Weapon.DropOnWave(gameLength) && (!guaranteeBloatDrop || zedType != ZedType.Bloat || droppableWeapon.Weapon.Slot != WeaponSlot.Pistol || !(droppableWeapon.Weapon.Name == "9mm")))
			{
				allowedWeapons.Add(droppableWeapon);
				num2 += droppableWeapon.Weight;
			}
		}
		return SelectAllowedWeapon(num2);
	}

	private Weapon WeaponToDropBoss()
	{
		MetaManager metaManager = MetaManager.Instance;
		if (metaManager != null)
		{
			float num = 0f;
			allowedWeapons.Clear();
			droppableWeapons.QuickShuffle();
			foreach (WeaponData droppableWeapon in droppableWeapons)
			{
				if (!metaManager.IsWeaponUnlocked(droppableWeapon.Weapon.Name) && !SpecialWeapon(droppableWeapon.Weapon.ShopDisplayMode))
				{
					allowedWeapons.Add(droppableWeapon);
					num += droppableWeapon.Weight;
				}
			}
			if (num > 0f)
			{
				return SelectAllowedWeapon(num);
			}
		}
		return WeaponToDrop((ZedType)(-1));
	}

	private Weapon SelectAllowedWeapon(float totalWeight)
	{
		if (totalWeight > 0f)
		{
			float num = Random.Range(0f, totalWeight);
			foreach (WeaponData allowedWeapon in allowedWeapons)
			{
				if (num <= allowedWeapon.Weight)
				{
					return allowedWeapon.Weapon;
				}
				num -= allowedWeapon.Weight;
			}
		}
		return null;
	}

	private float Random01()
	{
		return (float)Random.Range(0, 100) / 100f;
	}

	private bool SpecialWeapon(Weapon.ShopDisplayModes displayMode)
	{
		return displayMode == Weapon.ShopDisplayModes.OnlyIfUnlocked;
	}
}
