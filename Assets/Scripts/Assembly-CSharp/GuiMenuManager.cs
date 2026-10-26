using System;
using System.Collections.Generic;
using Calamity.Input;
using Meta;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Camera))]
public class GuiMenuManager : MonoBehaviour
{
	public enum Sound
	{
		Forward = 0,
		Backward = 1,
		Confirm = 2,
		UnConfim = 3,
		MenuButton = 4,
		CharButton = 5,
		TypeSound = 6,
		CheatComplete = 7
	}

	public class Active
	{
		public GameObject Object;

		public MenuBase Base;
	}

	[Serializable]
	public class AudioFile
	{
		public Sound Type;

		public AudioClip sound;
	}

	[Serializable]
	public class StateUI
	{
		public GameObject Prefab;

		public MenuStates state;
	}

	[Serializable]
	public class UnitSpawnData
	{
		public string Name;

		public GameObject Unit;
	}

	public float Volume = 0.182f;

	public UIRoot Root;

	public List<GameObject> SoldierSpawnPoints;

	public List<UnitSpawnData> SpawnDataList;

	private MenuStates currentState;

	private MenuStates NextState;

	private Active activeMenu = new Active();

	private AudioSource SoundPlayer;

	public List<StateUI> States;

	public List<StateUI> StateTouchOverrides;

	public AudioFile[] AudioLibrary;

	public MainMenu MenuState;

	public Camera mainCamera;

	public Camera UICamera;

	public GameObject Screen;

	public GameObject PurchasePrefab;

	public GameObject ConfirmWindow;

	private PopupWindow windowPopup;

	private bool isDemo;

	private static bool firstTimeInMenu = true;

	private CameraPathHandler pathHandler;

	private PurchasePopup purchaseAd;

	private bool isPerkDefinesLocalized;

	private bool isWeaponDefinesLocalized;

	private PerkMenuDefinitions perkDefines;

	private SoldierMenuDefinitions soldierDefines;

	private LevelMenuDefinitions levelDefines;

	private StatisticsPerkDefinitions perkStatsDefines;

	private StatisticsWeaponDefinitions weaponStatsDefines;

	private PlayerManager playerManager;

	private GameSettings settings = new GameSettings();

	private static GuiMenuManager instance;

	private MenuMain mainMenuInstance;

	private int tutorialPlayerID;

	private string selectedLevel;

	private bool purchasePopupBuy;

	private UICamera guiCamera;

	public static GuiMenuManager Instance
	{
		get
		{
			return instance;
		}
	}

	public MenuStates CurrentState
	{
		get
		{
			return currentState;
		}
	}

	public UICamera GuiCamera
	{
		get
		{
			return guiCamera;
		}
	}

	public bool LocalizedPerkStats
	{
		get
		{
			return isPerkDefinesLocalized;
		}
		set
		{
			isPerkDefinesLocalized = value;
		}
	}

	public bool LocalizedWeaponStats
	{
		get
		{
			return isWeaponDefinesLocalized;
		}
		set
		{
			isWeaponDefinesLocalized = value;
		}
	}

	public StatisticsPerkDefinitions StatisticsPerk
	{
		get
		{
			return perkStatsDefines;
		}
	}

	public StatisticsWeaponDefinitions StatisticsWeapons
	{
		get
		{
			return weaponStatsDefines;
		}
	}

	public GameSettings Settings
	{
		get
		{
			return settings;
		}
		set
		{
			settings = value;
		}
	}

	private void Awake()
	{
		instance = this;
		guiCamera = GetComponentInChildren<UICamera>();
		bool flag = DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch;
		guiCamera.useMouse = !flag;
		guiCamera.useTouch = flag;
		guiCamera.allowMultiTouch = flag;
	}

