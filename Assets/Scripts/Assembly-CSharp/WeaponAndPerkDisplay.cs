using UnityEngine;

public class WeaponAndPerkDisplay : MonoBehaviour
{
	public UILabel Label;

	public UISprite Sprite;

	public TweenAlpha LabelAlphaTween;

	public TweenAlpha SpriteAlphaTween;

	private void Start()
	{
	}

	public void SetData(GameObject eventReciever, string eventCall, string image, string text)
	{
		SpriteAlphaTween.Reset();
		LabelAlphaTween.Reset();
		SpriteAlphaTween.eventReceiver = eventReciever;
		SpriteAlphaTween.callWhenFinished = eventCall;
		Sprite.spriteName = image;
		Sprite.MakePixelPerfect();
		Label.text = text;
		SpriteAlphaTween.Play(true);
		LabelAlphaTween.Play(true);
	}
}
