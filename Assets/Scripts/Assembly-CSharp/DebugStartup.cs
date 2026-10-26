using UnityEngine;

public class DebugStartup : MonoBehaviour
{
	private static bool HasStarted;

	private void Start()
	{
		if (!HasStarted)
		{
			HasStarted = true;
		}
	}

	private void Update()
	{
	}
}