	private void Start()
	{
		if (OuyaPurchaseManager.Instance != null && OuyaPurchaseManager.Instance.GamePurchased)
		{
			isDemo = false;
		}
		SoundPlayer = base.GetComponent<AudioSource>();
		pathHandler = mainCamera.GetComponent<CameraPathHandler>();
		levelDefines = GetComponent<LevelMenuDefinitions>();
		soldierDefines = GetComponent<SoldierMenuDefinitions>();
		perkDefines = GetComponent<PerkMenuDefinitions>();
		perkStatsDefines = GetComponent<StatisticsPerkDefinitions>();
		weaponStatsDefines = GetComponent<StatisticsWeaponDefinitions>();
		if (isDemo && !firstTimeInMenu)
		{
			Invoke("SpawnPurchasePopup", 0.3f);
		}
		else
		{
			firstTimeInMenu = false;
			activeMenu.Object = NGUITools.AddChild(UICamera.gameObject, GetStatePrefab(MenuStates.Main));
			activeMenu.Object.GetComponent<UIAnchor>().uiCamera = UICamera;
			activeMenu.Base = activeMenu.Object.GetComponent<MenuMain>();
			activeMenu.Base.SetManagerReference(this);
			((MenuMain)activeMenu.Base).SetDemoLock(isDemo);
			SetCurrentState(MenuStates.Main);
		}
		if (levelDefines == null)
		{
			Debug.LogWarning("[GuiMenuManager] Failed to located level menu definitions.");
		}
		if (soldierDefines == null)
		{
			Debug.LogWarning("[GuiMenuManager] Failed to located soldier menu definitions.");
		}
	}

	private void SpawnPurchasePopup()
	{
		if (PurchasePrefab != null)
		{
			GameObject gameObject = NGUITools.AddChild(UICamera.gameObject, PurchasePrefab);
			purchaseAd = gameObject.GetComponent<PurchasePopup>();
			if (purchaseAd != null)
			{
				purchaseAd.SetManagerReference(this);
			}
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < States.Count; i++)
		{
			States[i].Prefab = null;
		}
		States.Clear();
		PurchasePrefab = null;
		if (purchaseAd != null)
		{
			UnityEngine.Object.Destroy(purchaseAd.gameObject);
			purchaseAd = null;
		}
		Screen = null;
		UICamera = null;
		instance = null;
	}

	public GameObject GetStatePrefab(MenuStates state)
	{
		StateUI stateUI = null;
		if (BuildRuntimeSettings.Current.SupportsTouch)
		{
			stateUI = StateTouchOverrides.Find((StateUI a) => a.state == state);
			if (stateUI != null)
			{
				return stateUI.Prefab;
			}
		}
		stateUI = States.Find((StateUI a) => a.state == state);
		return (stateUI != null) ? stateUI.Prefab : null;
	}

	private void TempStartTutorial()
	{
		SetSelectedLevel("tutorial_scene");
		playerManager.RemoveAllPlayers();
		playerManager.AddPlayer(0, tutorialPlayerID);
		playerManager.GetPlayerByID(0).SetSelectedSoldier(Perk.SupportSpecialist, null, "TutorialSupportSpecialist");
		SetGameMode(GameModeSetting.Tutorial);
		SetGameDiffictulty(DifficultySetting.Normal);
		Settings.NumPlayers = 0;
		OnGameStart();
	}

	public void TempStartShootingRange()
	{
		SetSelectedLevel("shooting_range_scene");
		playerManager.RemoveAllPlayers();
		playerManager.AddPlayer(0);
		playerManager.GetPlayerByID(0).SetSelectedSoldier(Perk.Commando, null, "Commando1");
		SetGameMode(GameModeSetting.ShootingRange);
		SetGameDiffictulty(DifficultySetting.Normal);
		OnGameStart();
	}

	public void PlaySound(Sound type)
	{
		SoundPlayer.volume = Volume * AudioManager.VolumeSetting;
		AudioFile[] audioLibrary = AudioLibrary;
		foreach (AudioFile audioFile in audioLibrary)
		{
			if (audioFile != null && audioFile.Type == type && audioFile.sound != null)
			{
				SoundPlayer.PlayOneShot(audioFile.sound);
				break;
			}
		}
	}

	public void Setup(MainMenu menu, PlayerManager playerManager, GameSettings gameSettings)
	{
		MenuState = menu;
		this.playerManager = playerManager;
	}

	private void OnPurchaseCallBack(OuyaPurchaseManager.PurchaseOutcome outcome, string message)
	{
		if (outcome == OuyaPurchaseManager.PurchaseOutcome.Success)
		{
			if (!purchasePopupBuy)
			{
				((MenuMain)activeMenu.Base).TweenOutDemo();
			}
			isDemo = false;
			if (purchasePopupBuy)
			{
				purchasePopupBuy = false;
			}
			CreatePurchaseOutcomeWindow("gui_purchase_succeeded");
		}
		else
		{
			CreatePurchaseOutcomeWindow("gui_purchase_failed");
		}
	}

