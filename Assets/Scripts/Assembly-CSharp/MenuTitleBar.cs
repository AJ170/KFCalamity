using UnityEngine;

public class MenuTitleBar : MonoBehaviour
{
	public UISprite LeftArrow;

	public UISprite RightArrow;

	public UISprite Background;

	public UILabel Label;

	private bool isPressed;

	private void Start()
	{
	}

	private void OnLeftPress()
	{
		isPressed = true;
		LeftArrow.spriteName = "arrow_left_white_outline";
		Invoke("OnButtonRelease", 0.1f);
	}

	private void OnRightPress()
	{
		isPressed = true;
		RightArrow.spriteName = "arrow_right_white_outline";
		Invoke("OnButtonRelease", 0.1f);
	}

	public void OnButtonRelease()
	{
		isPressed = false;
		LeftArrow.spriteName = "arrow_left_black_outline";
		RightArrow.spriteName = "arrow_right_black_outline";
	}

	public void OnHorizontalInput(int axis)
	{
		if (!isPressed)
		{
			switch (axis)
			{
			case -1:
				OnLeftPress();
				break;
			case 1:
				OnRightPress();
				break;
			default:
				OnButtonRelease();
				break;
			}
		}
	}

	public void SetText(string text)
	{
		Label.text = text;
	}

	public void OnFocusGained()
	{
		Label.color = Color.black;
		if (Background != null)
		{
			Background.spriteName = "level_select_bar_white";
		}
		LeftArrow.spriteName = "arrow_left_black_outline";
		RightArrow.spriteName = "arrow_right_black_outline";
	}

	public void OnFocusLost()
	{
		Label.color = Color.white;
		if (Background != null)
		{
			Background.spriteName = "level_select_bar_black";
		}
		LeftArrow.spriteName = "arrow_left_white_outline";
		RightArrow.spriteName = "arrow_right_white_outline";
	}
}
