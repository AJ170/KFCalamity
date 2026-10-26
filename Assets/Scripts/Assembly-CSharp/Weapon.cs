using EffectEvents;
using UnityEngine;

public class Weapon : EffectSystem, IMetaDamageBonuses
{
	public enum ShopDisplayModes
	{
		DontDisplay = 0,
		AlwaysUnlocked = 1,
		Unlockable = 2,
		OnlyIfUnlocked = 3
	}

	public string Name;

	public string Description;

	public WeaponType Type;

	public WeaponSlot Slot = WeaponSlot.Any;

	public Perk SoldierPerk = Perk.None;

	public string Image;

	public string TouchImage;

	public string EquipAnimation;

	public bool UseAmmunition;

	public AmmoType AmmunitionType;

	public int MagazineSize = 5;

	public int MaxTotalAmmunition = 10;

	public int StartingAmmunition = -1;

	public AmmoDisplayType AmmunitionDisplay = AmmoDisplayType.None;

	public int AmmunitionBoxRefill;

	public int AmmunitionConsumption = 1;

	public WeaponReloadType WeaponReloadType;

	public float ReloadTime;

	public float RoundCooldown = 1f;

	public bool RequireIronsight;

	public bool CanDrop;

	public float DropWeight = 1f;

	public int DropOnWaveShortGame;

	public int DropOnWaveNormalGame;

	public int DropOnWaveLongGame;

	public GameObject PickupPrefab;

	public ShopDisplayModes ShopDisplayMode;

	public int Price;

	public int AmmunitionPrice;

	public int AmmunitionBundleSize = 1;

	public WeaponOffhandModes OffhandMode;

	public GameObject PrimaryMesh;

	public GameObject SecondaryMesh;

	public Transform PrimaryMuzzleBone;

	public Transform SecondaryMuzzleBone;

	public SecondaryAttack SecondaryAttackModule;

	private int loadedAmmunition;

	private int spareAmmunition;

	private float activeRoundCooldown;

	private bool fireOffhand;

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

	public float ActiveRoundCooldown
	{
		get
		{
			return activeRoundCooldown;
		}
	}

	float IMetaDamageBonuses.GetDamageMultiplier(Perk perk)
	{
		PerkBonuses instance = PerkBonuses.Instance;
		if (instance != null)
		{
			if (PerkMatch(perk))
			{
				return instance.PerkWeaponDamageMultiplier(perk);
			}
			if (SlotMatch(WeaponSlot.Pistol))
			{
				if (IsDual())
				{
					return instance.DualPistolDamageMultiplier(perk);
				}
				return instance.PistolDamageMultiplier(perk);
			}
			if (NameMatch("Knife"))
			{
				return instance.KnifeDamageMultiplier(perk);
			}
		}
		return 1f;
	}

	float IMetaDamageBonuses.GetDamageMultiplier()
	{
		WeaponBonuses instance = WeaponBonuses.Instance;
		if (instance != null)
		{
			return instance.WeaponDamageMultiplier(Name);
		}
		return 1f;
	}

	float IMetaDamageBonuses.GetHeadshotMultiplier(Perk perk)
	{
		PerkBonuses instance = PerkBonuses.Instance;
		if (instance != null)
		{
			return instance.HeadshotDamageMultiplier(perk);
		}
		return 1f;
	}

