using System.Collections.Generic;
using UnityEngine;

public class TutorialDebug : MonoBehaviour
{
	public bool DoorsOpen;

	public TutorialDoorScript[] ExcludedDoors;

	public bool EnableAllAbilities;

	public int TutorialStartStep = 1;

	public GameObject StartStep;

	private TutorialManager tutorialManager;

	private void Start()
	{
	}

	public void Run()
	{
	}

	private void OpenAllDoors()
	{
		if (!DoorsOpen)
		{
			return;
		}
		List<TutorialDoorScript> list = new List<TutorialDoorScript>(ExcludedDoors);
		GameObject gameObject = GameObject.Find("StaticGeometry");
		TutorialDoorScript[] componentsInChildren = gameObject.GetComponentsInChildren<TutorialDoorScript>();
		TutorialDoorScript[] array = componentsInChildren;
		foreach (TutorialDoorScript tutorialDoorScript in array)
		{
			if (!list.Contains(tutorialDoorScript))
			{
				tutorialDoorScript.UnlockDoor();
				tutorialDoorScript.OpenDoor();
			}
		}
	}

	private void Update()
	{
	}
}
