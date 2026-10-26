using UnityEngine.AI;
using AIActionTransitions;
using UnityEngine;

public class MotionAIAction : AIAction
{
	private NavMeshAgent agent;

	private OffMeshLinkData link;

	private AIMotion motion;

	private AIAction secondaryAction;

	public MotionAIAction(NavMeshAgent agent, OffMeshLinkData link, AIMotion motion)
	{
		this.agent = agent;
		this.link = link;
		this.motion = motion;
	}

	public override void OnStart()
	{
		if (motion != null)
		{
			secondaryAction = motion.GetAction(agent, link);
		}
		if (secondaryAction == null)
		{
			secondaryAction = new JumpAIAction(agent, link);
		}
		if (secondaryAction != null)
		{
			actionController.Secondary.Push(secondaryAction);
			base.OnStart();
		}
		else
		{
			status = AIActionStatus.Failed;
		}
	}

	public override Transition Update()
	{
		if (secondaryAction == null || secondaryAction.IsDone())
		{
			status = AIActionStatus.Complete;
		}
		return base.Update();
	}

	public override void OnEnd()
	{
		secondaryAction = null;
	}
}
