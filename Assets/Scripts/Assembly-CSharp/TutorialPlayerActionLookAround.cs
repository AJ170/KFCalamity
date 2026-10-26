using UnityEngine;

public class TutorialPlayerActionLookAround : TutorialPlayerActionBase
{
	private float rotationAmount;

	private Transform transform;

	private Vector3 startLookDirection;

	public TutorialPlayerActionLookAround(GameObject player, float rotationAmount)
		: base(player)
	{
		this.rotationAmount = rotationAmount;
		transform = player.transform;
	}

	public override void OnStartEvent()
	{
		startLookDirection = transform.forward;
	}

	public override void Update()
	{
		float num = Vector3.Angle(startLookDirection, transform.forward);
		if (num > rotationAmount)
		{
			OnComplete();
		}
	}
}
