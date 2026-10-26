using UnityEngine;

public class MenuCharacterTitleBar : MonoBehaviour
{
	private const float DISABLED_ALPHA = 0.6f;

	public UISprite LeftArrow;

	public UISprite RightArrow;

	public UISprite Background;

	public UILabel Label;

	public UILabel TitleLabel;

	public UISprite Sprite;

	public bool ScaleOnFocus;

	public bool RequiresActivation;

	private bool isPressed;

	private TweenScale scaleTween;

	private bool isActive = true;

	private static Color DISABLED_COLOR = Color.gray;

	public bool Active
	{
		get
		{
			return isActive;
		}
		set
		{
			if (!isActive && value)
			{
				if (LeftArrow != null)
				{
					LeftArrow.spriteName = "arrow_left_black_outline";
				}
				if (RightArrow != null)
				{
					RightArrow.spriteName = "arrow_right_black_outline";
				}
				DoScale(true);
			}
			else if (isActive && !value)
			{
				if (LeftArrow != null)
				{
					LeftArrow.spriteName = "arrow_left_white_outline";
				}
				if (RightArrow != null)
				{
					RightArrow.spriteName = "arrow_right_white_outline";
				}
			}
			isActive = value;
			if (!isActive)
			{
				OnScaleDone(scaleTween);
			}
		}
	}

	private void Awake()
	{
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
		{
			RequiresActivation = true;
		}
	}

	private void Start()
	{
		if (RequiresActivation)
		{
			isActive = false;
		}
	}

	public void OnEnable()
	{
		OnToggleEnabled(true);
	}

	public void OnDisable()
	{
		OnToggleEnabled(false);
	}

	public void OnToggleEnabled(bool enabled)
	{
		if (HasArrows())
		{
			LeftArrow.gameObject.SetActive(enabled);
			RightArrow.gameObject.SetActive(enabled);
		}
		if (Label != null)
		{
			Label.color = ((!enabled) ? DISABLED_COLOR : Color.white);
			Label.alpha = ((!enabled) ? 0.6f : 1f);
		}
		if (TitleLabel != null)
		{
			TitleLabel.color = ((!enabled) ? DISABLED_COLOR : Color.white);
			TitleLabel.alpha = ((!enabled) ? 0.6f : 1f);
		}
	}

	private bool HasArrows()
	{
		return LeftArrow != null && RightArrow != null;
	}

	private void OnLeftPress()
	{
		if (LeftArrow != null && (!RequiresActivation || (RequiresActivation && Active)))
		{
			isPressed = true;
			LeftArrow.spriteName = "arrow_left_white_outline";
			Invoke("OnButtonRelease", 0.1f);
		}
	}

	private void OnRightPress()
	{
		if (RightArrow != null && (!RequiresActivation || (RequiresActivation && Active)))
		{
			isPressed = true;
			RightArrow.spriteName = "arrow_right_white_outline";
			Invoke("OnButtonRelease", 0.1f);
		}
	}

	public void OnButtonRelease()
	{
		if (HasArrows())
		{
			isPressed = false;
			LeftArrow.spriteName = "arrow_left_black_outline";
			RightArrow.spriteName = "arrow_right_black_outline";
		}
	}

	public void OnHorizontalInput(int axis)
	{
		if (isPressed)
		{
			return;
		}
		switch (axis)
		{
		case -1:
			OnLeftPress();
			return;
		case 1:
			OnRightPress();
			return;
		}
		if (!RequiresActivation)
		{
			OnButtonRelease();
		}
	}

	public void SetText(string text)
	{
		if (Label != null)
		{
			Label.text = text;
		}
	}

	public void OnFocusGained()
	{
		if (Label != null)
		{
			Label.color = Color.black;
		}
		if (Background != null)
		{
			Background.spriteName = "character_select_bar_white";
		}
		if (HasArrows() && !RequiresActivation)
		{
			LeftArrow.spriteName = "arrow_left_black_outline";
			RightArrow.spriteName = "arrow_right_black_outline";
		}
		DoScale(true);
	}

	private void OnScaleDone(UITweener tweener)
	{
		if (!Active)
		{
			TweenScale tweenScale = tweener as TweenScale;
			tweenScale.duration = 1.2f;
			tweenScale.style = UITweener.Style.PingPong;
			tweenScale.callWhenFinished = string.Empty;
			tweenScale.eventReceiver = null;
			tweenScale.Play(false);
		}
	}

	private void DoScale(bool forward)
	{
		if (ScaleOnFocus)
		{
			if (forward)
			{
				scaleTween = UITweener.Begin<TweenScale>(base.gameObject, 0.2f);
				scaleTween.from = base.transform.localScale;
				scaleTween.to = Vector3.one * 1.1f;
				scaleTween.method = UITweener.Method.EaseInOut;
				scaleTween.style = UITweener.Style.Once;
				scaleTween.duration = 0.15f;
				scaleTween.callWhenFinished = "OnScaleDone";
				scaleTween.eventReceiver = base.gameObject;
				scaleTween.Play(true);
				scaleTween.from = Vector3.one;
			}
			else if (scaleTween != null)
			{
				scaleTween.callWhenFinished = string.Empty;
				scaleTween.eventReceiver = null;
				scaleTween.method = UITweener.Method.EaseInOut;
				scaleTween.style = UITweener.Style.Once;
				scaleTween.duration = 0.3f;
				scaleTween.Play(false);
			}
		}
	}

	public void OnFocusLost()
	{
		if (Label != null)
		{
			Label.color = Color.white;
		}
		if (Background != null)
		{
			Background.spriteName = "character_select_bar_black";
		}
		if (HasArrows())
		{
			LeftArrow.spriteName = "arrow_left_white_outline";
			RightArrow.spriteName = "arrow_right_white_outline";
		}
		DoScale(false);
	}

	public void OnFocusGained2()
	{
		if (Background != null)
		{
			Background.spriteName = "character_select_bar_white";
		}
		if (HasArrows() && !RequiresActivation)
		{
			LeftArrow.spriteName = "arrow_left_black_outline";
			RightArrow.spriteName = "arrow_right_black_outline";
		}
		DoScale(true);
	}

	public void OnFocusLost2()
	{
		if (Background != null)
		{
			Background.spriteName = "character_select_bar_black";
		}
		if (HasArrows())
		{
			LeftArrow.spriteName = "arrow_left_white_outline";
			RightArrow.spriteName = "arrow_right_white_outline";
		}
		if (TitleLabel != null)
		{
			TitleLabel.effectStyle = UILabel.Effect.None;
			TitleLabel.color = Color.white;
			TitleLabel.MarkAsChanged();
		}
		DoScale(false);
	}
}
