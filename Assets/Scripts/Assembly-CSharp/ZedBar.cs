using UnityEngine;

public class ZedBar : MonoBehaviour
{
	public HealthBar HealthBar;

	public UIFollowTarget FollowTarget;

	private Health healthComponent;

	private float maxHealth = -1f;

	private bool lineOfSight;

	private bool takenDamage;

	public bool InLineOfSight
	{
		get
		{
			return lineOfSight;
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

	private void Start()
	{
	}

	public void SetComponentSettings(GameObject target, Health health)
	{
		if (target != null && health != null)
		{
			healthComponent = health;
			healthComponent.OnHealthUpdateSignal.AddReceiver(OnHealthUpdate);
			FollowTarget.target = target.transform;
			HealthBar.SetBarStartValue(healthComponent.CurrentHealth / healthComponent.MaxHealth);
			FollowTarget.enabled = true;
		}
	}

	public void InitializeCameras(Camera uiCamera, Camera gameCamera)
	{
		FollowTarget.uiCamera = uiCamera;
		FollowTarget.gameCamera = gameCamera;
	}

	public void Show()
	{
		if (takenDamage)
		{
			base.gameObject.SetActive(true);
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void OnHealthUpdate(Health unitHealthComponent)
	{
		takenDamage = true;
		if (InLineOfSight)
		{
			Show();
		}
		int num = unitHealthComponent.CurrentHealth;
		if (num < 0)
		{
			num = 0;
		}
		if (maxHealth <= -1f)
		{
			maxHealth = unitHealthComponent.MaxHealth;
		}
		float barTarget = (float)num / maxHealth;
		HealthBar.SetBarTarget(barTarget);
	}
}
