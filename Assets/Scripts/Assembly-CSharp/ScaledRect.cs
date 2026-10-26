using UnityEngine;

public class ScaledRect : MonoBehaviour
{
	public static float customWidth = 1280f;

	public static float customHeight = 720f;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public static Rect getRect(float x, float y, float width, float height, Alignment align)
	{
		x = x * (float)Screen.width / customWidth;
		y = y * (float)Screen.height / customHeight;
		width = width * (float)Screen.width / customWidth;
		height = height * (float)Screen.height / customHeight;
		Rect alignRect = new Rect(x, y, width, height);
		return getAlignPosition(align, alignRect);
	}

	public static Rect getRectInRect(float x, float y, float width, float height, Rect relativeRect, Alignment align)
	{
		x = x * relativeRect.width / customWidth;
		y = y * relativeRect.height / customHeight;
		width = width * relativeRect.width / customWidth;
		height = height * relativeRect.height / customHeight;
		Rect alignRect = new Rect(x, y, width, height);
		return getAlignPosition(align, alignRect);
	}

	private static Rect getAlignPosition(Alignment align, Rect AlignRect)
	{
		switch (align)
		{
		case Alignment.TopLeft:
			return AlignRect;
		case Alignment.TopCenter:
			return new Rect(AlignRect.x - AlignRect.width / 2f, AlignRect.y, AlignRect.width, AlignRect.height);
		case Alignment.TopRight:
			return new Rect(AlignRect.x - AlignRect.width, AlignRect.y, AlignRect.width, AlignRect.height);
		case Alignment.CenterLeft:
			return new Rect(AlignRect.x, AlignRect.y - AlignRect.height / 2f, AlignRect.width, AlignRect.height);
		case Alignment.Center:
			return new Rect(AlignRect.x - AlignRect.width / 2f, AlignRect.y - AlignRect.height / 2f, AlignRect.width, AlignRect.height);
		case Alignment.CenterRight:
			return new Rect(AlignRect.x - AlignRect.width, AlignRect.y - AlignRect.height / 2f, AlignRect.width, AlignRect.height);
		case Alignment.BottomLeft:
			return new Rect(AlignRect.x, AlignRect.y - AlignRect.height, AlignRect.width, AlignRect.height);
		case Alignment.BottomCenter:
			return new Rect(AlignRect.x - AlignRect.width / 2f, AlignRect.y - AlignRect.height, AlignRect.width, AlignRect.height);
		case Alignment.BottomRight:
			return new Rect(AlignRect.x - AlignRect.width, AlignRect.y - AlignRect.height, AlignRect.width, AlignRect.height);
		default:
			return AlignRect;
		}
	}
}
