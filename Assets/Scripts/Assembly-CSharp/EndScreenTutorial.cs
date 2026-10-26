using UnityEngine;

public class EndScreenTutorial : MonoBehaviour
{
	public PopupWindow Window;

	public UISprite Banner;

	private string bannerWin;

	private string bannerLose;

	private GuiInGameManager manager;

	private bool update = true;

	public void SetEndResult(bool isWin)
	{
		Window.SetInGameManagerReference(manager);
		string empty = string.Empty;
		if (isWin)
		{
			Window.SetPopupInfo(PopupType.Question, Localizer.GetString("gui_message_tutorial_end_screen_win"));
			empty = "tutorial_end_banner_win";
		}
		else
		{
			Window.SetPopupInfo(PopupType.Question, Localizer.GetString("gui_message_tutorial_end_screen_lose"));
			empty = "tutorial_end_banner_lose";
		}
		Banner.spriteName = empty;
		Banner.MakePixelPerfect();
		Window.SetButton1(Localizer.GetString("gui_message_tutorial_end_screen_again"), PopupWindow.PopupState.Option1);
		Window.SetButton2(Localizer.GetString("gui_message_tutorial_end_screen_return"), PopupWindow.PopupState.Option2);
	}

	public void SetManagerReference(GuiInGameManager manager)
	{
		this.manager = manager;
	}

	private void Update()
	{
		if (!update)
		{
			return;
		}
		if (Window.Status == PopupWindow.PopupState.Option1)
		{
			manager.PlaySound(GuiInGameManager.Sound.Forward);
			update = false;
			GameState.Instance.ReloadGameState();
		}
		else if (Window.Status == PopupWindow.PopupState.Option2)
		{
			manager.PlaySound(GuiInGameManager.Sound.Forward);
			update = false;
			if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch && OptionManager.Instance.IsStickTypeTemporary)
			{
				GuiInGameManager.Instance.SpawnStickTypePopup();
			}
			else
			{
				GameState.Instance.ReloadMainMenu();
			}
		}
	}
}
