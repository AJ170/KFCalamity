using Calamity.Input;
using UnityEngine;

public class BuildRuntimeSettings : MonoBehaviour
{
	private const string PREFAB_RESOURCE_PATH = "prefabs/misc/BuildRuntimeSettings";

	public BuildSetting BuildSetting;

	private static BuildRuntimeSettings instance;

	public static BuildRuntimeSettings Instance
	{
		get
		{
			return instance;
		}
	}

	public static BuildSetting Current
	{
		get
		{
			if (instance == null)
			{
				GameObject original = Resources.Load<GameObject>("prefabs/misc/BuildRuntimeSettings");
				GameObject gameObject = (GameObject)Object.Instantiate(original);
				instance = gameObject.GetComponent<BuildRuntimeSettings>();
				if (instance.BuildSetting == null)
				{
					instance.BuildSetting = new BuildSetting();
					ApplyPlatformSettings(instance.BuildSetting);
				}
			}
			return instance.BuildSetting;
		}
	}

	private void Awake()
	{
		instance = this;
		if (BuildSetting != null)
		{
			ApplyPlatformSettings(BuildSetting);
		}
		Object.DontDestroyOnLoad(base.gameObject);
		InterfaceType interfaceType = DeviceSwitchController.InterfaceTypeFromDeviceMode(BuildSetting.DefaultDeviceMode);
		ControllerType controllerType = DeviceSwitchController.ControllerFromDeviceMode(BuildSetting.DefaultDeviceMode);
		DeviceSwitchController.Initialize(BuildSetting);
	}

	// PS Vita port: the Android build ships with touch controls only; the Vita plays with its buttons and the
	// console (controller) HUD.
	private static void ApplyPlatformSettings(BuildSetting setting)
	{
		if (KFCompat.IsVitaControls)
		{
			setting.DefaultDeviceMode = DeviceMode.Controller;
			setting.SupportsContollers = true;
			setting.SupportsTouch = false;
			setting.SupportsMouseKeyboard = false;
		}
	}
}
