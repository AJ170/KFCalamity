using UnityEngine;

public class Health : MonoBehaviour
{
	public int MaxHealth = 100;

	public int HealthRegen;

	public ArmorType Armor;

	public int MaxArmor = 100;

	public float PiercingResistance = 1f;

	public bool DestroyOnDeath = true;

	public bool lesserInvulnerability;

	public HealthSignalSender1 OnHealthUpdateSignal;

	public HealthSignalSender1 OnArmorUpdateSignal;

	public HealthSignalSender1 OnDeathSignal;

	public HealthSignalSender2 OnDamageTakenSignal;

	public HealthSignalSender2 OnHealedSignal;

	public bool CanTriggerLowHealthSpeech;

	private int currentHealth = 100;

	private int currentArmor;

	private bool dead;

	private bool invulnerability;

	private BloodOverlay bloodOverlay;

	private bool killedByHeadshot;

	private GameObject killer;

	private int currentHealthRegen;

	private float healthRegenCounter;

	private AttributeModifier healthModifier;

	private AttributeModifier damageReceived;

	private AttributeModifier healingReceived;

	public int CurrentHealth
	{
		get
		{
			return currentHealth;
		}
	}

	public int CurrentHealthRegen
	{
		get
		{
			return currentHealthRegen;
		}
		set
		{
			currentHealthRegen = value;
		}
	}

	public int CurrentArmor
	{
		get
		{
			return currentArmor;
		}
	}

	public bool Dead
	{
		get
		{
			return dead;
		}
	}

	public bool Invulnerable
	{
		get
		{
			return invulnerability;
		}
		set
		{
			invulnerability = value;
		}
	}

	public bool LesserInvulnerability
	{
		get
		{
			return lesserInvulnerability;
		}
		set
		{
			lesserInvulnerability = value;
		}
	}

	public bool KilledByHeadshot
	{
		get
		{
			return killedByHeadshot;
		}
		set
		{
			killedByHeadshot = value;
		}
	}

	public GameObject Killer
	{
		get
		{
			return killer;
		}
		set
		{
			killer = value;
		}
	}

	private void Awake()
	{
		currentHealth = MaxHealth;
		bloodOverlay = GetComponent<BloodOverlay>();
	}

