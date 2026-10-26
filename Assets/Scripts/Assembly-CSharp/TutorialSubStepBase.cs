using UnityEngine;

public abstract class TutorialSubStepBase : MonoBehaviour
{
	protected TutorialManager TutorialManager;

	protected GameObject Player;

	private bool isComplete;

	public bool IsComplete
	{
		get
		{
			return isComplete;
		}
	}

	public abstract void OnStart();

	public void SetTutorialManager(TutorialManager tutorialManager)
	{
		TutorialManager = tutorialManager;
	}

	public void SetPlayer(GameObject player)
	{
		Player = player;
	}

	public void SetComplete(bool complete)
	{
		isComplete = complete;
	}

	private void LateUpdate()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{
			SetComplete(true);
		}
	}
}
