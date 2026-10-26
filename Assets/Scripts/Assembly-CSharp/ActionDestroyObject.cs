using UnityEngine;

public class ActionDestroyObject : TutorialSubStepBase
{
	public GameObject Object;

	public override void OnStart()
	{
		UnityEngine.Object.Destroy(Object);
		SetComplete(true);
	}
}
