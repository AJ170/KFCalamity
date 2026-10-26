using System;
using UnityEngine;

public class OuyaExampleCommon
{
	public static OuyaSDK.OuyaPlayer Player = OuyaSDK.OuyaPlayer.player1;

	public static float GetAxis(string ouyaMapping, OuyaSDK.OuyaPlayer player)
	{
		switch (ouyaMapping)
		{
		case "LB":
			return GetAxis(OuyaSDK.KeyEnum.BUTTON_LB, player);
		case "LT":
			return GetAxis(OuyaSDK.KeyEnum.BUTTON_LT, player);
		case "LX":
			return GetAxis(OuyaSDK.KeyEnum.AXIS_LSTICK_X, player);
		case "LY":
			return GetAxis(OuyaSDK.KeyEnum.AXIS_LSTICK_Y, player);
		case "RB":
			return GetAxis(OuyaSDK.KeyEnum.BUTTON_RB, player);
		case "RT":
			return GetAxis(OuyaSDK.KeyEnum.BUTTON_RT, player);
		case "RX":
			return GetAxis(OuyaSDK.KeyEnum.AXIS_RSTICK_X, player);
		case "RY":
			return GetAxis(OuyaSDK.KeyEnum.AXIS_RSTICK_Y, player);
		case "DL":
			return GetAxis(OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT, player);
		case "DR":
			return GetAxis(OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT, player);
		case "DU":
			return GetAxis(OuyaSDK.KeyEnum.BUTTON_DPAD_UP, player);
		case "DD":
			return GetAxis(OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN, player);
		default:
			return 0f;
		}
	}

