using UnityEngine;

public class MetaBonusesProjectileHelper : MonoBehaviour, IMetaDamageBonuses
{
	public GameObject WeaponPrefab;

	private IMetaDamageBonuses weaponPerkObject;

	float IMetaDamageBonuses.GetDamageMultiplier(Perk perk)
	{
		if (weaponPerkObject != null)
		{
			return weaponPerkObject.GetDamageMultiplier(perk);
		}
		return 1f;
	}

	float IMetaDamageBonuses.GetDamageMultiplier()
	{
		if (weaponPerkObject != null)
		{
			return weaponPerkObject.GetDamageMultiplier();
		}
		return 1f;
	}

	float IMetaDamageBonuses.GetHeadshotMultiplier(Perk perk)
	{
		if (weaponPerkObject != null)
		{
			return weaponPerkObject.GetHeadshotMultiplier(perk);
		}
		return 1f;
	}

	private void Awake()
	{
		if (WeaponPrefab != null)
		{
			Weapon component = WeaponPrefab.GetComponent<Weapon>();
			if (component != null)
			{
				weaponPerkObject = component;
			}
		}
	}
}
