using UnityEngine.AI;
using UnityEngine;

public interface AIMotion
{
	AIMotionType Type { get; }

	AIAction GetAction(NavMeshAgent agent, OffMeshLinkData link);
}
