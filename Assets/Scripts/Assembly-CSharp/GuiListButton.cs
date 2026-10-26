using System;
using UnityEngine;

public abstract class GuiListButton : MonoBehaviour
{
	protected UILabel label;

	protected TweenAlpha alphaTween;

	protected TweenPosition positionTween;

	protected TweenScale scaleTween;

	protected GuiList buttonList;

	protected bool hasMoved;

	protected int totalTweenDistance;

	public string Name;

	private int index;

	private ButtonActions callbacks;

	public int Index
	{
		get
		{
			return index;
		}
	}

	public object CustomButtonData
	{
		get
		{
			return (!(callbacks == null)) ? callbacks.CustomData : null;
		}
		set
		{
			if (callbacks != null)
			{
				callbacks.CustomData = value;
			}
			else
			{
				Debug.LogError("Tried to set custom button data on a button that has no ButtonActions component", base.gameObject);
			}
		}
	}

	private void Awake()
	{
		callbacks = GetComponent<ButtonActions>();
		if (callbacks == null)
		{
			callbacks = GetComponentInChildren<ButtonActions>();
		}
	}

	private void Start()
	{
		label = base.gameObject.GetComponent<UILabel>();
		alphaTween = base.gameObject.GetComponent<TweenAlpha>();
		positionTween = base.gameObject.GetComponent<TweenPosition>();
		scaleTween = base.gameObject.GetComponent<TweenScale>();
		if (label == null || alphaTween == null || positionTween == null || scaleTween == null)
		{
			Debug.LogWarning("[ListButton] is missing component(s); UILabel/TweenAlpha/TweenPosition/TweenScale");
			return;
		}
		base.transform.localScale = new Vector3(36f, 36f, 0f);
		positionTween.callWhenFinished = "OnTweenComplete";
	}

	public virtual void SetButtonList(GuiList buttonList)
	{
		this.buttonList = buttonList;
	}

	public virtual void SetPosition(Vector3 position)
	{
		base.transform.position = position;
		label.MakePositionPerfect();
	}

	public virtual void SetDepth(int depthIndex)
	{
		label.depth = depthIndex;
	}

	public virtual void SetAlpha(float amount)
	{
		if (alphaTween == null)
		{
			alphaTween = base.gameObject.GetComponent<TweenAlpha>();
		}
		alphaTween.alpha = amount;
	}

	public virtual void SetText(string text)
	{
		Name = text;
		if (label == null)
		{
			label = base.gameObject.GetComponent<UILabel>();
		}
		label.text = text;
	}

	public virtual void SetImage(string image)
	{
	}

	public virtual void Tween(Vector3 newPosition, TweenDirection direction, int amount, float alphaAmount, float tweenSpeed)
	{
		if (positionTween != null)
		{
			hasMoved = false;
			if (alphaAmount == 1f)
			{
				positionTween.eventReceiver = buttonList.gameObject;
			}
			else
			{
				positionTween.eventReceiver = null;
			}
			Vector3 localPosition = base.transform.localPosition;
			float alpha = alphaTween.alpha;
			positionTween.Reset();
			alphaTween.Reset2();
			positionTween.duration = tweenSpeed;
			alphaTween.duration = tweenSpeed;
			base.transform.localPosition = localPosition;
			positionTween.from = localPosition;
			alphaTween.from = alpha;
			positionTween.to = newPosition;
			alphaTween.to = alphaAmount;
			positionTween.Play(true);
			alphaTween.Play(true);
		}
	}

	public virtual void OnAlphaZero()
	{
		positionTween.Reset();
		alphaTween.Reset();
		hasMoved = true;
	}

	public virtual void OnSelect()
	{
		if (scaleTween == null)
		{
			scaleTween = base.gameObject.GetComponent<TweenScale>();
		}
		scaleTween.gameObject.SetActive(true);
		scaleTween.style = UITweener.Style.Loop;
		scaleTween.Play(true);
	}

	public virtual void OnDeselect()
	{
		if (scaleTween == null)
		{
			scaleTween = base.gameObject.GetComponent<TweenScale>();
		}
		scaleTween.Reset();
		scaleTween.style = UITweener.Style.Once;
		scaleTween.Play(false);
	}

	public void SetIndex(int index)
	{
		this.index = index;
	}

	public void SetOnPressedCallback(Action<ButtonActions> onPressedCallback)
	{
		if (callbacks != null)
		{
			callbacks.SetOnClickCallback(onPressedCallback);
		}
	}
}
