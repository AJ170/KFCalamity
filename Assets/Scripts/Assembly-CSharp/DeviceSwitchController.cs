using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public static class DeviceSwitchController
{
	public class SwitchArgs
	{
		private InterfaceType oldInterfaceType;

		private InterfaceType newInterfaceType;

		private ControllerType oldControllerType;

		private ControllerType newControllerType;

		private DeviceMode oldDeviceMode;

		private DeviceMode newDeviceMode;

		public InterfaceType OldInterfaceType
		{
			get
			{
				return oldInterfaceType;
			}
		}

		public InterfaceType NewInterfaceType
		{
			get
			{
				return newInterfaceType;
			}
		}

		public ControllerType OldControllerType
		{
			get
			{
				return oldControllerType;
			}
		}

		public ControllerType NewControllerType
		{
			get
			{
				return newControllerType;
			}
		}

		public DeviceMode OldDeviceMode
		{
			get
			{
				return oldDeviceMode;
			}
		}

		public DeviceMode NewDeviceMode
		{
			get
			{
				return newDeviceMode;
			}
		}

		public SwitchArgs(InterfaceType oldInterfaceType, InterfaceType newInterfaceType, ControllerType oldControllerType, ControllerType newControllerType, DeviceMode oldDeviceMode, DeviceMode newDeviceMode)
		{
			this.oldControllerType = oldControllerType;
			this.newControllerType = newControllerType;
			this.oldInterfaceType = oldInterfaceType;
			this.newInterfaceType = newInterfaceType;
			this.oldDeviceMode = oldDeviceMode;
			this.newDeviceMode = newDeviceMode;
		}
	}

	private struct SwitchCallback
	{
		public Action<SwitchArgs> Callback;

		public object Caller;
	}

	public static DeviceMode NewDeviceMode;

	private static float checkModeTime;

	private static float controllerModeUpdateFrequency = 1f;

	private static BuildSetting buildSetting;

	private static DeviceMode currentDeviceMode;

	private static InterfaceType currentInterfaceType;

	private static ControllerType currentControllerType;

	private static ControllerType newControllerType;

	private static List<SwitchCallback> onSwitch = new List<SwitchCallback>();

	private static BuildSetting deviceBuildSettings;

	public static DeviceMode CurrentDeviceMode
	{
		get
		{
			return currentDeviceMode;
		}
	}

	public static InterfaceType CurrentInterfaceType
	{
		get
		{
			return currentInterfaceType;
		}
	}

	public static ControllerType CurrentControllerType
	{
		get
		{
			return currentControllerType;
		}
	}

	private static BuildSetting DeviceBuildSettings
	{
		get
		{
			if (deviceBuildSettings == null)
			{
				deviceBuildSettings = BuildRuntimeSettings.Current;
			}
			return deviceBuildSettings;
		}
	}

	public static void Initialize(BuildSetting buildSettings)
	{
		currentDeviceMode = buildSettings.DefaultDeviceMode;
		currentInterfaceType = InterfaceTypeFromDeviceMode(currentDeviceMode);
		currentControllerType = ControllerFromDeviceMode(currentDeviceMode);
		UpdateUICameraSettings(currentControllerType);
		UpdateShowCursor(currentControllerType);
		buildSetting = buildSettings;
		NewDeviceMode = currentDeviceMode;
	}

	public static void SubscribeToSwitch(object caller, Action<SwitchArgs> callback)
	{
		SwitchCallback item = default(SwitchCallback);
		item.Caller = caller;
		item.Callback = callback;
		onSwitch.Add(item);
	}

	public static void UnsubscribeToSwitch(object caller)
	{
		int num = onSwitch.FindIndex((SwitchCallback a) => a.Caller == caller);
		if (num > -1)
		{
			onSwitch.RemoveAt(num);
		}
	}

	public static bool SwitchTo(DeviceMode deviceMode)
	{
		if (currentDeviceMode == deviceMode)
		{
			Debug.Log("Couldn't switch mode. Current mode and target mode is the same: " + deviceMode);
			return false;
		}
		if (!IsModeSupported(deviceMode))
		{
			return false;
		}
		ControllerType oldControllerType = currentControllerType;
		InterfaceType oldInterfaceType = currentInterfaceType;
		DeviceMode oldDeviceMode = currentDeviceMode;
		NewDeviceMode = deviceMode;
		InterfaceType interfaceType = InterfaceTypeFromDeviceMode(deviceMode);
		if (interfaceType != currentInterfaceType && GuiInGameManager.Instance != null)
		{
			GuiInGameManager.Instance.SwitchPlayerHud(interfaceType);
		}
		currentInterfaceType = interfaceType;
		newControllerType = ControllerFromDeviceMode(deviceMode);
		if (newControllerType != currentControllerType)
		{
			SetController(newControllerType);
		}
		currentDeviceMode = deviceMode;
		UpdateUICameraSettings(newControllerType);
		UpdateShowCursor(newControllerType);
		SwitchArgs obj = new SwitchArgs(oldInterfaceType, interfaceType, oldControllerType, newControllerType, oldDeviceMode, currentDeviceMode);
		foreach (SwitchCallback item in onSwitch)
		{
			item.Callback(obj);
		}
		Debug.Log("Switched to: " + deviceMode);
		return true;
	}

	private static bool IsModeSupported(DeviceMode newDeviceMode)
	{
		return (newDeviceMode == DeviceMode.Touch && DeviceBuildSettings.SupportsTouch) || (newDeviceMode == DeviceMode.Controller && DeviceBuildSettings.SupportsContollers) || (newDeviceMode == DeviceMode.MouseKeyboard && DeviceBuildSettings.SupportsMouseKeyboard);
	}

	private static void UpdateUICameraSettings(ControllerType controllerType)
	{
		UICamera uICamera = null;
		if (GuiInGameManager.Instance != null)
		{
			uICamera = GuiInGameManager.Instance.GUICamera;
		}
		else
		{
			if (!(GuiMenuManager.Instance != null))
			{
				return;
			}
			uICamera = GuiMenuManager.Instance.GuiCamera;
		}
		bool flag = CurrentDeviceMode == DeviceMode.Touch;
		uICamera.useMouse = !flag;
		uICamera.useTouch = flag;
		uICamera.allowMultiTouch = flag;
	}

	private static void UpdateShowCursor(ControllerType controllerType)
	{
		if (CurrentDeviceMode == DeviceMode.Touch)
		{
			Cursor.visible = false;
			return;
		}
		Cursor.visible = true;
		if (GuiMenuManager.Instance != null)
		{
			CursorManager.SetCursorByType(CursorType.Menu);
		}
		else if (GuiInGameManager.Instance != null)
		{
			CursorManager.SetCursorByType(CursorType.InGame);
		}
	}

	private static void SetController(ControllerType newControllerType)
	{
		InputManager.Instance.SwitchController(newControllerType);
		currentControllerType = newControllerType;
	}

	public static ControllerType ControllerFromDeviceMode(DeviceMode deviceMode)
	{
		switch (deviceMode)
		{
		case DeviceMode.MouseKeyboard:
			return ControllerType.Keyboard;
		case DeviceMode.Touch:
			return ControllerType.TouchControls;
		case DeviceMode.Controller:
			return (!KFCompat.IsVitaControls) ? ControllerType.XBOX360_Windows : ControllerType.PSVita;
		default:
			return ControllerType.Unknown;
		}
	}

	public static InterfaceType InterfaceTypeFromDeviceMode(DeviceMode deviceMode)
	{
		switch (deviceMode)
		{
		case DeviceMode.MouseKeyboard:
			return InterfaceType.Touch;
		case DeviceMode.Touch:
			return InterfaceType.Touch;
		case DeviceMode.Controller:
			return InterfaceType.Standard;
		default:
			return InterfaceType.Standard;
		}
	}
}
