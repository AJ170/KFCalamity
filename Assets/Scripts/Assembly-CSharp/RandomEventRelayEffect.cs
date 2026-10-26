using System;
using EffectEvents;
using EventHelpers;
using UnityEngine;

public class RandomEventRelayEffect : EffectBaseListener
{
	[Serializable]
	public class WeightedEvent
	{
		public string Name;

		public float Weight = 0.5f;
	}

	public TriggerId OutputEvent = TriggerId.Attack;

	public WeightedEvent[] RandomEvents;

	public bool TriggerInternal = true;

	public bool DontRepeat;

	private float totalWeight;

	private WeightedEvent lastTriggered;

	protected override void Start()
	{
		base.Start();
		WeightedEvent[] randomEvents = RandomEvents;
		foreach (WeightedEvent weightedEvent in randomEvents)
		{
			totalWeight += weightedEvent.Weight;
		}
	}

	protected override void Execute(TriggerBase e)
	{
		if ((!MatchCustomEvent(e.CustomName) && !string.IsNullOrEmpty(e.CustomName)) || RandomEvents.Length <= 0)
		{
			return;
		}
		string eventName = GetEventName();
		if (eventName == null)
		{
			return;
		}
		TriggerBase triggerBase = CreateEvent(eventName);
		if (triggerBase != null)
		{
			if (TriggerInternal)
			{
				effectSystem.TriggerInternal(triggerBase);
			}
			else
			{
				effectSystem.TriggerGlobal(triggerBase);
			}
		}
	}

	private string GetEventName()
	{
		float num = UnityEngine.Random.Range(0f, totalWeight);
		WeightedEvent[] randomEvents = RandomEvents;
		foreach (WeightedEvent weightedEvent in randomEvents)
		{
			if ((!DontRepeat || RandomEvents.Length <= 1 || weightedEvent != lastTriggered) && num <= weightedEvent.Weight)
			{
				lastTriggered = weightedEvent;
				return weightedEvent.Name;
			}
			num -= weightedEvent.Weight;
		}
		return null;
	}

	private TriggerBase CreateEvent(string customName)
	{
		TriggerBase result = null;
		switch (OutputEvent)
		{
		case TriggerId.Attack:
			result = new Attack(customName);
			break;
		case TriggerId.Cast:
			result = new Cast(customName);
			break;
		}
		return result;
	}
}