	private void CreatePurchaseOutcomeWindow(string message)
	{
		GameObject gameObject = NGUITools.AddChild(UICamera.gameObject, ConfirmWindow);
		windowPopup = gameObject.GetComponent<PopupWindow>();
		windowPopup.SetButton1(Localizer.GetString("gui_confirm"), PopupWindow.PopupState.Confirmed);
		windowPopup.SetPopupInfo(PopupType.Statement, Localizer.GetString(message));
		windowPopup.onConfirm = OnConfirm;
		windowPopup.SetManagerReference(this);
	}

	private void OnConfirm()
	{
		ChangeGui(MenuStates.Main);
		SetCurrentState(MenuStates.Main);
		UnityEngine.Object.Destroy(windowPopup.gameObject);
	}

	public void PurchasePopupAccept()
	{
		if (OuyaPurchaseManager.Instance != null && !OuyaPurchaseManager.Instance.GamePurchased)
		{
			purchasePopupBuy = true;
			if (purchaseAd != null)
			{
				purchaseAd.TweenOut();
			}
			OuyaPurchaseManager.Instance.PurchaseGame(OnPurchaseCallBack);
		}
	}

	public void PurchasePopupDeny()
	{
		activeMenu.Object = NGUITools.AddChild(UICamera.gameObject, GetStatePrefab(MenuStates.Main));
		activeMenu.Object.GetComponent<UIAnchor>().uiCamera = UICamera;
		activeMenu.Base = activeMenu.Object.GetComponent<MenuMain>();
		activeMenu.Base.SetManagerReference(this);
		((MenuMain)activeMenu.Base).SetDemoLock(isDemo);
		SetCurrentState(MenuStates.Main);
	}

	public void OnPurchasePressed()
	{
		if (OuyaPurchaseManager.Instance != null && !OuyaPurchaseManager.Instance.GamePurchased)
		{
			LoadState(MenuStates.Purchase);
		}
	}

	public void LoadState(MenuStates state, bool skip = false)
	{
		switch (state)
		{
		case MenuStates.UnlockPerks:
			LevelUpAllPerksByLevel(1);
			break;
		case MenuStates.UnlockWeapons:
			LevelUpAllWeaponsByLevel(1);
			break;
		case MenuStates.Purchase:
		case MenuStates.Credits:
		case MenuStates.Statistics:
			activeMenu.Base = null;
			UnityEngine.Object.Destroy(activeMenu.Object);
			activeMenu = new Active();
			NextState = state;
			ChangeGui(state);
			break;
		default:
			activeMenu.Base = null;
			UnityEngine.Object.Destroy(activeMenu.Object);
			activeMenu = new Active();
			NextState = state;
			pathHandler.Play(currentState, NextState, skip);
			break;
		}
	}

	private void SetCurrentState(MenuStates state)
	{
		currentState = state;
	}

	public void OnCameraTransitionComplete()
	{
		SetCurrentState(NextState);
		ChangeGui(currentState);
	}

