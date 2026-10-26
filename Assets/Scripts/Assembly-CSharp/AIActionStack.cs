using System.Collections.Generic;
using AIActionTransitions;
using UnityEngine;

public class AIActionStack
{
	private AIController controller;

	private AIActionController actionController;

	private AIAction activeAction;

	private Stack<AIAction> stack = new Stack<AIAction>(2);

	public AIAction ActiveAction
	{
		get
		{
			return activeAction;
		}
	}

	public AIActionStack(AIController controller, AIActionController actionController)
	{
		this.controller = controller;
		this.actionController = actionController;
	}

	public void Update()
	{
		UpdateActiveAction();
	}

	private void UpdateActiveAction()
	{
		if (activeAction != null)
		{
			AIAction aIAction = activeAction;
			Transition transition = activeAction.Update();
			if (transition != null && activeAction == aIAction)
			{
				ProcessTransition(transition);
			}
		}
	}

	public void Push(AIAction action)
	{
		if (activeAction != null)
		{
			SuspendActiveAction();
		}
		action.SetController(controller, actionController);
		SetActiveAction(action);
	}

	public void Queue(AIAction action)
	{
		action.SetController(controller, actionController);
		stack.Push(action);
	}

	public void Shutdown()
	{
		if (activeAction != null)
		{
			ShutdownAction(activeAction);
			activeAction = null;
		}
		foreach (AIAction item in stack)
		{
			ShutdownAction(item);
		}
		stack.Clear();
	}

	private void ShutdownAction(AIAction action)
	{
		if (!action.IsDone())
		{
			action.Abort();
		}
		action.OnEnd();
	}

	private void SuspendActiveAction()
	{
		activeAction.OnSuspended();
		stack.Push(activeAction);
		activeAction = null;
	}

	private void RemoveActiveAction()
	{
		activeAction.OnEnd();
		activeAction = null;
	}

	private void SetNextAction()
	{
		activeAction = null;
		while (stack.Count > 0 && activeAction == null)
		{
			AIAction aIAction = stack.Pop();
			if (!aIAction.IsDone())
			{
				SetActiveAction(aIAction);
				break;
			}
		}
	}

	private void SetActiveAction(AIAction action)
	{
		activeAction = action;
		switch (action.Status)
		{
		case AIActionStatus.Suspended:
			action.OnResumed();
			break;
		case AIActionStatus.Inactive:
			action.OnStart();
			break;
		}
	}

	private void AbortAction(AIAction action)
	{
		foreach (AIAction item in stack)
		{
			if (item == action && !action.IsDone())
			{
				item.Abort();
			}
		}
	}

	private void ProcessTransition(Transition transition)
	{
		switch (transition.Type)
		{
		case TransitionType.ChangeTo:
			OnChangeToTransition(transition);
			break;
		case TransitionType.SuspendFor:
			OnSuspendForTransition(transition);
			break;
		case TransitionType.Done:
			OnDoneTransition(transition);
			break;
		}
	}

	private void OnChangeToTransition(Transition transition)
	{
		if (transition.NextAction != null)
		{
			RemoveActiveAction();
			Push(transition.NextAction);
		}
		else
		{
			MonoBehaviour.print("[AIActionStack:OnChangeToTransition] Invalid NextAction in transition");
		}
	}

	private void OnSuspendForTransition(Transition transition)
	{
		if (transition.NextAction != null)
		{
			Push(transition.NextAction);
		}
		else
		{
			MonoBehaviour.print("[AIActionStack:OnSuspendForTransition] Invalid NextAction in transition");
		}
	}

	private void OnDoneTransition(Transition transition)
	{
		RemoveActiveAction();
		SetNextAction();
	}

	public bool OnAIEvent(AIEvent triggeredEvent)
	{
		Transition transition = null;
		if (activeAction != null)
		{
			transition = activeAction.OnAIEvent(triggeredEvent);
		}
		if (transition == null)
		{
			foreach (AIAction item in stack)
			{
				transition = item.OnAIEvent(triggeredEvent);
				if (transition != null)
				{
					break;
				}
			}
		}
		if (transition != null)
		{
			ProcessTransition(transition);
		}
		return false;
	}
}
