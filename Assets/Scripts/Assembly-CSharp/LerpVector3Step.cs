using System;
using UnityEngine;

public class LerpVector3Step
{
	private Vector3 start;

	private Vector3 goal;

	private Vector3 current;

	private Vector3 step;

	private float speed;

	private Action callback;

	private bool lerping;

	private float goalDistance;

	public Vector3 Current
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

	public void Update()
	{
		if (!lerping)
		{
			return;
		}
		current += step * (speed * Time.deltaTime);
		float num = SqrDistance(start, current);
		if (num >= goalDistance)
		{
			current = goal;
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
			step = (goal - start).normalized;
			this.speed = speed;
			this.callback = callback;
			goalDistance = SqrDistance(start, goal);
			current = start;
			lerping = true;
		}
		return lerping;
	}

	private float SqrDistance(Vector3 from, Vector3 to)
	{
		return (to - from).sqrMagnitude;
	}
}