	public static float GetAxis(OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player)
	{
		if (OuyaSDK.Joysticks == null)
		{
			return 0f;
		}
		int num = (int)(player - 1);
		if (num >= OuyaSDK.Joysticks.Length)
		{
			return 0f;
		}
		string text = OuyaSDK.Joysticks[num];
		if (text == null)
		{
			return 0f;
		}
		bool flag = false;
		string text2 = string.Empty;
		switch (text.ToUpper())
		{
		case "HARMONIX ROCK BAND DRUMKIT":
			return 0f;
		case "HARMONIX ROCK BAND GUITAR":
			switch (keyCode)
			{
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_PICKUP:
			case OuyaSDK.KeyEnum.BUTTON_LT:
				text2 = string.Format("Joy{0} Axis 3", (int)player);
				break;
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_WHAMMI:
			case OuyaSDK.KeyEnum.BUTTON_RT:
				text2 = string.Format("Joy{0} Axis 4", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
			case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
				text2 = string.Format("Joy{0} Axis 7", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
			case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
			case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_STRUM:
				text2 = string.Format("Joy{0} Axis 8", (int)player);
				break;
			default:
				return 0f;
			}
			break;
		case "BROADCOM BLUETOOTH HID":
		case "MOGA PRO HID":
			switch (keyCode)
			{
			case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
				text2 = string.Format("Joy{0} Axis 1", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
				text2 = string.Format("Joy{0} Axis 2", (int)player);
				flag = true;
				break;
			case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
				text2 = string.Format("Joy{0} Axis 3", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
				text2 = string.Format("Joy{0} Axis 4", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_LT:
				text2 = string.Format("Joy{0} Axis 8", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_RT:
				text2 = string.Format("Joy{0} Axis 7", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
				text2 = string.Format("Joy{0} Axis 5", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
				text2 = string.Format("Joy{0} Axis 5", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
				text2 = string.Format("Joy{0} Axis 6", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
				text2 = string.Format("Joy{0} Axis 6", (int)player);
				break;
			default:
				return 0f;
			}
			break;
		case "OUYA GAME CONTROLLER":
			switch (keyCode)
			{
			case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
				text2 = string.Format("Joy{0} Axis 1", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
				text2 = string.Format("Joy{0} Axis 2", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
				text2 = string.Format("Joy{0} Axis 3", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
				text2 = string.Format("Joy{0} Axis 4", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_LT:
				text2 = string.Format("Joy{0} Axis 5", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_RT:
				text2 = string.Format("Joy{0} Axis 6", (int)player);
				break;
			default:
				return 0f;
			}
			break;
		case "XBOX 360 WIRELESS RECEIVER":
			switch (keyCode)
			{
			case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
				text2 = string.Format("Joy{0} Axis 1", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
				text2 = string.Format("Joy{0} Axis 2", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
				text2 = string.Format("Joy{0} Axis 3", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
				text2 = string.Format("Joy{0} Axis 4", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_LT:
				text2 = string.Format("Joy{0} Axis 5", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_RT:
				text2 = string.Format("Joy{0} Axis 6", (int)player);
				break;
			default:
				return 0f;
			}
			break;
		case "CONTROLLER (AFTERGLOW GAMEPAD FOR XBOX 360)":
		case "CONTROLLER (ROCK CANDY GAMEPAD FOR XBOX 360)":
		case "CONTROLLER (XBOX 360 WIRELESS RECEIVER FOR WINDOWS)":
		case "MICROSOFT X-BOX 360 PAD":
		case "CONTROLLER (XBOX 360 FOR WINDOWS)":
		case "CONTROLLER (XBOX360 GAMEPAD)":
		case "XBOX 360 FOR WINDOWS (CONTROLLER)":
		case "MICROSOFT WIRELESS 360 CONTROLLER":
			switch (keyCode)
			{
			case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
				text2 = string.Format("Joy{0} Axis 1", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
				text2 = string.Format("Joy{0} Axis 2", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
				text2 = string.Format("Joy{0} Axis 3", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
				text2 = string.Format("Joy{0} Axis 4", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_LT:
				text2 = string.Format("Joy{0} Axis 7", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_RT:
				text2 = string.Format("Joy{0} Axis 8", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
				text2 = string.Format("Joy{0} Axis 5", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
				text2 = string.Format("Joy{0} Axis 5", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
				text2 = string.Format("Joy{0} Axis 6", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
				text2 = string.Format("Joy{0} Axis 6", (int)player);
				break;
			default:
				return 0f;
			}
			break;
		case "":
			switch (keyCode)
			{
			case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
				text2 = string.Format("Joy{0} Axis 1", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
				text2 = string.Format("Joy{0} Axis 2", (int)player);
				flag = true;
				break;
			case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
				text2 = string.Format("Joy{0} Axis 3", (int)player);
				break;
			case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
				text2 = string.Format("Joy{0} Axis 4", (int)player);
				flag = true;
				break;
			case OuyaSDK.KeyEnum.BUTTON_LT:
				text2 = string.Format("Joy{0} Axis 7", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_RT:
				text2 = string.Format("Joy{0} Axis 8", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
				text2 = string.Format("Joy{0} Axis 5", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
				text2 = string.Format("Joy{0} Axis 5", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
				text2 = string.Format("Joy{0} Axis 6", (int)player);
				break;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
				text2 = string.Format("Joy{0} Axis 6", (int)player);
				break;
			default:
				return 0f;
			}
			break;
		}
		if (string.IsNullOrEmpty(text2))
		{
			return 0f;
		}
		if (flag)
		{
			return 0f - Input.GetAxisRaw(text2);
		}
		return Input.GetAxisRaw(text2);
	}

	public static string GetKeyCode(int buttonNum, OuyaSDK.OuyaPlayer player)
	{
		if (player == OuyaSDK.OuyaPlayer.none)
		{
			return string.Format("JoystickButton{0}", buttonNum);
		}
		return string.Format("Joystick{0}Button{1}", (int)player, buttonNum);
	}

	public static bool GetButton(int buttonNum, OuyaSDK.OuyaPlayer player)
	{
		string keyCode = GetKeyCode(buttonNum, player);
		if (string.IsNullOrEmpty(keyCode))
		{
			return false;
		}
		OuyaKeyCode key = (OuyaKeyCode)(int)Enum.Parse(typeof(OuyaKeyCode), keyCode);
		return Input.GetKey((KeyCode)key);
	}

	public static bool GetButton(OuyaSDK.KeyEnum keyCode)
	{
		return GetButton(keyCode, OuyaSDK.OuyaPlayer.player1) || GetButton(keyCode, OuyaSDK.OuyaPlayer.player2) || GetButton(keyCode, OuyaSDK.OuyaPlayer.player3) || GetButton(keyCode, OuyaSDK.OuyaPlayer.player4) || GetButton(keyCode, OuyaSDK.OuyaPlayer.player5) || GetButton(keyCode, OuyaSDK.OuyaPlayer.player6) || GetButton(keyCode, OuyaSDK.OuyaPlayer.player7) || GetButton(keyCode, OuyaSDK.OuyaPlayer.player8);
	}

	public static bool GetButton(OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player)
	{
		if (OuyaSDK.Joysticks == null)
		{
			return false;
		}
		int num = (int)(player - 1);
		if (num >= OuyaSDK.Joysticks.Length)
		{
			return false;
		}
		string text = OuyaSDK.Joysticks[num];
		if (text == null)
		{
			return false;
		}
		switch (text.ToUpper())
		{
		case "HARMONIX ROCK BAND DRUMKIT":
			switch (keyCode)
			{
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_GREEN:
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_A:
			case OuyaSDK.KeyEnum.BUTTON_O:
				return GetButton(0, player);
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_RED:
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_B:
			case OuyaSDK.KeyEnum.BUTTON_A:
				return GetButton(1, player);
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_YELLOW:
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_Y:
			case OuyaSDK.KeyEnum.BUTTON_Y:
				return GetButton(4, player);
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_BLUE:
			case OuyaSDK.KeyEnum.BUTTON_ESCAPE:
			case OuyaSDK.KeyEnum.BUTTON_U:
				return GetButton(3, player);
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_DRUMKIT_ORANGE:
			case OuyaSDK.KeyEnum.BUTTON_L3:
				return GetButton(6, player);
			case OuyaSDK.KeyEnum.BUTTON_BACK:
			case OuyaSDK.KeyEnum.BUTTON_R3:
				return GetButton(10, player);
			case OuyaSDK.KeyEnum.BUTTON_START:
			case OuyaSDK.KeyEnum.BUTTON_LB:
				return GetButton(11, player);
			case OuyaSDK.KeyEnum.BUTTON_RB:
			case OuyaSDK.KeyEnum.BUTTON_SYSTEM:
				return GetButton(12, player);
			default:
				return false;
			}
		case "HARMONIX ROCK BAND GUITAR":
			switch (keyCode)
			{
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_GREEN:
			case OuyaSDK.KeyEnum.BUTTON_O:
				return GetButton(0, player);
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_RED:
			case OuyaSDK.KeyEnum.BUTTON_A:
				return GetButton(1, player);
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_YELLOW:
			case OuyaSDK.KeyEnum.BUTTON_Y:
				return GetButton(4, player);
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_BLUE:
			case OuyaSDK.KeyEnum.BUTTON_U:
				return GetButton(3, player);
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_ORANGE:
			case OuyaSDK.KeyEnum.BUTTON_L3:
				return GetButton(6, player);
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_LOWER:
				return GetButton(13, player);
			case OuyaSDK.KeyEnum.BUTTON_BACK:
			case OuyaSDK.KeyEnum.BUTTON_R3:
				return GetButton(10, player);
			case OuyaSDK.KeyEnum.BUTTON_START:
			case OuyaSDK.KeyEnum.BUTTON_LB:
				return GetButton(11, player);
			case OuyaSDK.KeyEnum.BUTTON_RB:
			case OuyaSDK.KeyEnum.BUTTON_SYSTEM:
				return GetButton(12, player);
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_PICKUP:
			case OuyaSDK.KeyEnum.BUTTON_LT:
				return false;
			case OuyaSDK.KeyEnum.HARMONIX_ROCK_BAND_GUITAR_WHAMMI:
			case OuyaSDK.KeyEnum.BUTTON_RT:
				return false;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
				return GetAxis(keyCode, player) > 0f;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
				return GetAxis(keyCode, player) < 0f;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
				return GetAxis(keyCode, player) > 0f;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
				return GetAxis(keyCode, player) < 0f;
			default:
				return false;
			}
		case "BROADCOM BLUETOOTH HID":
		case "MOGA PRO HID":
			switch (keyCode)
			{
			case OuyaSDK.KeyEnum.BUTTON_LB:
				return GetButton(6, player);
			case OuyaSDK.KeyEnum.BUTTON_RB:
				return GetButton(7, player);
			case OuyaSDK.KeyEnum.BUTTON_O:
				return GetButton(0, player);
			case OuyaSDK.KeyEnum.BUTTON_U:
				return GetButton(3, player);
			case OuyaSDK.KeyEnum.BUTTON_Y:
				return GetButton(4, player);
			case OuyaSDK.KeyEnum.BUTTON_A:
				return GetButton(1, player);
			case OuyaSDK.KeyEnum.BUTTON_L3:
				return GetButton(13, player);
			case OuyaSDK.KeyEnum.BUTTON_R3:
				return GetButton(14, player);
			case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
				return GetAxis(keyCode, player) > 0f;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
				return GetAxis(keyCode, player) < 0f;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
				return GetAxis(keyCode, player) < 0f;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
				return GetAxis(keyCode, player) > 0f;
			case OuyaSDK.KeyEnum.BUTTON_LT:
				return GetAxis(keyCode, player) > 0f;
			case OuyaSDK.KeyEnum.BUTTON_RT:
				return GetAxis(keyCode, player) > 0f;
			default:
				return false;
			}
		case "OUYA GAME CONTROLLER":
			switch (keyCode)
			{
			case OuyaSDK.KeyEnum.BUTTON_LB:
				return GetButton(4, player);
			case OuyaSDK.KeyEnum.BUTTON_RB:
				return GetButton(5, player);
			case OuyaSDK.KeyEnum.BUTTON_O:
				return GetButton(0, player);
			case OuyaSDK.KeyEnum.BUTTON_U:
				return GetButton(1, player);
			case OuyaSDK.KeyEnum.BUTTON_Y:
				return GetButton(2, player);
			case OuyaSDK.KeyEnum.BUTTON_A:
				return GetButton(3, player);
			case OuyaSDK.KeyEnum.BUTTON_L3:
				return GetButton(6, player);
			case OuyaSDK.KeyEnum.BUTTON_R3:
				return GetButton(7, player);
			case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
				return GetButton(8, player);
			case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
				return GetButton(9, player);
			case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
				return GetButton(10, player);
			case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
				return GetButton(11, player);
			case OuyaSDK.KeyEnum.BUTTON_LT:
				return GetButton(12, player);
			case OuyaSDK.KeyEnum.BUTTON_RT:
				return GetButton(13, player);
			default:
				return false;
			}
		case "XBOX 360 WIRELESS RECEIVER":
			switch (keyCode)
			{
			case OuyaSDK.KeyEnum.BUTTON_LB:
				return GetButton(6, player);
			case OuyaSDK.KeyEnum.BUTTON_RB:
				return GetButton(7, player);
			case OuyaSDK.KeyEnum.BUTTON_O:
				return GetButton(0, player);
			case OuyaSDK.KeyEnum.BUTTON_U:
				return GetButton(3, player);
			case OuyaSDK.KeyEnum.BUTTON_Y:
				return GetButton(4, player);
			case OuyaSDK.KeyEnum.BUTTON_A:
				return GetButton(1, player);
			case OuyaSDK.KeyEnum.BUTTON_L3:
				return GetButton(13, player);
			case OuyaSDK.KeyEnum.BUTTON_R3:
				return GetButton(14, player);
			case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
				return GetButton(2, player);
			case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
				return GetButton(3, player);
			case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
				return GetButton(0, player);
			case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
				return GetButton(1, player);
			case OuyaSDK.KeyEnum.BUTTON_LT:
				return GetAxis(keyCode, player) > 0f;
			case OuyaSDK.KeyEnum.BUTTON_RT:
				return GetAxis(keyCode, player) > 0f;
			default:
				return false;
			}
		case "CONTROLLER (AFTERGLOW GAMEPAD FOR XBOX 360)":
		case "CONTROLLER (ROCK CANDY GAMEPAD FOR XBOX 360)":
		case "CONTROLLER (XBOX 360 WIRELESS RECEIVER FOR WINDOWS)":
		case "MICROSOFT X-BOX 360 PAD":
		case "CONTROLLER (XBOX 360 FOR WINDOWS)":
		case "CONTROLLER (XBOX360 GAMEPAD)":
		case "XBOX 360 FOR WINDOWS (CONTROLLER)":
		case "MICROSOFT WIRELESS 360 CONTROLLER":
			switch (keyCode)
			{
			case OuyaSDK.KeyEnum.BUTTON_LB:
				return GetButton(6, player);
			case OuyaSDK.KeyEnum.BUTTON_RB:
				return GetButton(7, player);
			case OuyaSDK.KeyEnum.BUTTON_O:
				return GetButton(0, player);
			case OuyaSDK.KeyEnum.BUTTON_U:
				return GetButton(3, player);
			case OuyaSDK.KeyEnum.BUTTON_Y:
				return GetButton(4, player);
			case OuyaSDK.KeyEnum.BUTTON_A:
				return GetButton(1, player);
			case OuyaSDK.KeyEnum.BUTTON_L3:
				return GetButton(13, player);
			case OuyaSDK.KeyEnum.BUTTON_R3:
				return GetButton(14, player);
			case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
				return GetAxis(keyCode, player) > 0f;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
				return GetAxis(keyCode, player) < 0f;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
				return GetAxis(keyCode, player) < 0f;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
				return GetAxis(keyCode, player) > 0f;
			case OuyaSDK.KeyEnum.BUTTON_LT:
				return GetAxis(keyCode, player) > 0f;
			case OuyaSDK.KeyEnum.BUTTON_RT:
				return GetAxis(keyCode, player) > 0f;
			default:
				return false;
			}
		case "":
			switch (keyCode)
			{
			case OuyaSDK.KeyEnum.BUTTON_LB:
				return GetButton(6, player);
			case OuyaSDK.KeyEnum.BUTTON_RB:
				return GetButton(7, player);
			case OuyaSDK.KeyEnum.BUTTON_O:
				return GetButton(0, player);
			case OuyaSDK.KeyEnum.BUTTON_U:
				return GetButton(3, player);
			case OuyaSDK.KeyEnum.BUTTON_Y:
				return GetButton(4, player);
			case OuyaSDK.KeyEnum.BUTTON_A:
				return GetButton(1, player);
			case OuyaSDK.KeyEnum.BUTTON_L3:
				return GetButton(13, player);
			case OuyaSDK.KeyEnum.BUTTON_R3:
				return GetButton(14, player);
			case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
				return GetAxis(keyCode, player) > 0f;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
				return GetAxis(keyCode, player) < 0f;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
				return GetAxis(keyCode, player) < 0f;
			case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
				return GetAxis(keyCode, player) > 0f;
			case OuyaSDK.KeyEnum.BUTTON_LT:
				return GetAxis(keyCode, player) > 0f;
			case OuyaSDK.KeyEnum.BUTTON_RT:
				return GetAxis(keyCode, player) > 0f;
			default:
				return false;
			}
		default:
			return false;
		}
	}
}
