using UnityEngine;

public class Zed : MonoBehaviour
{
	public ZedType Type = (ZedType)(-1);

	public int Bounty = 7;

	public int Experience = 7;

	public float WeaponDropChance;

	private void Awake()
	{
	}

	private void Start()
	{
		ZedTimeManager.RegisterZedDeathReceiver(base.gameObject);
	}

	private void ScaleHealthAttribute()
	{
		AttributeModifier attributeModifier = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Health, base.gameObject);
		if (attributeModifier != null)
		{
			attributeModifier.Alter(this, HealthMultiplierByWave());
		}
	}

	private float HealthMultiplierByWave()
	{
		WaveManager instance = WaveManager.Instance;
		if (instance != null)
		{
			int num = instance.NumWaves();
			int num2 = instance.CurrentWaveIndex() + 4;
			if (num >= num2 && num2 >= 2)
			{
				return 1f + (float)(num2 - 1) / (float)(num - 1) * 0.5f;
			}
		}
		return 1f;
	}
}
