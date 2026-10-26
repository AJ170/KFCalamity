using System.Collections.Generic;
using UnityEngine;

namespace Calamity.Input
{
	internal class InputLinkProcess
	{
		private enum InternalState
		{
			Setup = 0,
			Linking = 1,
			WaitingForPopupCallback = 2,
			Completed = 3
		}

		private class LinkedController
		{
			public int PlayerID;

			public int ControllerID;

			public ControllerType Type;

			public InputWrapper DoneButtonWrapper;

			public InputWrapper RestartButtonWrapper;

			public InputWrapper UndoButtonWrapper;

			public LinkedController(int playerID, int controllerId, ControllerType type)
			{
				PlayerID = playerID;
				ControllerID = controllerId;
				Type = type;
				SetupInput();
			}

			public void SetupInput()
			{
				if (Type != 0)
				{
					if (Type == ControllerType.Keyboard)
					{
						DoneButtonWrapper = InputHelper.GetWrappedInput(InputKey.RT, Type, InputActionType.Down, InputPlayer.Player1);
						RestartButtonWrapper = InputHelper.GetWrappedKey(KeyCode.Y, InputActionType.Down);
						UndoButtonWrapper = InputHelper.GetWrappedKey(KeyCode.U, InputActionType.Down);
					}
					else
					{
						DoneButtonWrapper = InputHelper.GetWrappedInput(InputKey.U, Type, InputActionType.Down, (InputPlayer)(ControllerID - 1));
						RestartButtonWrapper = InputHelper.GetWrappedInput(InputKey.Y, Type, InputActionType.Down, (InputPlayer)(ControllerID - 1));
						UndoButtonWrapper = InputHelper.GetWrappedInput(InputKey.A, Type, InputActionType.Down, (InputPlayer)(ControllerID - 1));
					}
				}
			}
		}

		private int keyboardID = 1000;

		private bool[] controllersToLink;

		private List<int> availableControllers;

		private List<LinkedController> linkedControllers;

		private ControllerType dominantControllerType;

		private int activePlayer = -1;

		private InternalState state;

		private InputManager inputManager;

		private bool triggeredManually;

		private int activePlayerControllerID = -1;

		private InputWrapper activePlayerAxisInputWrapper;

		public bool IsDone
		{
			get
			{
				return state == InternalState.Completed;
			}
		}

		public InputLinkProcess(InputManager inputManager, bool triggeredManually = false)
			: this(inputManager, new bool[4] { true, true, true, true }, triggeredManually)
		{
		}

		public InputLinkProcess(InputManager inputManager, bool[] controllersToLink, bool triggeredManually = false)
		{
			this.inputManager = inputManager;
			this.controllersToLink = controllersToLink;
			if (ValidControllersToLink())
			{
				Setup();
				if (SetNextControllerActive())
				{
					state = InternalState.Linking;
				}
				else
				{
					state = InternalState.Completed;
				}
			}
			else
			{
				state = InternalState.Completed;
			}
		}

		public void Update()
		{
			if (state == InternalState.Linking)
			{
				UpdateLinking();
			}
			else if (state == InternalState.WaitingForPopupCallback && activePlayerAxisInputWrapper != null)
			{
				activePlayerAxisInputWrapper.Update();
			}
		}

		public void Cancel()
		{
			OnCancel();
		}

