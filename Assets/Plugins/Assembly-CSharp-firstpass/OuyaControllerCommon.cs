using System;
using UnityEngine;

public abstract class OuyaControllerCommon
{
	protected string CommonGetKeyString(int buttonNum, OuyaSDK.OuyaPlayer player)
	{
		if (player == OuyaSDK.OuyaPlayer.none)
		{
			return string.Format("JoystickButton{0}", buttonNum);
		}
		return string.Format("Joystick{0}Button{1}", (int)player, buttonNum);
	}

	protected KeyCode CommonGetUnityKeyCode(int buttonNum, OuyaSDK.OuyaPlayer player)
	{
		string value = CommonGetKeyString(buttonNum, player);
		if (string.IsNullOrEmpty(value))
		{
			return (KeyCode)(-1);
		}
		return (KeyCode)(int)Enum.Parse(typeof(OuyaKeyCode), value);
	}

	protected bool HasKeyCode(OuyaSDK.KeyEnum[] supportedCodes, OuyaSDK.KeyEnum keyCode)
	{
		foreach (OuyaSDK.KeyEnum keyEnum in supportedCodes)
		{
			if (keyEnum == keyCode)
			{
				return true;
			}
		}
		return false;
	}
}
