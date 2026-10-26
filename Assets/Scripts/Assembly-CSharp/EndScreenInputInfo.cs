using UnityEngine;

public class EndScreenInputInfo : MonoBehaviour
{
	public enum InputType
	{
		None = 0,
		Forward = 1,
		Backward = 2,
		Bumper = 3
	}

	public enum Row
	{
		Top = 0,
		Bottom = 1
	}

	public UILabel BottomLabel;

	public UILabel TopLabel;

	public UILabel TopScrollLabel;

	public UISprite BottomButtonSprite;

	public UISprite TopButtonSprite;

	public UISprite LeftBumperSprite;

	public UISprite RightBumperSprite;

	public TweenPosition PositionTween;

	public bool IsVisible;

	public void SetButtonInput(InputType type, Row row, string text, bool visible)
	{
	}

	public void HideAll()
	{
		BottomButtonSprite.gameObject.SetActive(false);
		TopButtonSprite.gameObject.SetActive(false);
		LeftBumperSprite.gameObject.SetActive(false);
		RightBumperSprite.gameObject.SetActive(false);
		BottomLabel.text = string.Empty;
		TopLabel.text = string.Empty;
		TopScrollLabel.text = string.Empty;
	}

	public void TweenIn()
	{
		PositionTween.enabled = true;
		PositionTween.Play(true);
		IsVisible = true;
	}

	public void TweenOut()
	{
		PositionTween.enabled = true;
		PositionTween.Play(false);
		IsVisible = false;
	}
}
