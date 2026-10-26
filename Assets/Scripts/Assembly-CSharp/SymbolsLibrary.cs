using System;
using UnityEngine;

public class SymbolsLibrary : MonoBehaviour
{
	[Serializable]
	public class SymbolEntry
	{
		public string EntryName;

		[Space(5f)]
		public string KeyboardEntry;

		public string KeyboardInputText;

		public bool IsSpriteKeyboardEntry;

		[Space(5f)]
		public string TouchEntry;

		public string TouchInputText;

		public bool IsSpriteTouchEntry = true;

		[Space(5f)]
		public string ControllerEntry;

		public string ControllerInputText;

		public bool IsSpriteControllerEntry = true;
	}

	private static SymbolsLibrary instance;

	public bool LocalizeKeyboardEntries;

	public bool LocalizeTouchEntries;

	public bool LocalizeControllerEntries;

	[Space(10f)]
	public string KeyboardSurroundingCharacters = "[ ]";

	[Space(10f)]
	public SymbolEntry[] Entries;

	private string keyboardEntryStart = string.Empty;

	private string keyboardEntryEnd = string.Empty;

	public static SymbolsLibrary Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject original = Resources.Load<GameObject>("Prefabs/Misc/Symbol Library");
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
				instance = gameObject.GetComponent<SymbolsLibrary>();
			}
			return instance;
		}
	}

	private void Awake()
	{
		if (!string.IsNullOrEmpty(KeyboardSurroundingCharacters))
		{
			string[] array = KeyboardSurroundingCharacters.Split(new string[1] { "%s" }, StringSplitOptions.None);
			if (array.Length != 2)
			{
				Debug.LogWarning("Could not split the KeyboardSurroundingCharacters into two parts. Use '%s' to separate the characters", this);
				return;
			}
			keyboardEntryStart = array[0];
			keyboardEntryEnd = array[1];
		}
	}

	private bool ShouldLocalize()
	{
		DeviceMode currentDeviceMode = DeviceSwitchController.CurrentDeviceMode;
		return (currentDeviceMode == DeviceMode.MouseKeyboard && LocalizeKeyboardEntries) || (currentDeviceMode == DeviceMode.Touch && LocalizeTouchEntries) || (currentDeviceMode == DeviceMode.Controller && LocalizeControllerEntries);
	}

	private string FormatEntryString(string entryValue)
	{
		string empty = string.Empty;
		empty = ((!ShouldLocalize() || !Localizer.HasKey(entryValue)) ? entryValue : Localizer.GetString(entryValue));
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.MouseKeyboard)
		{
			empty = keyboardEntryStart + empty + keyboardEntryEnd;
		}
		return empty;
	}

	private string GetInputTextString(string inputTextKey)
	{
		string result = string.Empty;
		if (!string.IsNullOrEmpty(inputTextKey))
		{
			result = Localizer.GetString(inputTextKey, true);
		}
		return result;
	}

	public SymbolReturnEntry GetSymbolForCurrentInputMode(string entryName)
	{
		SymbolEntry symbolEntry = Array.Find(Entries, (SymbolEntry a) => a.EntryName == entryName);
		SymbolReturnEntry result = null;
		if (symbolEntry == null)
		{
			Debug.LogError("Could not find entry: \"" + entryName + "\" in the SymbolLibrary");
			return null;
		}
		switch (DeviceSwitchController.CurrentDeviceMode)
		{
		case DeviceMode.MouseKeyboard:
			result = new SymbolReturnEntry(FormatEntryString(symbolEntry.KeyboardEntry), GetInputTextString(symbolEntry.KeyboardInputText), symbolEntry.IsSpriteKeyboardEntry);
			break;
		case DeviceMode.Touch:
			result = new SymbolReturnEntry(FormatEntryString(symbolEntry.TouchEntry), GetInputTextString(symbolEntry.TouchInputText), symbolEntry.IsSpriteTouchEntry);
			break;
		case DeviceMode.Controller:
			result = new SymbolReturnEntry(FormatEntryString(symbolEntry.ControllerEntry), GetInputTextString(symbolEntry.ControllerInputText), symbolEntry.IsSpriteControllerEntry);
			break;
		}
		return result;
	}
}
