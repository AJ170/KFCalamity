using GameEvents;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
	private enum ZedTimeState
	{
		Inactive = 0,
		Active = 1,
		Fading = 2
	}

	private static TimeScaleManager instance;

	private float timeScale = 1f;

	private bool gamePaused;

	private bool zedTimeActive;

	private ZedTimeState zedTimeState;

	private float zedTimeCounter;

	public float zedTimeFadeLength = 1f;

	public float zedTimeScale = 0.5f;

	public AudioClip ZedTimeEnterSound;

	public AudioClip ZedTimeExitSound;

	public float AudioVolume = 1f;

	private void Start()
	{
		if (instance != null)
		{
			Debug.LogWarning("[TimeScaleManager] Multiple TimeScaleManager instances detected");
			Object.Destroy(instance);
		}
		instance = this;
	}

	private void Update()
	{
		if (!gamePaused && zedTimeActive)
		{
			UpdateZedTime();
		}
	}

	public static void SetTimeScale(float scale)
	{
		if (instance != null)
		{
			instance.timeScale = scale;
			if (!instance.gamePaused)
			{
				instance.SetScale(scale);
			}
		}
	}

	public static void PauseGame()
	{
		if (instance != null)
		{
			instance.SetPaused(true);
		}
	}

	public static void UnpauseGame()
	{
		if (instance != null)
		{
			instance.SetPaused(false);
		}
	}

	private void UpdateZedTime()
	{
		if (zedTimeState == ZedTimeState.Active)
		{
			zedTimeCounter -= Time.deltaTime;
			if (!(zedTimeCounter <= 0f))
			{
				return;
			}
			if (zedTimeFadeLength > 0f)
			{
				zedTimeState = ZedTimeState.Fading;
				zedTimeCounter = 0f;
				if (ZedTimeExitSound != null)
				{
					AudioManager.PlayClipAtPoint(ZedTimeExitSound, Vector3.zero, AudioVolume);
				}
			}
			else
			{
				StopZedTime();
			}
		}
		else if (zedTimeState == ZedTimeState.Fading)
		{
			zedTimeCounter += Time.deltaTime;
			if (zedTimeCounter < zedTimeFadeLength)
			{
				float t = zedTimeCounter / zedTimeFadeLength;
				float num = Mathf.Lerp(zedTimeScale, 1f, t);
				timeScale = num;
				SetScale(timeScale);
			}
			else
			{
				StopZedTime();
			}
		}
	}

	public static void TriggerZedTime(float duration)
	{
		if (instance != null)
		{
			instance.StartZedTime(duration);
		}
	}

	public void StartZedTime(float duration)
	{
		if (zedTimeActive)
		{
			StopZedTime();
		}
		if (ZedTimeExitSound != null)
		{
			AudioManager.StopClip(ZedTimeExitSound);
		}
		zedTimeCounter = duration;
		zedTimeState = ZedTimeState.Active;
		timeScale = zedTimeScale;
		if (!gamePaused)
		{
			SetScale(timeScale);
		}
		if (!zedTimeActive && ZedTimeEnterSound != null)
		{
			AudioManager.PlayClipAtPoint(ZedTimeEnterSound, Vector3.zero, AudioVolume);
		}
		zedTimeActive = true;
		if (GuiInGameManager.Instance != null)
		{
			GuiInGameManager.Instance.TriggerZedTimeEffect(0.2f, duration, zedTimeFadeLength);
		}
		if (EventSystem.Instance != null)
		{
			EventSystem.Instance.Trigger(new ZedTimeStart());
		}
	}

	private void StopZedTime()
	{
		timeScale = 1f;
		SetScale(timeScale);
		zedTimeState = ZedTimeState.Inactive;
		zedTimeActive = false;
		if (EventSystem.Instance != null)
		{
			EventSystem.Instance.Trigger(new ZedTimeEnd());
		}
	}

	private void OnDestroy()
	{
		SetScale(1f);
		instance = null;
	}

	private void SetPaused(bool paused)
	{
		if (paused && !gamePaused)
		{
			SetScale(0f);
		}
		else if (!paused && gamePaused)
		{
			SetScale(timeScale);
		}
		gamePaused = paused;
	}

	private void SetScale(float scale)
	{
		Time.timeScale = scale;
	}
}
