using System;
using Calamity.Input;
using UnityEngine;

public class DoubleTapInterval : MonoBehaviour
{
	public float Interval = 0.5f;

	public float DoubleTapRange = 5f;

	public bool EnablingCountsAsPress = true;

	private BoxCollider cachedCollider;

	private Transform cachedTransform;

	private float lastPressTime;

	private bool waitingForSecondPress;

	private Vector3 firstTouchPos;

	private Action<DoubleTapInterval> onDoubleTap;

	public Action<DoubleTapInterval> OnDoubleTap
	{
		get
		{
			return onDoubleTap;
		}
		set
		{
			onDoubleTap = value;
		}
	}

	private void Awake()
	{
		cachedCollider = GetComponent<BoxCollider>();
		cachedTransform = GetComponent<Transform>();
	}

	private void OnEnable()
	{
		cachedCollider.enabled = true;
		if (EnablingCountsAsPress)
		{
			onPressReceived();
		}
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
		OnDoubleTap = null;
	}

	private void onPressReceived()
	{
		if (!waitingForSecondPress)
		{
			firstTouchPos = cachedTransform.localPosition;
			lastPressTime = Time.unscaledTime;
			waitingForSecondPress = true;
			return;
		}
		Vector3 localPosition = cachedTransform.localPosition;
		if ((localPosition - firstTouchPos).magnitude <= DoubleTapRange)
		{
			if (OnDoubleTap != null)
			{
				OnDoubleTap(this);
			}
			waitingForSecondPress = false;
			Disable();
		}
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			onPressReceived();
		}
	}

	private void Disable()
	{
		if (InputManager.Instance.StickType == TouchStickType.Dynamic)
		{
			cachedCollider.enabled = false;
		}
		waitingForSecondPress = false;
		lastPressTime = -1f;
	}

	public void Enable()
	{
		cachedCollider.enabled = true;
		if (EnablingCountsAsPress)
		{
			onPressReceived();
		}
	}

	private void Update()
	{
		if (lastPressTime > 0f && Time.unscaledTime - lastPressTime > Interval)
		{
			Disable();
		}
	}
}
