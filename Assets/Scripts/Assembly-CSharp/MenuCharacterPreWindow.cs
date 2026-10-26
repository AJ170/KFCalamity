using UnityEngine;

public class MenuCharacterPreWindow : MonoBehaviour
{
	public UILabel Label;

	public UISprite Sprite;

	public TweenAlpha LabelAlpha;

	public TweenAlpha SpriteAlpha;

	private bool isOutTweeened;

	public bool OutTweened
	{
		get
		{
			return isOutTweeened;
		}
	}

	public void SetText(string text)
	{
		Label.text = text;
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public void TweenOut()
	{
		isOutTweeened = true;
		LabelAlpha.Play(false);
		SpriteAlpha.Play(false);
	}

	public void TweenIn()
	{
		isOutTweeened = false;
		LabelAlpha.Play(true);
		SpriteAlpha.Play(true);
	}
}
