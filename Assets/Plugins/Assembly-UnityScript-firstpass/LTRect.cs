using System;
using UnityEngine;

[Serializable]
public class LTRect
{
	public Rect rect;

	public LTRect(float x, float y, float width, float height)
	{
		rect = new Rect(x, y, width, height);
	}
}
