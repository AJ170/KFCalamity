using UnityEngine.AI;
using UnityEngine;

public class AIMotionComponent : MonoBehaviour, AIMotion
{
	public virtual AIMotionType Type
	{
		get
		{
			return AIMotionType.Jump;
		}
	}

	public virtual AIAction GetAction(NavMeshAgent agent, OffMeshLinkData link)
	{
		return null;
	}
}
