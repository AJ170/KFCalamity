using System.Collections.Generic;
using UnityEngine;

namespace Calamity.Input
{
	public class PlayerInputController
	{
		private ControllerType controllerType;

		private int controllerID;

		private bool enabled = true;

		private InputMode inputMode;

		private InputControl inputControl;

		private List<SoldierInputController> registeredSoldierControllers = new List<SoldierInputController>();

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		public bool IsGamePad
		{
			get
			{
				return controllerType != ControllerType.Keyboard && controllerType != ControllerType.TouchControls && controllerType != ControllerType.Unknown;
			}
		}

		public ControllerType ControllerType
		{
			get
			{
				return controllerType;
			}
		}

		public PlayerInputController(InputPlayer playerID)
		{
		}

		public PlayerInputController(InputPlayer playerID, ControllerType controllerType, int controllerID)
			: this(playerID)
		{
			if (controllerType != 0)
			{
				SetController(controllerType, controllerID);
			}
		}

		public void Update()
		{
			if (inputControl != null)
			{
				inputControl.Update();
			}
		}

		public void SetController(ControllerType controllerType, int controllerID)
		{
			this.controllerType = controllerType;
			this.controllerID = controllerID;
			Setup();
		}

		private void Setup()
		{
			inputControl = InputControl.FromType((InputPlayer)controllerID, controllerType, inputMode);
			AddInputControlToSoldiers();
			UpdateSoldierForInputControl();
		}

		public bool GetBool(InputAction action)
		{
			if (enabled && inputControl != null)
			{
				return inputControl.GetBool(action);
			}
			return false;
		}

		public Vector3 GetVector(InputAction action)
		{
			if (enabled && inputControl != null)
			{
				return inputControl.GetVector(action);
			}
			return Vector3.zero;
		}

		public int GetInt(InputAction action)
		{
			if (enabled && inputControl != null)
			{
				return inputControl.GetInt(action);
			}
			return 0;
		}

		public float GetFloat(InputAction action)
		{
			if (enabled && inputControl != null)
			{
				return inputControl.GetFloat(action);
			}
			return 0f;
		}

		public void RegisterSoldierController(SoldierInputController soldierController)
		{
			registeredSoldierControllers.Add(soldierController);
			soldierController.SetControl(inputControl);
			inputControl.InitializeIngame();
			UpdateSoldierForInputControl();
		}

		public void UnregisterSoldierController(SoldierInputController soldierController)
		{
			registeredSoldierControllers.Remove(soldierController);
			UpdateSoldierForInputControl();
		}

		private void AddInputControlToSoldiers()
		{
			for (int i = 0; i < registeredSoldierControllers.Count; i++)
			{
				if (registeredSoldierControllers[i] != null)
				{
					registeredSoldierControllers[i].SetControl(inputControl);
				}
			}
		}

		private void UpdateSoldierForInputControl()
		{
			if (inputControl != null)
			{
				if (registeredSoldierControllers.Count > 0)
				{
					inputControl.UpdateGameObject(registeredSoldierControllers[registeredSoldierControllers.Count - 1].gameObject);
				}
				else
				{
					inputControl.UpdateGameObject(null);
				}
			}
		}

		public void SetInputMode(InputMode inputMode)
		{
			this.inputMode = inputMode;
			if (inputControl != null)
			{
				inputControl.SetInputMode(inputMode);
			}
		}

		public void OnLevelLoaded()
		{
			if (inputControl != null)
			{
				inputControl.OnLevelLoaded();
			}
		}
	}
}
