using System.Collections.Generic;
using Meta;
using UnityEngine;

public class PerkBonuses
{
	private static PerkBonuses instance;

	private MetaManager metaManager;

	private Dictionary<Perk, Meta.PerkData> perks = new Dictionary<Perk, Meta.PerkData>();

	public static PerkBonuses Instance
	{
		get
		{
			return instance;
		}
	}

	public PerkBonuses(MetaManager metaManager)
	{
		instance = this;
		this.metaManager = metaManager;
	}

	public void Shutdown()
	{
		instance = null;
	}

	public float PerkWeaponDamageMultiplier(Perk perk)
	{
		if (perk != Perk.None)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			float num = 0.05f;
			if (perk == Perk.Firebug)
			{
				num = 0.1f;
			}
			return 1f + (float)perkData.Level * num;
		}
		return 1f;
	}

	public float PerkAbilityDamageMultiplier(Perk perk)
	{
		if (perk != Perk.None)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			return 1f + (float)perkData.Level * 0.05f;
		}
		return 1f;
	}

	public float PistolDamageMultiplier(Perk perk)
	{
		if (perk == Perk.Sharpshooter)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			return 1f + (float)perkData.Level * 0.05f;
		}
		return 1f;
	}

	public float DualPistolDamageMultiplier(Perk perk)
	{
		if (perk == Perk.Commando)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			return 1f + (float)perkData.Level * 0.05f;
		}
		return 1f;
	}

	public float KnifeDamageMultiplier(Perk perk)
	{
		if (perk == Perk.Berserker)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			return 1f + (float)perkData.Level * 0.05f;
		}
		return 1f;
	}

	public float GrenadeDamageMultiplier(Perk perk)
	{
		if (perk != Perk.None)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			float num = 0.15f;
			if (perk == Perk.Demolition)
			{
				num = 0.35f;
			}
			return 1f + (float)perkData.Level * num;
		}
		return 1f;
	}

	public float HeadshotDamageMultiplier(Perk perk)
	{
		if (perk == Perk.Sharpshooter)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			return 1f + (float)perkData.Level * 0.025f;
		}
		return 1f;
	}

	public float DamageReduction(Perk perk)
	{
		if (perk == Perk.Berserker)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			return 1f - (float)perkData.Level * 0.025f;
		}
		return 1f;
	}

	public float PerkWeaponDiscountMultiplier(Perk perk)
	{
		if (perk != Perk.None)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			float num = 0.03f;
			if (perk == Perk.Medic)
			{
				num = 0.04f;
			}
			return 1f - (float)perkData.Level * num;
		}
		return 1f;
	}

	public float AmmoDiscountMultiplier(Perk perk)
	{
		if (perk == Perk.SupportSpecialist)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			return 1f - (float)perkData.Level * 0.04f;
		}
		return 1f;
	}

	public float GrenadeDiscountMultiplier(Perk perk)
	{
		if (perk == Perk.SupportSpecialist)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			return 1f - (float)perkData.Level * 0.04f;
		}
		return 1f;
	}

	public float ArmorDiscountMultiplier(Perk perk)
	{
		if (perk == Perk.SupportSpecialist)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			return 1f - (float)perkData.Level * 0.04f;
		}
		return 1f;
	}

	public float StealthDetectionMultiplier(Perk perk)
	{
		if (perk == Perk.Commando)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			return 1f + (float)perkData.Level * 0.05f;
		}
		return 1f;
	}

	public float SyringeHealMultiplier(Perk perk)
	{
		if (perk == Perk.Medic)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			return 1f + (float)perkData.Level * 0.1f;
		}
		return 1f;
	}

	public float ConstraintReductionMultiplier(Perk perk)
	{
		if (perk == Perk.Berserker)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			return Mathf.Max(1f - (float)perkData.Level * 0.05f, 0f);
		}
		return 1f;
	}

	public int ExtraSyringes(Perk perk)
	{
		if (perk == Perk.Medic)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			if (perkData.Level >= 10 && perkData.Level < 20)
			{
				return 1;
			}
			if (perkData.Level == 20)
			{
				return 2;
			}
		}
		return 0;
	}

	public int ExtraGrenades(Perk perk)
	{
		if (perk == Perk.Demolition)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			if (perkData.Level >= 10 && perkData.Level < 20)
			{
				return 1;
			}
			if (perkData.Level == 20)
			{
				return 2;
			}
		}
		return 0;
	}

	public int ExtraArmor(Perk perk)
	{
		if (perk == Perk.SupportSpecialist)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			if (perkData.Level >= 10 && perkData.Level < 20)
			{
				return 100;
			}
			if (perkData.Level == 20)
			{
				return 200;
			}
		}
		return 0;
	}

	public string StartingPistol(Perk perk)
	{
		if (perk == Perk.Sharpshooter || perk == Perk.Commando || perk == Perk.Firebug)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			if (perkData.Level >= 10 && perkData.Level < 20)
			{
				switch (perk)
				{
				case Perk.Sharpshooter:
					return "Revolver";
				case Perk.Commando:
					return "9mmDual";
				case Perk.Firebug:
					return "9mm_firebug";
				}
			}
			else if (perkData.Level == 20)
			{
				switch (perk)
				{
				case Perk.Sharpshooter:
					return "Handcannon";
				case Perk.Commando:
					return "HandcannonDual";
				case Perk.Firebug:
					return "Mac10";
				}
			}
		}
		return null;
	}

	public string StartingMeleeWeapon(Perk perk)
	{
		if (perk == Perk.Berserker)
		{
			Meta.PerkData perkData = GetPerkData(perk);
			if (perkData.Level >= 10 && perkData.Level < 20)
			{
				if (perk == Perk.Berserker)
				{
					return "Machete";
				}
			}
			else if (perkData.Level == 20 && perk == Perk.Berserker)
			{
				return "Axe";
			}
		}
		return null;
	}

	private Meta.PerkData GetPerkData(Perk perk)
	{
		if (perk != Perk.None)
		{
			if (perks.ContainsKey(perk))
			{
				return perks[perk];
			}
			return RegisterPerkData(perk);
		}
		return null;
	}

	private Meta.PerkData RegisterPerkData(Perk perk)
	{
		if (perk != Perk.None)
		{
			Meta.PerkData perkData = metaManager.CreateOrGetPerkData(perk);
			perks.Add(perk, perkData);
			return perkData;
		}
		return null;
	}
}
