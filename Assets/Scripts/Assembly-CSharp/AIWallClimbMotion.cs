using UnityEngine.AI;
using UnityEngine;

public class AIWallClimbMotion : AIMotionComponent
{
	public override AIMotionType Type
	{
		get
		{
			return AIMotionType.WallClimb;
		}
	}

	public override AIAction GetAction(NavMeshAgent agent, OffMeshLinkData link)
	{
		return new WallClimbAIAction(agent, link, this);
	}
}
