using UnityEngine;

public class DevelopmentDisplay : MonoBehaviour
{
	[HideInInspector]
	public static string OverrideString = string.Empty;

	public string Version;

	public bool ShowVersion = true;

	public bool ShowFPS = true;

	private GUIText gui;

	private float updateInterval = 1f;

	private float lastInterval;

	private int frames;

	private string fpsString = string.Empty;

	private void Start()
	{
		gui = base.gameObject.AddComponent<GUIText>();
		gui.pixelOffset = new Vector2(5f, 20f);
		lastInterval = Time.unscaledTime;
	}

	private void Update()
	{
		string text = string.Empty;
		if (!string.IsNullOrEmpty(OverrideString))
		{
			text = OverrideString;
		}
		else
		{
			if (ShowVersion)
			{
				text = text + "Build version: " + Version + "    ";
			}
			if (ShowFPS)
			{
				frames++;
				float unscaledTime = Time.unscaledTime;
				if (unscaledTime > lastInterval + updateInterval)
				{
					float a = (float)frames / (unscaledTime - lastInterval);
					fpsString = (1000f / Mathf.Max(a, 1E-05f)).ToString("F") + "ms " + a.ToString("F") + "FPS";
					frames = 0;
					lastInterval = unscaledTime;
				}
				text += fpsString;
			}
		}
		gui.text = text;
	}
}
