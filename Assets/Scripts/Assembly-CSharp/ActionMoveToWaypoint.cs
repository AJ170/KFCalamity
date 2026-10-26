using UnityEngine;

public class ActionMoveToWaypoint : TutorialSubStepBase
{
	public enum ActionCompleteMode
	{
		OnFirstStop = 0,
		immediately = 1,
		OnWaypointReached = 2
	}

	public WaypointPathController ObjectToMove;

	public ActionCompleteMode CompleteMode;

	public GameObject CompleteWaypoint;

	public GameObject DisableWaypoint;

	private bool hasStarted;

	private void Start()
	{
	}

	private void Update()
	{
		if (CompleteMode == ActionCompleteMode.OnFirstStop && hasStarted && ObjectToMove.Waiting && !base.IsComplete)
		{
			SetComplete(true);
		}
	}

	private void OnWaypointReached(GameObject waypoint)
	{
		if (waypoint == CompleteWaypoint)
		{
			SetComplete(true);
		}
	}

	public override void OnStart()
	{
		if (DisableWaypoint != null)
		{
			DisableWaypoint.GetComponent<ObjectWaypoint>().enabled = false;
		}
		if (ObjectToMove.HasStartedPath())
		{
			ObjectToMove.Resume();
		}
		else
		{
			ObjectToMove.StartPath();
		}
		hasStarted = true;
		if (CompleteMode == ActionCompleteMode.immediately)
		{
			SetComplete(true);
		}
		else if (CompleteMode == ActionCompleteMode.OnWaypointReached)
		{
			ObjectToMove.OnWaypointReached.Add(OnWaypointReached);
		}
	}
}
