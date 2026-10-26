using UnityEngine;

public class DamageText : MonoBehaviour
{
	public HUDText ScrollingText;

	public UIFollowTarget targetFollow;

	public GameObject FollowTargetChild;

	private Health HealthComponent;

	private bool isTriggeredDeath;

	private int damageStack;

	private DamageTextSetting settings;

	private Color currentTextColor;

	private Color healColor;

	private bool lineOfSight;

	private bool elementAdded;

	public bool InLineOfSight
	{
		get
		{
			return settings.IgnoreLineOfSight || lineOfSight;
		}
		set
		{
			lineOfSight = value;
			if (lineOfSight)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}
	}

	private void OnDestroy()
	{
		if (HealthComponent != null)
		{
			HealthComponent.OnDamageTakenSignal.RemoveReceiver(OnDamageTaken);
		}
	}

	private void Update()
	{
		if (elementAdded && !ScrollingText.isVisible)
		{
			elementAdded = false;
			FollowTargetChild.SetActive(false);
		}
		if (damageStack > 0)
		{
			SetHudText(damageStack);
		}
		else if (isTriggeredDeath && !ScrollingText.isVisible)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void SetSettings(DamageTextSetting damageSettings)
	{
		settings = damageSettings;
		healColor = settings.HealColor;
	}

	public void SetOffset(Vector3 offset)
	{
		targetFollow.PixelPositionOffest = offset;
	}

	public void SetHealthReference(Health unitHealthComponent)
	{
		HealthComponent = unitHealthComponent;
		HealthComponent.OnDamageTakenSignal.AddReceiver(OnDamageTaken);
		HealthComponent.OnHealedSignal.AddReceiver(OnHealthGained);
		targetFollow.target = unitHealthComponent.gameObject.transform;
	}

	public void SetCameraReferences(Camera mainCamera, Camera uiCamera)
	{
		targetFollow.uiCamera = uiCamera;
		targetFollow.gameCamera = mainCamera;
		targetFollow.enabled = true;
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void OnHealthGained(Health UnitHealthComponent, int amount, int rawAmount, float healingDealtMultiplier)
	{
		if (InLineOfSight)
		{
			Show();
		}
		else
		{
			Hide();
		}
		ScrollingText.Add(rawAmount.ToString(), healColor, 1f, "+");
		FollowTargetChild.SetActive(true);
		elementAdded = true;
	}

	public void OnDamageTaken(Health UnitHealthComponent, int damage, int rawDamage, float armorTypeMultiplier)
	{
		if (InLineOfSight)
		{
			Show();
		}
		else
		{
			Hide();
		}
		if (armorTypeMultiplier <= settings.WeakDamageThreshold)
		{
			currentTextColor = settings.WeakColor;
		}
		else if (armorTypeMultiplier >= settings.EffectiveDamageThreshold)
		{
			currentTextColor = settings.EffectiveColor;
		}
		else
		{
			currentTextColor = settings.NormalColor;
		}
		damageStack += rawDamage;
	}

	private void SetHudText(int damage)
	{
		ScrollingText.Add(damage.ToString(), currentTextColor, 1f);
		FollowTargetChild.SetActive(true);
		elementAdded = true;
		damageStack = 0;
	}

	public void TriggerDestroy()
	{
		targetFollow.TargetPosition = targetFollow.target.transform.position;
		if (!ScrollingText.isVisible)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			isTriggeredDeath = true;
		}
	}
}
