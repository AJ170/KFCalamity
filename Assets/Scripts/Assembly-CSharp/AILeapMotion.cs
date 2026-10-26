using UnityEngine.AI;
using UnityEngine;

public class AILeapMotion : AIMotion
{
	public AIMotionType Type
	{
		get
		{
			return AIMotionType.Leap;
		}
	}

	public AIAction GetAction(NavMeshAgent agent, OffMeshLinkData link)
	{
		return new LeapAIAction();
	}
}
