using System;
using System.Collections.Generic;
using EventHelpers;

public class EffectEventSystem
{
	private class Subscriber
	{
		private Action<TriggerBase> callback;

		private object objectReference;

		public Action<TriggerBase> Callback
		{
			get
			{
				return callback;
			}
		}

		public object ObjectReference
		{
			get
			{
				return objectReference;
			}
		}

		public Subscriber(object objRef, Action<TriggerBase> callbackMethod)
		{
			objectReference = objRef;
			callback = callbackMethod;
		}
	}

	private Dictionary<TriggerId, List<Subscriber>> subscriptions;

	public EffectEventSystem()
	{
		subscriptions = new Dictionary<TriggerId, List<Subscriber>>();
	}

	public void Subscribe(TriggerId triggerId, Action<TriggerBase> callback, object obj)
	{
		Subscriber item = new Subscriber(obj, callback);
		if (subscriptions.ContainsKey(triggerId))
		{
			if (!IsObjectSubscribingToTrigger(obj, triggerId))
			{
				subscriptions[triggerId].Add(item);
			}
		}
		else
		{
			subscriptions.Add(triggerId, new List<Subscriber> { item });
		}
	}

	public void Unsubscribe(TriggerId triggerId, object obj)
	{
		if (!subscriptions.ContainsKey(triggerId))
		{
			return;
		}
		List<Subscriber> list = subscriptions[triggerId];
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (obj == list[num].ObjectReference)
			{
				list.RemoveAt(num);
			}
		}
	}

	public void Trigger(TriggerBase e)
	{
		TriggerId id = e.Id;
		if (!subscriptions.ContainsKey(id))
		{
			return;
		}
		List<Subscriber> list = new List<Subscriber>(subscriptions[id]);
		foreach (Subscriber item in list)
		{
			item.Callback(e);
		}
	}

	public bool AnySubscriber(TriggerId id)
	{
		return subscriptions.ContainsKey(id);
	}

	private bool IsObjectSubscribingToTrigger(object obj, TriggerId triggerId)
	{
		if (subscriptions.ContainsKey(triggerId))
		{
			List<Subscriber> list = subscriptions[triggerId];
			foreach (Subscriber item in list)
			{
				if (item.ObjectReference == obj)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}
}
