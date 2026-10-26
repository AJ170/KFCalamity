using UnityEngine;

public class OuyaController : OuyaControllerCommon, IOuyaController
{
	private readonly string[] m_supportedJoysticks = new string[1] { "OUYA GAME CONTROLLER" };

	public string[] GetSupportedJoysicks()
	{
		return m_supportedJoysticks;
	}

	public OuyaSDK.KeyEnum[] GetSupportedAxises()
	{
		return new OuyaSDK.KeyEnum[6]
		{
			OuyaSDK.KeyEnum.AXIS_LSTICK_X,
			OuyaSDK.KeyEnum.AXIS_LSTICK_Y,
			OuyaSDK.KeyEnum.AXIS_RSTICK_X,
			OuyaSDK.KeyEnum.AXIS_RSTICK_Y,
			OuyaSDK.KeyEnum.BUTTON_LT,
			OuyaSDK.KeyEnum.BUTTON_RT
		};
	}

	public OuyaSDK.KeyEnum[] GetSupportedButtons()
	{
		return new OuyaSDK.KeyEnum[14]
		{
			OuyaSDK.KeyEnum.BUTTON_LB,
			OuyaSDK.KeyEnum.BUTTON_RB,
			OuyaSDK.KeyEnum.BUTTON_O,
			OuyaSDK.KeyEnum.BUTTON_U,
			OuyaSDK.KeyEnum.BUTTON_Y,
			OuyaSDK.KeyEnum.BUTTON_A,
			OuyaSDK.KeyEnum.BUTTON_L3,
			OuyaSDK.KeyEnum.BUTTON_R3,
			OuyaSDK.KeyEnum.BUTTON_DPAD_UP,
			OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN,
			OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT,
			OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT,
			OuyaSDK.KeyEnum.BUTTON_LT,
			OuyaSDK.KeyEnum.BUTTON_RT
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
			return string.Format("Joy{0} Axis 5", (int)player);
		case OuyaSDK.KeyEnum.BUTTON_RT:
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
			return CommonGetUnityKeyCode(4, player);
		case OuyaSDK.KeyEnum.BUTTON_RB:
			return CommonGetUnityKeyCode(5, player);
		case OuyaSDK.KeyEnum.BUTTON_O:
			return CommonGetUnityKeyCode(0, player);
		case OuyaSDK.KeyEnum.BUTTON_U:
			return CommonGetUnityKeyCode(1, player);
		case OuyaSDK.KeyEnum.BUTTON_Y:
			return CommonGetUnityKeyCode(2, player);
		case OuyaSDK.KeyEnum.BUTTON_A:
			return CommonGetUnityKeyCode(3, player);
		case OuyaSDK.KeyEnum.BUTTON_L3:
			return CommonGetUnityKeyCode(6, player);
		case OuyaSDK.KeyEnum.BUTTON_R3:
			return CommonGetUnityKeyCode(7, player);
		case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
			return CommonGetUnityKeyCode(8, player);
		case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
			return CommonGetUnityKeyCode(9, player);
		case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
			return CommonGetUnityKeyCode(10, player);
		case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
			return CommonGetUnityKeyCode(11, player);
		case OuyaSDK.KeyEnum.BUTTON_LT:
			return CommonGetUnityKeyCode(12, player);
		case OuyaSDK.KeyEnum.BUTTON_RT:
			return CommonGetUnityKeyCode(13, player);
		default:
			return (KeyCode)(-1);
		}
	}
}
