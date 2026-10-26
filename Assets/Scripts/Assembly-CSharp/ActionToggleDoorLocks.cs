public class ActionToggleDoorLocks : TutorialSubStepBase
{
	public TutorialDoorScript[] Doors;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void OnStart()
	{
		TutorialDoorScript[] doors = Doors;
		foreach (TutorialDoorScript tutorialDoorScript in doors)
		{
			if (!tutorialDoorScript.Locked)
			{
				tutorialDoorScript.LockDoor();
			}
			else
			{
				tutorialDoorScript.UnlockDoor();
			}
		}
		SetComplete(true);
	}
}
