using System.Collections.Generic;
using UnityEngine;

public class StealthManager : MonoBehaviour
{
	public int UpdatesPerSecond = 2;

	private static StealthManager instance;

	private float updateInterval = 1f;

	private float updateTimer;

	private List<StealthController> stealthers = new List<StealthController>();

	private List<StealthDetection> detectors = new List<StealthDetection>();

	public static StealthManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		Mathf.Clamp(UpdatesPerSecond, 1, 5);
		updateInterval = 1f / (float)UpdatesPerSecond;
	}

	private void Update()
	{
		TickUpdateTimer();
		if (TimeToUpdate())
		{
			ProcessStealth();
			updateTimer = 0f;
		}
	}

	private void TickUpdateTimer()
	{
		updateTimer = Mathf.Min(updateTimer + Time.deltaTime, updateInterval);
	}

	private bool TimeToUpdate()
	{
		return updateTimer == updateInterval;
	}

	public void RegisterStealther(StealthController stealther)
	{
		stealthers.Add(stealther);
	}

	public void UnregisterStealther(StealthController stealther)
	{
		stealthers.Remove(stealther);
	}

	public void RegisterDetector(StealthDetection detector)
	{
		detectors.Add(detector);
	}

	public void UnregisterDetector(StealthDetection detector)
	{
		detectors.Remove(detector);
	}

	private void ProcessStealth()
	{
		foreach (StealthController stealther in stealthers)
		{
			if (stealther.IsStealthed && stealther.CanBeDetected && !stealther.IsDetected)
			{
				StealthDetection stealthDetection = DetectorInRange(stealther.transform.position);
				if (stealthDetection != null)
				{
					OnDetected(stealther, stealthDetection);
				}
			}
			else if (stealther.IsStealthed && stealther.IsDetected)
			{
				StealthDetection stealthDetection2 = DetectorInRange(stealther.transform.position);
				if (stealthDetection2 == null)
				{
					OnRestealth(stealther);
				}
			}
		}
	}

	private StealthDetection DetectorInRange(Vector3 position)
	{
		foreach (StealthDetection detector in detectors)
		{
			float num = Vector3.Distance(position, detector.transform.position);
			if (num <= detector.DetectionRange)
			{
				return detector;
			}
		}
		return null;
	}

	private void OnDetected(StealthController stealther, StealthDetection detector)
	{
		stealther.OnDetected();
	}

	private void OnRestealth(StealthController stealther)
	{
		stealther.OnRestealth();
	}
}
