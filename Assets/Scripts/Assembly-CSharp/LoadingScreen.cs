using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
	public Texture[] Screens;

	public Font Font;

	private string loadingText;

	private GUIStyle textStyle;

	private Rect bgRect;

	private Rect textRect;

	private Rect textRect2;

	private float rotation;

	private int randomTextureIndex;

	private void Start()
	{
		bgRect = new Rect(0f, 0f, Screen.width, Screen.height);
		textRect = ScaledRect.getRect(ScaledRect.customWidth * 0.95f, ScaledRect.customHeight * 0.95f, 128f, 42f, Alignment.BottomRight);
		textRect2 = ScaledRect.getRect(ScaledRect.customWidth * 0.9515f, ScaledRect.customHeight * 0.954f, 128f, 42f, Alignment.BottomRight);
		textStyle = new GUIStyle();
		textStyle.alignment = TextAnchor.UpperLeft;
		textStyle.font = Font;
		textStyle.fontSize = 30;
		textStyle.normal.textColor = Color.white;
		loadingText = "Loading";
		randomTextureIndex = Random.Range(0, Screens.Length);
	}

	private void OnDestroy()
	{
		Screens = null;
		Resources.UnloadUnusedAssets();
	}

	private void Update()
	{
		rotation += 210f * Time.deltaTime;
		if (rotation >= 360f)
		{
			rotation = 0f;
		}
		if (rotation < 360f && rotation > 150f)
		{
			loadingText = "Loading...";
		}
		if (rotation < 150f && rotation > 75f)
		{
			loadingText = "Loading..";
		}
		if (rotation > 0f && rotation < 75f)
		{
			loadingText = "Loading.";
		}
	}

	private void OnGUI()
	{
		if (Screens[randomTextureIndex] != null)
		{
			GUI.DrawTexture(bgRect, Screens[randomTextureIndex]);
		}
		textStyle.normal.textColor = Color.black;
		GUI.Label(textRect2, loadingText, textStyle);
		textStyle.normal.textColor = Color.white;
		GUI.Label(textRect, loadingText, textStyle);
	}
}
