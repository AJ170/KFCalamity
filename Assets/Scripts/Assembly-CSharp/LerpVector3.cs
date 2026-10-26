using System;
using UnityEngine;

public class LerpVector3
{
	private Vector3 start;

	private Vector3 goal;

	private Vector3 current;

	private float speed;

	private Action callback;

	private bool lerping;

	private float progress;

	public Vector3 Current
	{
		get
		{
			return current;
		}
	}

	public float Progress
	{
		get
		{
			return progress;
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
		current = Vector3.Lerp(start, goal, progress);
		if (progress == 1f)
		{
			lerping = false;
			if (callback != null)
			{
				callback();
			}
		}
	}

	public bool Lerp(Vector3 start, Vector3 goal, float speed, Action callback, bool forceLerp = false)
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
}
