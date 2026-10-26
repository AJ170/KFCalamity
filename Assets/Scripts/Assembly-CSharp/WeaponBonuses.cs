using System.Collections.Generic;
using Meta;

public class WeaponBonuses
{
	private static WeaponBonuses instance;

	private MetaManager metaManager;

	private Dictionary<string, WeaponData> weapons = new Dictionary<string, WeaponData>();

	public static WeaponBonuses Instance
	{
		get
		{
			return instance;
		}
	}

	public WeaponBonuses(MetaManager metaManager)
	{
		instance = this;
		this.metaManager = metaManager;
	}

	public void Shutdown()
	{
		instance = null;
	}

	public float WeaponDamageMultiplier(string weaponName)
	{
		if (!string.IsNullOrEmpty(weaponName))
		{
			WeaponData weaponData = GetWeaponData(weaponName);
			int level = weaponData.Level;
			if (level > 1)
			{
				return 1f + (float)level * 0.05f;
			}
		}
		return 1f;
	}

	private WeaponData GetWeaponData(string weaponName)
	{
		if (!string.IsNullOrEmpty(weaponName))
		{
			if (weapons.ContainsKey(weaponName))
			{
				return weapons[weaponName];
			}
			return RegisterPerkData(weaponName);
		}
		return null;
	}

	private WeaponData RegisterPerkData(string weaponName)
	{
		if (!string.IsNullOrEmpty(weaponName))
		{
			WeaponData weaponData = metaManager.CreateOrGetWeaponData(weaponName);
			weapons.Add(weaponName, weaponData);
			return weaponData;
		}
		return null;
	}
}