	public void ChangeGui(MenuStates state, bool forceCamera = false)
	{
		if (activeMenu.Base != null)
		{
			activeMenu.Base = null;
			UnityEngine.Object.Destroy(activeMenu.Object);
			activeMenu = new Active();
			SetCurrentState(state);
		}
		switch (state)
		{
		case MenuStates.Main:
			activeMenu.Object = NGUITools.AddChild(UICamera.gameObject, GetStatePrefab(MenuStates.Main));
			activeMenu.Base = activeMenu.Object.GetComponent<MenuMain>();
			activeMenu.Base.SetManagerReference(this);
			((MenuMain)activeMenu.Base).SetDemoLock(isDemo);
			break;
		case MenuStates.Options:
			activeMenu.Object = NGUITools.AddChild(UICamera.gameObject, GetStatePrefab(MenuStates.Options));
			activeMenu.Base = activeMenu.Object.GetComponent<MenuOptions>();
			activeMenu.Base.SetManagerReference(this);
			break;
		case MenuStates.GameSetup:
			activeMenu.Object = NGUITools.AddChild(UICamera.gameObject, GetStatePrefab(MenuStates.GameSetup));
			activeMenu.Base = activeMenu.Object.GetComponent<MenuGameSetup>();
			activeMenu.Base.SetManagerReference(this);
			((MenuGameSetup)activeMenu.Base).SetDemoLock(isDemo);
			((MenuGameSetup)activeMenu.Base).SetLevelData(levelDefines.LevelData);
			break;
		case MenuStates.CharacterSelect:
			activeMenu.Object = NGUITools.AddChild(UICamera.gameObject, GetStatePrefab(MenuStates.CharacterSelect));
			activeMenu.Base = activeMenu.Object.GetComponent<MenuCharacterSelect>();
			activeMenu.Base.SetManagerReference(this);
			((MenuCharacterSelect)activeMenu.Base).SetDemoLock(isDemo);
			((MenuCharacterSelect)activeMenu.Base).SetPlayerManagerReference(playerManager);
			((MenuCharacterSelect)activeMenu.Base).PopulateCharacterList(soldierDefines, perkDefines);
			((MenuCharacterSelect)activeMenu.Base).SetSoldierSpawnData(SoldierSpawnPoints, SpawnDataList);
			break;
		case MenuStates.Credits:
			activeMenu.Object = NGUITools.AddChild(UICamera.gameObject, GetStatePrefab(MenuStates.Credits));
			activeMenu.Base = activeMenu.Object.GetComponent<MenuCredits>();
			activeMenu.Base.SetManagerReference(this);
			break;
		case MenuStates.Statistics:
			activeMenu.Object = NGUITools.AddChild(UICamera.gameObject, GetStatePrefab(MenuStates.Statistics));
			activeMenu.Base = activeMenu.Object.GetComponent<MenuStatistics>();
			activeMenu.Base.SetManagerReference(this);
			((MenuStatistics)activeMenu.Base).PerkMenuDefines = perkDefines;
			((MenuStatistics)activeMenu.Base).IsDemo = isDemo;
			break;
		case MenuStates.Purchase:
			if (PurchasePrefab != null)
			{
				activeMenu.Object = NGUITools.AddChild(UICamera.gameObject, PurchasePrefab);
				purchaseAd = activeMenu.Object.GetComponent<PurchasePopup>();
				if (purchaseAd != null)
				{
					purchaseAd.SetManagerReference(this);
				}
			}
			break;
		case MenuStates.Tutorial:
			TempStartTutorial();
			break;
		case MenuStates.GameStart:
			OnGameStart();
			break;
		case MenuStates.Exit:
			Application.Quit();
			break;
		}
	}

	public LevelDisplayData[] GetLevelsData()
	{
		return levelDefines.LevelData;
	}

	public void SetSelectedLevel(string level)
	{
		settings.LevelChosenAtRandom = false;
		if (level == "random")
		{
			LevelDisplayData levelDisplayData = levelDefines.LevelData[UnityEngine.Random.Range(0, levelDefines.LevelData.Length)];
			if (isDemo)
			{
				List<LevelDisplayData> list = new List<LevelDisplayData>();
				for (int i = 0; i < levelDefines.LevelData.Length; i++)
				{
					if (!levelDefines.LevelData[i].DemoLocked && levelDefines.LevelData[i].name != "random")
					{
						list.Add(levelDefines.LevelData[i]);
					}
				}
				if (list.Count > 0)
				{
					levelDisplayData = list[UnityEngine.Random.Range(0, list.Count)];
				}
				else
				{
					Debug.Log("[GuiMenuManager]:SetSelectedLevel() - Could not find any unlocked level (Make sure atleast 1 level is unlocked in order to start the game!)");
				}
			}
			else
			{
				while (levelDisplayData.name == level)
				{
					levelDisplayData = levelDefines.LevelData[UnityEngine.Random.Range(0, levelDefines.LevelData.Length)];
				}
			}
			settings.LevelName = levelDisplayData.name;
			settings.GuiLevelName = levelDisplayData.guiName;
			settings.LevelChosenAtRandom = true;
			return;
		}
		if (level == "tutorial_scene")
		{
			settings.LevelName = level;
			return;
		}
		for (int j = 0; j < levelDefines.LevelData.Length; j++)
		{
			if (levelDefines.LevelData[j].name == level)
			{
				settings.LevelName = levelDefines.LevelData[j].name;
				settings.GuiLevelName = levelDefines.LevelData[j].guiName;
			}
		}
	}

	public void PerformControllerLinkAction(ControllerLinkGUIAction action, int ID)
	{
		if (currentState == MenuStates.Options)
		{
			UILinkController controllerLinkMenu = ((MenuOptions)activeMenu.Base).ControllerLinkMenu;
			if (controllerLinkMenu != null)
			{
				controllerLinkMenu.PerformControllerLinkAction(action, ID);
			}
		}
	}

