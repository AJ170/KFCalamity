using UnityEngine;

public class ObjectWaypoint : MonoBehaviour
{
	public float NewSpeed = -1f;

	public bool ResetSpeed;

	public bool WaitForMoveOrder;

	public bool TeleportToNextWaypoint;

	private bool hasBeenReached;

	public bool HasBeenReached
	{
		get
		{
			return hasBeenReached;
		}
		set
		{
			hasBeenReached = value;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
