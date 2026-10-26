using UnityEngine;

public class StealthDetection : MonoBehaviour
{
	public float DetectionRange = 5f;

	private StealthManager stealthManager;

	private float defaultDetectionRange;

	public float DefaultDetectionRange
	{
		get
		{
			return defaultDetectionRange;
		}
	}

	private void Awake()
	{
		stealthManager = StealthManager.Instance;
		defaultDetectionRange = DetectionRange;
		if (stealthManager != null)
		{
			stealthManager.RegisterDetector(this);
		}
	}

	private void OnDestroy()
	{
		if (stealthManager != null)
		{
			stealthManager.UnregisterDetector(this);
		}
	}
}
