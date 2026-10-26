using ActorEvents;
using UnityEngine;

public class TutorialPlayerActionReload : TutorialPlayerActionBase
{
	private ActorEventController eventController;

	public TutorialPlayerActionReload(GameObject player)
		: base(player)
	{
		eventController = player.GetComponent<ActorEventController>();
		eventController.Register(ActorEventID.ReloadWeapon, OnReload);
	}

	public override void OnShutdown()
	{
		eventController.Unregister(ActorEventID.ReloadWeapon, OnReload);
	}

	private void OnReload(ActorEventBase e)
	{
		OnComplete();
	}
}
