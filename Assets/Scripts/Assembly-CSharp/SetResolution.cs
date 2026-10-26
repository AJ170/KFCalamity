using UnityEngine;

public class SetResolution : MonoBehaviour
{
	public int ScreenWidth = 1280;

	public int ScreenHeight = 720;

	public Vector2 iPadResolution = new Vector2(1024f, 768f);

	public bool debugResolution;

	private void Start()
	{
		// (PS Vita port) for the Android TV / Ouya builds; the PS Vita keeps its 960 x 544 screen
		if (!KFCompat.IsAndroid)
		{
			return;
		}
		Screen.SetResolution(ScreenWidth, ScreenHeight, true);
	}

	private void OnGUI()
	{
		if (debugResolution)
		{
			GUI.TextArea(new Rect((float)Screen.width * 0.1f, (float)Screen.height * 0.1f, 250f, 50f), "Width : " + Screen.width + " Height : " + Screen.height);
		}
	}
}
