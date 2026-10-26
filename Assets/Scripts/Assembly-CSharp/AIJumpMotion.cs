using UnityEngine.AI;
using UnityEngine;

public class AIJumpMotion : AIMotionComponent
{
	public float JumpHeight = 0.5f;

	public float Duration = 1f;

	public override AIMotionType Type
	{
		get
		{
			return AIMotionType.Jump;
		}
	}

	public override AIAction GetAction(NavMeshAgent agent, OffMeshLinkData link)
	{
		return new JumpAIAction(agent, link, this);
	}
}
