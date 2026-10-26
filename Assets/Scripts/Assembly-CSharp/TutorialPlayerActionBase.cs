using System;
using UnityEngine;

public abstract class TutorialPlayerActionBase
{
	private Action onComplete;

	private Action onFailed;

	private bool isComplete;

	protected GameObject Player;

	protected GameObject questID;

	public bool IsComplete
	{
		get
		{
			return isComplete;
		}
	}

	public TutorialPlayerActionBase(GameObject player)
	{
		Player = player;
	}

	public virtual void Update()
	{
	}

	public virtual void OnStartEvent()
	{
	}

	public virtual void OnEndEvent()
	{
	}

	public virtual void OnShutdown()
	{
	}

	private void Shutdown()
	{
		onComplete = null;
		onFailed = null;
		OnShutdown();
	}

	protected void OnComplete()
	{
		if (onComplete != null && !isComplete)
		{
			onComplete();
			isComplete = true;
			Shutdown();
		}
	}

	protected void OnFailed()
	{
		if (onFailed != null)
		{
			onFailed();
		}
	}

	public void SetFailedCallback(Action failedCallback)
	{
		onFailed = failedCallback;
	}

	public void SetCompletedCallback(Action completeCallback)
	{
		onComplete = completeCallback;
	}

	public void SetQuestID(GameObject questID)
	{
		this.questID = questID;
	}
}
