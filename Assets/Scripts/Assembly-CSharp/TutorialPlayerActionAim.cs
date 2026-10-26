using UnityEngine;

public class TutorialPlayerActionAim : TutorialPlayerActionBase
{
	private AimController aimController;

	public TutorialPlayerActionAim(GameObject player)
		: base(player)
	{
	}

	public override void OnStartEvent()
	{
		aimController = Player.GetComponent<AimController>();
	}

	public override void Update()
	{
		if (aimController.IsAiming())
		{
			OnComplete();
		}
	}
}
