using UnityEngine;

public class EndGameScreen
{
	private Texture endScreenTexture;

	private bool visible;

	private Rect endGameRect;

	private Rect endTitleRect;

	private string endScreenText;

	private GUIStyle endStyle;

	public EndGameScreen(bool isWinner, GUIStyle Style)
	{
		endStyle = new GUIStyle();
		endStyle.alignment = TextAnchor.UpperCenter;
		endStyle.font = Style.font;
		endStyle.normal.textColor = Color.white;
		endStyle.fontSize = Style.fontSize;
		if (isWinner)
		{
			endScreenTexture = Style.normal.background;
			endScreenText = Localizer.GetString("gui_win");
		}
		else
		{
			endScreenTexture = Style.hover.background;
			endScreenText = Localizer.GetString("gui_lose");
		}
		endGameRect = ScaledRect.getRect(ScaledRect.customWidth / 2f, ScaledRect.customHeight / 2f, ScaledRect.customWidth, ScaledRect.customHeight, Alignment.Center);
		endTitleRect = ScaledRect.getRect(ScaledRect.customWidth / 2f, ScaledRect.customHeight / 2f, 400f, 120f, Alignment.Center);
		visible = true;
	}

	public void Display()
	{
		if (visible)
		{
			GUI.DrawTexture(endGameRect, endScreenTexture);
			GUI.Label(endTitleRect, endScreenText, endStyle);
		}
	}
}
