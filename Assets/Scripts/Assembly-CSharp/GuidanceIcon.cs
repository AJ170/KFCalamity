using UnityEngine;

public class GuidanceIcon
{
	private GUIStyle guidanceSkin;

	private Rect guidanceRect;

	private Vector2 guidancePivot;

	private Transform guidanceTarget;

	private Vector3 guidanceTargetPosition;

	private Vector3 guidanceStartPosition;

	private float rotationAngle;

	private Texture icon;

	private Texture arrow;

	private Matrix4x4 guiMatrixBackup;

	private bool update;

	private bool visible;

	public GuidanceIcon(GUIStyle style, Transform target)
	{
		if (style != null)
		{
			guidanceTarget = target;
			guidanceSkin = style;
			icon = style.normal.background;
			arrow = style.hover.background;
			guidanceRect = ScaledRect.getRect(0f, 0f, guidanceSkin.fixedWidth, guidanceSkin.fixedHeight, Alignment.TopLeft);
			guidancePivot = new Vector2(0f, 0f);
			guidanceStartPosition = new Vector3(0f, 0f, 0f);
			update = true;
			visible = true;
		}
	}

	public void Update()
	{
		if (update)
		{
			guidanceTargetPosition = Camera.main.WorldToScreenPoint(guidanceTarget.position);
			float num = guidanceTargetPosition.x;
			float num2 = guidanceTargetPosition.y * -1f;
			Debug.Log(num + " - " + num2);
			if (num >= (float)Screen.width)
			{
				num = Screen.width - 100;
			}
			else if (num <= 0f)
			{
				num = 100f;
			}
			if (num2 >= (float)Screen.height)
			{
				num2 = Screen.height - 100;
			}
			else if (num2 <= 0f)
			{
				num2 = 100f;
			}
			guidanceRect = ScaledRect.getRect(num, num2, guidanceSkin.fixedWidth, guidanceSkin.fixedHeight, Alignment.Center);
			guidanceStartPosition.x = Screen.width / 2;
			guidanceStartPosition.y = Screen.height / 2;
			guidanceTargetPosition.y = (float)Screen.height - guidanceTargetPosition.y;
			if (guidanceTargetPosition.x < 0f)
			{
				num = 10f;
			}
			else if (!(guidanceTargetPosition.x > ScaledRect.customWidth))
			{
			}
			if (guidanceTargetPosition.y < 0f)
			{
				num2 = 10f;
			}
			guidancePivot.x = guidanceRect.x + guidanceRect.width / 2f;
			guidancePivot.y = guidanceRect.y + guidanceRect.height / 2f;
			Transform transform = Camera.main.transform;
			Vector3 position = transform.position;
			position.y = 0f;
			Vector3 position2 = guidanceTarget.position;
			position2.y = 0f;
			Vector3 from = position2 - position;
			Vector3 to = transform.TransformDirection(Vector3.forward);
			to.y = 0f;
			to.Normalize();
			float num3 = Vector3.Angle(from, to);
			Vector3 vector = guidanceTargetPosition.normalized - guidanceStartPosition.normalized;
			if (vector.x < 0f)
			{
				rotationAngle = 360f - num3;
			}
			else
			{
				rotationAngle = num3;
			}
			if (vector.x >= 0.22f)
			{
				visible = false;
			}
			else
			{
				visible = true;
			}
		}
	}

	public void Display()
	{
		if (visible)
		{
			GUI.Button(new Rect(guidanceStartPosition.x, guidanceStartPosition.y, 20f, 15f), "a");
			GUI.Button(new Rect(guidanceTargetPosition.x, guidanceTargetPosition.y, 20f, 15f), "b");
			guiMatrixBackup = GUI.matrix;
			GUI.DrawTexture(guidanceRect, icon);
			GUIUtility.RotateAroundPivot(rotationAngle, guidancePivot);
			GUI.DrawTexture(guidanceRect, arrow);
			GUI.matrix = guiMatrixBackup;
		}
	}

	public void Show()
	{
		visible = true;
	}

	public void Hide()
	{
		visible = false;
	}
}
