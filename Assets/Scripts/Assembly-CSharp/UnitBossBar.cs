using System.Collections.Generic;
using UnityEngine;

public class UnitBossBar : MonoBehaviour
{
	public TweenPosition PositionTween;

	public Transform ProgressBar;

	public Transform Text;

	public TweenAlpha Alpha;

	public TweenAlpha[] SyringesTweens;

	private Health healthScript;

	private float maxHealth = -1f;

	public UILabel Label;

	private BossBar bar;

	private List<GameObject> colliderList = new List<GameObject>();

	private List<TweenAlpha> syringes;

	private void Start()
	{
		if (Text != null)
		{
			Label = Text.GetComponent<UILabel>();
		}
		if (ProgressBar != null)
		{
			bar = ProgressBar.GetComponent<BossBar>();
		}
		if (SyringesTweens != null)
		{
			syringes = new List<TweenAlpha>(SyringesTweens);
		}
	}

	private void Update()
	{
		CheckForNullColliders();
	}

	public void TweenIn()
	{
		PositionTween.callWhenFinished = string.Empty;
		PositionTween.enabled = true;
		PositionTween.Play(true);
	}

	public void TweenOut()
	{
		PositionTween.callWhenFinished = "OnTweenComplete";
		PositionTween.Play(false);
	}

	public void OnTweenComplete()
	{
		base.gameObject.SetActive(false);
	}

	private void FadeOut()
	{
		Alpha.Reset2();
		Alpha.to = 0.5f;
		Alpha.from = Alpha.alpha;
		Alpha.enabled = true;
	}

	private void FadeIn()
	{
		Alpha.Reset2();
		Alpha.to = 1f;
		Alpha.from = Alpha.alpha;
		Alpha.enabled = true;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!colliderList.Contains(collider.gameObject))
		{
			colliderList.Add(collider.gameObject);
			if (colliderList.Count == 1)
			{
				FadeOut();
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (colliderList.Contains(collider.gameObject))
		{
			colliderList.Remove(collider.gameObject);
			if (colliderList.Count == 0)
			{
				FadeIn();
			}
		}
	}

	public void CheckForNullColliders()
	{
		if (colliderList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < colliderList.Count; i++)
		{
			if (colliderList[i] == null || !colliderList[i].activeSelf)
			{
				colliderList.RemoveAt(i);
				break;
			}
		}
		if (colliderList.Count == 0)
		{
			FadeIn();
		}
	}

	public void SetTarget(GameObject unit)
	{
		if (unit != null)
		{
			if (ProgressBar != null && bar == null)
			{
				bar = ProgressBar.GetComponent<BossBar>();
			}
			if (bar != null)
			{
				bar.SetBarStartValue(1f);
			}
			Label.text = unit.name;
			healthScript = unit.GetComponent<Health>();
			healthScript.OnHealthUpdateSignal.AddReceiver(OnHealthUpdate);
		}
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		TweenIn();
	}

	public void Hide()
	{
		TweenOut();
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
		bar.SetBarTarget(barTarget);
	}

	public void DecreaseSyringes()
	{
		if (syringes.Count > 0)
		{
			syringes[0].Reset2();
			syringes[0].Play(true);
			syringes.RemoveAt(0);
		}
	}
}
