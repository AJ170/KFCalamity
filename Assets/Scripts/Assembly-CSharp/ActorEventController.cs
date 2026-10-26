using System;
using System.Collections.Generic;
using ActorEvents;
using UnityEngine;

public class ActorEventController : MonoBehaviour
{
	public delegate void ActorEventListener(ActorEventBase e);

	private Dictionary<ActorEventID, ActorEventListener> listeners = new Dictionary<ActorEventID, ActorEventListener>();

	public void Register(ActorEventID eventID, ActorEventListener callback)
	{
		if (listeners.ContainsKey(eventID))
		{
			if (!IsRegistered(eventID, callback))
			{
				Dictionary<ActorEventID, ActorEventListener> dictionary;
				Dictionary<ActorEventID, ActorEventListener> dictionary2 = (dictionary = listeners);
				ActorEventID key;
				ActorEventID key2 = (key = eventID);
				ActorEventListener a = dictionary[key];
				dictionary2[key2] = (ActorEventListener)Delegate.Combine(a, callback);
			}
		}
		else
		{
			listeners.Add(eventID, callback);
		}
	}

	public bool IsRegistered(ActorEventID eventID, ActorEventListener callback)
	{
		if (listeners.ContainsKey(eventID))
		{
			return listeners[eventID] == callback;
		}
		return false;
	}

	public void Unregister(ActorEventID eventID, ActorEventListener callback)
	{
		if (listeners.ContainsKey(eventID))
		{
			Dictionary<ActorEventID, ActorEventListener> dictionary;
			Dictionary<ActorEventID, ActorEventListener> dictionary2 = (dictionary = listeners);
			ActorEventID key;
			ActorEventID key2 = (key = eventID);
			ActorEventListener source = dictionary[key];
			dictionary2[key2] = (ActorEventListener)Delegate.Remove(source, callback);
			if (listeners[eventID] == null)
			{
				listeners.Remove(eventID);
			}
		}
	}

	public void UnregisterAll(ActorEventID eventID)
	{
		listeners.Remove(eventID);
	}

	public void Trigger(ActorEventBase e)
	{
		if (e == null)
		{
			return;
		}
		ActorEventID iD = e.ID;
		if (listeners.ContainsKey(iD))
		{
			ActorEventListener actorEventListener = listeners[iD];
			if (actorEventListener != null)
			{
				actorEventListener(e);
			}
		}
	}
}
