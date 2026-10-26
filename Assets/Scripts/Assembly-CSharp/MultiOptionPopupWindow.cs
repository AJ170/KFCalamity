using System;

public class MultiOptionPopupWindow : PopupWindow
{
	public UILabel MainHeader;

	public UILabel Header1;

	public UILabel Header2;

	public UILabel Text1;

	public UILabel Text2;

	protected override void OnStart()
	{
	}

	public override void SetPopupInfo(PopupType type, string text)
	{
		throw new NotImplementedException("SetPopupInfo(PopupType, string): Can only be called on the PopupWindow class");
	}

	public override void SetMultiOptionPopupWindowInfo(string mainHeader, string header1, string header2, string text1, string text2)
	{
		if (MainHeader != null)
		{
			MainHeader.text = mainHeader;
		}
		if (Header1 != null)
		{
			Header1.text = header1;
		}
		if (header2 != null)
		{
			Header2.text = header2;
		}
		if (Text1 != null)
		{
			Text1.text = text1;
		}
		if (Text2 != null)
		{
			Text2.text = text2;
		}
		AddButton(Button1);
		AddButton(Button2);
		handleInput = true;
	}

	protected override void Update()
	{
		base.Update();
	}
}
