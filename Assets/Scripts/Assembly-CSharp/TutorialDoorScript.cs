using UnityEngine;

public class TutorialDoorScript : MonoBehaviour
{
	public GameObject DoorObject;

	public bool Locked;

	public bool StaysOpen;

	public bool BlockPassageOnClose;

	public float OpenTime = 1f;

	public LayerMask Layers;

	public Animation DoorAnimator;

	public AnimationClip OpenClip;

	public AnimationClip CloseClip;

	private bool dontOpen;

	private int objectsInTrigger;

	private bool isOpen;

	public bool IsOpen
	{
		get
		{
			return isOpen;
		}
		set
		{
			isOpen = value;
		}
	}

	public void OpenDoor()
	{
		if (Locked)
		{
			Debug.LogWarning("DoorScript:OpenDoor() - Trying to open locked door: " + base.name);
		}
		else if (!IsOpen)
		{
			if (DoorAnimator.IsPlaying(CloseClip.name))
			{
				DoorAnimator[OpenClip.name].time = DoorAnimator[CloseClip.name].length - DoorAnimator[CloseClip.name].time;
			}
			DoorAnimator.Play(OpenClip.name);
			isOpen = true;
		}
	}

	public void CloseDoor()
	{
		if (DoorAnimator.IsPlaying(OpenClip.name))
		{
			DoorAnimator[CloseClip.name].time = DoorAnimator[OpenClip.name].length - DoorAnimator[OpenClip.name].time;
		}
		DoorAnimator.Play(CloseClip.name);
		isOpen = false;
		if (BlockPassageOnClose)
		{
			Transform transform = base.transform.FindChild("door_blocker");
			BoxCollider boxCollider = (BoxCollider)transform.GetComponent<Collider>();
			boxCollider.enabled = true;
		}
	}

	public void UnlockDoor()
	{
		Locked = false;
	}

	public void LockDoor()
	{
		Locked = true;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!dontOpen && !Locked && (Layers.value & (1 << collider.gameObject.layer)) != 0)
		{
			objectsInTrigger++;
			if (StaysOpen)
			{
				dontOpen = true;
			}
			OpenDoor();
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (!StaysOpen && (Layers.value & (1 << collider.gameObject.layer)) != 0)
		{
			objectsInTrigger--;
			if (objectsInTrigger == 0)
			{
				CloseDoor();
			}
		}
	}
}
