using UnityEngine;

public class TutorialPlayerSurvive : TutorialEventBase, IMissionCounter
{
	public float Timer;

	private float timer;

	private int lastValue;

	private bool needsUpdate = true;

	private CallbackItem<Health, int, int, float> callbackItem;

	public bool NeedsUpdate
	{
		get
		{
			return needsUpdate;
		}
		set
		{
			needsUpdate = value;
		}
	}

	public bool ShowCounter
	{
		get
		{
			return true;
		}
	}

	public QuestEntry.EntryType DisplayType
	{
		get
		{
			return QuestEntry.EntryType.Timer;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!(timer > 0f))
		{
			return;
		}
		lastValue = (int)timer;
		timer = Mathf.Max(0f, timer - Time.deltaTime);
		if (lastValue != (int)timer)
		{
			NeedsUpdate = true;
		}
		if (timer == 0f)
		{
			needsUpdate = true;
			Health component = TutorialManager.Soldier.GetComponent<Health>();
			if (TutorialManager.Soldier != null && !component.Dead)
			{
				SetComplete(true);
			}
			component.OnDamageTakenSignal.RemoveReceiver(callbackItem);
		}
	}

	private void OnPlayerDamageTaken(Health health, int value, int rawValue, float armorTypeMultiplier)
	{
		timer = Timer;
	}

	public override void OnStart()
	{
		timer = Timer;
		Health component = TutorialManager.Soldier.GetComponent<Health>();
		callbackItem = component.OnDamageTakenSignal.AddReceiver(OnPlayerDamageTaken);
	}

	public int GetCurrentCount()
	{
		return Mathf.CeilToInt(timer);
	}

	public int GetMaxCount()
	{
		return 0;
	}

	bool IMissionCounter.IsComplete
	{
		get
		{
			return base.IsComplete;
		}
	}
}
