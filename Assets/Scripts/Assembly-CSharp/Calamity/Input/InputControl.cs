using System.Collections.Generic;
using UnityEngine;

namespace Calamity.Input
{
	public class InputControl
	{
		private InputPlayer playerID;

		private ControllerType controllerType;

		private InputMode inputMode;

		protected Dictionary<InputAction, InputWrapper> inputs = new Dictionary<InputAction, InputWrapper>();

		private Dictionary<InputAction, InputWrapper> secondaryInputs;

		protected List<InputWrapper> updatingInputs = new List<InputWrapper>();

		public ControllerType Type
		{
			get
			{
				return controllerType;
			}
		}

		public InputControl(InputPlayer playerID, ControllerType controllerType, InputMode inputMode = InputMode.Normal)
		{
			this.playerID = playerID;
			this.controllerType = controllerType;
			this.inputMode = inputMode;
			Setup();
		}

		public void InitializeIngame()
		{
			foreach (InputWrapper value in inputs.Values)
			{
				value.InitializeIngame();
			}
		}

		public void Update()
		{
			for (int i = 0; i < updatingInputs.Count; i++)
			{
				updatingInputs[i].Update();
			}
		}

		public void UpdateGameObject(GameObject gameObject)
		{
			foreach (InputWrapper value in inputs.Values)
			{
				value.OnGameObjectUpdated(gameObject);
			}
		}

		public void OnLevelLoaded()
		{
			foreach (InputWrapper value in inputs.Values)
			{
				value.OnLevelLoaded();
			}
		}

		public bool GetBool(InputAction action)
		{
			InputWrapper value;
			if (inputs.TryGetValue(action, out value))
			{
				return value.GetBool();
			}
			return false;
		}

		public Vector3 GetVector(InputAction action)
		{
			InputWrapper value;
			if (inputs.TryGetValue(action, out value))
			{
				return value.GetVector();
			}
			return Vector3.zero;
		}

		public int GetInt(InputAction action)
		{
			InputWrapper value;
			if (inputs.TryGetValue(action, out value))
			{
				return value.GetInt();
			}
			return 0;
		}

		public float GetFloat(InputAction action)
		{
			InputWrapper value;
			if (inputs.TryGetValue(action, out value))
			{
				return value.GetFloat();
			}
			return 0f;
		}

		protected virtual void Setup()
		{
			Clear();
			Dictionary<InputAction, MappedAction> mappedActions = InputHelper.GetMappedActions(inputMode);
			if (mappedActions == null)
			{
				return;
			}
			foreach (MappedAction value in mappedActions.Values)
			{
				SetupAction(value);
			}
		}

		protected virtual void SetupAction(MappedAction mappedAction)
		{
			InputWrapper wrappedInput = InputHelper.GetWrappedInput(mappedAction.Key, controllerType, mappedAction.Type, playerID);
			if (wrappedInput != null)
			{
				AddInput(mappedAction.Action, wrappedInput);
			}
		}

		protected void AddInput(InputAction action, InputWrapper wrapper)
		{
			if (wrapper != null)
			{
				inputs.Add(action, wrapper);
				if (wrapper.RequiresUpdate)
				{
					updatingInputs.Add(wrapper);
				}
			}
		}

		public virtual void SetInputMode(InputMode inputMode)
		{
			if (this.inputMode != inputMode)
			{
				this.inputMode = inputMode;
				Setup();
			}
		}

		public void Shutdown()
		{
			foreach (InputWrapper value in inputs.Values)
			{
				value.Shutdown();
			}
		}

		private void Clear()
		{
			Shutdown();
			inputs.Clear();
			updatingInputs.Clear();
		}

		public static InputControl FromType(InputPlayer playerID, ControllerType controllerType, InputMode inputMode = InputMode.Normal)
		{
			switch (controllerType)
			{
			case ControllerType.None:
				return null;
			case ControllerType.Keyboard:
				return new InputControlKeyboard(playerID, controllerType, inputMode);
			case ControllerType.TouchControls:
				return new InputControlTouch(playerID, controllerType, inputMode);
			case ControllerType.HybridTouchKeyboard:
				return new InputControlHybrid(playerID, controllerType, inputMode);
			default:
				return new InputControl(playerID, controllerType, inputMode);
			}
		}
	}
}
