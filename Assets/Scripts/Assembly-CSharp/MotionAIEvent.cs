using UnityEngine.AI;
using UnityEngine;

public class MotionAIEvent : AIEvent
{
	public readonly NavMeshAgent Agent;

	public readonly OffMeshLinkData Link;

	public readonly AIMotion Motion;

	public MotionAIEvent(NavMeshAgent agent, OffMeshLinkData link)
		: this(agent, link, null)
	{
	}

	public MotionAIEvent(NavMeshAgent agent, OffMeshLinkData link, AIMotion motion)
		: base(AIEventID.Motion)
	{
		Agent = agent;
		Link = link;
		Motion = motion;
	}

	public MotionAIEvent(AIMotion motion)
		: base(AIEventID.Motion)
	{
		Motion = motion;
	}
}
