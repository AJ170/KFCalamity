using EventHelpers;
using UnityEngine;

public class StealthEffect : EffectBaseListener
{
	public float StealthVisibility = 0.1f;

	public float FadeSpeed = 0.5f;

	public bool CanBeDetected = true;

	private StealthController stealthController;

	protected override void Start()
	{
		base.Start();
	}

	private void OnMounted(GameObject parent)
	{
		stealthController = parent.GetComponent<StealthController>();
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && stealthController != null)
		{
			stealthController.StealthVisibility = StealthVisibility;
			stealthController.FadeSpeed = FadeSpeed;
			stealthController.CanBeDetected = CanBeDetected;
			stealthController.Stealth();
		}
	}
}
