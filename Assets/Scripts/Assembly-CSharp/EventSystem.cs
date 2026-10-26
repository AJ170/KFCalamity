using System;
using System.Collections.Generic;
using EventHelpers;

public class EventSystem
{
	private class Subscriber
	{
		private Action<EventBase> callback;

		private object objectReference;

		public Action<EventBase> Callback
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

		public Subscriber(object objRef, Action<EventBase> callbackMethod)
		{
			objectReference = objRef;
			callback = callbackMethod;
		}
	}

	private static EventSystem instance;

	private Dictionary<EventId, List<Subscriber>> subscriptions;

	public static EventSystem Instance
	{
		get
		{
			return instance;
		}
	}

	public EventSystem()
	{
		instance = this;
		subscriptions = new Dictionary<EventId, List<Subscriber>>();
	}

	public void Subscribe(EventId eventId, Action<EventBase> callback, object obj)
	{
		Subscriber item = new Subscriber(obj, callback);
		if (subscriptions.ContainsKey(eventId))
		{
			if (!IsObjectSubscribingToEvent(obj, eventId))
			{
				subscriptions[eventId].Add(item);
			}
		}
		else
		{
			subscriptions.Add(eventId, new List<Subscriber> { item });
		}
	}

	public void Unsubscribe(EventId eventId, object obj)
	{
		if (!subscriptions.ContainsKey(eventId))
		{
			return;
		}
		List<Subscriber> list = subscriptions[eventId];
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (obj == list[num].ObjectReference)
			{
				list.RemoveAt(num);
			}
		}
	}

	public void Unsubscribe(object obj)
	{
		foreach (KeyValuePair<EventId, List<Subscriber>> subscription in subscriptions)
		{
			List<Subscriber> value = subscription.Value;
			for (int num = value.Count - 1; num >= 0; num--)
			{
				if (obj == value[num].ObjectReference)
				{
					value.RemoveAt(num);
				}
			}
		}
	}

	public void UnsubscribeAll()
	{
		subscriptions.Clear();
	}

	public void Trigger(EventBase e)
	{
		EventId id = e.Id;
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

	public bool AnySubscriber(EventId eventId)
	{
		return subscriptions.ContainsKey(eventId);
	}

	private bool IsObjectSubscribingToEvent(object obj, EventId eventId)
	{
		if (subscriptions.ContainsKey(eventId))
		{
			List<Subscriber> list = subscriptions[eventId];
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
