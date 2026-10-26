using System;
using System.Collections.Generic;
using Calamity.Camera;
using Calamity.Input;
using EventHelpers;
using GameEvents;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Camera))]
public class GuiInGameManager : MonoBehaviour
{
	public enum Sound
	{
		Forward = 0,
		Backward = 1,
		MenuButton = 2,
		ShopList = 3,
		ShopButton = 4,
		ShopPurchaseOther = 5,
		ShopPurchaseWeapon = 6,
		ShopPurchaseUnable = 7,
		ShopSell = 8,
		HudUnlock = 9,
		MedalEarned = 10,
		AchievementEarned = 11,
		MessageSound = 12,
		ProgressBar = 13
	}

	public enum PauseModeEnum
	{
		None = 0,
		Normal = 1,
		WeaponTray = 2,
		Shop = 3
	}

	[Serializable]
	public class AudioFile
	{
		public Sound Type;

		public AudioClip sound;
	}

	private const string PLAYER_HUD_RESOURCE_PATH = "GUI/Player Hud";

	private const string PLAYER_TOUCH_HUD_RESOURCE_PATH = "GUI/Player Touch Hud";

	public GUISkin ShopSkin;

	public float Volume = 0.182f;

	public UILinkController LinkController;

	public GameObject UIHud;

	public GameObject UIOtherPanel;

	public GameObject UIMovingWidgetPanel;

	public GameObject EndScreenPrefab;

	public GameObject EndScreenTutorialPrefab;

	public GameObject DamageTextPrefab;

	public GameObject ZedBarPrefab;

	public Camera uiCamera;

	public Vector2[] playerHudPixelOffsets;

	public AudioFile[] AudioLibrary;

	public ShopController ShopController;

	public UnitBossBar bossBar;

	public MessageBoard MessageDisplay;

	public GameInfo GameStatistics;

	public PauseMenu PauseWindow;

	public GuiUnlockController UnlockController;

	public UITexture ZedTimeTexture;

	public TweenAlpha ZedTimeAlphaTween;

	public TweenScale ZedTimeScaleTween;

	public UILabel ShopBackButtonLabel;

	public GameObject StickTypePopup;

	public ControlOverlay[] ControlOverlays;

	private GameObject questLog;

	private GameObject dialogMessage;

	private GameObject endScreen;

	private AudioSource SoundPlayer;

	private float zedTimeDuration;

	private float zedTimeFadeInDuration;

	private float zedTimeFadeOutDuration;

	private bool isZedTimeActive;

	private bool isZedTimeTimerActive;

	private bool isZedTimeBlendingOut;

	private PlayerGUI[] PlayerGuis = new PlayerGUI[4];

	private Camera mainCamera;

	private CameraController cameraController;

	private bool isCameraInitialized;

	private bool endScreenVisible;

	private bool gamePaused;

	private bool bossBarVisible;

	private bool showGameInfo;

	private bool pauseMenuVisable;

	private bool inputLocked;

	private bool cameraEffectVisible;

	private int currentWave = -1;

	private int maxWaves;

	private int tutorialPlayerInputID;

	private GameObject dualSticks;

	private WeaponTrayDrag weaponTray;

	private GameObject oldFallThrough;

	private UICamera guiCamera;

	private bool isSwitchingGUI;

	private InterfaceType currentInterfaceType;

	private int enemiesLeft;

	private PopupWindow stickTypePopup;

	private GameObject oldGameStatistics;

	private static PauseModeEnum pauseMode;

	private static GuiInGameManager instance;

	public static PauseModeEnum PauseMode
	{
		get
		{
			return pauseMode;
		}
	}

	public WeaponTrayDrag WeaponTrayDrag
	{
		get
		{
			return weaponTray;
		}
	}

	public bool IsEndScreenVisible
	{
		get
		{
			return endScreenVisible;
		}
	}

	public bool IsGamePaused
	{
		get
		{
			return gamePaused;
		}
	}

	public bool IsWeaponTrayVisible
	{
		get
		{
			return !(weaponTray == null) && weaponTray.IsOpen();
		}
	}

	public UICamera GUICamera
	{
		get
		{
			return guiCamera;
		}
	}

	public bool IsSwitchingGUI
	{
		get
		{
			return isSwitchingGUI;
		}
	}

	public InterfaceType CurrentInterfaceType
	{
		get
		{
			return currentInterfaceType;
		}
	}

	public Camera GameCamera
	{
		get
		{
			return mainCamera;
		}
	}

