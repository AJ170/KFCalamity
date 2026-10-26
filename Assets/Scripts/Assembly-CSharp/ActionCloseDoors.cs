public class ActionCloseDoors : TutorialSubStepBase
{
	public bool LockOnClose;

	public TutorialDoorScript[] Doors;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void OnStart()
	{
		if (base.IsComplete)
		{
			return;
		}
		TutorialDoorScript[] doors = Doors;
		foreach (TutorialDoorScript tutorialDoorScript in doors)
		{
			tutorialDoorScript.CloseDoor();
			if (LockOnClose)
			{
				tutorialDoorScript.Locked = true;
			}
		}
		SetComplete(true);
	}
}
