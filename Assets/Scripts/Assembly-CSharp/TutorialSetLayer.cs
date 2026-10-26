using UnityEngine;

public class TutorialSetLayer : TutorialSubStepBase
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
	}
}