	protected override void Awake()
	{
		systemType = EffectSystemType.Weapon;
		SetupAmmunition();
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

	public bool CanAttackSecondary()
	{
		bool result = false;
		if (SecondaryAttackModule != null && (!SecondaryAttackModule.ObeyPrimaryRoundCooldown || IsRoundCooldownReady()) && (!SecondaryAttackModule.UsePrimaryAmmunition || HasAmmunitionLoaded()) && SecondaryAttackModule.CanAttack())
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

	public void Attack(bool isAiming, bool isAimingAtHead, GameObject aimTarget)
	{
		TriggerAimedAttackEvent(isAiming, isAimingAtHead, aimTarget);
		TriggerRoundCooldown();
		ConsumeAmmunition();
	}

	public void AttackSecondary()
	{
		if (SecondaryAttackModule != null)
		{
			SecondaryAttackModule.Attack();
			if (SecondaryAttackModule.AffectPrimaryRoundCooldown && activeRoundCooldown < SecondaryAttackModule.RoundCooldown)
			{
				TriggerRoundCooldown(SecondaryAttackModule.RoundCooldown);
			}
		}
	}

	private void TriggerAttackEvent()
	{
		if (OffhandMode == WeaponOffhandModes.OffhandWeapon)
		{
			if (fireOffhand)
			{
				TriggerInternal(new Attack("AttackOffhand"));
			}
			else
			{
				TriggerInternal(new Attack("AttackPrimary"));
			}
			fireOffhand = !fireOffhand;
		}
		else
		{
			TriggerInternal(new Attack());
		}
	}

	private void TriggerAimedAttackEvent(bool isAiming, bool isAimingAtHead, GameObject aimTarget)
	{
		if (OffhandMode == WeaponOffhandModes.OffhandWeapon)
		{
			if (fireOffhand)
			{
				TriggerInternal(new Attack("AttackOffhand", isAiming, isAimingAtHead, aimTarget));
			}
			else
			{
				TriggerInternal(new Attack("AttackPrimary", isAiming, isAimingAtHead, aimTarget));
			}
			fireOffhand = !fireOffhand;
		}
		else
		{
			TriggerInternal(new Attack(isAiming, isAimingAtHead, aimTarget));
		}
	}

	private void SetupAmmunition()
	{
		if (UseAmmunition)
		{
			loadedAmmunition = MagazineSize;
			spareAmmunition = MaxTotalAmmunition - MagazineSize;
		}
	}

	public void ConsumeAmmunition()
	{
		if (UseAmmunition)
		{
			loadedAmmunition -= AmmunitionConsumption;
			SaveWeaponData();
		}
	}

	public void ConsumeAmmunition(int consumed)
	{
		if (UseAmmunition)
		{
			loadedAmmunition -= consumed;
			SaveWeaponData();
		}
	}

	public void RestoreAmmunition(int restore)
	{
		if (UseAmmunition)
		{
			loadedAmmunition = Mathf.Min(loadedAmmunition + restore, MaxTotalAmmunition);
			SaveWeaponData();
		}
	}

	public bool HasAmmunitionLoaded()
	{
		return loadedAmmunition >= AmmunitionConsumption;
	}

	public bool HasSpareAmmunition()
	{
		return spareAmmunition > 0;
	}

	public bool CanReload()
	{
		bool result = false;
		if (UseAmmunition && HasSpareAmmunition() && loadedAmmunition < MagazineSize)
		{
			result = true;
		}
		return result;
	}

	public bool NeedsReload()
	{
		bool result = false;
		if (UseAmmunition && loadedAmmunition == 0)
		{
			result = true;
		}
		return result;
	}

	public void ReloadMagazine()
	{
		ReloadAmmunition(MagazineSize);
	}

	public void ReloadAmmunition(int count)
	{
		int num = MagazineSize - loadedAmmunition;
		int num2 = Mathf.Min(num, spareAmmunition, count);
		loadedAmmunition += num2;
		spareAmmunition -= num2;
		SaveWeaponData();
	}

	public int LoadedAmmunitionDeficit()
	{
		int a = MagazineSize - loadedAmmunition;
		return Mathf.Min(a, spareAmmunition);
	}

	public int TotalAmmunitionDeficit()
	{
		return Mathf.Max(MaxTotalAmmunition - TotalAmmunition, 0);
	}

	public void RefillSpareAmmunition()
	{
		if (UseAmmunition)
		{
			spareAmmunition = MaxTotalAmmunition;
			SaveWeaponData();
		}
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
		TriggerRoundCooldown(RoundCooldown);
	}

	public void TriggerRoundCooldown(float cooldown)
	{
		activeRoundCooldown = cooldown;
	}

	public bool IsRoundCooldownReady()
	{
		return activeRoundCooldown <= 0f;
	}

	public void SetInitialStoredAmmunition(StoredWeaponData weaponData)
	{
		if (UseAmmunition)
		{
			int num = ((StartingAmmunition <= -1) ? MaxTotalAmmunition : StartingAmmunition);
			int num2 = Mathf.Min(num, MagazineSize);
			int num3 = Mathf.Max(num - num2, 0);
			weaponData.LoadedAmmunition = num2;
			weaponData.SpareAmmunition = num3;
		}
		if (SecondaryAttackModule != null)
		{
			SecondaryAttackModule.SetInitialStoredAmmunition(weaponData);
		}
	}

	public void SetWeaponData(StoredWeaponData weaponData)
	{
		this.weaponData = weaponData;
		if (SecondaryAttackModule != null)
		{
			SecondaryAttackModule.SetWeaponData(weaponData);
		}
		LoadWeaponData();
	}

	public void SaveWeaponData()
	{
		if (UseAmmunition && weaponData != null)
		{
			weaponData.LoadedAmmunition = loadedAmmunition;
			weaponData.SpareAmmunition = spareAmmunition;
		}
		if (SecondaryAttackModule != null)
		{
			SecondaryAttackModule.SaveWeaponData();
		}
	}

	public void LoadWeaponData()
	{
		if (UseAmmunition && weaponData != null)
		{
			loadedAmmunition = weaponData.LoadedAmmunition;
			spareAmmunition = weaponData.SpareAmmunition;
		}
		if (SecondaryAttackModule != null)
		{
			SecondaryAttackModule.LoadWeaponData();
		}
	}

	public int CalculatePurchaseCost(GameObject parentSoldier)
	{
		Soldier component = parentSoldier.GetComponent<Soldier>();
		if (component != null && PerkMatch(component.Perk))
		{
			return Mathf.RoundToInt((float)Price * component.PerkWeaponDiscountMultiplier());
		}
		return Price;
	}

	public int CalculateBundleAmmunitionCost(GameObject parentSoldier)
	{
		int result = 0;
		if (UseAmmunition)
		{
			result = DiscountCost(parentSoldier, AmmunitionPrice);
		}
		return result;
	}

	public int CalculateBundleAmmunitionCost(GameObject parentSoldier, StoredWeaponData data)
	{
		int result = 0;
		if (UseAmmunition && data.TotalAmmunitionDeficit() > 0)
		{
			result = DiscountCost(parentSoldier, AmmunitionPrice);
		}
		return result;
	}

	public int CalculateTotalAmmunitionCost(GameObject parentSoldier, StoredWeaponData data)
	{
		int result = 0;
		if (UseAmmunition)
		{
			int num = MissingAmmunitionBundles(data);
			result = num * DiscountCost(parentSoldier, AmmunitionPrice);
		}
		return result;
	}

	public int MissingAmmunitionBundles(StoredWeaponData data)
	{
		int result = 0;
		int num = data.TotalAmmunitionDeficit();
		if (UseAmmunition && num > 0)
		{
			result = Mathf.CeilToInt((float)num / (float)AmmunitionBundleSize);
		}
		return result;
	}

	public int CalculateSellPrice(GameObject parentSoldier)
	{
		int purchasePrice = weaponData.PurchasePrice;
		purchasePrice = ((purchasePrice <= 0) ? CalculatePurchaseCost(parentSoldier) : purchasePrice);
		return purchasePrice / 2;
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

	public bool HasSecondaryModule()
	{
		return SecondaryAttackModule != null;
	}

	public bool IsDual()
	{
		return OffhandMode == WeaponOffhandModes.OffhandWeapon;
	}

	public bool PerkMatch(Perk perk)
	{
		return SoldierPerk == perk;
	}

	public bool NameMatch(string weaponName)
	{
		return Name == weaponName;
	}

	public bool SlotMatch(WeaponSlot weaponSlot)
	{
		return Slot == weaponSlot;
	}

	public int DropOnWave(GameLengthSetting gameLength)
	{
		switch (gameLength)
		{
		case GameLengthSetting.Short:
			return DropOnWaveShortGame;
		case GameLengthSetting.Normal:
			return DropOnWaveNormalGame;
		case GameLengthSetting.Long:
			return DropOnWaveLongGame;
		default:
			return 0;
		}
	}
}
