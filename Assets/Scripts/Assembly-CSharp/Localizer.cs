using System;
using System.Collections.Generic;
using UnityEngine;

public class Localizer : MonoBehaviour
{
	[Serializable]
	public class LanguageEntry
	{
		public string DisplayName;

		public TextAsset File;
	}

	public const string APPEND_LOCALIZE_KEYBOARD = "_keyboard";

	public const string APPEND_LOCALIZE_TOUCH = "_touch";

	public const string APPEND_LOCALIZE_CONTROLLER = "_controller";

	private const char splitChar = '=';

	public string StartingLanguage = "English";

	public LanguageEntry[] Languages;

	private Dictionary<string, string> localizedStrings = new Dictionary<string, string>();

	private string currentLanguage;

	private static bool initialized;

	private static Localizer instance;

	public bool Initialized
	{
		get
		{
			return initialized;
		}
	}

	public string CurrentLanguage
	{
		get
		{
			return currentLanguage;
		}
		set
		{
			SetLocalizationLanguage(value);
		}
	}

	public string[] LanguageDisplayNames
	{
		get
		{
			return Array.ConvertAll(Languages, (LanguageEntry a) => a.DisplayName);
		}
	}

	public static Localizer Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		SetLocalizationLanguage(StartingLanguage);
	}

	public void SetLocalizationLanguage(string displayName)
	{
		if (Languages == null || Languages.Length == 0)
		{
			return;
		}
		int num = Array.FindIndex(Languages, (LanguageEntry a) => a.DisplayName == StartingLanguage);
		TextAsset textAsset = null;
		if (num > -1)
		{
			textAsset = Languages[num].File;
			localizedStrings = new Dictionary<string, string>();
			string[] array = textAsset.text.Split('\n');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.StartsWith("//") || text.StartsWith("#") || text == string.Empty || text == " ")
				{
					continue;
				}
				int num2 = text.IndexOf('=');
				if (num2 != -1)
				{
					string text2 = text.Substring(0, num2).Trim();
					string text3 = text.Substring(num2 + 1);
					text3 = text3.Trim();
					FormatString(ref text3);
					if (localizedStrings.ContainsKey(text2))
					{
						Debug.LogError("A key with the name: " + text2 + " already exists in the localized strings. This means you have a duplicate key in " + textAsset.name + ".txt");
					}
					else
					{
						localizedStrings.Add(text2, text3);
					}
				}
				else
				{
					Debug.Log("[Localizer] Invalid string format: " + text);
				}
			}
			currentLanguage = displayName;
			UIRoot.Broadcast("OnLocalize", this);
			initialized = true;
		}
		else
		{
			Debug.LogError("Language not supported in Localzier " + displayName);
		}
	}

	public static bool HasKey(string key)
	{
		return instance.localizedStrings.ContainsKey(key);
	}

	public static string GetString(string key, bool localizeForInputMode = false)
	{
		string text = key;
		if (localizeForInputMode)
		{
			switch (DeviceSwitchController.CurrentDeviceMode)
			{
			case DeviceMode.MouseKeyboard:
				if (!text.ToLower().EndsWith("_keyboard".ToLower()))
				{
					text += "_keyboard";
				}
				break;
			case DeviceMode.Touch:
				if (!text.ToLower().EndsWith("_touch".ToLower()))
				{
					text += "_touch";
				}
				break;
			case DeviceMode.Controller:
				if (!text.ToLower().EndsWith("_controller".ToLower()))
				{
					text += "_controller";
				}
				break;
			}
		}
		string text2 = null;
		if (HasKey(text))
		{
			text2 = instance.localizedStrings[text];
		}
		else if (localizeForInputMode && HasKey(key))
		{
			text2 = instance.localizedStrings[key];
		}
		if (string.IsNullOrEmpty(text2))
		{
			text2 = string.Empty;
			Debug.Log("[Localizer] Could not find value for: " + key);
		}
		return text2;
	}

	private static void FormatString(ref string value)
	{
		if (value.Contains("<br>"))
		{
			value = value.Replace("<br>", "\n");
		}
	}
}
