using UnityEngine;

public class ScreenLock : MonoBehaviour
{
	private void Start()
	{
		Screen.lockCursor = true;
	}

	private void Update()
	{
		if (!Screen.lockCursor && Input.GetMouseButtonDown(0))
		{
			Screen.lockCursor = true;
		}
	}
}