		private void Setup()
		{
			availableControllers = new List<int>(10) { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
			if (InputHelper.Platform == Platform.Windows)
			{
				availableControllers.Add(keyboardID);
			}
			linkedControllers = new List<LinkedController>();
			CheckConnectedControllers();
			CallGUI(ControllerLinkGUIAction.Open);
			for (int i = 0; i < controllersToLink.Length; i++)
			{
				if (controllersToLink[i])
				{
					CallGUI(ControllerLinkGUIAction.PlayerEnabled, i);
				}
			}
		}

		private bool ValidControllersToLink()
		{
			if (controllersToLink != null)
			{
				for (int i = 0; i < controllersToLink.Length; i++)
				{
					if (controllersToLink[i])
					{
						return true;
					}
				}
			}
			return false;
		}

		private void CheckConnectedControllers()
		{
			string[] joystickNames = UnityEngine.Input.GetJoystickNames();
			if (joystickNames.Length == 1)
			{
				dominantControllerType = InputHelper.ControllerNameToType(joystickNames[0]);
			}
			else if (joystickNames.Length > 1)
			{
				Dictionary<ControllerType, int> dictionary = ControllerTypesByCount();
				if (dictionary.Count == 1)
				{
					dominantControllerType = InputHelper.ControllerNameToType(joystickNames[0]);
					return;
				}
				int num = 0;
				foreach (KeyValuePair<ControllerType, int> item in dictionary)
				{
					if (item.Value > num)
					{
						ControllerType key = item.Key;
						if (key != ControllerType.Unknown)
						{
							num = item.Value;
							dominantControllerType = key;
						}
					}
				}
				if (num == 0)
				{
					dominantControllerType = ControllerType.Unknown;
				}
			}
			else
			{
				dominantControllerType = ControllerType.None;
			}
		}

		private bool SetNextControllerActive()
		{
			for (int i = 0; i < controllersToLink.Length; i++)
			{
				if (controllersToLink[i] && !IsPlayerLinked(i))
				{
					activePlayer = i;
					CallGUI(ControllerLinkGUIAction.PlayerActive, activePlayer);
					return true;
				}
			}
			activePlayer = -1;
			return false;
		}

		private bool IsPlayerLinked(int id)
		{
			for (int i = 0; i < linkedControllers.Count; i++)
			{
				if (linkedControllers[i].PlayerID == id)
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateLinking()
		{
			CheckInputForActivePlayer();
			if (triggeredManually && linkedControllers.Count == 0)
			{
				CheckCancelInput();
			}
			CheckInputForLinkedPlayers();
		}

		private void LinkController(int playerID, int controllerID)
		{
			ControllerType controllerType = ((controllerID != keyboardID) ? dominantControllerType : ControllerType.Keyboard);
			LinkController(playerID, controllerID, controllerType);
		}

		private void LinkController(int playerID, int controllerID, ControllerType controllerType)
		{
			linkedControllers.Add(new LinkedController(playerID, controllerID, controllerType));
			availableControllers.Remove(controllerID);
			CallGUI(ControllerLinkGUIAction.PlayerLinked, playerID);
		}

		private void OnLinked(int controllerID)
		{
			if (controllerID != keyboardID && MultipleControllerTypesAvailable())
			{
				OpenControllerTypeSelection(controllerID);
				return;
			}
			LinkController(activePlayer, controllerID);
			if (!SetNextControllerActive())
			{
				CallGUI(ControllerLinkGUIAction.Done);
			}
		}

		private void OnDone()
		{
			Dictionary<int, PlayerInputController> playerControllers = inputManager.GetPlayerControllers();
			foreach (KeyValuePair<int, PlayerInputController> item in playerControllers)
			{
				LinkedController linkedController = LinkedControllerByPlayerID(item.Key);
				if (linkedController != null)
				{
					item.Value.SetController(linkedController.Type, linkedController.ControllerID - 1);
				}
				else
				{
					item.Value.SetController(ControllerType.None, 0);
				}
			}
			CallGUI(ControllerLinkGUIAction.Close);
			state = InternalState.Completed;
		}

		public void Restart()
		{
			for (int i = 0; i < linkedControllers.Count; i++)
			{
				LinkedController linkedController = linkedControllers[i];
				availableControllers.Add(linkedController.ControllerID);
				CallGUI(ControllerLinkGUIAction.PlayerUnlinked, linkedController.PlayerID);
			}
			linkedControllers.Clear();
			CheckConnectedControllers();
			CallGUI(ControllerLinkGUIAction.Reset);
			activePlayerControllerID = -1;
			SetNextControllerActive();
		}

		private void OnUndo()
		{
			if (linkedControllers.Count > 0)
			{
				LinkedController linkedController = linkedControllers[linkedControllers.Count - 1];
				availableControllers.Add(linkedController.ControllerID);
				CallGUI(ControllerLinkGUIAction.PlayerUnlinked, linkedController.PlayerID);
				linkedControllers.Remove(linkedController);
				SetNextControllerActive();
			}
		}

		private void OnCancel()
		{
			CallGUI(ControllerLinkGUIAction.Close);
			state = InternalState.Completed;
		}

		private LinkedController LinkedControllerByPlayerID(int id)
		{
			for (int i = 0; i < linkedControllers.Count; i++)
			{
				LinkedController linkedController = linkedControllers[i];
				if (linkedController.PlayerID == id)
				{
					return linkedController;
				}
			}
			return null;
		}

		private void ControllerSelectionCallback(ControllerType controllerType)
		{
			if (state != InternalState.WaitingForPopupCallback)
			{
				return;
			}
			if (controllerType != 0)
			{
				LinkController(activePlayer, activePlayerControllerID, controllerType);
				activePlayerAxisInputWrapper = null;
				if (!SetNextControllerActive())
				{
					CallGUI(ControllerLinkGUIAction.Done);
				}
			}
			state = InternalState.Linking;
		}

		private void CheckInputForActivePlayer()
		{
			if (activePlayer == -1)
			{
				return;
			}
			for (int i = 0; i < availableControllers.Count; i++)
			{
				int controllerID = availableControllers[i];
				if (ControllerConfirmInput(controllerID))
				{
					OnLinked(controllerID);
				}
			}
		}

		private void CheckInputForLinkedPlayers()
		{
			if (CheckDoneInputForLinkedPlayers())
			{
				OnDone();
			}
			else if (CheckRestartInputForLinkedPlayers())
			{
				Restart();
			}
			else if (CheckUndoInputForLinkedPlayers())
			{
				OnUndo();
			}
		}

		private bool CheckDoneInputForLinkedPlayers()
		{
			for (int i = 0; i < linkedControllers.Count; i++)
			{
				LinkedController linkedController = linkedControllers[i];
				if (linkedController.DoneButtonWrapper != null && linkedController.DoneButtonWrapper.GetBool())
				{
					return true;
				}
			}
			return false;
		}

		private bool CheckRestartInputForLinkedPlayers()
		{
			for (int i = 0; i < linkedControllers.Count; i++)
			{
				LinkedController linkedController = linkedControllers[i];
				if (linkedController.RestartButtonWrapper != null && linkedController.RestartButtonWrapper.GetBool())
				{
					return true;
				}
			}
			return false;
		}

		private bool CheckUndoInputForLinkedPlayers()
		{
			for (int i = 0; i < linkedControllers.Count; i++)
			{
				LinkedController linkedController = linkedControllers[i];
				if (linkedController.UndoButtonWrapper != null && linkedController.UndoButtonWrapper.GetBool())
				{
					return true;
				}
			}
			return false;
		}

		private void CheckCancelInput()
		{
			Dictionary<int, PlayerInputController> playerControllers = inputManager.GetPlayerControllers();
			for (int i = 0; i < playerControllers.Count; i++)
			{
				if (playerControllers[i].GetBool(InputAction.MenuBackward))
				{
					OnCancel();
					break;
				}
			}
		}

		private int ActivePlayerAxisInput()
		{
			if (activePlayerAxisInputWrapper != null)
			{
				return activePlayerAxisInputWrapper.GetInt();
			}
			return 0;
		}

		private bool ActivePlayerConfirmInput()
		{
			if (activePlayerControllerID != -1)
			{
				return ControllerConfirmInput(activePlayerControllerID);
			}
			return false;
		}

		private bool ControllerConfirmInput(int controllerID)
		{
			if (controllerID == keyboardID)
			{
				return UnityEngine.Input.GetKeyDown(KeyCode.Space);
			}
			return UnityEngine.Input.GetKeyDown(InputHelper.GetKeyCode(0, controllerID)) || UnityEngine.Input.GetKeyDown(InputHelper.GetKeyCode(14, controllerID));
		}

		private Dictionary<string, int> ControllersByCount()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			string[] joystickNames = UnityEngine.Input.GetJoystickNames();
			string[] array = joystickNames;
			foreach (string key in array)
			{
				if (dictionary.ContainsKey(key))
				{
					dictionary[key]++;
				}
				else
				{
					dictionary.Add(key, 1);
				}
			}
			return dictionary;
		}

		private Dictionary<ControllerType, int> ControllerTypesByCount()
		{
			Dictionary<ControllerType, int> dictionary = new Dictionary<ControllerType, int>();
			string[] joystickNames = UnityEngine.Input.GetJoystickNames();
			string[] array = joystickNames;
			foreach (string controllerName in array)
			{
				ControllerType key = InputHelper.ControllerNameToType(controllerName);
				if (dictionary.ContainsKey(key))
				{
					dictionary[key]++;
				}
				else
				{
					dictionary.Add(key, 1);
				}
			}
			return dictionary;
		}

		private List<ControllerType> AvailableControllerTypes()
		{
			Dictionary<ControllerType, int> dictionary = ControllerTypesByCount();
			if (linkedControllers.Count > 0)
			{
				for (int i = 0; i < linkedControllers.Count; i++)
				{
					LinkedController linkedController = linkedControllers[i];
					if (dictionary.ContainsKey(linkedController.Type))
					{
						int num = dictionary[linkedController.Type];
						if (num > 1)
						{
							dictionary[linkedController.Type] = num - 1;
						}
						else
						{
							dictionary.Remove(linkedController.Type);
						}
					}
				}
			}
			return new List<ControllerType>(dictionary.Keys);
		}

		private bool MultipleControllerTypesAvailable()
		{
			return AvailableControllerTypes().Count > 1;
		}

		private void CallGUI(ControllerLinkGUIAction action, int id = -1)
		{
			if (IsInMenu())
			{
				GuiMenuManager.Instance.PerformControllerLinkAction(action, id);
			}
			else if (IsIngame())
			{
				GuiInGameManager.Instance.PerformControllerLinkAction(action, id);
			}
		}

		private void OpenControllerTypeSelection(int controllerID)
		{
			if (controllerID == keyboardID)
			{
				activePlayerAxisInputWrapper = InputHelper.GetWrappedInput(InputKey.LStick_Y, ControllerType.Keyboard, InputActionType.Down, (InputPlayer)(controllerID - 1));
			}
			else
			{
				activePlayerAxisInputWrapper = InputHelper.GetWrappedInput(InputKey.LStick_Y, dominantControllerType, InputActionType.Down, (InputPlayer)(controllerID - 1));
			}
			activePlayerControllerID = controllerID;
			state = InternalState.WaitingForPopupCallback;
			if (IsInMenu())
			{
				if (GuiMenuManager.Instance != null)
				{
					GuiMenuManager.Instance.TriggerControllerLinkSelection(ControllerSelectionCallback, ActivePlayerAxisInput, ActivePlayerConfirmInput, AvailableControllerTypes());
				}
			}
			else if (IsIngame())
			{
				if (GuiInGameManager.Instance != null)
				{
					GuiInGameManager.Instance.TriggerControllerLinkSelection(ControllerSelectionCallback, ActivePlayerAxisInput, ActivePlayerConfirmInput, AvailableControllerTypes());
				}
			}
			else
			{
				ControllerSelectionCallback(dominantControllerType);
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
	}
}