	private void Start()
	{
		healthModifier = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Health, base.gameObject);
		healthModifier.ForceRecalculate();
		damageReceived = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageReceived, base.gameObject);
		healingReceived = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.HealingReceived, base.gameObject);
		MaxHealth = (int)((float)MaxHealth * healthModifier.Multiplier);
		currentHealth = MaxHealth;
		currentHealthRegen = HealthRegen;
		OnDeathSignal.TriggerOnce = true;
	}

	private void Update()
	{
		if (currentHealth <= 0 && !dead)
		{
			OnDeathSignal.Send(this);
			base.gameObject.SendMessage("OnDeath", this, SendMessageOptions.DontRequireReceiver);
			if (DestroyOnDeath)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				dead = true;
			}
		}
		UpdateHealthRegen();
	}

	private void OnDestroy()
	{
		OnHealthUpdateSignal.Shutdown();
		OnArmorUpdateSignal.Shutdown();
		OnDeathSignal.Shutdown();
		OnDamageTakenSignal.Shutdown();
		OnHealedSignal.Shutdown();
		killer = null;
		healthModifier = null;
		damageReceived = null;
		healingReceived = null;
	}

	public int TakeDamage(int amount, DamageType damageType, float damageDealtMultiplier = 1f, bool ignoreArmor = false)
	{
		if (!invulnerability)
		{
			float num = amount;
			float num2 = DamageBonuses.Multiplier(Armor, damageType);
			num *= num2 * damageDealtMultiplier * damageReceived.Multiplier;
			amount = (int)num;
			int rawDamage = amount;
			int num3 = 0;
			if (!ignoreArmor && amount > 0 && currentArmor > 0)
			{
				int num4 = currentArmor;
				float num5 = DamageBonuses.ArmorDamageMultiplier(damageType);
				amount = (int)((float)amount * num5);
				currentArmor = Mathf.Max(num4 - amount, 0);
				num3 = num4 - currentArmor;
				amount = Mathf.Max(amount - num4, 0);
				SendOnArmorUpdateSignal();
				if (amount > 0)
				{
					amount = (int)((float)amount / num5);
				}
			}
			int num6 = 0;
			if (amount > 0 && currentHealth > 0)
			{
				int num7 = Mathf.Max(currentHealth - amount, lesserInvulnerability ? 1 : 0);
				num6 = currentHealth - num7;
				if (num6 > 0)
				{
					float num8 = (float)currentHealth / (float)MaxHealth;
					if (CanTriggerLowHealthSpeech && num7 > 0 && num8 > 0.5f)
					{
						num8 = (float)num7 / (float)MaxHealth;
						if (num8 < 0.5f)
						{
							SpeechManager.PlaySpeech(SpeechManager.SpeechLine.LowHealth, base.gameObject);
						}
					}
					if (bloodOverlay != null)
					{
						bloodOverlay.OnHit(Mathf.Max(0.2f, 1f - num8));
					}
					currentHealth = num7;
					SendOnHealthUpdateSignal();
				}
			}
			int num9 = num6 + num3;
			if (num9 > 0)
			{
				SendOnDamageTakenSignal(num9, rawDamage, num2);
			}
			return num9;
		}
		return 0;
	}

	public int RestoreHealth(int amount, float healingDealtMultiplier)
	{
		if (currentHealth != MaxHealth && currentHealth != 0)
		{
			float num = amount;
			num *= healingDealtMultiplier * healingReceived.Multiplier;
			amount = (int)num;
			return RestoreHealthExact(amount);
		}
		return 0;
	}

	public int RestoreHealthExact(int amount)
	{
		if (currentHealth != MaxHealth && currentHealth != 0)
		{
			int num = Mathf.Min(currentHealth + amount, MaxHealth);
			int num2 = num - currentHealth;
			if (num2 > 0)
			{
				currentHealth = num;
				SendOnHealthUpdateSignal();
				SendOnHealTakenSignal(num2, amount);
			}
			return num2;
		}
		return 0;
	}

	public void SetHealth(int amount)
	{
		if (amount > 0 && amount <= MaxHealth && !dead && !invulnerability)
		{
			currentHealth = amount;
			SendOnHealthUpdateSignal();
		}
	}

	public void ResetHealth()
	{
		if (currentHealth < MaxHealth)
		{
			currentHealth = MaxHealth;
			dead = false;
			SendOnHealthUpdateSignal();
		}
	}

	private void SendOnHealthUpdateSignal()
	{
		OnHealthUpdateSignal.Send(this);
	}

	private void SendOnArmorUpdateSignal()
	{
		OnArmorUpdateSignal.Send(this);
	}

	private void SendOnDamageTakenSignal(int damageTaken, int rawDamage, float armorTypeMultiplier)
	{
		OnDamageTakenSignal.Send(this, damageTaken, rawDamage, armorTypeMultiplier);
	}

	private void SendOnHealTakenSignal(int healAmount, int rawAmount)
	{
		OnHealedSignal.Send(this, healAmount, rawAmount, 1f);
	}

	private void UpdateHealthRegen()
	{
		if (currentHealthRegen > 0 && !dead && currentHealth < MaxHealth && currentHealth > 0)
		{
			healthRegenCounter = Mathf.Min(healthRegenCounter + Time.deltaTime, 1f);
			if (healthRegenCounter == 1f)
			{
				currentHealth = Mathf.Min(currentHealth + currentHealthRegen, MaxHealth);
				SendOnHealthUpdateSignal();
				healthRegenCounter = 0f;
			}
		}
	}

	public void RestoreHealthRegen()
	{
		currentHealthRegen = HealthRegen;
	}

	public void RefillArmor(int armor)
	{
		if (currentArmor != MaxArmor)
		{
			currentArmor = Mathf.Min(currentArmor + armor, MaxArmor);
			SendOnArmorUpdateSignal();
		}
	}

	public void Kill()
	{
		currentHealth = 0;
		currentHealthRegen = 0;
	}
}
