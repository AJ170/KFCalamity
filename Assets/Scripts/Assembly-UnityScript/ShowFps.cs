using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class ShowFps : MonoBehaviour
{
	private GUIText gui;

	private float updateInterval;

	private double lastInterval;

	private int frames;

	public ShowFps()
	{
		updateInterval = 1f;
	}

	public virtual void Start()
	{
		lastInterval = Time.realtimeSinceStartup;
		frames = 0;
	}

	public virtual void OnDisable()
	{
		if ((bool)gui)
		{
			UnityEngine.Object.DestroyImmediate(gui.gameObject);
		}
	}

	public virtual void Update()
	{
		frames++;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (!((double)realtimeSinceStartup <= lastInterval + (double)updateInterval))
		{
			if (!gui)
			{
				GameObject gameObject = new GameObject("FPS Display", typeof(GUIText));
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				gameObject.transform.position = new Vector3(0f, 0f, 0f);
				gui = gameObject.GetComponent<GUIText>();
				gui.pixelOffset = new Vector2(5f, 55f);
			}
			float a = (float)((double)frames / ((double)realtimeSinceStartup - lastInterval));
			float num = 1000f / Mathf.Max(a, 1E-05f);
			gui.text = num.ToString("f1") + "ms " + a.ToString("f2") + "FPS";
			frames = 0;
			lastInterval = realtimeSinceStartup;
		}
	}

	public virtual void Main()
	{
	}
}
