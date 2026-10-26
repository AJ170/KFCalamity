using UnityEngine;

public class MetaBonusesAbilityHelper : MonoBehaviour, IMetaDamageBonuses
{
	private PerkBonuses perkBonuses;

	float IMetaDamageBonuses.GetDamageMultiplier(Perk perk)
	{
		if (perkBonuses != null)
		{
			return perkBonuses.PerkAbilityDamageMultiplier(perk);
		}
		return 1f;
	}

	float IMetaDamageBonuses.GetDamageMultiplier()
	{
		return 1f;
	}

	float IMetaDamageBonuses.GetHeadshotMultiplier(Perk perk)
	{
		return 1f;
	}

	private void Awake()
	{
		perkBonuses = PerkBonuses.Instance;
	}
}
