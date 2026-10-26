using UnityEngine;

internal class TargetChangedAIEvent : AIEvent
{
	private GameObject target;

	private GameObject previousTarget;

	public GameObject Target
	{
		get
		{
			return target;
		}
	}

	public GameObject PreviousTarget
	{
		get
		{
			return previousTarget;
		}
	}

	public TargetChangedAIEvent(GameObject target, GameObject previousTarget)
		: base(AIEventID.TargetChanged)
	{
		this.target = target;
		this.previousTarget = previousTarget;
	}

	public TargetChangedAIEvent(GameObject target)
		: this(target, null)
	{
	}
}
