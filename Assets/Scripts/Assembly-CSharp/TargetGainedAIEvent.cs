using UnityEngine;

internal class TargetGainedAIEvent : AIEvent
{
	private GameObject target;

	public GameObject Target
	{
		get
		{
			return target;
		}
	}

	public TargetGainedAIEvent(GameObject target)
		: base(AIEventID.TargetGained)
	{
		this.target = target;
	}
}
