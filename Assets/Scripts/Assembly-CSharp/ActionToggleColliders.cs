using UnityEngine;

public class ActionToggleColliders : TutorialSubStepBase
{
	public Collider[] Colliders;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void OnStart()
	{
		Collider[] colliders = Colliders;
		foreach (Collider collider in colliders)
		{
			collider.enabled = !collider.enabled;
		}
		SetComplete(true);
	}
}
