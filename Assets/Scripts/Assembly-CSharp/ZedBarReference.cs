using UnityEngine;

public class ZedBarReference : MonoBehaviour
{
	public bool UseHealthBar = true;

	public Health HealthComponent;

	private GuiInGameManager guiManager;

	private ZedBar bar;

	private bool inLineOfSight;

	private void Awake()
	{
		LineOfSightTarget component = GetComponent<LineOfSightTarget>();
		if (component != null)
		{
			component.RegisterGainVisibilityListener(OnGainVisibility);
			component.RegisterLostVisibilityListener(OnLostVisibility);
		}
	}

	private void Start()
	{
		if (UseHealthBar && GuiInGameManager.Instance != null)
		{
			guiManager = GuiInGameManager.Instance;
			if (HealthComponent != null)
			{
				bar = guiManager.AddZedHealthBar(base.gameObject, HealthComponent);
				bar.InLineOfSight = inLineOfSight;
			}
		}
	}

	public void InLineOfSight(bool inSight)
	{
		inLineOfSight = inSight;
		if (bar != null)
		{
			bar.InLineOfSight = inSight;
		}
	}

	public void Show()
	{
		if (bar != null)
		{
			bar.Show();
		}
	}

	public void Hide()
	{
		if (bar != null)
		{
			bar.Hide();
		}
	}

	private void OnDestroy()
	{
		if (bar != null)
		{
			Object.Destroy(bar.gameObject);
		}
		LineOfSightTarget component = GetComponent<LineOfSightTarget>();
		if (component != null)
		{
			component.UnregisterGainVisibilityListener(OnGainVisibility);
			component.UnregisterLostVisibilityListener(OnLostVisibility);
		}
	}

	private void OnGainVisibility()
	{
		InLineOfSight(true);
	}

	private void OnLostVisibility()
	{
		InLineOfSight(false);
	}
}
