public class DamageBonuses
{
	public static float Multiplier(ArmorType armor, DamageType damage)
	{
		int type = Clamp(0, 7, (int)damage);
		switch (armor)
		{
		case ArmorType.Berserker:
			return BerserkerArmor(type);
		case ArmorType.Bloat:
			return BloatArmor(type);
		case ArmorType.Clot:
			return ClotArmor(type);
		case ArmorType.Commando:
			return CommandoArmor(type);
		case ArmorType.Crawler:
			return CrawlerArmor(type);
		case ArmorType.Demolition:
			return DemolitionArmor(type);
		case ArmorType.Device:
			return DeviceArmor(type);
		case ArmorType.Firebug:
			return FirebugArmor(type);
		case ArmorType.Fleshpound:
			return FleshpoundArmor(type);
		case ArmorType.Gorefast:
			return GorefastArmor(type);
		case ArmorType.Husk:
			return HuskArmor(type);
		case ArmorType.Medic:
			return MedicArmor(type);
		case ArmorType.Patriarch:
			return PatriarchArmor(type);
		case ArmorType.Scrake:
			return ScrakeArmor(type);
		case ArmorType.Sharpshooter:
			return SharpshooterArmor(type);
		case ArmorType.Siren:
			return SirenArmor(type);
		case ArmorType.Stalker:
			return StalkerArmor(type);
		case ArmorType.SupportSpecialist:
			return SupportSpecialistArmor(type);
		case ArmorType.DestructableObjects:
			return DestructableObjectsArmor(type);
		default:
			return 1f;
		}
	}

	public static float ArmorDamageMultiplier(DamageType damage)
	{
		if (damage == DamageType.Bile)
		{
			return 2f;
		}
		return 1f;
	}

	public static float BerserkerArmor(int type)
	{
		float[] array = new float[8] { 1f, 0.25f, 1f, 1f, 0.5f, 1f, 1f, 1f };
		return array[type];
	}

	public static float CommandoArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f };
		return array[type];
	}

	public static float DemolitionArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 0.25f, 1f, 1f, 1f, 0.25f, 1f };
		return array[type];
	}

	public static float FirebugArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 0.1f, 1f, 1f, 1f, 1f };
		return array[type];
	}

	public static float MedicArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
		return array[type];
	}

	public static float SharpshooterArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
		return array[type];
	}

	public static float SupportSpecialistArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
		return array[type];
	}

	public static float ClotArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
		return array[type];
	}

	public static float GorefastArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 1f, 0.75f, 1f, 1f, 1f };
		return array[type];
	}

	public static float BloatArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 2f, 1f, 1f, 1f, 1f };
		return array[type];
	}

	public static float HuskArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 0.25f, 1f, 1f, 1f, 1f };
		return array[type];
	}

	public static float CrawlerArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
		return array[type];
	}

	public static float StalkerArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
		return array[type];
	}

	public static float SirenArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
		return array[type];
	}

	public static float ScrakeArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
		return array[type];
	}

	public static float FleshpoundArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 2f, 1f, 0.75f, 0.5f, 2f, 1f };
		return array[type];
	}

	public static float PatriarchArmor(int type)
	{
		float[] array = new float[8] { 1f, 0.5f, 0.5f, 0.75f, 0.5f, 0.5f, 0.5f, 1f };
		return array[type];
	}

	public static float DeviceArmor(int type)
	{
		float[] array = new float[8] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
		return array[type];
	}

	public static float DestructableObjectsArmor(int type)
	{
		float[] array = new float[8] { 1f, 0f, 1f, 0.25f, 0.25f, 0.25f, 3f, 10f };
		return array[type];
	}

	public static int Clamp(int min, int max, int value)
	{
		if (value > max)
		{
			value = max;
		}
		else if (value < min)
		{
			value = min;
		}
		return value;
	}
}
