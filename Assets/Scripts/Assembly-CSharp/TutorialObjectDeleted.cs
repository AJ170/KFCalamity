using UnityEngine;

public class TutorialObjectDeleted : TutorialEventBase
{
	public GameObject[] Objects;

	private void Start()
	{
	}

	private void Update()
	{
		bool flag = true;
		GameObject[] objects = Objects;
		foreach (GameObject gameObject in objects)
		{
			if (gameObject != null)
			{
				flag = false;
			}
		}
		if (flag)
		{
			SetComplete(true);
		}
	}

	public override void OnStart()
	{
	}
}
