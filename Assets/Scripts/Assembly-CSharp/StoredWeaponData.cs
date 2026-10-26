using UnityEngine;

public class StoredWeaponData
{
	public GameObject Prefab;

	public InventoryWeaponSlot Slot;

	public int LoadedAmmunition = -1;

	public int SpareAmmunition = -1;

	public int SecondaryLoadedAmmunition = -1;

	public int SecondarySpareAmmunition = -1;

	public Weapon Weapon;

	public int PurchasePrice;

	public int TotalAmmunition
	{
		get
		{
			return LoadedAmmunition + SpareAmmunition;
		}
	}

	public int SecondaryTotalAmmunition
	{
		get
		{
			return SecondaryLoadedAmmunition + SecondarySpareAmmunition;
		}
	}

	public StoredWeaponData(GameObject prefab, InventoryWeaponSlot slot)
	{
		Prefab = prefab;
		Slot = slot;
		Weapon = prefab.GetComponent<Weapon>();
		Weapon.SetWeaponData(this);
		Weapon.SetInitialStoredAmmunition(this);
	}

	public void RefillMagazine(int magCount = 1)
	{
		if (Weapon.MagazineSize < Weapon.MaxTotalAmmunition)
		{
			SpareAmmunition += Mathf.Min(TotalAmmunitionDeficit(), Weapon.AmmunitionBundleSize * magCount);
		}
		else
		{
			LoadedAmmunition += TotalAmmunitionDeficit();
		}
	}

	public void RefillTotal()
	{
		if (Weapon.MagazineSize < Weapon.MaxTotalAmmunition)
		{
			SpareAmmunition += TotalAmmunitionDeficit();
		}
		else
		{
			LoadedAmmunition += TotalAmmunitionDeficit();
		}
	}

	public void RefillSecondary(int magCount = 1)
	{
		if (Weapon.HasSecondaryModule())
		{
			int num = Mathf.Min(SecondaryAmmunitionDeficit(), Weapon.SecondaryAttackModule.AmmunitionBundleSize * magCount);
			if (Weapon.SecondaryAttackModule.UseReload)
			{
				SecondarySpareAmmunition += num;
			}
			else
			{
				SecondaryLoadedAmmunition += num;
			}
		}
	}

	public void RefillAmmunitionBox()
	{
		if (Weapon.MagazineSize < Weapon.MaxTotalAmmunition)
		{
			SpareAmmunition += Mathf.Min(TotalAmmunitionDeficit(), Weapon.AmmunitionBoxRefill);
		}
		else
		{
			LoadedAmmunition += Mathf.Min(TotalAmmunitionDeficit(), Weapon.AmmunitionBoxRefill);
		}
		if (Weapon.HasSecondaryModule())
		{
			int num = Mathf.Min(SecondaryAmmunitionDeficit(), Weapon.SecondaryAttackModule.AmmunitionBoxRefill);
			if (Weapon.SecondaryAttackModule.UseReload)
			{
				SecondarySpareAmmunition += num;
			}
			else
			{
				SecondaryLoadedAmmunition += num;
			}
		}
	}

	public bool AmmunitionFull()
	{
		bool flag = PrimaryAmmunitionFull();
		if (flag)
		{
			flag = SecondaryAmmunitionFull();
		}
		return flag;
	}

	public bool PrimaryAmmunitionFull()
	{
		if (Weapon.UseAmmunition)
		{
			return TotalAmmunition >= Weapon.MaxTotalAmmunition;
		}
		return true;
	}

	public bool SecondaryAmmunitionFull()
	{
		return !Weapon.HasSecondaryModule() || !Weapon.SecondaryAttackModule.UseAmmunition || SecondaryTotalAmmunition >= Weapon.SecondaryAttackModule.MaxAmmunition;
	}

	public int TotalAmmunitionDeficit()
	{
		return Mathf.Max(Weapon.MaxTotalAmmunition - TotalAmmunition, 0);
	}

	public int SecondaryAmmunitionDeficit()
	{
		int result = 0;
		if (Weapon.HasSecondaryModule())
		{
			result = Mathf.Max(Weapon.SecondaryAttackModule.MaxAmmunition - SecondaryTotalAmmunition, 0);
		}
		return result;
	}

	public bool HasAmmunition()
	{
		bool result = true;
		if (Weapon.UseAmmunition && LoadedAmmunition <= 0 && SpareAmmunition <= 0)
		{
			result = false;
		}
		return result;
	}
}
