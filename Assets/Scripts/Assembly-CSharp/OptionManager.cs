using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class OptionManager : MonoBehaviour
{
	private struct Settings
	{
		public float musicVolume;

		public float sfxVolume;

		public float brightness;

		public InputMode inputMode;

		public TouchStickType StickType;

		public Vector2 buttonBarPosition;

		public Resolution currentResolution;

		public bool showOverlay;

		public int buttonLayout;

		public Settings(Resolution defaultResolution)
		{
			musicVolume = 0.5f;
			sfxVolume = 0.5f;
			brightness = 0.5f;
			inputMode = InputMode.Normal;
			StickType = TouchStickType.Unknown;
			buttonBarPosition = ButtonBarDefaultPosition;
			currentResolution = defaultResolution;
			showOverlay = true;
			buttonLayout = 2;
		}
	}

	private const char splitChar = ':';

	public FPSController FpsController;

	private static OptionManager instance;

	private Settings gameSettings;

	private DifficultySetting difficultySetting = DifficultySetting.Beginner;

	private GameLengthSetting gameLengthSetting = GameLengthSetting.Normal;

	private string levelSetting = string.Empty;

	private int hasDisplayedTutorialPopup;

	private bool isStickTypeTemporary;

	private List<string> settings = new List<string>();

	private FileManager fileManager;

	private static string filename = "options_settings";

	private static Vector2 ButtonBarDefaultPosition = new Vector2(0.84f, 0.46f);

	public static OptionManager Instance
	{
		get
		{
			return instance;
		}
	}

	public float MusicVolume
	{
		get
		{
			return gameSettings.musicVolume;
		}
		set
		{
			gameSettings.musicVolume = value;
			ApplyMusicSetting();
		}
	}

	public float SfxVolume
	{
		get
		{
			return gameSettings.sfxVolume;
		}
		set
		{
			gameSettings.sfxVolume = value;
			ApplySfxSetting();
		}
	}

	public float Brightness
	{
		get
		{
			return gameSettings.brightness;
		}
		set
		{
			gameSettings.brightness = value;
			ApplyBrightness();
		}
	}

	public InputMode InputMode
	{
		get
		{
			return gameSettings.inputMode;
		}
		set
		{
			gameSettings.inputMode = value;
			ApplyInputSetting();
		}
	}

	public TouchStickType StickType
	{
		get
		{
			return gameSettings.StickType;
		}
		set
		{
			gameSettings.StickType = value;
			ApplyTouchStickType();
		}
	}

	public bool ShowOverlay
	{
		get
		{
			return gameSettings.showOverlay;
		}
		set
		{
			gameSettings.showOverlay = value;
		}
	}

	public GameLengthSetting GameLengthSetting
	{
		get
		{
			return gameLengthSetting;
		}
		set
		{
			gameLengthSetting = value;
		}
	}

	public DifficultySetting DifficultySetting
	{
		get
		{
			return difficultySetting;
		}
		set
		{
			difficultySetting = value;
		}
	}

	public string LevelSetting
	{
		get
		{
			return levelSetting;
		}
		set
		{
			levelSetting = value;
		}
	}

	public int TutorialPopup
	{
		get
		{
			return hasDisplayedTutorialPopup;
		}
		set
		{
			hasDisplayedTutorialPopup = value;
		}
	}

	public Vector2 ButtonBarPosition
	{
		get
		{
			return gameSettings.buttonBarPosition;
		}
		set
		{
			gameSettings.buttonBarPosition = value;
		}
	}

	public int ButtonLayout
	{
		get
		{
			return gameSettings.buttonLayout;
		}
		set
		{
			gameSettings.buttonLayout = value;
		}
	}

	public Resolution CurrentResolution
	{
		get
		{
			return gameSettings.currentResolution;
		}
		set
		{
			gameSettings.currentResolution = value;
		}
	}

	public bool IsStickTypeTemporary
	{
		get
		{
			return isStickTypeTemporary;
		}
		set
		{
			isStickTypeTemporary = value;
		}
	}

	private Resolution GetDefaultResolution()
	{
		Resolution result = default(Resolution);
		int num = 1280;
		double num2 = (double)Screen.currentResolution.width / (double)Screen.currentResolution.height;
		result.width = num;
		result.height = Mathf.RoundToInt((float)((double)num / num2));
		return result;
	}

	private void Awake()
	{
		instance = this;
		fileManager = FileManager.Instance;
		if (FpsController != null)
		{
			FpsController.enabled = false;
		}
	}

	private void Start()
	{
		LoadSettings();
		ApplyAllSettings();
		SetSaveData(Serialize());
		Save();
		if (FpsController != null)
		{
			FpsController.enabled = true;
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void ApplyMusicSetting()
	{
		MusicManager.SetGlobalVolume(gameSettings.musicVolume);
	}

	private void ApplySfxSetting()
	{
		AudioManager.SetGlobalVolume(gameSettings.sfxVolume);
	}

	private void ApplyInputSetting()
	{
		InputManager.SetInputMode(gameSettings.inputMode);
	}

	public void ApplyResoultion()
	{
		Screen.SetResolution(gameSettings.currentResolution.width, gameSettings.currentResolution.height, true);
	}

	public void ApplyTouchStickType()
	{
		InputManager.Instance.SetTouchStickType(gameSettings.StickType);
	}

	public void ApplyBrightness()
	{
	}

	public void RestoreDefaults()
	{
		gameSettings = new Settings(GetDefaultResolution());
		SetSaveData(Serialize());
		Save();
		LoadSettings();
		ApplyAllSettings();
	}

	public void SetSaveData(List<string> settings)
	{
		this.settings = settings;
	}

	public void Save()
	{
		fileManager.Save(fileManager.RootPath + filename, settings);
	}

	public void LoadSettings()
	{
		gameSettings = new Settings(GetDefaultResolution());
		List<string> list = fileManager.Load(fileManager.RootPath + filename);
		if (list != null)
		{
			settings = list;
			Deserialize(list);
		}
	}

	public void ApplyAllSettings()
	{
		ApplyResoultion();
		ApplyMusicSetting();
		ApplySfxSetting();
		ApplyInputSetting();
		ApplyTouchStickType();
	}

	public List<string> Serialize()
	{
		List<string> list = new List<string>();
		list.Add("music:" + MusicVolume * 10f);
		list.Add("sfx:" + SfxVolume * 10f);
		int inputMode = (int)gameSettings.inputMode;
		list.Add("input:" + inputMode);
		list.Add("gameLengthSetting:" + GameLengthSetting);
		list.Add("difficultySetting:" + DifficultySetting);
		list.Add("levelSetting:" + LevelSetting);
		list.Add("tutorial:" + hasDisplayedTutorialPopup);
		list.Add("buttonBarPosition:" + ButtonBarPosition.x + ";" + ButtonBarPosition.y);
		list.Add("resolution:" + gameSettings.currentResolution.width + "x" + gameSettings.currentResolution.height);
		list.Add("brightness:" + gameSettings.brightness * 10f);
		list.Add("overlay:" + (gameSettings.showOverlay ? 1 : 0));
		list.Add("buttonLayout:" + gameSettings.buttonLayout);
		if (isStickTypeTemporary)
		{
			gameSettings.StickType = TouchStickType.Unknown;
		}
		int stickType = (int)gameSettings.StickType;
		list.Add("sticktype:" + stickType);
		if (isStickTypeTemporary)
		{
			gameSettings.StickType = TouchStickType.Locked;
		}
		return list;
	}

	public virtual void Deserialize(List<string> lines)
	{
		foreach (string line in lines)
		{
			int num = line.IndexOf(':');
			if (num == -1)
			{
				continue;
			}
			string text = line.Substring(0, num).Trim();
			string text2 = line.Substring(num + 1);
			text2 = text2.Trim();
			switch (text)
			{
			case "music":
			{
				int num2 = int.Parse(text2);
				MusicVolume = Mathf.Clamp01((float)num2 * 0.1f);
				break;
			}
			case "sfx":
			{
				int num3 = int.Parse(text2);
				SfxVolume = Mathf.Clamp01((float)num3 * 0.1f);
				break;
			}
			case "input":
				gameSettings.inputMode = (InputMode)int.Parse(text2);
				break;
			case "difficultySetting":
				difficultySetting = (DifficultySetting)(int)Enum.Parse(typeof(DifficultySetting), text2);
				break;
			case "gameLengthSetting":
				gameLengthSetting = (GameLengthSetting)(int)Enum.Parse(typeof(GameLengthSetting), text2);
				break;
			case "levelSetting":
				levelSetting = text2;
				break;
			case "tutorial":
				hasDisplayedTutorialPopup = int.Parse(text2);
				break;
			case "brightness":
			{
				int num6 = int.Parse(text2);
				Brightness = (float)num6 * 0.1f;
				break;
			}
			case "sticktype":
			{
				int num5 = int.Parse(text2);
				TouchStickType stickType = (TouchStickType)num5;
				StickType = stickType;
				break;
			}
			case "overlay":
			{
				int num4 = int.Parse(text2);
				gameSettings.showOverlay = num4 == 1;
				break;
			}
			case "buttonLayout":
			{
				int buttonLayout = int.Parse(text2);
				gameSettings.buttonLayout = buttonLayout;
				break;
			}
			case "buttonBarPosition":
			{
				string[] array2 = text2.Split(';');
				float result3 = 0f;
				float result4 = 0f;
				bool flag2 = true;
				if (array2.Length == 2)
				{
					if (!float.TryParse(array2[0].Trim(), out result3) || !float.TryParse(array2[1].Trim(), out result4))
					{
						flag2 = false;
					}
				}
				else
				{
					flag2 = false;
				}
				if (!flag2)
				{
					Debug.LogWarning("Unable to parse the buttonbar position! Reverting to default position");
				}
				else
				{
					gameSettings.buttonBarPosition = new Vector2(result3, result4);
				}
				break;
			}
			case "resolution":
			{
				string[] array = text2.Split('x');
				int result = 0;
				int result2 = 0;
				bool flag = true;
				if (array.Length == 2)
				{
					if (!int.TryParse(array[0].Trim(), out result) || !int.TryParse(array[1].Trim(), out result2))
					{
						flag = false;
					}
					if (result == 0 || result2 == 0)
					{
						flag = false;
					}
				}
				else
				{
					flag = false;
				}
				if (!flag)
				{
					Debug.LogWarning("Unable to load resolution from settings file. Setting resolution to current screen resolution: " + Screen.currentResolution.width + "x" + Screen.currentResolution.height);
				}
				else
				{
					gameSettings.currentResolution.width = result;
					gameSettings.currentResolution.height = result2;
				}
				break;
			}
			}
		}
	}
}
