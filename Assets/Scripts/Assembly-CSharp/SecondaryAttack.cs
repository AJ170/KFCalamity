using EffectEvents;
using UnityEngine;

public class SecondaryAttack : EffectSystem
{
	public float RoundCooldown = 1f;

	public bool ObeyPrimaryRoundCooldown = true;

	public bool AffectPrimaryRoundCooldown;

	public bool UseAmmunition;

	public int MaxAmmunition = 10;

	public int StartingAmmunition = -1;

	public int AmmunitionPrice;

	public int AmmunitionBundleSize = 1;

	public int AmmunitionBoxRefill;

	public bool UseReload;

	public float ReloadTime = 1f;

	public bool UsePrimaryAmmunition;

	private float activeRoundCooldown;

	private int loadedAmmunition;

	private int spareAmmunition;

	private StoredWeaponData weaponData;

	public int LoadedAmmunition
	{
		get
		{
			return loadedAmmunition;
		}
	}

	public int SpareAmmunition
	{
		get
		{
			return spareAmmunition;
		}
	}

	public int TotalAmmunition
	{
		get
		{
			return loadedAmmunition + spareAmmunition;
		}
	}

	private void Update()
	{
		UpdateRoundCooldown();
	}

	public bool CanAttack()
	{
		bool result = false;
		if (IsRoundCooldownReady() && (!UseAmmunition || HasAmmunitionLoaded()))
		{
			result = true;
		}
		return result;
	}

	public void Attack()
	{
		TriggerAttackEvent();
		TriggerRoundCooldown();
		ConsumeAmmunition();
	}

	private void TriggerAttackEvent()
	{
		TriggerInternal(new Attack());
	}

	private bool HasAmmunition()
	{
		return TotalAmmunition > 0;
	}

	private bool HasAmmunitionLoaded()
	{
		return loadedAmmunition > 0;
	}

	private bool HasSpareAmmunition()
	{
		return spareAmmunition > 0;
	}

	public void ConsumeAmmunition()
	{
		if (UseAmmunition)
		{
			loadedAmmunition--;
			SaveWeaponData();
		}
	}

	public void FillAmmunition(int amount = 1)
	{
		if (UseAmmunition)
		{
			loadedAmmunition += amount;
			SaveWeaponData();
		}
	}

	public bool CanReload()
	{
		return UseAmmunition && UseReload && HasSpareAmmunition();
	}

	public bool NeedsReload()
	{
		return UseAmmunition && UseReload && loadedAmmunition == 0;
	}

	public void Reload()
	{
		loadedAmmunition = 1;
		spareAmmunition--;
		SaveWeaponData();
	}

	private void UpdateRoundCooldown()
	{
		if (activeRoundCooldown > 0f)
		{
			activeRoundCooldown = Mathf.Max(activeRoundCooldown - Time.deltaTime, 0f);
		}
	}

	public void TriggerRoundCooldown()
	{
		activeRoundCooldown = RoundCooldown;
	}

	public bool IsRoundCooldownReady()
	{
		return activeRoundCooldown <= 0f;
	}

	public void SetWeaponData(StoredWeaponData weaponData)
	{
		this.weaponData = weaponData;
	}

	public void SetInitialStoredAmmunition(StoredWeaponData weaponData)
	{
		if (UseAmmunition)
		{
			int num = ((StartingAmmunition <= -1) ? MaxAmmunition : StartingAmmunition);
			if (UseReload)
			{
				weaponData.SecondaryLoadedAmmunition = Mathf.Min(num, 1);
				weaponData.SecondarySpareAmmunition = Mathf.Max(num - weaponData.SecondaryLoadedAmmunition, 0);
			}
			else
			{
				weaponData.SecondaryLoadedAmmunition = num;
				weaponData.SecondarySpareAmmunition = 0;
			}
		}
	}

	public void SaveWeaponData()
	{
		if (UseAmmunition && weaponData != null)
		{
			weaponData.SecondaryLoadedAmmunition = loadedAmmunition;
			weaponData.SecondarySpareAmmunition = spareAmmunition;
		}
	}

	public void LoadWeaponData()
	{
		if (UseAmmunition && weaponData != null)
		{
			loadedAmmunition = weaponData.SecondaryLoadedAmmunition;
			spareAmmunition = weaponData.SecondarySpareAmmunition;
		}
	}

	public int CalculateBundleAmmunitionCost(GameObject parentSoldier, StoredWeaponData data)
	{
		int result = 0;
		if (UseAmmunition && data.SecondaryLoadedAmmunition < MaxAmmunition)
		{
			result = DiscountCost(parentSoldier, AmmunitionPrice);
		}
		return result;
	}

	public int MissingAmmunitionBundles(StoredWeaponData data)
	{
		int result = 0;
		int num = data.SecondaryAmmunitionDeficit();
		if (UseAmmunition && num > 0)
		{
			result = Mathf.CeilToInt((float)num / (float)AmmunitionBundleSize);
		}
		return result;
	}

	private int DiscountCost(GameObject parentSoldier, int cost)
	{
		Soldier component = parentSoldier.GetComponent<Soldier>();
		if (component != null)
		{
			cost = Mathf.RoundToInt((float)cost * component.AmmoDiscountMultiplier());
		}
		return cost;
	}
}
