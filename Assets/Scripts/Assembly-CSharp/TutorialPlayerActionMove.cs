using UnityEngine;

public class TutorialPlayerActionMove : TutorialPlayerActionBase
{
	private float moveDistance;

	private float currentMovedDistance;

	private Vector3 startPosition;

	private Transform transform;

	public TutorialPlayerActionMove(GameObject player, float moveDistance)
		: base(player)
	{
		this.moveDistance = moveDistance;
		transform = player.transform;
	}

	public override void OnStartEvent()
	{
		startPosition = transform.position;
	}

	public override void Update()
	{
		currentMovedDistance += (transform.position - startPosition).magnitude;
		startPosition = transform.position;
		if (currentMovedDistance >= moveDistance)
		{
			OnComplete();
		}
	}
}
