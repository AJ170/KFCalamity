using UnityEngine;

public class StatisticsPageBase : GuiListButton
{
	private UISprite sprite;

	private Vector3 startSize = new Vector3(278f, 262f, 0f);

	private Vector3 maxSize = new Vector3(412f, 389f, 0f);

	private void Start()
	{
	}

	public override void SetButtonList(GuiList buttonList)
	{
		base.buttonList = buttonList;
	}

	public override void SetPosition(Vector3 position)
	{
		base.transform.position = position;
	}

	public override void SetAlpha(float amount)
	{
		if (alphaTween == null)
		{
			alphaTween = base.gameObject.GetComponent<TweenAlpha>();
		}
		alphaTween.to = amount;
		alphaTween.from = amount;
		alphaTween.duration = 0.1f;
		alphaTween.Play(true);
	}

	public override void SetImage(string image)
	{
		if (sprite == null)
		{
			sprite = base.gameObject.GetComponent<UISprite>();
		}
		sprite.spriteName = image;
	}

	public override void SetDepth(int depthIndex)
	{
		sprite.depth = depthIndex;
	}

	public void SetStartScale()
	{
		base.transform.localScale = startSize;
	}

	public void SetMaxScale()
	{
		base.transform.localScale = maxSize;
	}

	public override void Tween(Vector3 newPosition, TweenDirection direction, int amount, float alphaAmount, float tweenSpeed)
	{
		if (alphaTween == null)
		{
			alphaTween = base.gameObject.GetComponent<TweenAlpha>();
		}
		if (sprite == null)
		{
			sprite = base.gameObject.GetComponent<UISprite>();
		}
		if (positionTween == null)
		{
			positionTween = base.gameObject.GetComponent<TweenPosition>();
		}
		if (scaleTween == null)
		{
			scaleTween = base.gameObject.GetComponent<TweenScale>();
		}
		hasMoved = false;
		alphaTween.eventReceiver = null;
		Vector3 localPosition = base.transform.localPosition;
		float alpha = alphaTween.alpha;
		alphaTween.Reset2();
		if (alphaAmount == 1f)
		{
			scaleTween.Reset2();
			scaleTween.duration = tweenSpeed;
			scaleTween.to = maxSize;
			scaleTween.from = startSize;
			scaleTween.Play(true);
			if (alphaTween.eventReceiver == null)
			{
				alphaTween.eventReceiver = buttonList.gameObject;
			}
		}
		else if (alpha == 1f)
		{
			scaleTween.Reset2();
			scaleTween.duration = tweenSpeed;
			scaleTween.to = startSize;
			scaleTween.from = maxSize;
			scaleTween.Play(true);
		}
		positionTween.Reset();
		positionTween.duration = tweenSpeed;
		positionTween.to = newPosition;
		base.transform.localPosition = localPosition;
		positionTween.from = localPosition;
		alphaTween.from = alpha;
		alphaTween.duration = tweenSpeed;
		alphaTween.to = alphaAmount;
		positionTween.Play(true);
		alphaTween.Play(true);
	}

	public override void OnAlphaZero()
	{
		positionTween.Reset();
		alphaTween.Reset();
		hasMoved = true;
	}

	public override void OnSelect()
	{
	}

	public override void OnDeselect()
	{
	}
}
