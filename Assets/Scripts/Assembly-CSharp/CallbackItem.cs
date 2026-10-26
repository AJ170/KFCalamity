using System;

public class CallbackItem<T>
{
	private Action<T> callback;

	public Action<T> Callback
	{
		get
		{
			return callback;
		}
	}

	public CallbackItem(Action<T> callback)
	{
		this.callback = callback;
	}

	public void Send(T sender)
	{
		if (callback != null)
		{
			callback(sender);
		}
	}
}
public class CallbackItem<T, K>
{
	private Action<T, K> callback;

	public Action<T, K> Callback
	{
		get
		{
			return callback;
		}
	}

	public CallbackItem(Action<T, K> callback)
	{
		this.callback = callback;
	}

	public void Send(T sender, K value)
	{
		if (callback != null)
		{
			callback(sender, value);
		}
	}
}
public class CallbackItem<T, K, L, M>
{
	private Action<T, K, L, M> callback;

	public Action<T, K, L, M> Callback
	{
		get
		{
			return callback;
		}
	}

	public CallbackItem(Action<T, K, L, M> callback)
	{
		this.callback = callback;
	}

	public void Send(T sender, K value, L value2, M value3)
	{
		if (callback != null)
		{
			callback(sender, value, value2, value3);
		}
	}
}