	public void TriggerControllerLinkSelection(RelinkControllerTypeCallback callback, RelinkAxisInput axisInput, RelinkConfirmInput confirmInput, List<ControllerType> connectedControllerTypes)
	{
		if (currentState == MenuStates.Options)
		{
			UILinkController controllerLinkMenu = ((MenuOptions)activeMenu.Base).ControllerLinkMenu;
			if (controllerLinkMenu != null)
			{
				controllerLinkMenu.TriggerControllerLinkSelection(callback, axisInput, confirmInput, connectedControllerTypes);
			}
		}
	}

	public void SetGameMode(GameModeSetting mode)
	{
		settings.GameMode = mode;
	}

	public void SetGameDiffictulty(DifficultySetting difficulty)
	{
		settings.Difficulty = difficulty;
	}

	public void SetGameLength(GameLengthSetting length)
	{
		settings.GameLength = length;
	}

	public void OnGameStart()
	{
		settings.NumPlayers = PlayerManager.Instance.NumberOfPlayers;
		MenuState.OnGameStart(settings);
		Root.gameObject.SetActive(false);
		Root = null;
	}

	public void ChangeScreenMaterial(Material mat)
	{
		if (mat != null)
		{
			Screen.GetComponent<MeshRenderer>().material = mat;
		}
	}

	public void SetScreenRandomUVEnableState(bool isOn)
	{
		if (Screen != null)
		{
			AnimatedUVsRandom component = Screen.GetComponent<AnimatedUVsRandom>();
			if (component != null)
			{
				component.enabled = isOn;
			}
		}
	}

	public void SetTutorialPlayerID(int id)
	{
		tutorialPlayerID = id;
	}

	private void LevelUpAllPerksByLevel(int level)
	{
		MetaManager metaManager = MetaManager.Instance;
		PerkStatisticsDisplayData[] perkData = StatisticsPerk.PerkData;
		if (metaManager != null && perkData != null)
		{
			PerkStatisticsDisplayData[] array = perkData;
			foreach (PerkStatisticsDisplayData perkStatisticsDisplayData in array)
			{
				Meta.PerkData perkData2 = metaManager.CreateOrGetPerkData(perkStatisticsDisplayData.Perk);
				int level2 = level + perkData2.Level;
				perkData2.SetData(level2, metaManager.PerkExperienceByLevel(level2));
			}
		}
	}

	private void LevelUpAllWeaponsByLevel(int level)
	{
		MetaManager metaManager = MetaManager.Instance;
		WeaponStatisticsDisplayData[] weaponData = StatisticsWeapons.WeaponData;
		if (metaManager != null && weaponData != null)
		{
			WeaponStatisticsDisplayData[] array = weaponData;
			foreach (WeaponStatisticsDisplayData weaponStatisticsDisplayData in array)
			{
				WeaponData weaponData2 = metaManager.CreateOrGetWeaponData(weaponStatisticsDisplayData.Name);
				int level2 = level + weaponData2.Level;
				weaponData2.SetData(level2, metaManager.WeaponExperienceByLevel(level2));
			}
		}
	}

	private void UnlockPerks(int level)
	{
		MetaManager metaManager = MetaManager.Instance;
		PerkStatisticsDisplayData[] perkData = StatisticsPerk.PerkData;
		if (metaManager != null && perkData != null)
		{
			PerkStatisticsDisplayData[] array = perkData;
			foreach (PerkStatisticsDisplayData perkStatisticsDisplayData in array)
			{
				Meta.PerkData perkData2 = metaManager.CreateOrGetPerkData(perkStatisticsDisplayData.Perk);
				perkData2.SetData(level, metaManager.PerkExperienceByLevel(level));
			}
		}
	}

	private void UnlockWeapons(int level)
	{
		MetaManager metaManager = MetaManager.Instance;
		WeaponStatisticsDisplayData[] weaponData = StatisticsWeapons.WeaponData;
		if (metaManager != null && weaponData != null)
		{
			WeaponStatisticsDisplayData[] array = weaponData;
			foreach (WeaponStatisticsDisplayData weaponStatisticsDisplayData in array)
			{
				WeaponData weaponData2 = metaManager.CreateOrGetWeaponData(weaponStatisticsDisplayData.Name);
				weaponData2.SetData(level, metaManager.WeaponExperienceByLevel(level));
			}
		}
	}
}
