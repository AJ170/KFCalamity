using UnityEngine;

public class PauseMenuButton : MonoBehaviour
{
	public UILabel Label;

	public UISprite Sprite;

	public void SetText(string text)
	{
		Label.text = text;
		OnDeselect();
	}

	public void OnSelect()
	{
		Sprite.spriteName = "scoreboard_bar_white";
		Label.color = Color.black;
	}

	public void OnDeselect()
	{
		Sprite.spriteName = "scoreboard_bar_black";
		Label.color = Color.white;
	}
}
