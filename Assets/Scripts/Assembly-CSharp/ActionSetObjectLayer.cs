using UnityEngine;

public class ActionSetObjectLayer : TutorialSubStepBase
{
	public GameObject Object;

	public string NewLayer;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void OnStart()
	{
		Object.layer = LayerMask.NameToLayer(NewLayer);
		SetComplete(true);
	}
}
