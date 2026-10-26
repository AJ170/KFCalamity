using UnityEngine;

public class ActionWaitForTime : TutorialSubStepBase
{
	public float WaitTime;

	private float timer = -1f;

	private void Start()
	{
	}

	private void Update()
	{
		timer = Mathf.Max(0f, timer - Time.deltaTime);
		if (timer == 0f)
		{
			SetComplete(true);
		}
	}

	public override void OnStart()
	{
		timer = WaitTime;
	}
}
