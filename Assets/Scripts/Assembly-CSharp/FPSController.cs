using System.Runtime.InteropServices;
using UnityEngine;

public class FPSController : MonoBehaviour
{
	public enum PowerStatus
	{
		Unknown = 0,
		Online = 1,
		Offline = 2
	}

	public bool ShowFPS;

	public bool ShowPowerStatus;

	public bool DebugInputMode;

	public float FPSUpdateInterval = 1f;

	public float PowerStatusUpdateInterval = 10f;

	private GUIText fpsGui;

	private GUIText powerStatusGui;

	private float lastInterval;

	private int frames;

	private float powerStatusTimer;

	private float lastPowerCheckInterval;

	private PowerStatus currentPowerStatus;

	private bool debugInputDone;

	[DllImport("TouchOverlay")]
	public static extern bool IsPowerOnline();

	private void Start()
	{
	}

	private void OnDisable()
	{
		if ((bool)fpsGui)
		{
			Object.DestroyImmediate(fpsGui.gameObject);
		}
		if ((bool)powerStatusGui)
		{
			Object.DestroyImmediate(powerStatusGui.gameObject);
		}
	}

	private void UpdateInput()
	{
	}

	private void Update()
	{
		UpdateInput();
		UpdatePowerStatus();
		DisplayFPS();
	}

	private void OnPowerOnline()
	{
		if (QualitySettings.vSyncCount != 1)
		{
			QualitySettings.vSyncCount = 1;
			Application.targetFrameRate = -1;
		}
		currentPowerStatus = PowerStatus.Online;
		if (ShowPowerStatus)
		{
			Debug.Log("Power is now online!");
			DisplayPowerStatus();
		}
	}

	private void OnPowerOffline()
	{
		if (QualitySettings.vSyncCount != 0)
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 30;
		}
		currentPowerStatus = PowerStatus.Offline;
		if (ShowPowerStatus)
		{
			Debug.Log("Power is now offline!");
			DisplayPowerStatus();
		}
	}

	private void CheckPowerStatus()
	{
	}

	private void UpdatePowerStatus()
	{
	}

	private void DisplayPowerStatus()
	{
		if (!powerStatusGui)
		{
			GameObject gameObject = new GameObject("Power Status Display", typeof(GUIText));
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			gameObject.transform.position = Vector3.zero;
			powerStatusGui = gameObject.GetComponent<GUIText>();
			powerStatusGui.pixelOffset = new Vector2(5f, 75f);
		}
		powerStatusGui.text = "Power Status: " + currentPowerStatus;
	}

	private void DisplayFPS()
	{
		if (!ShowFPS)
		{
			return;
		}
		frames++;
		float unscaledTime = Time.unscaledTime;
		if (unscaledTime > lastInterval + FPSUpdateInterval)
		{
			if (!fpsGui)
			{
				GameObject gameObject = new GameObject("FPS Display", typeof(GUIText));
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				gameObject.transform.position = Vector3.zero;
				fpsGui = gameObject.GetComponent<GUIText>();
				fpsGui.pixelOffset = new Vector2(5f, 55f);
			}
			float num = (float)frames / (unscaledTime - lastInterval);
			float num2 = 1000f / Mathf.Max(num, 1E-05f);
			fpsGui.text = string.Format("{0:0.00} ms {1:0.00}", num2, num);
			frames = 0;
			lastInterval = unscaledTime;
		}
	}
}
