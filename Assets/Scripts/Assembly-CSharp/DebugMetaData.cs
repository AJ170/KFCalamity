using DebugHelpers;
using Meta;
using UnityEngine;

public class DebugMetaData : MonoBehaviour
{
	private MetaManager metaManager;

	public bool ApplyPerkSettings = true;

	public int OverridePerkLevel;

	public PerkInfo[] Perks;

	public bool ApplyWeaponSettings = true;

	public int OverrideWeaponLevel;

	public WeaponInfo[] Weapons;

	private void Start()
	{
		metaManager = MetaManager.Instance;
		if (ApplyPerkSettings)
		{
			ForceAllPerksToLevel();
		}
		if (ApplyWeaponSettings)
		{
			ForceAllWeaponsToLevel();
		}
	}

	private void ForceAllPerksToLevel()
	{
		PerkInfo[] perks = Perks;
		foreach (PerkInfo perkInfo in perks)
		{
			SetPerkData(perkInfo.Perk, (OverridePerkLevel <= 0) ? perkInfo.Level : OverridePerkLevel);
		}
	}

	private void SetPerkData(Perk perk, int level)
	{
		if (metaManager != null && perk != Perk.None)
		{
			Meta.PerkData perkData = metaManager.CreateOrGetPerkData(perk);
			int experience = metaManager.PerkExperienceByLevel(level);
			perkData.SetData(level, experience);
		}
	}

	private void ForceAllWeaponsToLevel()
	{
		WeaponInfo[] weapons = Weapons;
		foreach (WeaponInfo weaponInfo in weapons)
		{
			SetWeaponData(weaponInfo.Weapon.name, (OverrideWeaponLevel <= 0) ? weaponInfo.Level : OverrideWeaponLevel);
		}
	}

	private void SetWeaponData(string weapon, int level)
	{
		if (metaManager != null && !string.IsNullOrEmpty(weapon))
		{
			WeaponData weaponData = metaManager.CreateOrGetWeaponData(weapon);
			weaponData.SetData(level, metaManager.WeaponExperienceByLevel(level));
		}
	}
}
