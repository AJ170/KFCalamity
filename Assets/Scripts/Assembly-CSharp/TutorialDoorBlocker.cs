using UnityEngine;

public class TutorialDoorBlocker : MonoBehaviour
{
	public TutorialDoorScript Door;

	public bool CloseDoors = true;

	public TutorialDoorScript[] Doors;

	private bool hasRun;

	private void Start()
	{
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Soldier" && !hasRun)
		{
			BoxCollider boxCollider = (BoxCollider)Door.transform.FindChild("door_blocker").GetComponent<Collider>();
			boxCollider.enabled = true;
			TutorialDoorScript[] doors = Doors;
			foreach (TutorialDoorScript tutorialDoorScript in doors)
			{
				tutorialDoorScript.CloseDoor();
				tutorialDoorScript.Locked = true;
			}
			hasRun = true;
			base.enabled = false;
		}
	}

	private void Update()
	{
	}
}
