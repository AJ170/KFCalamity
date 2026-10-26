using System;
using System.Collections.Generic;

public class SignalSenders<T> : SignalSendersBase
{
	private List<CallbackItem<T>> callbacks = new List<CallbackItem<T>>();

	public CallbackItem<T> AddReceiver(Action<T> callback)
	{
		callbacks.Add(new CallbackItem<T>(callback));
		return callbacks.LastItem();
	}

	public void RemoveReceiver(CallbackItem<T> item)
	{
		callbacks.Remove(item);
	}

	public void RemoveReceiver(Action<T> callback)
	{
		for (int num = callbacks.Count - 1; num >= 0; num--)
		{
			if (callbacks[num].Callback == callback)
			{
				callbacks.RemoveAt(num);
				break;
			}
		}
	}

	public void Send(T sender)
	{
		if (hasTriggered && TriggerOnce)
		{
			return;
		}
		foreach (CallbackItem<T> callback in callbacks)
		{
			callback.Send(sender);
		}
		hasTriggered = true;
	}

	public override void Shutdown()
	{
		callbacks.Clear();
	}
}
public class SignalSenders<T, K> : SignalSendersBase
{
	private List<CallbackItem<T, K>> callbacks = new List<CallbackItem<T, K>>();

	public CallbackItem<T, K> AddReceiver(Action<T, K> callback)
	{
		callbacks.Add(new CallbackItem<T, K>(callback));
		return callbacks.LastItem();
	}

	public void RemoveReceiver(CallbackItem<T, K> item)
	{
		callbacks.Remove(item);
	}

	public void RemoveReceiver(Action<T, K> callback)
	{
		for (int num = callbacks.Count - 1; num >= 0; num--)
		{
			if (callbacks[num].Callback == callback)
			{
				callbacks.RemoveAt(num);
				break;
			}
		}
	}

	public void Send(T sender, K value)
	{
		if (hasTriggered && TriggerOnce)
		{
			return;
		}
		foreach (CallbackItem<T, K> callback in callbacks)
		{
			callback.Send(sender, value);
		}
		hasTriggered = true;
	}

	public override void Shutdown()
	{
		callbacks.Clear();
	}
}
public class SignalSenders<T, K, L, M> : SignalSendersBase
{
	private List<CallbackItem<T, K, L, M>> callbacks = new List<CallbackItem<T, K, L, M>>();

	public CallbackItem<T, K, L, M> AddReceiver(Action<T, K, L, M> callback)
	{
		callbacks.Add(new CallbackItem<T, K, L, M>(callback));
		return callbacks.LastItem();
	}

	public void RemoveReceiver(CallbackItem<T, K, L, M> item)
	{
		callbacks.Remove(item);
	}

	public void RemoveReceiver(Action<T, K, L, M> callback)
	{
		for (int num = callbacks.Count - 1; num >= 0; num--)
		{
			if (callbacks[num].Callback == callback)
			{
				callbacks.RemoveAt(num);
				break;
			}
		}
	}

	public void Send(T sender, K value, L value2, M value3)
	{
		if (hasTriggered && TriggerOnce)
		{
			return;
		}
		foreach (CallbackItem<T, K, L, M> callback in callbacks)
		{
			callback.Send(sender, value, value2, value3);
		}
		hasTriggered = true;
	}

	public override void Shutdown()
	{
		callbacks.Clear();
	}
}
