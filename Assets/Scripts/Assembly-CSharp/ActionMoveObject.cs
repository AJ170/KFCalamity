using System.Collections.Generic;
using UnityEngine;

public class ActionMoveObject : TutorialSubStepBase
{
	public GameObject Object;

	public GameObject[] Waypoints;

	public string WaypointEventObjectName;

	public bool MoveAsync;

	private Transform commanderTransform;

	private AIMovementMotor aiMovement;

	private bool hasRun;

	private float baseSpeed;

	private List<GameObject> waypoints;

	private ObjectWaypoint targetWaypoint;

	private int waypointIndex = -1;

	private static ActionMoveObject currentMoveAction;

	private void Start()
	{
	}

	private void Awake()
	{
		aiMovement = Object.GetComponent<AIMovementMotor>();
		baseSpeed = aiMovement.Speed;
	}

	private void Update()
	{
		if (Object == null)
		{
			base.enabled = false;
			SetComplete(true);
			return;
		}
		if (MoveAsync && !base.IsComplete && hasRun)
		{
			SetComplete(true);
		}
		if (hasRun && aiMovement.DestinationReached() && !targetWaypoint.HasBeenReached)
		{
			OnReachedWaypoint();
			if (++waypointIndex < waypoints.Count)
			{
				MoveToWaypoint(waypoints[waypointIndex]);
			}
			else if (hasRun)
			{
				SetComplete(true);
				base.enabled = false;
			}
		}
	}

	private void OnReachedWaypoint()
	{
		if (targetWaypoint != null)
		{
			targetWaypoint.HasBeenReached = true;
			if (targetWaypoint.ResetSpeed)
			{
				aiMovement.Speed = baseSpeed;
			}
			else if (targetWaypoint.NewSpeed > -1f)
			{
				aiMovement.Speed = targetWaypoint.NewSpeed;
			}
		}
	}

	private void MoveToWaypoint(GameObject waypoint)
	{
		targetWaypoint = waypoint.GetComponent<ObjectWaypoint>();
		aiMovement.MoveToTargetPosition(waypoint.transform.position);
	}

	public void ClearWaypoints()
	{
		waypoints.Clear();
	}

	private GameObject FindWaypoint(string objectName)
	{
		GameObject gameObject = null;
		gameObject = GameObject.Find(objectName + "(Clone)");
		if (gameObject != null)
		{
			return gameObject;
		}
		return GameObject.Find(objectName);
	}

	private void PickupTooCloseHack(GameObject pickupObject)
	{
		Pickup component = pickupObject.GetComponent<Pickup>();
		if (component != null)
		{
			component.ForceCollidePlayer(Object.GetComponent<Collider>());
		}
	}

	public override void OnStart()
	{
		if (currentMoveAction != null)
		{
			currentMoveAction.enabled = false;
		}
		currentMoveAction = this;
		waypoints = new List<GameObject>(Waypoints);
		if (!string.IsNullOrEmpty(WaypointEventObjectName))
		{
			GameObject gameObject = FindWaypoint(WaypointEventObjectName);
			gameObject.AddComponent<ObjectWaypoint>();
			waypoints.Add(gameObject);
			if (Vector3.Distance(gameObject.transform.position, Object.transform.position) < 1.6f && gameObject.GetComponent<Pickup>() != null)
			{
				PickupTooCloseHack(gameObject);
			}
		}
		if (waypoints.Count > 0)
		{
			MoveToWaypoint(waypoints[++waypointIndex]);
			hasRun = true;
		}
		else
		{
			Debug.LogWarning("ActionMoveObject:OnStart() - No waypoints assigned! Proceeding to next step");
			SetComplete(true);
		}
	}
}
