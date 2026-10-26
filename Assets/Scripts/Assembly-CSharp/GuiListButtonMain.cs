using UnityEngine;

public class GuiListButtonMain : GuiListButton
{
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

	private void Update()
	{
	}

	public override void OnAlphaZero()
	{
		base.OnAlphaZero();
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

	public override void SetButtonList(GuiList buttonList)
	{
		base.SetButtonList(buttonList);
	}

	public override void SetImage(string image)
	{
	}

	public override void SetPosition(Vector3 position)
	{
		base.SetPosition(position);
	}

	public override void SetText(string text)
	{
		Name = text;
		if (label == null)
		{
			label = base.gameObject.GetComponent<UILabel>();
		}
		label.text = Localizer.GetString(text);
	}

	public override void Tween(Vector3 newPosition, TweenDirection direction, int amount, float alphaAmount, float tweenSpeed)
	{
		base.Tween(newPosition, direction, amount, alphaAmount, tweenSpeed);
	}

	public override void OnSelect()
	{
		base.OnSelect();
	}

	public override void OnDeselect()
	{
		base.OnDeselect();
	}
}
