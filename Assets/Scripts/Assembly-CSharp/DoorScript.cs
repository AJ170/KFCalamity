using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
	private enum InternalState
	{
		Closed = 0,
		Opening = 1,
		Open = 2,
		ClosingDelay = 3,
		Closing = 4
	}

	public float CloseDelay = 1f;

	public Animation Animator;

	public AnimationClip OpenClip;

	public AnimationClip CloseClip;

	private InternalState state;

	private List<Collider> colliders = new List<Collider>();

	private float counter;

	private void Start()
	{
		if (Animator == null)
		{
			Animator = GetComponent<Animation>();
		}
	}

	private void Update()
	{
		if (counter > 0f)
		{
			counter -= Time.deltaTime;
			if (counter <= 0f)
			{
				OnTimerEnd();
			}
		}
		else
		{
			if (colliders.Count <= 0)
			{
				return;
			}
			for (int num = colliders.Count - 1; num >= 0; num--)
			{
				if (colliders[num] == null || !colliders[num].gameObject.activeSelf)
				{
					colliders.RemoveAt(num);
					if (colliders.Count == 0)
					{
						OnAllCollidersGone();
					}
				}
			}
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		colliders.Add(collider);
		if (state == InternalState.Closed || state == InternalState.Closing)
		{
			Open();
		}
		else if (state == InternalState.ClosingDelay)
		{
			counter = 0f;
			state = InternalState.Open;
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (colliders.Remove(collider) && colliders.Count == 0)
		{
			OnAllCollidersGone();
		}
	}

	private void OnAllCollidersGone()
	{
		if (state == InternalState.Open)
		{
			if (CloseDelay > 0f)
			{
				StartCounter(CloseDelay);
				state = InternalState.ClosingDelay;
			}
			else
			{
				Close();
			}
		}
	}

	private void Open()
	{
		float num = 0f;
		if (state == InternalState.Closing)
		{
			num = Animator[CloseClip.name].length - Animator[CloseClip.name].time;
			Animator[OpenClip.name].time = num;
		}
		else
		{
			num = Animator[OpenClip.name].length;
		}
		Animator.Play(OpenClip.name);
		StartCounter(num);
		state = InternalState.Opening;
	}

	private void Close()
	{
		Animator.Play(CloseClip.name);
		StartCounter(CloseClip.length);
		state = InternalState.Closing;
	}

	private void StartCounter(float duration)
	{
		counter = duration;
	}

	private void StopCounter()
	{
		counter = 0f;
	}

	private void OnTimerEnd()
	{
		switch (state)
		{
		case InternalState.Opening:
			if (colliders.Count > 0)
			{
				state = InternalState.Open;
			}
			else if (CloseDelay > 0f)
			{
				StartCounter(CloseDelay);
				state = InternalState.ClosingDelay;
			}
			else
			{
				Close();
			}
			break;
		case InternalState.ClosingDelay:
			Close();
			break;
		case InternalState.Closing:
			state = InternalState.Closed;
			break;
		case InternalState.Open:
			break;
		}
	}
}
