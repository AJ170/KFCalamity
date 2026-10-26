using UnityEngine;

public class ActionSetObjectPosition : TutorialSubStepBase
{
	public GameObject ObjectToMove;

	public GameObject Waypoint;

	public Vector3 StaticPosition;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void MoveObject()
	{
		Vector3 position = ((!(Waypoint != null)) ? StaticPosition : Waypoint.transform.position);
		if (ObjectToMove.GetComponent<Rigidbody>() != null)
		{
			ObjectToMove.GetComponent<Rigidbody>().MovePosition(position);
		}
		else
		{
			ObjectToMove.transform.position = position;
		}
	}

	public override void OnStart()
	{
		MoveObject();
		SetComplete(true);
	}
}
