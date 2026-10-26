using System.Collections.Generic;
using UnityEngine;

public class ControllerTypePopup : MonoBehaviour
{
	public UILabel TextLabel;

	private bool handleAxisInput;

	private int buttonIndex;

	public List<GuiButton> Buttons;

	private List<GuiButton> activeButtonList;

	private GuiMenuManager manager;

	private GuiInGameManager managerInGame;

	public List<GuiButton> ActiveButtonList
	{
		get
		{
			return activeButtonList;
		}
		set
		{
			activeButtonList = value;
		}
	}

	public int ButtonIndex
	{
		get
		{
			return buttonIndex;
		}
		set
		{
			buttonIndex = value;
		}
	}

	private void Start()
	{
		if (activeButtonList != null && activeButtonList[buttonIndex] != null)
		{
			activeButtonList[buttonIndex].OnSelect();
		}
	}

	public void SelectReset()
	{
		buttonIndex = 0;
		if (activeButtonList != null && activeButtonList[buttonIndex] != null)
		{
			activeButtonList[buttonIndex].OnSelect();
		}
	}

	public void SetActiveButtons(string titleText, int visibleButtons)
	{
		TextLabel.text = titleText;
		ActiveButtonList = new List<GuiButton>();
		for (int i = 0; i < Buttons.Count; i++)
		{
			if (visibleButtons > i)
			{
				Buttons[i].gameObject.SetActive(true);
				activeButtonList.Add(Buttons[i]);
			}
			else if (i != 4)
			{
				Buttons[i].gameObject.SetActive(false);
			}
		}
		activeButtonList.Add(Buttons[4]);
		Buttons[4].SetText(Localizer.GetString("gui_options_cancel"));
		activeButtonList[buttonIndex].OnSelect();
		handleAxisInput = true;
	}

	public void Reset()
	{
		TextLabel.text = string.Empty;
		activeButtonList[buttonIndex].OnDeselect();
		buttonIndex = 0;
		handleAxisInput = false;
		ActiveButtonList = null;
	}

	public void SetManagerReference(GuiMenuManager reference)
	{
		manager = reference;
	}

	public void SetInGameManagerReference(GuiInGameManager reference)
	{
		managerInGame = reference;
	}

	public void SetButtonTextByIndex(int index, string text)
	{
		if (activeButtonList[index] != null)
		{
			activeButtonList[index].SetText(text);
		}
	}

	public void HandleVerticalAxisInput(int axis)
	{
		if (axis == 0 || !handleAxisInput)
		{
			return;
		}
		if (manager != null)
		{
			manager.PlaySound(GuiMenuManager.Sound.CharButton);
		}
		else if (managerInGame != null)
		{
			managerInGame.PlaySound(GuiInGameManager.Sound.ShopList);
		}
		activeButtonList[buttonIndex].OnDeselect();
		if (axis > 0)
		{
			if (buttonIndex == 0)
			{
				buttonIndex = activeButtonList.Count - 1;
			}
			else
			{
				buttonIndex--;
			}
		}
		else if (axis < 0)
		{
			if (buttonIndex == activeButtonList.Count - 1)
			{
				buttonIndex = 0;
			}
			else
			{
				buttonIndex++;
			}
		}
		activeButtonList[buttonIndex].OnSelect();
	}

	private void ResetDelay()
	{
		handleAxisInput = true;
	}

	private void OnDestroy()
	{
	}
}
