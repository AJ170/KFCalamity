using UnityEngine;

public class ScaledFont : MonoBehaviour
{
	public static float customWidth = 1280f;

	public static float customHeight = 720f;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public static int getFontSize(int fontSize)
	{
		float num = (float)Screen.width / customWidth;
		float f = num * (float)fontSize;
		return Mathf.CeilToInt(f);
	}
}
