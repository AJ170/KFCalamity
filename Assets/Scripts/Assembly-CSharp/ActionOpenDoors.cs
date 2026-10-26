using UnityEngine;

public class ActionOpenDoors : TutorialSubStepBase
{
	public float StartDelay;

	public float OpenInterval;

	public bool UnlockFirst = true;

	public TutorialDoorScript[] Doors;

	private int index = -1;

	private float timer = -1f;

	private void Start()
	{
	}

	private void OpenDoor(TutorialDoorScript door)
	{
		if (UnlockFirst)
		{
			door.Locked = false;
		}
		door.OpenDoor();
	}

	private void Update()
	{
		if (base.IsComplete || !(timer >= 0f))
		{
			return;
		}
		timer = Mathf.Max(0f, timer - Time.deltaTime);
		if (timer == 0f)
		{
			index = Mathf.Min(index + 1, Doors.Length - 1);
			OpenDoor(Doors[index]);
			if (index < Doors.Length - 1)
			{
				OpenDoors();
				return;
			}
			timer = -1f;
			SetComplete(true);
		}
	}

	public void OpenDoors()
	{
		timer = ((index != -1) ? OpenInterval : StartDelay);
	}

	public override void OnStart()
	{
		OpenDoors();
	}

	public bool CheckComplete()
	{
		bool result = true;
		TutorialDoorScript[] doors = Doors;
		foreach (TutorialDoorScript tutorialDoorScript in doors)
		{
			if (!tutorialDoorScript.IsOpen)
			{
				result = false;
			}
		}
		return result;
	}
}
