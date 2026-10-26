using System;
using Calamity.Input;

public class CheatButtonEntry : CheatEntryBase
{
	private int correctButtonIndex = -1;

	private InputAction[] buttons;

	public CheatButtonEntry(CheatButton button)
	{
		buttons = new InputAction[4];
		buttons[0] = InputAction.MenuForward;
		buttons[1] = InputAction.MenuU;
		buttons[2] = InputAction.MenuY;
		buttons[3] = InputAction.MenuBackward;
		correctButtonIndex = ActionIndexFromCheatButton(button);
	}

	private int ActionIndexFromCheatButton(CheatButton button)
	{
		InputAction action = InputAction.Ability;
		switch (button)
		{
		case CheatButton.O:
			action = InputAction.MenuForward;
			break;
		case CheatButton.U:
			action = InputAction.MenuU;
			break;
		case CheatButton.Y:
			action = InputAction.MenuY;
			break;
		case CheatButton.A:
			action = InputAction.MenuBackward;
			break;
		}
		return Array.FindIndex(buttons, (InputAction a) => a == action);
	}

	public override CheatEntryReturnStatus Check(int horAxis, int vertAxis)
	{
		CheatEntryReturnStatus result = CheatEntryReturnStatus.NoInput;
		if (InputManager.GetBool(buttons[correctButtonIndex]))
		{
			result = CheatEntryReturnStatus.Success;
		}
		for (int i = 0; i < buttons.Length; i++)
		{
			if (i != correctButtonIndex && InputManager.GetBool(buttons[i]))
			{
				result = CheatEntryReturnStatus.WrongInput;
			}
		}
		return result;
	}
}
