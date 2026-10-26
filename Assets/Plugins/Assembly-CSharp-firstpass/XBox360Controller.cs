using UnityEngine;

public class XBox360Controller : OuyaControllerCommon, IOuyaController
{
	private readonly string[] m_supportedJoysticks = new string[8] { "CONTROLLER (AFTERGLOW GAMEPAD FOR XBOX 360)", "CONTROLLER (ROCK CANDY GAMEPAD FOR XBOX 360)", "CONTROLLER (XBOX 360 WIRELESS RECEIVER FOR WINDOWS)", "MICROSOFT X-BOX 360 PAD", "CONTROLLER (XBOX 360 FOR WINDOWS)", "CONTROLLER (XBOX360 GAMEPAD)", "XBOX 360 FOR WINDOWS (CONTROLLER)", "MICROSOFT WIRELESS 360 CONTROLLER" };

	public string[] GetSupportedJoysicks()
	{
		return m_supportedJoysticks;
	}

	public OuyaSDK.KeyEnum[] GetSupportedAxises()
	{
		return new OuyaSDK.KeyEnum[10]
		{
			OuyaSDK.KeyEnum.AXIS_LSTICK_X,
			OuyaSDK.KeyEnum.AXIS_LSTICK_Y,
			OuyaSDK.KeyEnum.AXIS_RSTICK_X,
			OuyaSDK.KeyEnum.AXIS_RSTICK_Y,
			OuyaSDK.KeyEnum.BUTTON_LT,
			OuyaSDK.KeyEnum.BUTTON_RT,
			OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT,
			OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT,
			OuyaSDK.KeyEnum.BUTTON_DPAD_UP,
			OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN
		};
	}

	public OuyaSDK.KeyEnum[] GetSupportedButtons()
	{
		return new OuyaSDK.KeyEnum[8]
		{
			OuyaSDK.KeyEnum.BUTTON_LB,
			OuyaSDK.KeyEnum.BUTTON_RB,
			OuyaSDK.KeyEnum.BUTTON_O,
			OuyaSDK.KeyEnum.BUTTON_U,
			OuyaSDK.KeyEnum.BUTTON_Y,
			OuyaSDK.KeyEnum.BUTTON_A,
			OuyaSDK.KeyEnum.BUTTON_L3,
			OuyaSDK.KeyEnum.BUTTON_R3
		};
	}

	public bool HasAxis(OuyaSDK.KeyEnum keyCode)
	{
		OuyaSDK.KeyEnum[] supportedAxises = GetSupportedAxises();
		return HasKeyCode(supportedAxises, keyCode);
	}

	public bool HasButton(OuyaSDK.KeyEnum keyCode)
	{
		OuyaSDK.KeyEnum[] supportedButtons = GetSupportedButtons();
		return HasKeyCode(supportedButtons, keyCode);
	}

	public bool GetAxisInverted(OuyaSDK.KeyEnum keyCode)
	{
		switch (keyCode)
		{
		case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
			return false;
		case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
			return false;
		case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
			return false;
		case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
			return false;
		case OuyaSDK.KeyEnum.BUTTON_LT:
			return false;
		case OuyaSDK.KeyEnum.BUTTON_RT:
			return false;
		default:
			return false;
		}
	}

	public string GetUnityAxisName(OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player)
	{
		switch (keyCode)
		{
		case OuyaSDK.KeyEnum.AXIS_LSTICK_X:
			return string.Format("Joy{0} Axis 1", (int)player);
		case OuyaSDK.KeyEnum.AXIS_LSTICK_Y:
			return string.Format("Joy{0} Axis 2", (int)player);
		case OuyaSDK.KeyEnum.AXIS_RSTICK_X:
			return string.Format("Joy{0} Axis 3", (int)player);
		case OuyaSDK.KeyEnum.AXIS_RSTICK_Y:
			return string.Format("Joy{0} Axis 4", (int)player);
		case OuyaSDK.KeyEnum.BUTTON_LT:
			return string.Format("Joy{0} Axis 7", (int)player);
		case OuyaSDK.KeyEnum.BUTTON_RT:
			return string.Format("Joy{0} Axis 8", (int)player);
		case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
			return string.Format("Joy{0} Axis 5", (int)player);
		case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
			return string.Format("Joy{0} Axis 5", (int)player);
		case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
			return string.Format("Joy{0} Axis 6", (int)player);
		case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
			return string.Format("Joy{0} Axis 6", (int)player);
		default:
			return string.Empty;
		}
	}

	public KeyCode GetUnityKeyCode(OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player)
	{
		switch (keyCode)
		{
		case OuyaSDK.KeyEnum.BUTTON_LB:
			return CommonGetUnityKeyCode(6, player);
		case OuyaSDK.KeyEnum.BUTTON_RB:
			return CommonGetUnityKeyCode(7, player);
		case OuyaSDK.KeyEnum.BUTTON_O:
			return CommonGetUnityKeyCode(0, player);
		case OuyaSDK.KeyEnum.BUTTON_U:
			return CommonGetUnityKeyCode(3, player);
		case OuyaSDK.KeyEnum.BUTTON_Y:
			return CommonGetUnityKeyCode(4, player);
		case OuyaSDK.KeyEnum.BUTTON_A:
			return CommonGetUnityKeyCode(1, player);
		case OuyaSDK.KeyEnum.BUTTON_L3:
			return CommonGetUnityKeyCode(13, player);
		case OuyaSDK.KeyEnum.BUTTON_R3:
			return CommonGetUnityKeyCode(14, player);
		case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
			return (KeyCode)(-1);
		case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
			return (KeyCode)(-1);
		case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
			return (KeyCode)(-1);
		case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
			return (KeyCode)(-1);
		case OuyaSDK.KeyEnum.BUTTON_LT:
			return (KeyCode)(-1);
		case OuyaSDK.KeyEnum.BUTTON_RT:
			return (KeyCode)(-1);
		default:
			return (KeyCode)(-1);
		}
	}
}
