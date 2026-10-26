using UnityEngine;

public class MetaBonusesGrenadeHelper : MonoBehaviour, IMetaDamageBonuses
{
	private PerkBonuses perkBonuses;

	float IMetaDamageBonuses.GetDamageMultiplier(Perk perk)
	{
		if (perkBonuses != null)
		{
			return perkBonuses.GrenadeDamageMultiplier(perk);
		}
		return 1f;
	}

	float IMetaDamageBonuses.GetDamageMultiplier()
	{
		return 1f;
	}

	float IMetaDamageBonuses.GetHeadshotMultiplier(Perk perk)
	{
		if (perkBonuses != null)
		{
			return perkBonuses.HeadshotDamageMultiplier(perk);
		}
		return 1f;
	}

	private void Awake()
	{
		perkBonuses = PerkBonuses.Instance;
	}
}
