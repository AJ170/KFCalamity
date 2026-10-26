using System;
using UnityEngine;

public class LerpColor
{
	private Color start;

	private Color goal;

	private Color current;

	private float speed;

	private Action callback;

	private bool lerping;

	private float progress;

	public Color CurrentColor
	{
		get
		{
			return current;
		}
		set
		{
			current = value;
		}
	}

	public bool Lerping
	{
		get
		{
			return lerping;
		}
	}

	public void Update()
	{
		if (!lerping)
		{
			return;
		}
		progress = Mathf.Min(progress + speed * Time.deltaTime, 1f);
		current = Color.Lerp(start, goal, progress);
		if (progress == 1f)
		{
			lerping = false;
			if (callback != null)
			{
				callback();
			}
		}
	}

	public bool Lerp(Color start, Color goal, float speed, Action callback, bool forceLerp = false)
	{
		if (!lerping || forceLerp)
		{
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
