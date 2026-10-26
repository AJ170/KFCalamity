using EventHelpers;
using UnityEngine;

public class MeleeSwingTrail : EffectBaseListener
{
	public float Delay = 0.5f;

	public float Duration = 0.2f;

	public GameObject Object;

	public Vector3 Offset;

	private float timer = -1f;

	protected override void Start()
	{
		Object.transform.localPosition += Offset;
		base.Start();
	}

	private void Update()
	{
		if (!(timer > 0f))
		{
			return;
		}
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			bool activeSelf = Object.activeSelf;
			ToggleObject(!Object.activeSelf);
			if (!activeSelf)
			{
				timer = Duration;
			}
		}
	}

	private void ToggleObject(bool toggle)
	{
		if (Object != null)
		{
			Object.SetActive(toggle);
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			timer = Delay;
		}
	}
}
