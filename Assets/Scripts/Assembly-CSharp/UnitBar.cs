using UnityEngine;

public class UnitBar : MonoBehaviour
{
	public Transform CombatText;

	public UILabel statusLabel;

	public HealthBar BarHealth;

	public ArmorBar BarArmor;

	public TimerBar BarReload;

	private UIFollowTarget followTarget;

	private Health healthScript;

	private bool reloading;

	private PlayerStatusText combatStatus;

	private CallbackItem<Health> onArmorUpdateCallback;

	private CallbackItem<Health> onHealthUpdateCallback;

	private float maxHealth = -1f;

	private float maxArmor = -1f;

	private void Start()
	{
		combatStatus = new PlayerStatusText();
	}

	private void Update()
	{
		if (combatStatus == null)
		{
			return;
		}
		if (combatStatus.HasStatus())
		{
			statusLabel.gameObject.SetActive(true);
			statusLabel.text = combatStatus.GetCurrentStatus();
			if (reloading && !BarReload.CalculateBar)
			{
				StopReload();
			}
		}
		else if (statusLabel.gameObject.activeSelf)
		{
			statusLabel.text = string.Empty;
			statusLabel.gameObject.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		healthScript.OnArmorUpdateSignal.RemoveReceiver(onArmorUpdateCallback);
		healthScript.OnHealthUpdateSignal.RemoveReceiver(onHealthUpdateCallback);
	}

	public void InitializeCameras(Camera uiCamera, Camera gameCamera)
	{
		followTarget = GetComponent<UIFollowTarget>();
		followTarget.uiCamera = uiCamera;
		followTarget.gameCamera = gameCamera;
	}

	public void Reset()
	{
		followTarget.enabled = false;
		if (BarHealth != null && healthScript != null)
		{
			BarHealth.ResetToFull();
			BarArmor.ResetToEmpty();
		}
	}

	public void SetTarget(GameObject soldier)
	{
		if (!(soldier != null))
		{
			return;
		}
		healthScript = soldier.GetComponent<Health>();
		onHealthUpdateCallback = healthScript.OnHealthUpdateSignal.AddReceiver(OnHealthUpdate);
		onArmorUpdateCallback = healthScript.OnArmorUpdateSignal.AddReceiver(OnArmorUpdate);
		followTarget.target = soldier.transform;
		followTarget.enabled = true;
		if (BarHealth != null && healthScript != null)
		{
			BarHealth.SetBarStartValue(healthScript.CurrentHealth / healthScript.MaxHealth);
			if (healthScript.CurrentArmor > 0)
			{
				BarArmor.SetBarStartValue(healthScript.CurrentArmor / healthScript.MaxArmor);
			}
			else
			{
				BarArmor.SetBarStartValue(0f);
			}
		}
	}

	public void SetPlayerID(int id)
	{
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void OnHealthUpdate(Health UnitHealthComponent)
	{
		int num = UnitHealthComponent.CurrentHealth;
		if (num < 0)
		{
			num = 0;
		}
		if (maxHealth <= -1f)
		{
			maxHealth = UnitHealthComponent.MaxHealth;
		}
		float barTarget = (float)num / maxHealth;
		BarHealth.SetBarTarget(barTarget);
	}

	public void OnArmorUpdate(Health UnitHealthComponent)
	{
		int num = UnitHealthComponent.CurrentArmor;
		if (num < 0)
		{
			num = 0;
		}
		if (maxArmor == -1f)
		{
			maxArmor = UnitHealthComponent.MaxArmor;
		}
		float barTarget = (float)num / maxArmor;
		BarArmor.SetBarTarget(barTarget);
	}

	public void SetAmmoStatus(bool isVisible)
	{
		if (combatStatus == null)
		{
			combatStatus = new PlayerStatusText();
		}
		if (isVisible)
		{
			combatStatus.SetNoAmmoStatus(false);
		}
		else
		{
			combatStatus.SetNoAmmoStatus(true);
		}
	}

	public void StartReload(float time)
	{
		reloading = true;
		combatStatus.SetReloadStatus(reloading);
		BarReload.StopTimer();
		BarReload.SetBarTime(time);
	}

	public void StopReload()
	{
		reloading = false;
		combatStatus.SetReloadStatus(reloading);
		BarReload.StopTimer();
	}
}
