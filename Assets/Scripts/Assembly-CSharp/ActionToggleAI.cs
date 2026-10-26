using UnityEngine;

public class ActionToggleAI : TutorialSubStepBase
{
	public AIController[] Objects;

	public float StartDelay;

	private float timer;

	private void Start()
	{
	}

	private void Update()
	{
		if (!base.IsComplete)
		{
			timer = Mathf.Max(0f, timer - Time.deltaTime);
			if (timer == 0f)
			{
				Toggle();
			}
		}
	}

	private void Toggle()
	{
		AIController[] objects = Objects;
		foreach (AIController aIController in objects)
		{
			aIController.enabled = !aIController.enabled;
			SetComplete(true);
		}
	}

	public override void OnStart()
	{
		if (StartDelay > 0f)
		{
			timer = StartDelay;
		}
		else
		{
			Toggle();
		}
	}
}
