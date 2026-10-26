using System;
using UnityEngine;

public class Lerp01
{
	private float start;

	private float goal;

	private float speed;

	private Action callback;

	protected bool lerping;

	private float progress;

	protected float current;

	public float Current
	{
		get
		{
			return current;
		}
	}

	public bool Lerping
	{
		get
		{
			return lerping;
		}
	}

	public virtual void Update()
	{
		if (!lerping)
		{
			return;
		}
		progress = Mathf.Min(progress + speed * Time.deltaTime, 1f);
		current = Mathf.Lerp(start, goal, progress);
		if (progress == 1f)
		{
			lerping = false;
			if (callback != null)
			{
				callback();
			}
		}
	}

	public virtual bool Lerp(float start, float goal, float speed, Action callback, bool forceLerp = false)
	{
		if (!lerping || forceLerp)
		{
			Mathf.Clamp01(start);
			Mathf.Clamp01(goal);
			this.start = start;
			this.goal = goal;
			this.speed = speed;
			this.callback = callback;
			current = start;
			progress = 0f;
			lerping = true;
		}
		return lerping;
	}

	public void Stop(bool triggerCallback = false)
	{
		if (lerping)
		{
			lerping = false;
			if (triggerCallback && callback != null)
			{
				callback();
			}
		}
	}
}
