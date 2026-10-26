using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public ControllerType DefaultController;

	private static InputManager instance;

	private Dictionary<int, PlayerInputController> playerControllers;

	private float joystickUpdateFrequency = 1f;

	private float controllerModeUpdateFrequency = 1f;

	private DateTime joystickUpdateTimer = DateTime.MinValue;

	private string[] connectedJoysticks;

	private InputLinkProcess activeLinkProcess;

	private float checkModeTime;

	private bool isInSlateMode;

	private bool touchSupported = true;

	private bool controllerSupported;

	private int numberOfConnectedJoysticks;

	private int joystickIndex = 1;

	private TouchStickType stickType;

	public Vector3 RealMousePos;

	private static bool mouseEnabled = true;

	private string[,] axisLookup = new string[2, 12];

	public static InputManager Instance
	{
		get
		{
			return instance;
		}
	}

	public TouchStickType StickType
	{
		get
		{
			return stickType;
		}
	}

	public int NumberOfConnectedJoysticks
	{
		get
		{
			return numberOfConnectedJoysticks;
		}
	}

	private bool LinkInProgress
	{
		get
		{
			return activeLinkProcess != null;
		}
	}

	public static bool MouseEnabled
	{
		get
		{
			return mouseEnabled;
		}
		set
		{
			mouseEnabled = value;
		}
	}

	private void Awake()
	{
		instance = this;
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 12; j++)
			{
				axisLookup[i, j] = "Joy" + (i + 1) + " Axis " + (j + 1);
			}
		}
	}

	private void Start()
	{
		playerControllers = new Dictionary<int, PlayerInputController>(4);
		BuildSetting current = BuildRuntimeSettings.Current;
		DefaultController = DeviceSwitchController.ControllerFromDeviceMode(current.DefaultDeviceMode);
		controllerSupported = current.SupportsContollers;
		checkModeTime = Time.unscaledTime;
		SetupDefaultControllers();
		SetupJoysticks();
	}

	public void ForceSetNewController(ControllerType controllerType)
	{
		if (DefaultController != controllerType)
		{
			SoldierInputController[] array = UnityEngine.Object.FindObjectsOfType<SoldierInputController>();
			SoldierInputController[] array2 = array;
			foreach (SoldierInputController soldierInputController in array2)
			{
				PlayerInputController playerController = GetPlayerController(soldierInputController.PlayerID);
				playerController.UnregisterSoldierController(soldierInputController);
			}
			playerControllers.Clear();
			DefaultController = controllerType;
			SetupDefaultControllers();
			SoldierInputController[] array3 = array;
			foreach (SoldierInputController soldierController in array3)
			{
				RegisterSoldierController(soldierController);
			}
		}
	}

	public void SwitchController(ControllerType controllerType)
	{
		ForceSetNewController(controllerType);
	}

	private void SwitchModeCheck()
	{
		DeviceMode currentDeviceMode = DeviceSwitchController.CurrentDeviceMode;
		if (currentDeviceMode != DeviceMode.Touch)
		{
			bool flag = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2);
			if (Input.touchCount > 0 && PlayerManager.Instance.NumberOfPlayers <= 1)
			{
				DeviceSwitchController.SwitchTo(DeviceMode.Touch);
			}
			if (PlayerManager.Instance.NumberOfPlayers > 1 && currentDeviceMode == DeviceMode.Touch)
			{
				DeviceSwitchController.SwitchTo(DeviceMode.Controller);
			}
		}
		else
		{
			if (currentDeviceMode != DeviceMode.Touch)
			{
				return;
			}
			if (Input.anyKeyDown && BuildRuntimeSettings.Current.SupportsMouseKeyboard)
			{
				bool flag2 = false;
				for (int i = 0; i < 3; i++)
				{
					if (Input.GetMouseButtonDown(i))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					DeviceSwitchController.SwitchTo(DeviceMode.MouseKeyboard);
				}
			}
			else
			{
				if (!controllerSupported)
				{
					return;
				}
				bool flag3 = false;
				for (int j = 330; j <= 349; j++)
				{
					if (Input.GetKeyDown((KeyCode)j))
					{
						flag3 = true;
						break;
					}
				}
				if (!flag3)
				{
					joystickIndex %= 2;
					for (int k = 0; k < 12; k++)
					{
						if (Input.GetAxis(axisLookup[joystickIndex, k]) > 0.3f)
						{
							flag3 = true;
							break;
						}
					}
				}
				if (flag3)
				{
					DeviceSwitchController.SwitchTo(DeviceMode.Controller);
				}
			}
		}
	}

	private void Update()
	{
		SwitchModeCheck();
		foreach (PlayerInputController value in playerControllers.Values)
		{
			value.Update();
		}
		if (controllerSupported)
		{
			UpdateJoysticks();
			UpdateLinkProcess();
		}
	}

	private void OnLevelWasLoaded(int levelID)
	{
		if (levelID <= 1)
		{
			return;
		}
		foreach (PlayerInputController value in playerControllers.Values)
		{
			value.OnLevelLoaded();
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public static bool GetBool(InputAction action)
	{
		if (instance != null)
		{
			foreach (PlayerInputController value in instance.playerControllers.Values)
			{
				if (value.GetBool(action))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool GetBool(InputAction action, int playerID)
	{
		PlayerInputController value;
		if (instance != null && instance.playerControllers.TryGetValue(playerID, out value))
		{
			return value.GetBool(action);
		}
		return false;
	}

	public static bool GetBool(InputAction primaryAction, InputAction secondaryAction)
	{
		if (instance != null)
		{
			foreach (PlayerInputController value in instance.playerControllers.Values)
			{
				if (value.GetBool(primaryAction) || value.GetBool(secondaryAction))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool GetBool(InputAction primaryAction, InputAction secondaryAction, int playerID)
	{
		PlayerInputController value;
		if (instance != null && instance.playerControllers.TryGetValue(playerID, out value))
		{
			return value.GetBool(primaryAction) || value.GetBool(secondaryAction);
		}
		return false;
	}

	public static Vector3 GetVector(InputAction action)
	{
		Vector3 vector = Vector3.zero;
		if (instance != null)
		{
			foreach (PlayerInputController value in instance.playerControllers.Values)
			{
				vector = value.GetVector(action);
				if (vector != Vector3.zero)
				{
					break;
				}
			}
		}
		return vector;
	}

	public static Vector3 GetVector(InputAction action, int playerID)
	{
		PlayerInputController value;
		if (instance != null && instance.playerControllers.TryGetValue(playerID, out value))
		{
			return value.GetVector(action);
		}
		return Vector3.zero;
	}

	public static Vector3 GetVector(InputAction primaryAction, InputAction secondaryAction)
	{
		Vector3 vector = Vector3.zero;
		if (instance != null)
		{
			foreach (PlayerInputController value in instance.playerControllers.Values)
			{
				vector = value.GetVector(primaryAction);
				if (vector == Vector3.zero)
				{
					vector = value.GetVector(secondaryAction);
				}
				if (vector != Vector3.zero)
				{
					break;
				}
			}
		}
		return vector;
	}

	public static Vector3 GetVector(InputAction primaryAction, InputAction secondaryAction, int playerID)
	{
		PlayerInputController value;
		if (instance != null && instance.playerControllers.TryGetValue(playerID, out value))
		{
			Vector3 vector = value.GetVector(primaryAction);
			if (vector == Vector3.zero)
			{
				vector = value.GetVector(secondaryAction);
			}
			return vector;
		}
		return Vector3.zero;
	}

	public static int GetInt(InputAction action)
	{
		int num = 0;
		if (instance != null)
		{
			foreach (PlayerInputController value in instance.playerControllers.Values)
			{
				num = value.GetInt(action);
				if (num != 0)
				{
					break;
				}
			}
		}
		return num;
	}

	public static int GetInt(InputAction action, int playerID)
	{
		PlayerInputController value;
		if (instance != null && instance.playerControllers.TryGetValue(playerID, out value))
		{
			return value.GetInt(action);
		}
		return 0;
	}

	public static int GetInt(InputAction primaryAction, InputAction secondaryAction)
	{
		int num = 0;
		if (instance != null)
		{
			foreach (PlayerInputController value in instance.playerControllers.Values)
			{
				num = value.GetInt(primaryAction);
				if (num == 0)
				{
					num = value.GetInt(secondaryAction);
				}
				if (num != 0)
				{
					break;
				}
			}
		}
		return num;
	}

	public static int GetInt(InputAction primaryAction, InputAction secondaryAction, int playerID)
	{
		PlayerInputController value;
		if (instance != null && instance.playerControllers.TryGetValue(playerID, out value))
		{
			int @int = value.GetInt(primaryAction);
			if (@int == 0)
			{
				@int = value.GetInt(secondaryAction);
			}
			return @int;
		}
		return 0;
	}

	public static float GetFloat(InputAction action)
	{
		float num = 0f;
		if (instance != null)
		{
			foreach (PlayerInputController value in instance.playerControllers.Values)
			{
				num = value.GetFloat(action);
				if (num != 0f)
				{
					break;
				}
			}
		}
		return num;
	}

	public static float GetFloat(InputAction action, int playerID)
	{
		PlayerInputController value;
		if (instance != null && instance.playerControllers.TryGetValue(playerID, out value))
		{
			return value.GetFloat(action);
		}
		return 0f;
	}

	public static float GetFloat(InputAction primaryAction, InputAction secondaryAction)
	{
		float num = 0f;
		if (instance != null)
		{
			foreach (PlayerInputController value in instance.playerControllers.Values)
			{
				num = value.GetFloat(primaryAction);
				if (num == 0f)
				{
					num = value.GetFloat(secondaryAction);
				}
				if (num != 0f)
				{
					break;
				}
			}
		}
		return num;
	}

	public static float GetFloat(InputAction primaryAction, InputAction secondaryAction, int playerID)
	{
		PlayerInputController value;
		if (instance != null && instance.playerControllers.TryGetValue(playerID, out value))
		{
			float @float = value.GetFloat(primaryAction);
			if (@float == 0f)
			{
				@float = value.GetFloat(secondaryAction);
			}
			return @float;
		}
		return 0f;
	}

	public static bool GetPause()
	{
		return Input.GetKeyDown(KeyCode.Menu) || (instance != null && GetBool(InputAction.Pause));
	}

	private void SetupDefaultControllers()
	{
		if (BuildRuntimeSettings.Current.SupportsContollers && DefaultController != ControllerType.Keyboard && DefaultController != ControllerType.TouchControls && DefaultController != 0)
		{
			string[] joystickNames = Input.GetJoystickNames();
			int num = Mathf.Min(joystickNames.Length, 4);
			for (int i = 0; i < num; i++)
			{
				ControllerType controllerType = InputHelper.ControllerNameToType(joystickNames[i]);
				PlayerInputController value = new PlayerInputController((InputPlayer)i, controllerType, i);
				playerControllers.Add(i, value);
			}
		}
		if (InputHelper.Platform == Platform.Windows && playerControllers.Count == 0)
		{
			PlayerInputController value2 = new PlayerInputController(InputPlayer.Player1, DefaultController, 0);
			playerControllers.Add(0, value2);
		}
		if (InputHelper.Platform == Platform.Android && playerControllers.Count == 0)
		{
			PlayerInputController value3 = new PlayerInputController(InputPlayer.Player1, DefaultController, 0);
			playerControllers.Add(0, value3);
		}
		if ((InputHelper.Platform == Platform.MacOS || InputHelper.Platform == Platform.iOS) && playerControllers.Count == 0)
		{
			PlayerInputController value4 = new PlayerInputController(InputPlayer.Player1, DefaultController, 0);
			playerControllers.Add(0, value4);
		}
		if (playerControllers.Count >= 4)
		{
			return;
		}
		ControllerType controllerType2 = ControllerType.None;
		if (playerControllers.Count > 0)
		{
			for (int j = 0; j < playerControllers.Count; j++)
			{
				ControllerType controllerType3 = playerControllers[j].ControllerType;
				if (controllerType3 != 0 && controllerType3 != ControllerType.Keyboard && controllerType3 != ControllerType.TouchControls)
				{
					controllerType2 = controllerType3;
				}
			}
		}
		if (controllerType2 == ControllerType.None)
		{
			controllerType2 = ((InputHelper.Platform != 0) ? (BuildRuntimeSettings.Current.SupportsContollers ? ControllerType.Ouya_Android : ControllerType.None) : (BuildRuntimeSettings.Current.SupportsContollers ? ControllerType.XBOX360_Windows : ControllerType.None));
		}
		for (int k = playerControllers.Count; k < 4; k++)
		{
			PlayerInputController value5 = new PlayerInputController((InputPlayer)k, controllerType2, k);
			playerControllers.Add(k, value5);
		}
	}

	private PlayerInputController GetPlayerController(int playerID)
	{
		if (playerControllers.ContainsKey(playerID))
		{
			return playerControllers[playerID];
		}
		return null;
	}

	public Dictionary<int, PlayerInputController> GetPlayerControllers()
	{
		return playerControllers;
	}

	public void RegisterSoldierController(SoldierInputController soldierController)
	{
		PlayerInputController playerController = GetPlayerController(soldierController.PlayerID);
		if (playerController != null)
		{
			playerController.RegisterSoldierController(soldierController);
		}
	}

	public void UnregisterSoldierController(SoldierInputController soldierController)
	{
		PlayerInputController playerController = GetPlayerController(soldierController.PlayerID);
		if (playerController != null)
		{
			playerController.UnregisterSoldierController(soldierController);
		}
	}

	public static void SetInputMode(InputMode inputMode)
	{
		if (!(instance != null))
		{
			return;
		}
		foreach (PlayerInputController value in instance.playerControllers.Values)
		{
			value.SetInputMode(inputMode);
		}
	}

	public static void SetInputMode(InputMode inputMode, int playerID)
	{
		PlayerInputController value;
		if (instance != null && instance.playerControllers.TryGetValue(playerID, out value))
		{
			value.SetInputMode(inputMode);
		}
	}

	private void UpdateLinkProcess()
	{
		if (LinkInProgress)
		{
			activeLinkProcess.Update();
			if (activeLinkProcess.IsDone)
			{
				OnLinkProcessDone();
			}
		}
	}

	private void OnLinkProcessStart()
	{
		foreach (PlayerInputController value in playerControllers.Values)
		{
			value.Enabled = false;
		}
	}

	private void OnLinkProcessDone()
	{
		activeLinkProcess = null;
		foreach (PlayerInputController value in playerControllers.Values)
		{
			value.Enabled = true;
		}
	}

	public void StartLinkProcess(bool manually = false)
	{
		if (CanStartLinkProcess())
		{
			StartLinkProcess(new bool[4] { true, true, true, true }, manually);
		}
	}

	public void StartLinkProcess(bool[] controllers, bool manually = false)
	{
		if (CanStartLinkProcess())
		{
			activeLinkProcess = new InputLinkProcess(this, controllers, manually);
			OnLinkProcessStart();
		}
	}

	public void CancelLinkProcess()
	{
		if (activeLinkProcess != null)
		{
			activeLinkProcess.Cancel();
			OnLinkProcessDone();
		}
	}

	private void StartLinkProcessForPlayers()
	{
		if (CanStartLinkProcess())
		{
			PlayerManager playerManager = PlayerManager.Instance;
			if (playerManager != null)
			{
				bool[] array = new bool[4];
				bool flag = false;
				for (int i = 0; i < 4; i++)
				{
					if (playerManager.DoesPlayerWithIDExist(i))
					{
						array[i] = true;
						flag = true;
					}
				}
				if (flag)
				{
					StartLinkProcess(array);
				}
			}
			else
			{
				StartLinkProcess();
			}
		}
		else if (activeLinkProcess != null)
		{
			activeLinkProcess.Restart();
		}
	}

	public bool CanStartLinkProcess()
	{
		return activeLinkProcess == null || (activeLinkProcess.IsDone && (IsInMenu() || IsIngame()));
	}

	private void SetupJoysticks()
	{
		connectedJoysticks = Input.GetJoystickNames();
	}

	private void UpdateJoysticks()
	{
		if (!(joystickUpdateTimer < DateTime.Now))
		{
			return;
		}
		string[] joystickNames = Input.GetJoystickNames();
		int num = connectedJoysticks.Length;
		int num2 = joystickNames.Length;
		connectedJoysticks = joystickNames;
		if (num != num2)
		{
			if (num > 0 && num2 == 0)
			{
				DeviceSwitchController.SwitchTo(DeviceMode.Touch);
			}
			else if (num == 0 && num2 > 0)
			{
				DeviceSwitchController.SwitchTo(DeviceMode.Controller);
			}
			if (num < num2)
			{
				OnJoystickConnected();
			}
			else if (num2 > 0)
			{
				OnJoystickDisconnected();
			}
		}
		numberOfConnectedJoysticks = num2;
		joystickUpdateTimer = DateTime.Now + TimeSpan.FromSeconds(1f / joystickUpdateFrequency);
	}

	private void OnJoystickConnected()
	{
		if (IsInMenu())
		{
			if (connectedJoysticks.Length != 1)
			{
				return;
			}
			string controllerName = connectedJoysticks[0];
			ControllerType controllerType = InputHelper.ControllerNameToType(controllerName);
			int num = 0;
			for (int i = 0; i < playerControllers.Count; i++)
			{
				PlayerInputController playerInputController = playerControllers[i];
				if (playerInputController.ControllerType != ControllerType.Keyboard)
				{
					playerInputController.SetController(controllerType, num);
					num++;
				}
			}
		}
		else if (IsIngame())
		{
			StartLinkProcessForPlayers();
		}
	}

	private void OnJoystickDisconnected()
	{
		if (activeLinkProcess != null && !activeLinkProcess.IsDone)
		{
			activeLinkProcess.Restart();
		}
		else if (IsIngame())
		{
			StartLinkProcessForPlayers();
		}
	}

	private bool IsInMenu()
	{
		return GuiMenuManager.Instance != null;
	}

	private bool IsIngame()
	{
		return GuiInGameManager.Instance != null;
	}

	public void SetTouchStickType(TouchStickType stickType)
	{
		this.stickType = stickType;
	}
}
