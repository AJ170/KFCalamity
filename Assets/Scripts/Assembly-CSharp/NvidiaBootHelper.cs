using System;
using System.Collections.Generic;
using UnityEngine;

public class NvidiaBootHelper : MonoBehaviour
{
	[Serializable]
	public class SettingsData
	{
		public string ModelName;

		public string QualitySetting;

		public bool TouchSupport;

		public DeviceMode DefaultDeviceMode;
	}

	[Serializable]
	public enum DeviceSimulationEnum
	{
		None = 0,
		NvidiaK1 = 1,
		NvidiaShield = 2,
		NvidiaShieldConsole = 3
	}

	public SettingsData[] ModelSettings;

	public DeviceSimulationEnum DeviceSimulation;

	private Dictionary<int, string> supportedDevices = new Dictionary<int, string>
	{
		{ 1, "NVIDIA SHIELD Tablet" },
		{ 2, "NVIDIA SHIELD" },
		{ 3, "NVIDIA SHIELD Console" }
	};

	private void Awake()
	{
		SettingsData settingsData = SettingsByDeviceModel();
		if (settingsData != null)
		{
			Debug.Log("[NvidiaBootHelper] Apply Device Settings for " + settingsData.ModelName);
			BuildRuntimeSettings.Current.SupportsTouch = settingsData.TouchSupport;
			BuildRuntimeSettings.Current.DefaultDeviceMode = settingsData.DefaultDeviceMode;
			ApplyQualitySettingsByName(settingsData.QualitySetting);
		}
		else
		{
			Debug.LogError("[NvidiaBootHelper] Couldn't find ModelSetting for device " + SystemInfo.deviceModel);
		}
	}

	private SettingsData SettingsByDeviceModel()
	{
		string text = null;
		text = ((DeviceSimulation == DeviceSimulationEnum.None) ? SystemInfo.deviceModel : supportedDevices[(int)DeviceSimulation]);
		SettingsData[] modelSettings = ModelSettings;
		foreach (SettingsData settingsData in modelSettings)
		{
			if (text == settingsData.ModelName)
			{
				return settingsData;
			}
		}
		return null;
	}

	private void ApplyQualitySettingsByName(string qualityName)
	{
		string[] names = QualitySettings.names;
		for (short num = 0; num < names.Length; num++)
		{
			if (qualityName == names[num])
			{
				Debug.Log("[NvidiaBootHelper] Set Quality Settings to " + qualityName);
				QualitySettings.SetQualityLevel(num, true);
				break;
			}
		}
	}
}
