using UnityEngine.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPathController : MonoBehaviour
{
	public string PathName;

	private WaypointPathManager.WaypointPath waypointPath;

	private ObjectWaypoint nextWaypoint;

	private AIMovementMotor movementMotor;

	private int waypointIndex = -1;

	private List<Action<GameObject>> onWaypointReached = new List<Action<GameObject>>();

	private bool waiting;

	public List<Action<GameObject>> OnWaypointReached
	{
		get
		{
			return onWaypointReached;
		}
	}

	public bool Waiting
	{
		get
		{
			return waiting;
		}
	}

	public bool HasStartedPath()
	{
		return waypointIndex > -1;
	}

	private void Start()
	{
		movementMotor = GetComponent<AIMovementMotor>();
		if (!string.IsNullOrEmpty(PathName))
		{
			SetPath(PathName);
		}
	}

	private void Update()
	{
		if (waypointPath != null && waypointIndex > -1 && movementMotor.DestinationReached() && !nextWaypoint.HasBeenReached)
		{
			OnReachedWaypoint(nextWaypoint.gameObject);
			if (nextWaypoint.WaitForMoveOrder)
			{
				Stop();
				waiting = true;
			}
			if (!waiting && !MoveToNextWaypoint())
			{
				Stop();
				waypointIndex = -1;
			}
		}
	}

	private void OnReachedWaypoint(GameObject waypoint)
	{
		ObjectWaypoint component = waypoint.GetComponent<ObjectWaypoint>();
		component.HasBeenReached = true;
		if (component.WaitForMoveOrder)
		{
			Stop();
			waiting = true;
		}
		if (component.TeleportToNextWaypoint)
		{
			ObjectWaypoint objectWaypoint = GetNextWaypoint();
			if (objectWaypoint != null)
			{
				InstantMoveToWaypoint(objectWaypoint);
			}
		}
		foreach (Action<GameObject> item in onWaypointReached)
		{
			item(waypoint);
		}
	}

	private bool MoveToWaypoint(ObjectWaypoint waypoint)
	{
		if (waypoint == null)
		{
			return false;
		}
		nextWaypoint = waypoint;
		nextWaypoint.HasBeenReached = false;
		movementMotor.MoveToTargetPosition(nextWaypoint.transform.position);
		return true;
	}

	private ObjectWaypoint GetNextWaypoint()
	{
		while (waypointIndex++ < waypointPath.Waypoints.Length && !waypointPath.Waypoints[waypointIndex].enabled)
		{
		}
		return (waypointIndex >= waypointPath.Waypoints.Length) ? null : waypointPath.Waypoints[waypointIndex];
	}

	private bool MoveToNextWaypoint()
	{
		return MoveToWaypoint(GetNextWaypoint());
	}

	public void SetPath(string pathName)
	{
		Stop();
		waypointPath = WaypointPathManager.Instance.GetPath(pathName);
		if (waypointPath == null)
		{
			Debug.LogError("WaypointPathController:SetPath() - No path with name: '" + pathName + "' found!");
		}
	}

	public void StartPath()
	{
		if (waypointPath != null)
		{
			waypointIndex = -1;
			MoveToNextWaypoint();
		}
	}

	public void Stop()
	{
		movementMotor.Stop();
	}

	public void Resume()
	{
		movementMotor.Resume();
		waiting = false;
		MoveToNextWaypoint();
	}

	public void InstantMoveToWaypoint(ObjectWaypoint waypoint)
	{
		if (waypointPath != null)
		{
			int num = Array.IndexOf(waypointPath.Waypoints, waypoint);
			if (num > -1)
			{
				Stop();
				base.GetComponent<Collider>().enabled = false;
				GetComponent<NavMeshAgent>().enabled = false;
				waypointIndex = num;
				nextWaypoint = waypoint;
				base.transform.position = waypoint.transform.position;
				base.GetComponent<Collider>().enabled = true;
				GetComponent<NavMeshAgent>().enabled = true;
			}
		}
	}
}
