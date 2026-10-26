using UnityEngine;

public class EndScreenMedal : MonoBehaviour
{
	public UISprite Sprite;

	public TweenAlpha Alpha;

	public TweenAlpha LabelAlpha;

	public TweenScale Scale;

	public UILabel Label;

	private Vector3 endPosition;

	private Vector3 endScale;

	private bool hasFaded;

	private GameObject endEventReciever;

	private string endEventCall;

	public void SetEndData(Vector3 position, Vector3 scale, EndScreen eventReciever, string eventCall, string medalImage, string text)
	{
		endPosition = position;
		endScale = scale;
		endEventReciever = eventReciever.gameObject;
		endEventCall = eventCall;
		Sprite.spriteName = medalImage;
		Label.text = text;
		LabelAlpha.enabled = true;
		LabelAlpha.Play(true);
	}

	public void Reposition()
	{
		Sprite.gameObject.transform.localPosition = endPosition;
		Alpha.Reset();
		Scale.Reset();
		Scale.to = endScale;
		Scale.duration = 0.3f;
		Scale.delay = 0f;
		Scale.eventReceiver = endEventReciever;
		Scale.callWhenFinished = endEventCall;
		Scale.Play(true);
		Alpha.Play(true);
	}

	public void OnTweenComplete()
	{
		Invoke("FadeOut", 2f);
	}

	private void FadeOut()
	{
		if (!hasFaded)
		{
			hasFaded = true;
			Alpha.callWhenFinished = "Reposition";
			Alpha.eventReceiver = base.gameObject;
			Alpha.Play(false);
			LabelAlpha.Play(false);
		}
	}
}
