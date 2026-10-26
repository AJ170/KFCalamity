using EventHelpers;
using UnityEngine;

public class ToggleLightEffect : EffectBaseListener
{
	public string LightObjectName;

	public float Duration;

	private GameObject lightObject;

	private float counter;

	protected override void Start()
	{
		base.Start();
		GameObject gameObject = GameObject.Find(LightObjectName);
		lightObject = gameObject;
	}

	private void ActivateTimer()
	{
		counter = Duration;
	}

	private void ToggleLight()
	{
		if (lightObject != null)
		{
			lightObject.SetActive(!lightObject.activeSelf);
		}
	}

	private void Update()
	{
		if (counter > 0f)
		{
			counter -= Time.deltaTime;
			if (counter <= 0f)
			{
				ToggleLight();
			}
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			ActivateTimer();
		}
	}
}