	public static GuiInGameManager Instance
	{
		get
		{
			return instance;
		}
	}

	public void ReturnToMainMenu(bool levelSelect = false)
	{
		GameState.Instance.ReloadMainMenu(levelSelect);
	}

	public void RetryLevel()
	{
		GameState.Instance.ReloadGameState();
	}

	private void Awake()
	{
		instance = this;
		EventSystem eventSystem = EventSystem.Instance;
		if (eventSystem != null)
		{
			eventSystem.Subscribe(EventId.PerkLevelUp, OnMetaEvent, this);
			eventSystem.Subscribe(EventId.WeaponUnlocked, OnMetaEvent, this);
			eventSystem.Subscribe(EventId.WeaponLevelUp, OnMetaEvent, this);
		}
		oldGameStatistics = GameStatistics.gameObject;
		guiCamera = GetComponentInChildren<UICamera>();
		bool flag = DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch;
		guiCamera.useMouse = !flag;
		guiCamera.useTouch = flag;
		guiCamera.allowMultiTouch = flag;
	}

	private ControlOverlay FindOverlayByName(string overlayName)
	{
		return (ControlOverlays == null) ? null : Array.Find(ControlOverlays, (ControlOverlay a) => a.Name == overlayName);
	}

	private void ActivateOverlay()
	{
		GameSettings gameSettings = GameState.Instance.GameSettings;
		if (gameSettings != null && gameSettings.GameMode == GameModeSetting.Tutorial)
		{
			return;
		}
		OptionManager optionManager = OptionManager.Instance;
		if (optionManager.ShowOverlay && DeviceSwitchController.CurrentDeviceMode != DeviceMode.Touch)
		{
			ControlOverlay controlOverlay = null;
			string overlayName = string.Empty;
			if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.MouseKeyboard)
			{
				overlayName = "Keyboard Overlay";
			}
			else if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
			{
				overlayName = "Controller Overlay";
			}
			controlOverlay = FindOverlayByName(overlayName);
			if (controlOverlay != null)
			{
				controlOverlay.ToggleActive(true);
			}
		}
	}

	private void Start()
	{
		InitializeUI();
		ActivateOverlay();
	}

	private void OnDestroy()
	{
		instance = null;
		EndScreenPrefab = null;
		ZedBarPrefab = null;
		DamageTextPrefab = null;
		UIHud = null;
		UIOtherPanel = null;
		UIMovingWidgetPanel = null;
		PauseWindow = null;
		GameStatistics = null;
		MessageDisplay = null;
		bossBar = null;
		UnlockController = null;
		PlayerGuis = null;
		if (LinkController != null)
		{
			UnityEngine.Object.Destroy(LinkController.gameObject);
			LinkController = null;
		}
		if (EventSystem.Instance != null)
		{
			EventSystem.Instance.Unsubscribe(this);
		}
	}

	private void InitializeUI()
	{
		PauseWindow.SetManagerReference(this);
		UnlockController.SetManagerReference(this);
		MessageDisplay.SetManagerReference(this);
	}

	private void InitializeCamera()
	{
		if (mainCamera == null)
		{
			mainCamera = Camera.main;
			if (mainCamera != null)
			{
				cameraController = mainCamera.GetComponent<CameraController>();
			}
			SoundPlayer = mainCamera.GetComponent("AudioSource") as AudioSource;
			SoundPlayer.volume = Volume * AudioManager.VolumeSetting;
		}
		isCameraInitialized = true;
	}

	private void Update()
	{
		if (DeviceSwitchController.CurrentInterfaceType == InterfaceType.Touch)
		{
			CheckWeaponTrayInput();
		}
		if (stickTypePopup != null && stickTypePopup.Status != 0)
		{
			OnStickTypePopupPressed();
		}
		CheckPauseInput();
		OnZedTimeTick();
	}

	private void OnStickTypePopupPressed()
	{
		TouchStickType stickType = TouchStickType.Unknown;
		if (stickTypePopup.Status == PopupWindow.PopupState.Option1)
		{
			stickType = TouchStickType.Locked;
		}
		else if (stickTypePopup.Status == PopupWindow.PopupState.Option2)
		{
			stickType = TouchStickType.Dynamic;
		}
		OptionManager optionManager = OptionManager.Instance;
		optionManager.StickType = stickType;
		optionManager.IsStickTypeTemporary = false;
		optionManager.SetSaveData(optionManager.Serialize());
		optionManager.Save();
		UnityEngine.Object.Destroy(stickTypePopup.gameObject);
		stickTypePopup = null;
		GameState.Instance.ReloadMainMenu();
	}

	private void OnGUI()
	{
		if (ShopController.IsShopOpen() && !gamePaused)
		{
			ShopController.Display();
		}
	}

	public void PlaySound(Sound type)
	{
		if (!SoundPlayer)
		{
			return;
		}
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

	public void TriggerControllerLinkSelection(RelinkControllerTypeCallback callback, RelinkAxisInput axisInput, RelinkConfirmInput confirmInput, List<ControllerType> connectedControllerTypes)
	{
		if (LinkController != null)
		{
			LinkController.TriggerControllerLinkSelection(callback, axisInput, confirmInput, connectedControllerTypes);
		}
	}

	public void PerformControllerLinkAction(ControllerLinkGUIAction action, int ID)
	{
		if (!(LinkController != null))
		{
			return;
		}
		switch (action)
		{
		case ControllerLinkGUIAction.Open:
			PauseGame();
			ShowCameraEffects();
			if (pauseMenuVisable)
			{
				PauseWindow.gameObject.SetActive(false);
			}
			if (endScreenVisible)
			{
				endScreen.SetActive(false);
			}
			if (ShopController.IsShopOpen())
			{
				ShopController.gameObject.SetActive(false);
			}
			break;
		case ControllerLinkGUIAction.Close:
			if (endScreenVisible)
			{
				endScreen.SetActive(true);
			}
			if (pauseMenuVisable)
			{
				PauseWindow.gameObject.SetActive(true);
			}
			else
			{
				UnPauseGame();
				HideCameraEffects();
			}
			if (ShopController.IsShopOpen())
			{
				ShopController.gameObject.SetActive(true);
			}
			break;
		}
		LinkController.PerformControllerLinkAction(action, ID);
	}

	private bool ResetIfPossible()
	{
		if (!LinkController.gameObject.activeSelf)
		{
			return true;
		}
		return false;
	}

	public DamageText AddDamageText(GameObject unit, Health healthComponent, DamageTextSetting settings)
	{
		GameObject gameObject = NGUITools.AddChild(UIMovingWidgetPanel, DamageTextPrefab);
		DamageText component = gameObject.GetComponent<DamageText>();
		if (component != null)
		{
			component.SetSettings(settings);
			component.SetHealthReference(healthComponent);
			component.SetCameraReferences(mainCamera, uiCamera);
			return component;
		}
		UnityEngine.Object.Destroy(gameObject);
		return null;
	}

	public ZedBar AddZedHealthBar(GameObject unit, Health healthComponent)
	{
		GameObject gameObject = NGUITools.AddChild(UIMovingWidgetPanel, ZedBarPrefab);
		ZedBar component = gameObject.GetComponent<ZedBar>();
		if (component != null)
		{
			component.InitializeCameras(uiCamera, mainCamera);
			component.SetComponentSettings(unit, healthComponent);
			return component;
		}
		UnityEngine.Object.Destroy(gameObject);
		return null;
	}

	public GameObject CreateVirtualSticks()
	{
		GameObject original = Resources.Load("GUI/DualJoysticks") as GameObject;
		return UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
	}

	public GameObject CreateUnitBar(int playerID)
	{
		GameObject gameObject = NGUITools.AddChild(UIMovingWidgetPanel, Resources.Load("GUI/UnitBar") as GameObject);
		UnitBar component = gameObject.GetComponent<UnitBar>();
		component.InitializeCameras(uiCamera, mainCamera);
		component.SetPlayerID(playerID);
		return gameObject;
	}

	public PlayerGUI CreatePlayerGui(Player player, PlayerShopWindow shopWindow = null)
	{
		if (!isCameraInitialized)
		{
			InitializeCamera();
		}
		string empty = string.Empty;
		InterfaceType interfaceType = DeviceSwitchController.InterfaceTypeFromDeviceMode(DeviceSwitchController.NewDeviceMode);
		empty = ((interfaceType != InterfaceType.Standard) ? "GUI/Player Touch Hud" : "GUI/Player Hud");
		currentInterfaceType = interfaceType;
		PlayerGUI playerGUI = new PlayerGUI(player, this);
		GameObject gameObject = NGUITools.AddChild(UIHud, Resources.Load(empty) as GameObject);
		if (shopWindow != null)
		{
			playerGUI.Shop = shopWindow;
		}
		else
		{
			PlayerShopWindow shop = new PlayerShopWindow(ShopController, playerGUI, ShopSkin, player.ID, player.InputID, player.SelectedPerk);
			playerGUI.Shop = shop;
		}
		playerGUI.HudObject = gameObject;
		playerGUI.Hud = gameObject.GetComponent<PlayerHudBase>();
		int num = ((PlayerManager.Instance.MaxPlayers != 2) ? player.ID : (player.ID * 3));
		playerGUI.Hud.Initialize(this, playerGUI, player.ID, playerHudPixelOffsets[num], uiCamera);
		playerGUI.UnitBarObject = CreateUnitBar(player.ID);
		playerGUI.UnitBar = playerGUI.UnitBarObject.GetComponent<UnitBar>();
		PlayerGuis[player.ID] = playerGUI;
		if (shopWindow == null)
		{
			ShopController.AddShopWindow(playerGUI.Shop);
		}
		if (currentInterfaceType == InterfaceType.Touch)
		{
			GameStatistics.gameObject.SetActive(false);
			GameStatistics = playerGUI.Hud.GetComponentInChildren<GameInfo>();
			showGameInfo = true;
			weaponTray = playerGUI.Hud.GetWeaponTray();
		}
		else
		{
			GameStatistics = oldGameStatistics.GetComponent<GameInfo>();
			GameStatistics.gameObject.SetActive(true);
		}
		playerGUI.Hud.GameStatistics = GameStatistics;
		return playerGUI;
	}

	public void DestroyPlayerGui(int playerID)
	{
		NGUITools.Destroy(PlayerGuis[playerID].HudObject);
		NGUITools.Destroy(PlayerGuis[playerID].UnitBarObject);
		PlayerGuis[playerID] = null;
	}

	public void SwitchPlayerHud(InterfaceType interfaceType)
	{
		if (interfaceType == currentInterfaceType || IsEndScreenVisible || PlayerManager.Instance.GetAllSoldiers().Count == 0)
		{
			return;
		}
		if (weaponTray != null && weaponTray.IsOpen())
		{
			pauseMode = PauseModeEnum.None;
			UnPauseGame();
		}
		isSwitchingGUI = true;
		PlayerManager playerManager = PlayerManager.Instance;
		List<Player> allPlayers = playerManager.GetAllPlayers();
		foreach (Player item in allPlayers)
		{
			PlayerHudBase hud = item.PlayerGUI.Hud;
			PlayerGUI playerGUI = item.PlayerGUI;
			DestroyPlayerGui(item.ID);
			item.CreateIngameGUI(this, playerGUI.Shop);
			item.PlayerGUI.Hud.SyncGui(hud);
			item.PlayerGUI.SwitchGui(playerGUI);
			PlayerGUILink component = item.GetSoldier().GetComponent<PlayerGUILink>();
			component.SwitchGui(item.PlayerGUI);
			if (IsShopOpen() || IsEndScreenVisible || IsGamePaused)
			{
				item.PlayerGUI.Hud.ToggleAllTweeens(false);
			}
		}
		SetWave(currentWave, maxWaves);
		SetEnemiesLeft(enemiesLeft);
		isSwitchingGUI = false;
	}

	public void SetWave(int wave, int maxWaves)
	{
		currentWave = wave;
		this.maxWaves = maxWaves;
		if (GameStatistics != null)
		{
			GameStatistics.SetWave(wave, maxWaves);
		}
	}

	public void SetEnemiesLeft(int enemies)
	{
		enemiesLeft = enemies;
		if (GameStatistics != null)
		{
			GameStatistics.SetEnemiesLeft(enemies);
		}
	}

	public void SetBossWave()
	{
		if (GameStatistics != null)
		{
			GameStatistics.SetBossWave();
		}
	}

	public void OnWaveSpawn()
	{
		showGameInfo = true;
		SetGameInfoVisibility(showGameInfo);
	}

	public void OnWaveComplete()
	{
		showGameInfo = false;
		SetGameInfoVisibility(showGameInfo);
	}

	public void DisplayEndScreen(EndScreenType endType)
	{
		UnlockController.gameObject.SetActive(false);
		bool flag = false;
		switch (endType)
		{
		case EndScreenType.SurvivalWin:
		case EndScreenType.SurvivalLose:
		{
			if (endType == EndScreenType.SurvivalWin)
			{
				flag = true;
			}
			endScreen = NGUITools.AddChild(UIOtherPanel, EndScreenPrefab);
			EndScreen component2 = endScreen.GetComponent<EndScreen>();
			if ((bool)component2)
			{
				component2.SetPlayerGuiReferences(PlayerGuis);
				component2.SetManagerReference(this);
				component2.SetEndResult(flag, currentWave, maxWaves);
				ShowCameraEffects();
				endScreenVisible = true;
			}
			else
			{
				UnityEngine.Object.Destroy(endScreen);
			}
			break;
		}
		case EndScreenType.TutorialWin:
		case EndScreenType.TutorialLose:
		{
			if (endType == EndScreenType.TutorialWin)
			{
				flag = true;
			}
			endScreen = NGUITools.AddChild(UIOtherPanel, EndScreenTutorialPrefab);
			EndScreenTutorial component = endScreen.GetComponent<EndScreenTutorial>();
			if ((bool)component)
			{
				component.SetManagerReference(this);
				component.SetEndResult(flag);
				ShowCameraEffects();
				endScreenVisible = true;
			}
			else
			{
				UnityEngine.Object.Destroy(endScreen);
			}
			break;
		}
		}
	}

	public void DestroyEndScreen()
	{
		endScreenVisible = false;
		HideCameraEffects();
	}

	public void DisplayMessage(string text, float time)
	{
		if (MessageDisplay != null)
		{
			MessageDisplay.StartDisplayText(text, time);
		}
	}

	public void DisplayMessage(string text, float time, MessageBoard.FontSize size)
	{
		if (MessageDisplay != null)
		{
			MessageDisplay.StartDisplayText(text, time, size);
		}
	}

	public void DisplayTimerMessage(string text, float time)
	{
		if (MessageDisplay != null)
		{
			MessageDisplay.StartDisplayTextWithTime(text, time);
		}
	}

	public void DisplayTimerMessage(string text, float time, MessageBoard.FontSize size)
	{
		if (MessageDisplay != null)
		{
			MessageDisplay.StartDisplayTextWithTime(text, time, size);
		}
	}

	public void ShowBossBar()
	{
		bossBarVisible = true;
		if (bossBar != null)
		{
			bossBar.Show();
		}
	}

	public void HideBossBar()
	{
		bossBarVisible = false;
		if (bossBar != null)
		{
			bossBar.Hide();
		}
	}

	public void SetBossBarTarget(GameObject unit)
	{
		if (bossBar != null)
		{
			bossBar.SetTarget(unit);
		}
	}

	public void SetGameInfoVisibility(bool visible)
	{
		GameStatistics.SetGameInfoVisibility(visible);
	}

	public void SetPlayerHudVisibility(bool visible)
	{
		for (int i = 0; i < PlayerGuis.Length; i++)
		{
			if (PlayerGuis[i] != null)
			{
				if (visible)
				{
					PlayerGuis[i].Hud.TweenIn();
				}
				else
				{
					PlayerGuis[i].Hud.TweenOut();
				}
			}
		}
	}

	public void SetUnitBarVisibility(bool isVisible)
	{
		if (!(UIMovingWidgetPanel != null))
		{
			return;
		}
		UIPanel component = UIMovingWidgetPanel.GetComponent<UIPanel>();
		if (!(component != null))
		{
			return;
		}
		if (isVisible)
		{
			component.enabled = true;
			PlayerGUI[] playerGuis = PlayerGuis;
			foreach (PlayerGUI playerGUI in playerGuis)
			{
				if (playerGUI != null)
				{
					playerGUI.ShowUnitBar();
				}
			}
			return;
		}
		component.enabled = false;
		PlayerGUI[] playerGuis2 = PlayerGuis;
		foreach (PlayerGUI playerGUI2 in playerGuis2)
		{
			if (playerGUI2 != null)
			{
				playerGUI2.HideUnitBar();
			}
		}
	}

	public void ShowCameraEffects()
	{
		if (questLog != null)
		{
			if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch)
			{
				questLog.gameObject.SetActive(false);
			}
			else if (DeviceSwitchController.CurrentDeviceMode != DeviceMode.Touch)
			{
				questLog.gameObject.SetActive(false);
			}
		}
		if (dialogMessage != null)
		{
			dialogMessage.gameObject.SetActive(false);
		}
		if (!isCameraInitialized)
		{
			InitializeCamera();
		}
		if (ZedTimeTexture != null)
		{
			ZedTimeTexture.enabled = false;
		}
		if (cameraController != null)
		{
			cameraController.AddEffect(new GreyscaleCameraEffect());
			cameraController.AddEffect(new BlurCameraEffect());
		}
		if (showGameInfo && currentWave != -1)
		{
			SetGameInfoVisibility(false);
		}
		SetPlayerHudVisibility(false);
		SetUnitBarVisibility(false);
		MessageDisplay.Hide();
		if (bossBarVisible)
		{
			bossBar.Hide();
		}
	}

	public void HideCameraEffects()
	{
		if (questLog != null)
		{
			questLog.gameObject.SetActive(true);
		}
		if (dialogMessage != null)
		{
			dialogMessage.gameObject.SetActive(true);
		}
		if (!ShopController.IsShopOpen() && !endScreenVisible)
		{
			if (ZedTimeTexture != null)
			{
				ZedTimeTexture.enabled = true;
			}
			if (cameraController != null)
			{
				cameraController.RemoveEffectByType(CameraEffectType.Blur);
				cameraController.RemoveEffectByType(CameraEffectType.Greyscale);
			}
			if (showGameInfo && currentWave != -1)
			{
				SetGameInfoVisibility(true);
			}
			SetPlayerHudVisibility(true);
			SetUnitBarVisibility(true);
			MessageDisplay.Show();
			if (bossBarVisible)
			{
				bossBar.Show();
			}
		}
	}

	public void OnZedTimeBlendComplete()
	{
		if (isZedTimeBlendingOut)
		{
			zedTimeDuration = 0f;
			isZedTimeActive = false;
			isZedTimeBlendingOut = false;
			ZedTimeAlphaTween.gameObject.SetActive(false);
		}
		else
		{
			StartZedTimer();
		}
	}

	public void OnZedTimeScaleComplete()
	{
		ZedTimeTexture.transform.localScale = ZedTimeScaleTween.from;
	}

	private void StartZedTimer()
	{
		isZedTimeTimerActive = true;
	}

	private void OnZedTimeTick()
	{
		if (isZedTimeTimerActive)
		{
			zedTimeDuration -= Time.deltaTime;
			if (zedTimeDuration <= 0f)
			{
				ZedTimeAlphaTween.duration = zedTimeFadeOutDuration;
				ZedTimeAlphaTween.Play(false);
				ZedTimeScaleTween.enabled = true;
				ZedTimeScaleTween.Reset();
				ZedTimeScaleTween.duration = zedTimeFadeOutDuration;
				ZedTimeScaleTween.Play(true);
				isZedTimeBlendingOut = true;
				isZedTimeTimerActive = false;
			}
		}
	}

	public void TriggerZedTimeEffect(float fadeInDuration, float durationTillFadeOut, float fadeOutDuration)
	{
		if (ZedTimeAlphaTween != null)
		{
			ZedTimeAlphaTween.gameObject.SetActive(true);
			zedTimeDuration = durationTillFadeOut;
			zedTimeFadeInDuration = fadeInDuration;
			zedTimeFadeOutDuration = fadeOutDuration;
			if (!isZedTimeActive)
			{
				ZedTimeAlphaTween.gameObject.SetActive(true);
				isZedTimeActive = true;
				ZedTimeAlphaTween.duration = fadeInDuration;
				ZedTimeAlphaTween.Play(true);
				StartZedTimer();
			}
			else if (isZedTimeBlendingOut)
			{
				ZedTimeAlphaTween.duration = ZedTimeAlphaTween.TimePast / zedTimeFadeInDuration * fadeInDuration;
				ZedTimeAlphaTween.Play(true);
				ZedTimeScaleTween.duration = ZedTimeAlphaTween.TimePast / zedTimeFadeInDuration * fadeInDuration;
				ZedTimeScaleTween.Play(false);
				isZedTimeBlendingOut = false;
				StartZedTimer();
			}
		}
	}

	public void OpenShop()
	{
		pauseMode = PauseModeEnum.Shop;
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch)
		{
			StickTouchRecognizer.Instance.ToggleSticksEnabled(false);
		}
		CursorManager.SetCursorByType(CursorType.Menu);
		ShowCameraEffects();
		Invoke("InvokeOpenShop", 0.3f);
		UnlockController.gameObject.SetActive(false);
	}

	public void InvokeOpenShop()
	{
		ShopController.OpenShop();
	}

	public void InvokeActivateShop()
	{
		ShopController.gameObject.SetActive(true);
	}

	public void InvokeDeActivateSHop()
	{
		ShopController.gameObject.SetActive(false);
	}

	public void CloseShop()
	{
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch)
		{
			StickTouchRecognizer.Instance.ToggleSticksEnabled(true);
		}
		CursorManager.SetCursorByType(CursorType.InGame);
		ShopController.CloseShop();
		UnlockController.gameObject.SetActive(true);
		HideCameraEffects();
	}

	public bool IsShopPhaseDone()
	{
		return ShopController.IsShopPhaseDone();
	}

	public bool IsShopOpen()
	{
		return ShopController.IsShopOpen();
	}

	private void CheckWeaponTrayInput()
	{
		if (InputManager.GetBool(InputAction.OpenWeaponTray) && !endScreenVisible)
		{
			if (weaponTray.IsOpen())
			{
				UICamera.fallThrough = oldFallThrough;
			}
			else
			{
				oldFallThrough = UICamera.fallThrough;
				UICamera.fallThrough = base.gameObject;
			}
			pauseMode = PauseModeEnum.WeaponTray;
			PlayerGuis[0].Hud.ToggleWeaponTray();
		}
	}

	private void OnPress(bool isDown)
	{
	}

	private void CheckPauseInput()
	{
		if (InputManager.GetPause() && !endScreenVisible && !gamePaused)
		{
			OpenPauseMenu();
		}
	}

	public void OpenPauseMenu()
	{
		pauseMode = PauseModeEnum.Normal;
		CursorManager.SetCursorByType(CursorType.Menu);
		pauseMenuVisable = true;
		gamePaused = true;
		PlayerGuis[0].Hud.OnPauseMenuChanged(true);
		EventSystem.Instance.Trigger(new PauseToggled(gamePaused));
		TimeScaleManager.PauseGame();
		ShowCameraEffects();
		PauseWindow.Open();
		ShopController.gameObject.SetActive(false);
		if (SoldierManager.Instance != null)
		{
			SoldierManager.Instance.SetLockedInput(true, this, true);
		}
		UnlockController.gameObject.SetActive(false);
	}

	public void ClosePauseMenu()
	{
		if (ShopController.IsShopOpen())
		{
			TimeScaleManager.UnpauseGame();
			PauseWindow.Close();
			Invoke("InvokePasueMenu", 0.3f);
			ShopController.gameObject.SetActive(true);
		}
		else
		{
			pauseMenuVisable = false;
			PauseWindow.Close();
			gamePaused = false;
			PlayerGuis[0].Hud.OnPauseMenuChanged(false);
			TimeScaleManager.UnpauseGame();
			EventSystem.Instance.Trigger(new PauseToggled(gamePaused));
			HideCameraEffects();
			if (SoldierManager.Instance != null)
			{
				SoldierManager.Instance.SetLockedInput(false, this);
			}
			UnlockController.gameObject.SetActive(true);
			pauseMode = PauseModeEnum.None;
		}
		CursorManager.SetCursorByType(CursorType.InGame);
	}

	public void PauseGame(bool cameraEffect = false, bool lockInput = false)
	{
		if (pauseMode == PauseModeEnum.None)
		{
			pauseMode = PauseModeEnum.Normal;
		}
		if (cameraEffect)
		{
			ShowCameraEffects();
		}
		if (lockInput)
		{
			if (SoldierManager.Instance != null)
			{
				SoldierManager.Instance.SetLockedInput(true, this, true);
			}
			UnlockController.gameObject.SetActive(false);
		}
		cameraEffectVisible = cameraEffect;
		inputLocked = lockInput;
		gamePaused = true;
		EventSystem.Instance.Trigger(new PauseToggled(gamePaused));
		TimeScaleManager.PauseGame();
	}

	public void UnPauseGame()
	{
		if (inputLocked)
		{
			if (SoldierManager.Instance != null)
			{
				SoldierManager.Instance.SetLockedInput(false, this);
			}
			UnlockController.gameObject.SetActive(true);
		}
		if (cameraEffectVisible)
		{
			HideCameraEffects();
		}
		gamePaused = false;
		TimeScaleManager.UnpauseGame();
		pauseMode = PauseModeEnum.None;
		EventSystem.Instance.Trigger(new PauseToggled(gamePaused));
	}

	private void InvokePasueMenu()
	{
		pauseMenuVisable = false;
		ShopController.gameObject.SetActive(true);
		gamePaused = false;
		PlayerGuis[0].Hud.OnPauseMenuChanged(false);
		HideCameraEffects();
		if (SoldierManager.Instance != null)
		{
			SoldierManager.Instance.SetLockedInput(false, this);
		}
		UnlockController.gameObject.SetActive(true);
		pauseMode = PauseModeEnum.None;
	}

	private void OnMetaEvent(EventBase e)
	{
		if (e.Id == EventId.PerkLevelUp)
		{
			PerkLevelUp perkLevelUp = e as PerkLevelUp;
			OnPerkLevelUp(perkLevelUp.Perk);
		}
		else if (e.Id == EventId.WeaponUnlocked)
		{
			WeaponUnlocked weaponUnlocked = e as WeaponUnlocked;
			OnWeaponUnlocked(weaponUnlocked.Weapon);
		}
		else if (e.Id == EventId.WeaponLevelUp)
		{
			WeaponLevelUp weaponLevelUp = e as WeaponLevelUp;
			OnWeaponLevelUp(weaponLevelUp.Weapon);
		}
	}

	private void OnWeaponUnlocked(Weapon weapon)
	{
		UnlockController.AddWeaponUnlock(weapon);
	}

	private void OnWeaponLevelUp(Weapon weapon)
	{
		UnlockController.AddWeaponLevelUp(weapon);
	}

	private void OnPerkLevelUp(Perk perk)
	{
		UnlockController.AddPerkLevelUp(perk);
	}

	public void SetTutorialPlayerInputID(int id)
	{
		tutorialPlayerInputID = id;
	}

	public QuestLog AddQuestLog()
	{
		if (questLog == null)
		{
			questLog = NGUITools.AddChild(UIOtherPanel, Resources.Load("GUI/QuestLog") as GameObject);
			QuestLog result = NGUITools.FindInParents<QuestLog>(questLog);
			UIAnchor uIAnchor = NGUITools.FindInParents<UIAnchor>(questLog);
			if (uIAnchor != null)
			{
				uIAnchor.uiCamera = uiCamera;
			}
			return result;
		}
		return null;
	}

	public void RemoveQuestLog()
	{
		if (this.questLog != null)
		{
			QuestLog questLog = NGUITools.FindInParents<QuestLog>(this.questLog);
			if (questLog != null)
			{
				questLog.ClearLog();
			}
			UnityEngine.Object.Destroy(this.questLog);
		}
	}

	public TutorialMessage DisplayTutorialMessage(Action callback, MessageSender sender, string message, float duration, bool fixedDuration)
	{
		if (dialogMessage != null)
		{
			UnityEngine.Object.Destroy(dialogMessage);
		}
		string empty = string.Empty;
		dialogMessage = NGUITools.AddChild(prefab: Resources.Load((DeviceSwitchController.CurrentInterfaceType != InterfaceType.Standard) ? "GUI/TutorialMessageTouch" : "GUI/TutorialMessage") as GameObject, parent: UIOtherPanel);
		TutorialMessage tutorialMessage = NGUITools.FindInParents<TutorialMessage>(dialogMessage);
		tutorialMessage.SetInputID(tutorialPlayerInputID);
		tutorialMessage.SetMessage(sender, message, duration, fixedDuration);
		if (callback != null)
		{
			tutorialMessage.AddAlphaCompleteListener(callback);
		}
		UIAnchor uIAnchor = NGUITools.FindInParents<UIAnchor>(dialogMessage);
		if (uIAnchor != null)
		{
			uIAnchor.uiCamera = uiCamera;
		}
		return tutorialMessage;
	}

	public void SpawnStickTypePopup()
	{
		if (StickTypePopup != null)
		{
			if (IsEndScreenVisible)
			{
				UnityEngine.Object.Destroy(endScreen);
			}
			GameObject gameObject = NGUITools.AddChild(UIOtherPanel, StickTypePopup);
			stickTypePopup = gameObject.GetComponent<PopupWindow>();
			if (stickTypePopup != null)
			{
				stickTypePopup.SetButton1(Localizer.GetString("gui_popup_select"), PopupWindow.PopupState.Option1);
				stickTypePopup.SetButton2(Localizer.GetString("gui_popup_select"), PopupWindow.PopupState.Option2);
				stickTypePopup.SetMultiOptionPopupWindowInfo(Localizer.GetString("gui_popup_stick_type_header"), Localizer.GetString("gui_popup_stick_type_locked_header"), Localizer.GetString("gui_popup_stick_type_dynamic_header"), Localizer.GetString("gui_popup_stick_type_locked_message"), Localizer.GetString("gui_popup_stick_type_dynamic_message"));
				stickTypePopup.SetInGameManagerReference(this);
			}
			else
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}
}
