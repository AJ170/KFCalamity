using UnityEngine;

internal class TargetDiedAIEvent : AIEvent
{
	private GameObject target;

	public GameObject Target
	{
		get
		{
			return target;
		}
	}

	public TargetDiedAIEvent(GameObject target)
		: base(AIEventID.TargetDied)
	{
		this.target = target;
	}
}
