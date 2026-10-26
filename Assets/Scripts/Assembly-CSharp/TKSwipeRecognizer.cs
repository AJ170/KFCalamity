using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TKSwipeRecognizer : TKAbstractGestureRecognizer
{
	public float timeToSwipe = 0.5f;

	private float _allowedVariance = 35f;

	private float _minimumDistance = 40f;

	private TKSwipeDirection _swipesToDetect = TKSwipeDirection.All;

	private Vector2 _startPoint;

	private Vector2 _endPoint;

	private float _startTime;

	private TKSwipeDirection _swipeDetectionState;

	public float swipeVelocity { get; private set; }

	public TKSwipeDirection completedSwipeDirection { get; private set; }

	public Vector2 startPoint
	{
		get
		{
			return _startPoint;
		}
	}

	public Vector2 endPoint
	{
		get
		{
			return _endPoint;
		}
	}

	[method: MethodImpl(32)]
	public event Action<TKSwipeRecognizer> gestureRecognizedEvent;

	public TKSwipeRecognizer()
		: this(40f, 35f)
	{
	}

	public TKSwipeRecognizer(TKSwipeDirection swipesToDetect)
		: this(swipesToDetect, 40f, 35f)
	{
	}

	public TKSwipeRecognizer(float minimumDistance, float allowedVariance)
		: this(TKSwipeDirection.All, minimumDistance, allowedVariance)
	{
	}

	public TKSwipeRecognizer(TKSwipeDirection swipesToDetect, float minimumDistance, float allowedVariance)
	{
		_swipesToDetect = swipesToDetect;
		_minimumDistance = minimumDistance * TouchKit.instance.runtimeDistanceModifier;
		_allowedVariance = allowedVariance * TouchKit.instance.runtimeDistanceModifier;
	}

	private bool checkForSwipeCompletion(TKTouch touch)
	{
		if (timeToSwipe > 0f && Time.time - _startTime > timeToSwipe)
		{
			base.state = TKGestureRecognizerState.FailedOrEnded;
			return false;
		}
		if (touch.deltaPosition.x > 0f)
		{
			_swipeDetectionState &= ~TKSwipeDirection.Left;
		}
		if (touch.deltaPosition.x < 0f)
		{
			_swipeDetectionState &= ~TKSwipeDirection.Right;
		}
		if (touch.deltaPosition.y < 0f)
		{
			_swipeDetectionState &= ~TKSwipeDirection.Up;
		}
		if (touch.deltaPosition.y > 0f)
		{
			_swipeDetectionState &= ~TKSwipeDirection.Down;
		}
		float num = Mathf.Abs(_startPoint.x - touch.position.x);
		float num2 = Mathf.Abs(_startPoint.y - touch.position.y);
		_endPoint = touch.position;
		if ((_swipeDetectionState & TKSwipeDirection.Left) != 0 && num > _minimumDistance)
		{
			if (num2 < _allowedVariance)
			{
				completedSwipeDirection = TKSwipeDirection.Left;
				swipeVelocity = num / (Time.time - _startTime);
				return true;
			}
			_swipeDetectionState &= ~TKSwipeDirection.Left;
		}
		if ((_swipeDetectionState & TKSwipeDirection.Right) != 0 && num > _minimumDistance)
		{
			if (num2 < _allowedVariance)
			{
				completedSwipeDirection = TKSwipeDirection.Right;
				swipeVelocity = num / (Time.time - _startTime);
				return true;
			}
			_swipeDetectionState &= ~TKSwipeDirection.Right;
		}
		if ((_swipeDetectionState & TKSwipeDirection.Up) != 0 && num2 > _minimumDistance)
		{
			if (num < _allowedVariance)
			{
				completedSwipeDirection = TKSwipeDirection.Up;
				swipeVelocity = num2 / (Time.time - _startTime);
				return true;
			}
			_swipeDetectionState &= ~TKSwipeDirection.Up;
		}
		if ((_swipeDetectionState & TKSwipeDirection.Down) != 0 && num2 > _minimumDistance)
		{
			if (num < _allowedVariance)
			{
				completedSwipeDirection = TKSwipeDirection.Down;
				swipeVelocity = num2 / (Time.time - _startTime);
				return true;
			}
			_swipeDetectionState &= ~TKSwipeDirection.Down;
		}
		return false;
	}

	internal override void fireRecognizedEvent()
	{
		if (this.gestureRecognizedEvent != null)
		{
			this.gestureRecognizedEvent(this);
		}
	}

	internal override bool touchesBegan(List<TKTouch> touches)
	{
		if (base.state == TKGestureRecognizerState.Possible)
		{
			_swipeDetectionState = _swipesToDetect;
			_startPoint = touches[0].position;
			_startTime = Time.time;
			_trackingTouches.Add(touches[0]);
			base.state = TKGestureRecognizerState.Began;
		}
		return false;
	}

	internal override void touchesMoved(List<TKTouch> touches)
	{
		if (base.state == TKGestureRecognizerState.Began && checkForSwipeCompletion(touches[0]))
		{
			base.state = TKGestureRecognizerState.Recognized;
		}
	}

	internal override void touchesEnded(List<TKTouch> touches)
	{
		base.state = TKGestureRecognizerState.FailedOrEnded;
	}

	public override string ToString()
	{
		return string.Format("{0}, swipe direction: {1}, swipe velocity: {2}, start point: {3}, end point: {4}", base.ToString(), completedSwipeDirection, swipeVelocity, startPoint, endPoint);
	}
}
