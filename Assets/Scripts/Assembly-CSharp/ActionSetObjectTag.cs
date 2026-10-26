using Calamity.Camera;
using UnityEngine;

public class ActionSetObjectTag : TutorialSubStepBase
{
	public GameObject Object;

	public string NewTag;

	public override void OnStart()
	{
		if (!string.IsNullOrEmpty(NewTag))
		{
			Object.tag = NewTag;
			Camera main = Camera.main;
			if (main != null)
			{
				CameraController component = main.GetComponent<CameraController>();
				if (component != null)
				{
					if (NewTag == "Soldier")
					{
						component.SetActiveState(CameraStateType.Idle);
					}
					else
					{
						component.SetActiveState(CameraStateType.Follow);
					}
				}
			}
		}
		SetComplete(true);
	}
}
