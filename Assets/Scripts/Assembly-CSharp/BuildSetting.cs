using System;

[Serializable]
public class BuildSetting
{
	public DeviceMode DefaultDeviceMode;

	public bool SupportsMouseKeyboard = true;

	public bool SupportsTouch = true;

	public bool SupportsContollers;
}
