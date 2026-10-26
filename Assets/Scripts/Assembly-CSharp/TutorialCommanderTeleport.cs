using UnityEngine;

public class TutorialCommanderTeleport : TutorialSubStepBase
{
	public enum TeleportTriggerType
	{
		Instant = 0,
		OnTrigger = 1
	}

	public GameObject Commander;

	public ObjectWaypoint Waypoint;

	public TeleportTriggerType TriggerType;

	public ObjectWaypoint TeleportIfNotReached;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void DoTeleport()
	{
		if (!(TeleportIfNotReached != null) || !TeleportIfNotReached.HasBeenReached)
		{
			WaypointPathController component = Commander.GetComponent<WaypointPathController>();
			component.InstantMoveToWaypoint(Waypoint);
		}
	}

	public void OnTriggerEnter(Collider collider)
	{
		if (TriggerType == TeleportTriggerType.OnTrigger)
		{
			DoTeleport();
			SetComplete(true);
		}
	}

	public override void OnStart()
	{
		if (TriggerType == TeleportTriggerType.Instant)
		{
			DoTeleport();
			SetComplete(true);
		}
	}
}
