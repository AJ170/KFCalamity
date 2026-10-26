using EventHelpers;
using UnityEngine;

public class ZedTimeManager : MonoBehaviour
{
	private static ZedTimeManager instance;

	public float TickPerSecond = 1f;

	public int DecayPerTick = 5;

	public float ZedTimeDuration = 6f;

	private float timer;

	private int meter;

	private int meterThreshold;

	private bool blockZedTime;

	public static bool BlockZedTime
	{
		get
		{
			return instance.blockZedTime;
		}
		set
		{
			instance.blockZedTime = value;
		}
	}

	public static bool RegisterZedDeathReceiver(GameObject zed)
	{
		if (instance != null && zed != null)
		{
			return GlobalHelpers.AddDeathReceiver(zed, instance.OnZedDeathSignal) != null;
		}
		return false;
	}

	public static bool RegisterSoldierDeathReceiver(GameObject soldier)
	{
		if (instance != null && soldier != null)
		{
			return GlobalHelpers.AddDeathReceiver(soldier, instance.OnSoldierDeathSignal) != null;
		}
		return false;
	}

	private void OnZedDeathSignal(Health health)
	{
		if (blockZedTime || !(health != null))
		{
			return;
		}
		GameObject killer = health.Killer;
		Zed component = health.GetComponent<Zed>();
		if (!(killer != null) || !(component != null) || !(killer.tag == "Soldier"))
		{
			return;
		}
		bool doubleReward = false;
		if (health.KilledByHeadshot)
		{
			doubleReward = true;
		}
		else
		{
			RagdollController component2 = health.GetComponent<RagdollController>();
			if (component2 != null && component2.Enabled && component2.ShouldBeGibbed())
			{
				doubleReward = true;
			}
		}
		OnKilledZed(component, doubleReward);
	}

	private void OnSoldierDeathSignal(Health health)
	{
		if (!blockZedTime && health != null)
		{
			GameObject killer = health.Killer;
			GameObject gameObject = health.gameObject;
			if (killer != null && killer == gameObject)
			{
				TimeScaleManager.TriggerZedTime(ZedTimeDuration);
				meter = 0;
			}
		}
	}

	private void OnGameEvent(EventBase e)
	{
		if (e.Id == EventId.WaveStart)
		{
			UpdateMeterThreshold();
		}
	}

	private void Awake()
	{
		instance = this;
		meterThreshold = 50;
		EventSystem eventSystem = EventSystem.Instance;
		if (eventSystem != null)
		{
			eventSystem.Subscribe(EventId.WaveStart, OnGameEvent, this);
		}
	}

	private void OnDestroy()
	{
		EventSystem eventSystem = EventSystem.Instance;
		if (eventSystem != null)
		{
			eventSystem.Unsubscribe(EventId.WaveStart, this);
		}
		instance = null;
	}

	private void Update()
	{
		timer = Mathf.Max(timer - Time.deltaTime, 0f);
		if (timer == 0f)
		{
			meter = Mathf.Max(meter - DecayPerTick, 0);
			timer = 1f / TickPerSecond;
		}
	}

	private void UpdateMeterThreshold()
	{
		WaveManager waveManager = WaveManager.Instance;
		if (waveManager != null)
		{
			int num = waveManager.CurrentWaveIndex() + 1;
			int num2 = waveManager.NumWaves();
			meterThreshold = 50 + (int)((float)num / (float)num2 * 250f);
			meterThreshold = (int)((float)meterThreshold * MeterModifier());
		}
	}

	private float MeterModifier()
	{
		float num = ScalingByGameSettings.ZedTimeMeterThresholdByGameMode();
		if (SoldierManager.ContainsPerk(Perk.Commando))
		{
			num *= 0.8f;
		}
		return num;
	}

	private void OnKilledZed(Zed zed, bool doubleReward)
	{
		meter += zed.Bounty * ((!doubleReward) ? 1 : 2);
		if (meter >= meterThreshold)
		{
			TimeScaleManager.TriggerZedTime(ZedTimeDuration);
			meter = 0;
		}
	}
}
