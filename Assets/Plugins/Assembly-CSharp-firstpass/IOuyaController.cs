using UnityEngine;

public interface IOuyaController
{
	string[] GetSupportedJoysicks();

	OuyaSDK.KeyEnum[] GetSupportedAxises();

	OuyaSDK.KeyEnum[] GetSupportedButtons();

	bool HasAxis(OuyaSDK.KeyEnum keyCode);

	bool HasButton(OuyaSDK.KeyEnum keyCode);

	bool GetAxisInverted(OuyaSDK.KeyEnum keyCode);

	string GetUnityAxisName(OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player);

	KeyCode GetUnityKeyCode(OuyaSDK.KeyEnum keyCode, OuyaSDK.OuyaPlayer player);
}
