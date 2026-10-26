using System.Collections.Generic;
using UnityEngine;

namespace Calamity.Input
{
	public static class InputHelper
	{
		private static Platform platform;

		private static Dictionary<InputMode, Dictionary<InputAction, MappedAction>> actionMapping;

		private static float defaultThreshold;

		private static Dictionary<ControllerType, float> thresholdOverrides;

		public static Platform Platform
		{
			get
			{
				return platform;
			}
		}

		static InputHelper()
		{
			platform = Platform.Android;
			defaultThreshold = 0.1f;
			SetupPlatform();
			SetupActionMapping();
			SetupThreshold();
		}

		private static void SetupPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.OSXPlayer:
				platform = Platform.MacOS;
				break;
			case RuntimePlatform.OSXEditor:
				platform = Platform.MacOS;
				break;
			case RuntimePlatform.WindowsPlayer:
				platform = Platform.Windows;
				break;
			case RuntimePlatform.WindowsEditor:
				platform = Platform.Windows;
				break;
			case RuntimePlatform.Android:
				platform = Platform.Android;
				break;
			case RuntimePlatform.IPhonePlayer:
				platform = Platform.iOS;
				break;
			case (RuntimePlatform)6:
			case RuntimePlatform.PS3:
			case RuntimePlatform.XBOX360:
				break;
			}
		}

		private static void SetupActionMapping()
		{
			actionMapping = new Dictionary<InputMode, Dictionary<InputAction, MappedAction>>();
			if (KFCompat.IsVitaControls)
			{
				SetupVitaActionMapping();
				return;
			}
			AddActionMap(InputMode.Normal, InputAction.Movement, InputKey.LStick);
			AddActionMap(InputMode.Normal, InputAction.Rotation, InputKey.RStick);
			AddActionMap(InputMode.Normal, InputAction.Dodge, InputKey.O, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.Grenade, InputKey.U, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.Ability, InputKey.Y, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.Syringe, InputKey.A);
			AddActionMap(InputMode.Normal, InputAction.Reload, InputKey.R3, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.InventoryMelee, InputKey.Dpad_Down, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.InventoryPistol, InputKey.Dpad_Left);
			AddActionMap(InputMode.Normal, InputAction.InventoryPrimary1, InputKey.Dpad_Up);
			AddActionMap(InputMode.Normal, InputAction.InventoryPrimary2, InputKey.Dpad_Right);
			AddActionMap(InputMode.Normal, InputAction.Fire, InputKey.RT);
			AddActionMap(InputMode.Normal, InputAction.FireSecondary, InputKey.RB);
			AddActionMap(InputMode.Normal, InputAction.Aim, InputKey.LT);
			AddActionMap(InputMode.Normal, InputAction.ThrowMoney, InputKey.LB, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.MenuForward, InputKey.O, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.MenuBackward, InputKey.A, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.MenuU, InputKey.U, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.MenuY, InputKey.Y, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.MenuHorizontal, InputKey.LStick_X, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.MenuVertical, InputKey.LStick_Y, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.MenuHorizontalAlternate, InputKey.Dpad_Horizontal, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.MenuVerticalAlternate, InputKey.Dpad_Vertical, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.MenuBumperLeft, InputKey.LB, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.MenuBumperRight, InputKey.RB, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.SkipSplashScreen, InputKey.O, InputActionType.Down);
			AddActionMap(InputMode.Normal, InputAction.Pause, InputKey.Start, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.Movement, InputKey.LStick);
			AddActionMap(InputMode.Alternate, InputAction.Rotation, InputKey.RStick);
			AddActionMap(InputMode.Alternate, InputAction.Dodge, InputKey.O, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.Grenade, InputKey.U, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.Ability, InputKey.Y, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.Syringe, InputKey.A);
			AddActionMap(InputMode.Alternate, InputAction.Reload, InputKey.R3, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.InventoryMelee, InputKey.Dpad_Down, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.InventoryPistol, InputKey.Dpad_Left);
			AddActionMap(InputMode.Alternate, InputAction.InventoryPrimary1, InputKey.Dpad_Up);
			AddActionMap(InputMode.Alternate, InputAction.InventoryPrimary2, InputKey.Dpad_Right);
			AddActionMap(InputMode.Alternate, InputAction.Fire, InputKey.RB);
			AddActionMap(InputMode.Alternate, InputAction.FireSecondary, InputKey.RT);
			AddActionMap(InputMode.Alternate, InputAction.Aim, InputKey.LB);
			AddActionMap(InputMode.Alternate, InputAction.ThrowMoney, InputKey.LT, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.MenuForward, InputKey.O, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.MenuBackward, InputKey.A, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.MenuU, InputKey.U, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.MenuY, InputKey.Y, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.MenuHorizontal, InputKey.LStick_X, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.MenuVertical, InputKey.LStick_Y, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.MenuHorizontalAlternate, InputKey.Dpad_Horizontal, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.MenuVerticalAlternate, InputKey.Dpad_Vertical, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.MenuBumperLeft, InputKey.LB, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.MenuBumperRight, InputKey.RB, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.SkipSplashScreen, InputKey.O, InputActionType.Down);
			AddActionMap(InputMode.Alternate, InputAction.Pause, InputKey.Start, InputActionType.Down);
		}

		// PS Vita port: the Vita has no L2/R2/L3/R3, so its layout differs from the console one. The D-pad cycles
		// weapons (tap) and drops the weapon in hand (hold), the rear touch pad's centre fires the secondary.
		private static void SetupVitaActionMapping()
		{
			InputMode[] array = new InputMode[2]
			{
				InputMode.Normal,
				InputMode.Alternate
			};
			foreach (InputMode mode in array)
			{
				AddActionMap(mode, InputAction.Movement, InputKey.LStick);
				AddActionMap(mode, InputAction.Rotation, InputKey.RStick);
				AddActionMap(mode, InputAction.Fire, InputKey.RT);
				AddActionMap(mode, InputAction.Aim, InputKey.LT);
				AddActionMap(mode, InputAction.FireSecondary, InputKey.RearTouchCenter);
				AddActionMap(mode, InputAction.Dodge, InputKey.O, InputActionType.Down);
				AddActionMap(mode, InputAction.Syringe, InputKey.A);
				AddActionMap(mode, InputAction.Reload, InputKey.U, InputActionType.Down);
				AddActionMap(mode, InputAction.Grenade, InputKey.Y, InputActionType.Down);
				AddActionMap(mode, InputAction.ThrowMoney, InputKey.Dpad_Up, InputActionType.Down);
				AddActionMap(mode, InputAction.Ability, InputKey.Dpad_Down, InputActionType.Down);
				AddActionMap(mode, InputAction.PreviousWeapon, InputKey.Dpad_Left);
				AddActionMap(mode, InputAction.NextWeapon, InputKey.Dpad_Right);
				AddActionMap(mode, InputAction.MenuForward, InputKey.O, InputActionType.Down);
				AddActionMap(mode, InputAction.MenuBackward, InputKey.A, InputActionType.Down);
				AddActionMap(mode, InputAction.MenuU, InputKey.U, InputActionType.Down);
				AddActionMap(mode, InputAction.MenuY, InputKey.Y, InputActionType.Down);
				AddActionMap(mode, InputAction.MenuHorizontal, InputKey.LStick_X, InputActionType.Down);
				AddActionMap(mode, InputAction.MenuVertical, InputKey.LStick_Y, InputActionType.Down);
				AddActionMap(mode, InputAction.MenuHorizontalAlternate, InputKey.Dpad_Horizontal, InputActionType.Down);
				AddActionMap(mode, InputAction.MenuVerticalAlternate, InputKey.Dpad_Vertical, InputActionType.Down);
				AddActionMap(mode, InputAction.MenuBumperLeft, InputKey.LB, InputActionType.Down);
				AddActionMap(mode, InputAction.MenuBumperRight, InputKey.RB, InputActionType.Down);
				AddActionMap(mode, InputAction.SkipSplashScreen, InputKey.O, InputActionType.Down);
				AddActionMap(mode, InputAction.Pause, InputKey.Start, InputActionType.Down);
			}
		}

		private static void AddActionMap(InputMode mode, InputAction action, InputKey key, InputActionType type = InputActionType.Normal)
		{
			if (!actionMapping.ContainsKey(mode))
			{
				actionMapping.Add(mode, new Dictionary<InputAction, MappedAction>(13));
			}
			Dictionary<InputAction, MappedAction> dictionary = actionMapping[mode];
			if (dictionary.ContainsKey(action))
			{
				dictionary[action] = new MappedAction(action, key, type);
			}
			else
			{
				dictionary.Add(action, new MappedAction(action, key, type));
			}
		}

		public static Dictionary<InputAction, MappedAction> GetMappedActions(InputMode mode)
		{
			if (actionMapping.ContainsKey(mode))
			{
				return actionMapping[mode];
			}
			return null;
		}

		public static MappedAction? GetMappedAction(InputAction action, InputMode mode = InputMode.Normal)
		{
			if (actionMapping.ContainsKey(mode))
			{
				Dictionary<InputAction, MappedAction> dictionary = actionMapping[mode];
				if (dictionary.ContainsKey(action))
				{
					return dictionary[action];
				}
			}
			return null;
		}

		public static InputWrapper GetWrappedInput(InputKey key, ControllerType controllerType, InputActionType inputType, InputPlayer playerID)
		{
			InputWrapper result = null;
			if (controllerType == ControllerType.XBOX360_Windows)
			{
				controllerType = ControllerType.XBOX360_Android;
			}
			switch (controllerType)
			{
			case ControllerType.Keyboard:
				result = WrapKeyboardInput(key, controllerType, inputType, playerID);
				break;
			case ControllerType.Broadcom_Android:
			case ControllerType.GameStick_Android:
			case ControllerType.Generic_Android:
			case ControllerType.MogaPro_Android:
			case ControllerType.Ouya_Android:
			case ControllerType.Shield_Android:
			case ControllerType.Fire_TV_Android:
			case ControllerType.Nyko_Playpad_Pro_Android:
			case ControllerType.PS3_Android:
			case ControllerType.XBOX360_Android:
			case ControllerType.XBOX360_Wireless_Android:
				result = WrapAndroidInput(key, controllerType, inputType, playerID);
				break;
			case ControllerType.ShieldController_Windows:
			case ControllerType.Ouya_Windows:
			case ControllerType.GameStick_Windows:
			case ControllerType.PS3_Windows:
			case ControllerType.PS4_Windows:
			case ControllerType.XBOX360_Windows:
			case ControllerType.MotionInJoy_Windows:
				result = WrapWindowsInput(key, controllerType, inputType, playerID);
				break;
			case ControllerType.TouchControls:
				result = WrapTouchInput(key, controllerType, inputType, playerID);
				break;
			case ControllerType.HybridTouchKeyboard:
				result = WrapHybridTouchKeyboardInput(key, controllerType, inputType, playerID);
				break;
			case ControllerType.PSVita:
				result = WrapVitaInput(key, controllerType, inputType, playerID);
				break;
			case ControllerType.Unknown:
				result = WrapDefaultInput(key, controllerType, inputType, playerID);
				break;
			}
			return result;
		}

		private static InputWrapper WrapHybridTouchKeyboardInput(InputKey key, ControllerType controllerType, InputActionType inputType, InputPlayer playerID)
		{
			InputWrapper result = null;
			switch (key)
			{
			case InputKey.LStick:
				result = GetWrappedStickOrVirtualStick("Horizontal Keyboard", "Vertical Keyboard", "LeftStickParent", false, false);
				break;
			case InputKey.RStick:
				result = GetWrappedVirtualStickOrMouseRotationWrapper("RightStickParent");
				break;
			case InputKey.LT:
				result = GetWrappedVirtualStickThresholdOrMouseButtonWrapper("RightStickParent", "Mouse Fire2", inputType, "Aim", 0.1f, true);
				break;
			case InputKey.RT:
				result = GetWrappedVirtualStickThresholdOrMouseButtonWrapper("RightStickParent", "Mouse Fire", inputType, "Auto");
				break;
			case InputKey.O:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.Space, inputType, "touch_Button_utility_ability_3");
				break;
			case InputKey.U:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.G, inputType, "touch_Button_utility_ability_2");
				break;
			case InputKey.Y:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.E, inputType, "touch_Button_ability");
				break;
			case InputKey.A:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.Q, inputType, "touch_Button_utility_ability_1");
				break;
			case InputKey.LB:
				result = GetWrappedKey(KeyCode.F, inputType);
				break;
			case InputKey.RB:
				result = GetWrappedButton("Mouse Fire2", inputType);
				break;
			case InputKey.L3:
				result = GetWrappedKey(KeyCode.V, inputType);
				break;
			case InputKey.R3:
				result = GetWrappedDoubleTapOrKeyWrapper("RightStickParent", KeyCode.R, inputType);
				break;
			case InputKey.Dpad_Up:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.Alpha3, inputType, "WeaponSlotPrimary1");
				break;
			case InputKey.Dpad_Down:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.Alpha1, inputType, "WeaponSlotMelee");
				break;
			case InputKey.Dpad_Left:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.Alpha2, inputType, "WeaponSlotPistol");
				break;
			case InputKey.Dpad_Right:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.Alpha4, inputType, "WeaponSlotPrimary2");
				break;
			case InputKey.Dpad_Horizontal:
				result = GetWrappedKeyAxis(KeyCode.LeftArrow, KeyCode.RightArrow, inputType);
				break;
			case InputKey.Dpad_Vertical:
				result = GetWrappedKeyAxis(KeyCode.DownArrow, KeyCode.UpArrow, inputType);
				break;
			}
			return result;
		}

		private static InputWrapper WrapTouchInput(InputKey key, ControllerType controllerType, InputActionType inputType, InputPlayer playerID)
		{
			InputWrapper result = null;
			switch (key)
			{
			case InputKey.LStick:
				result = GetWrappedVirtualStick("LeftStickParent");
				break;
			case InputKey.RStick:
				result = GetWrappedVirtualStick("RightStickParent");
				break;
			case InputKey.LT:
				result = GetWrappedVirtualThresholdStick("RightStickParent", "Aim", 0.1f, true);
				break;
			case InputKey.RT:
				result = GetWrappedVirtualThresholdStick("RightStickParent", "Auto", 0f);
				break;
			case InputKey.O:
				result = GetWrappedTouchButton("touch_Button_utility_ability_3");
				break;
			case InputKey.U:
				result = GetWrappedTouchButton("touch_Button_utility_ability_2");
				break;
			case InputKey.Y:
				result = GetWrappedTouchButton("touch_Button_ability");
				break;
			case InputKey.A:
				result = GetWrappedTouchButton("touch_Button_utility_ability_1", false, 0f, true);
				break;
			case InputKey.LB:
				result = GetWrappedKey(KeyCode.F, inputType);
				break;
			case InputKey.RB:
				result = GetWrappedKey(KeyCode.M, inputType);
				break;
			case InputKey.L3:
				result = GetWrappedKey(KeyCode.V, inputType);
				break;
			case InputKey.R3:
				result = GetWrappedMultiInputWrapper(GetWrappedDoubleTapInput("RightStickParent"), GetWrappedTouchButton("touch_Button_reload"));
				break;
			case InputKey.Dpad_Horizontal:
				result = GetWrappedKeyAxis(KeyCode.LeftArrow, KeyCode.RightArrow, inputType);
				break;
			case InputKey.Dpad_Vertical:
				result = GetWrappedKeyAxis(KeyCode.DownArrow, KeyCode.UpArrow, inputType);
				break;
			}
			return result;
		}

		private static InputWrapper WrapKeyboardInput(InputKey key, ControllerType controllerType, InputActionType inputType, InputPlayer playerID)
		{
			InputWrapper result = null;
			switch (key)
			{
			case InputKey.LStick:
				result = GetWrappedStick("Horizontal Keyboard", "Vertical Keyboard", false, false);
				break;
			case InputKey.RStick:
				result = GetWrappedMouseRotation();
				break;
			case InputKey.LStick_X:
				result = GetWrappedDualAxis("Horizontal Keyboard", inputType);
				break;
			case InputKey.LStick_Y:
				result = GetWrappedDualAxis("Vertical Keyboard", inputType);
				break;
			case InputKey.LT:
				result = GetWrappedButton("Mouse Fire2", inputType);
				break;
			case InputKey.RT:
				result = GetWrappedButton("Mouse Fire", inputType);
				break;
			case InputKey.O:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.Space, inputType, "touch_Button_utility_ability_3");
				break;
			case InputKey.U:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.G, inputType, "touch_Button_utility_ability_2");
				break;
			case InputKey.Y:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.E, inputType, "touch_Button_ability");
				break;
			case InputKey.A:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.Q, inputType, "touch_Button_utility_ability_1", false, 0f, true);
				break;
			case InputKey.LB:
				result = GetWrappedKey(KeyCode.F, inputType);
				break;
			case InputKey.L3:
				result = GetWrappedKey(KeyCode.V, inputType);
				break;
			case InputKey.R3:
				result = GetWrappedKey(KeyCode.R, inputType);
				break;
			case InputKey.Dpad_Up:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.Alpha3, inputType, "WeaponSlotPrimary1");
				break;
			case InputKey.Dpad_Down:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.Alpha1, inputType, "WeaponSlotMelee");
				break;
			case InputKey.Dpad_Left:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.Alpha2, inputType, "WeaponSlotPistol");
				break;
			case InputKey.Dpad_Right:
				result = GetWrappedKeyOrTouchButtonInput(KeyCode.Alpha4, inputType, "WeaponSlotPrimary2");
				break;
			case InputKey.Dpad_Horizontal:
				result = GetWrappedKeyAxis(KeyCode.LeftArrow, KeyCode.RightArrow, inputType);
				break;
			case InputKey.Dpad_Vertical:
				result = GetWrappedKeyAxis(KeyCode.DownArrow, KeyCode.UpArrow, inputType);
				break;
			}
			return result;
		}

		private static InputWrapper WrapAndroidInput(InputKey key, ControllerType controllerType, InputActionType inputType, InputPlayer playerID)
		{
			InputWrapper result = null;
			switch (controllerType)
			{
			case ControllerType.Broadcom_Android:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(4, 5, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(4, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(5, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RT:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(5, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.Start:
					result = GetWrappedKey(KeyCode.Return, inputType);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedKey(8, inputType, playerID);
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedKey(9, inputType, playerID);
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedKey(10, inputType, playerID);
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedKey(11, inputType, playerID);
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedKeyAxis(10, 11, inputType, playerID);
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedKeyAxis(8, 9, inputType, playerID);
					break;
				}
				break;
			case ControllerType.GameStick_Android:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(3, 4, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(4, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(13, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(14, inputType, playerID);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedAxis(6, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedAxis(5, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Start:
					result = GetWrappedKey(11, inputType, playerID);
					break;
				case InputKey.None:
					result = GetWrappedKey(27, inputType, playerID);
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedDualAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedDualAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				}
				break;
			case ControllerType.Generic_Android:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(4, 5, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(4, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(5, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RT:
					result = GetWrappedAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(5, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedKey(8, inputType, playerID);
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedKey(9, inputType, playerID);
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedKey(10, inputType, playerID);
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedKey(11, inputType, playerID);
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedKeyAxis(10, 11, inputType, playerID);
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedKeyAxis(9, 8, inputType, playerID);
					break;
				}
				break;
			case ControllerType.Shield_Android:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(3, 4, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(4, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedAxis(7, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RT:
					result = GetWrappedAxis(8, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(5, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(13, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(9, inputType, playerID);
					break;
				case InputKey.Start:
					result = GetWrappedKey(KeyCode.Return, inputType);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedAxis(6, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedAxis(5, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedDualAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedDualAxis(6, true, inputType, playerID, GetThreshold(controllerType));
					break;
				}
				break;
			case ControllerType.Fire_TV_Android:
			case ControllerType.Nyko_Playpad_Pro_Android:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(3, 4, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(4, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedAxis(13, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RT:
					result = GetWrappedAxis(12, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(5, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(8, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(9, inputType, playerID);
					break;
				case InputKey.Start:
					result = GetWrappedKey(KeyCode.Return, inputType);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedAxis(6, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedAxis(5, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedDualAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedDualAxis(6, true, inputType, playerID, GetThreshold(controllerType));
					break;
				}
				break;
			case ControllerType.MogaPro_Android:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(3, 4, false, false, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(4, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedAxis(8, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RT:
					result = GetWrappedAxis(7, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(13, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(14, inputType, playerID);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedAxis(6, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedAxis(5, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedDualAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedDualAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				}
				break;
			case ControllerType.Ouya_Android:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(3, 4, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(4, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RT:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(5, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedKey(8, inputType, playerID);
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedKey(9, inputType, playerID);
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedKey(10, inputType, playerID);
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedKey(11, inputType, playerID);
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedKeyAxis(10, 11, inputType, playerID);
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedKeyAxis(9, 8, inputType, playerID);
					break;
				}
				break;
			case ControllerType.PS3_Android:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(3, 4, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(4, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedKey(8, inputType, playerID);
					break;
				case InputKey.RT:
					result = GetWrappedKey(9, inputType, playerID);
					break;
				case InputKey.O:
					result = GetWrappedKey(14, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(15, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(12, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(13, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(10, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(11, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedKey(5, inputType, playerID);
					break;
				case InputKey.Start:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.None:
					result = GetWrappedKey(27, inputType, playerID);
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedKeyAxis(7, 5, inputType, playerID);
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedKeyAxis(6, 4, inputType, playerID);
					break;
				}
				break;
			case ControllerType.XBOX360_Android:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(3, 4, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(4, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedAxis(7, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RT:
					result = GetWrappedAxis(8, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(13, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(14, inputType, playerID);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedAxis(6, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedAxis(5, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Start:
					result = GetWrappedKey(11, inputType, playerID);
					break;
				case InputKey.None:
					result = GetWrappedKey(27, inputType, playerID);
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedDualAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedDualAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				}
				break;
			case ControllerType.XBOX360_Wireless_Android:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(3, 4, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(4, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RT:
					result = GetWrappedAxis(7, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(13, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(14, inputType, playerID);
					break;
				case InputKey.Start:
					result = GetWrappedKey(11, inputType, playerID);
					break;
				case InputKey.None:
					result = GetWrappedKey(27, inputType, playerID);
					break;
				}
				break;
			}
			return result;
		}

		private static InputWrapper WrapWindowsInput(InputKey key, ControllerType controllerType, InputActionType inputType, InputPlayer playerID)
		{
			InputWrapper result = null;
			switch (controllerType)
			{
			case ControllerType.Ouya_Windows:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(4, 5, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(4, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(5, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RT:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(5, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedKey(8, inputType, playerID);
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedKey(9, inputType, playerID);
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedKey(10, inputType, playerID);
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedKey(11, inputType, playerID);
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedKeyAxis(10, 11, inputType, playerID);
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedKeyAxis(9, 8, inputType, playerID);
					break;
				}
				break;
			case ControllerType.XBOX360_Windows:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(4, 5, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(4, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(5, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedAxis(9, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RT:
					result = GetWrappedAxis(10, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(5, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(8, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(9, inputType, playerID);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedAxis(7, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedAxis(7, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedAxis(6, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Start:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.None:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedDualAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedDualAxis(7, inputType, playerID, GetThreshold(controllerType));
					break;
				}
				break;
			case ControllerType.PS3_Windows:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(3, 4, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(4, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedKey(8, inputType, playerID);
					break;
				case InputKey.RT:
					result = GetWrappedKey(9, inputType, playerID);
					break;
				case InputKey.O:
					result = GetWrappedKey(14, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(15, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(12, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(13, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(10, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(11, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedKey(5, inputType, playerID);
					break;
				case InputKey.Start:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.None:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedKeyAxis(7, 5, inputType, playerID);
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedKeyAxis(6, 4, inputType, playerID);
					break;
				}
				break;
			case ControllerType.PS4_Windows:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(3, 4, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(4, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.RT:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.O:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(10, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(11, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(14, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedKey(5, inputType, playerID);
					break;
				case InputKey.Start:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.None:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedKeyAxis(7, 5, inputType, playerID);
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedKeyAxis(6, 4, inputType, playerID);
					break;
				}
				break;
			case ControllerType.MotionInJoy_Windows:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(4, 5, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(4, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(5, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RT:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(5, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(8, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(9, inputType, playerID);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedKey(13, inputType, playerID);
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedKey(15, inputType, playerID);
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedKey(16, inputType, playerID);
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedKey(14, inputType, playerID);
					break;
				case InputKey.Start:
					result = GetWrappedKey(11, inputType, playerID);
					break;
				case InputKey.None:
					result = GetWrappedKey(10, inputType, playerID);
					break;
				case InputKey.System:
					result = GetWrappedKey(12, inputType, playerID);
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedKeyAxis(16, 14, inputType, playerID);
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedKeyAxis(15, 13, inputType, playerID);
					break;
				}
				break;
			case ControllerType.GameStick_Windows:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(3, 4, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(4, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(13, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(14, inputType, playerID);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedAxis(6, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedAxis(5, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Start:
					result = GetWrappedKey(11, inputType, playerID);
					break;
				case InputKey.None:
					result = GetWrappedKey(27, inputType, playerID);
					break;
				case InputKey.Dpad_Horizontal:
					result = GetWrappedDualAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Vertical:
					result = GetWrappedDualAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				}
				break;
			case ControllerType.Shield_Android:
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(3, 4, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(4, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedAxis(7, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RT:
					result = GetWrappedAxis(8, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(2, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(13, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(14, inputType, playerID);
					break;
				case InputKey.Start:
					result = GetWrappedKey(KeyCode.Return, inputType);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedAxis(6, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedAxis(5, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				}
				break;
			}
			return result;
		}

		private static InputWrapper WrapOSXInput(InputKey key, ControllerType controllerType, InputActionType inputType, InputPlayer playerID)
		{
			return null;
		}

		// PS Vita buttons are joystick buttons 0-11: cross, circle, square, triangle, L, R, select, start, up, right,
		// down, left. Sticks: left = Joy axes 1/2, right = 4/5, up reads negative.
		private static InputWrapper WrapVitaInput(InputKey key, ControllerType controllerType, InputActionType inputType, InputPlayer playerID)
		{
			InputWrapper result = null;
			switch (key)
			{
			case InputKey.LStick:
				result = GetWrappedStick(1, 2, false, true, playerID);
				break;
			case InputKey.RStick:
				result = GetWrappedStick(4, 5, false, true, playerID);
				break;
			case InputKey.LStick_X:
				result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
				break;
			case InputKey.LStick_Y:
				result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
				break;
			case InputKey.RStick_X:
				result = GetWrappedDualAxis(4, inputType, playerID, GetThreshold(controllerType));
				break;
			case InputKey.RStick_Y:
				result = GetWrappedDualAxis(5, true, inputType, playerID, GetThreshold(controllerType));
				break;
			case InputKey.O:
				result = GetWrappedKey(0, inputType, playerID);
				break;
			case InputKey.A:
				result = GetWrappedKey(1, inputType, playerID);
				break;
			case InputKey.U:
				result = GetWrappedKey(2, inputType, playerID);
				break;
			case InputKey.Y:
				result = GetWrappedKey(3, inputType, playerID);
				break;
			case InputKey.LT:
			case InputKey.LB:
				result = GetWrappedKey(4, inputType, playerID);
				break;
			case InputKey.RT:
			case InputKey.RB:
				result = GetWrappedKey(5, inputType, playerID);
				break;
			case InputKey.Start:
				result = GetWrappedKey(7, inputType, playerID);
				break;
			case InputKey.Dpad_Up:
				result = GetWrappedKey(8, inputType, playerID);
				break;
			case InputKey.Dpad_Right:
				result = GetWrappedKey(9, inputType, playerID);
				break;
			case InputKey.Dpad_Down:
				result = GetWrappedKey(10, inputType, playerID);
				break;
			case InputKey.Dpad_Left:
				result = GetWrappedKey(11, inputType, playerID);
				break;
			case InputKey.Dpad_Horizontal:
				result = GetWrappedKeyAxis(11, 9, inputType, playerID);
				break;
			case InputKey.Dpad_Vertical:
				result = GetWrappedKeyAxis(10, 8, inputType, playerID);
				break;
			case InputKey.RearTouchCenter:
				result = new VitaRearTouchWrapper(inputType);
				break;
			}
			return result;
		}

		private static InputWrapper WrapDefaultInput(InputKey key, ControllerType controllerType, InputActionType inputType, InputPlayer playerID)
		{
			InputWrapper result = null;
			if (controllerType == ControllerType.Unknown)
			{
				switch (key)
				{
				case InputKey.LStick:
					result = GetWrappedStick(1, 2, false, true, playerID);
					break;
				case InputKey.RStick:
					result = GetWrappedStick(3, 4, false, true, playerID);
					break;
				case InputKey.LStick_X:
					result = GetWrappedDualAxis(1, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LStick_Y:
					result = GetWrappedDualAxis(2, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_X:
					result = GetWrappedDualAxis(3, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RStick_Y:
					result = GetWrappedDualAxis(4, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.LT:
					result = GetWrappedAxis(7, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.RT:
					result = GetWrappedAxis(8, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.O:
					result = GetWrappedKey(0, inputType, playerID);
					break;
				case InputKey.U:
					result = GetWrappedKey(3, inputType, playerID);
					break;
				case InputKey.Y:
					result = GetWrappedKey(4, inputType, playerID);
					break;
				case InputKey.A:
					result = GetWrappedKey(1, inputType, playerID);
					break;
				case InputKey.LB:
					result = GetWrappedKey(6, inputType, playerID);
					break;
				case InputKey.RB:
					result = GetWrappedKey(7, inputType, playerID);
					break;
				case InputKey.L3:
					result = GetWrappedKey(13, inputType, playerID);
					break;
				case InputKey.R3:
					result = GetWrappedKey(14, inputType, playerID);
					break;
				case InputKey.Dpad_Up:
					result = GetWrappedAxis(6, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Down:
					result = GetWrappedAxis(6, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Left:
					result = GetWrappedAxis(5, true, inputType, playerID, GetThreshold(controllerType));
					break;
				case InputKey.Dpad_Right:
					result = GetWrappedAxis(5, inputType, playerID, GetThreshold(controllerType));
					break;
				}
			}
			return result;
		}

		private static InputWrapper GetWrappedStick(int horizontalAxisID, int verticalAxisID, bool invertHorizontal, bool invertVertical, InputPlayer playerID)
		{
			return GetWrappedStick(GetAxisName(horizontalAxisID, playerID), GetAxisName(verticalAxisID, playerID), invertHorizontal, invertVertical);
		}

		private static InputWrapper GetWrappedStick(string horizontalAxis, string verticalAxis, bool invertHorizontal, bool invertVertical)
		{
			return new StickInputWrapper(horizontalAxis, verticalAxis, invertHorizontal, invertVertical);
		}

		private static InputWrapper GetWrappedDualAxis(int axisID, InputActionType inputType, InputPlayer playerID, float threshold)
		{
			return GetWrappedDualAxis(axisID, false, inputType, playerID, threshold);
		}

		private static InputWrapper GetWrappedDualAxis(int axisID, bool inverted, InputActionType inputType, InputPlayer playerID, float threshold)
		{
			return GetWrappedDualAxis(GetAxisName(axisID, playerID), inverted, inputType, threshold);
		}

		private static InputWrapper GetWrappedDualAxis(string axis, InputActionType inputType)
		{
			return GetWrappedDualAxis(axis, false, inputType, defaultThreshold);
		}

		private static InputWrapper GetWrappedDualAxis(string axis, InputActionType inputType, float threshold)
		{
			return GetWrappedDualAxis(axis, false, inputType, threshold);
		}

		private static InputWrapper GetWrappedDualAxis(string axis, bool inverted, InputActionType inputType, float threshold)
		{
			switch (inputType)
			{
			case InputActionType.Down:
			case InputActionType.Up:
				return new DualAxisDownInputWrapper(axis, inverted, threshold);
			default:
				return new DualAxisInputWrapper(axis, inverted, threshold);
			}
		}

		private static InputWrapper GetWrappedAxis(int axisID, InputActionType inputType, InputPlayer playerID, float threshold)
		{
			return GetWrappedAxis(axisID, false, inputType, playerID, threshold);
		}

		private static InputWrapper GetWrappedAxis(int axisID, bool inverted, InputActionType inputType, InputPlayer playerID, float threshold)
		{
			return GetWrappedAxis(GetAxisName(axisID, playerID), inverted, inputType, threshold);
		}

		private static InputWrapper GetWrappedAxis(string axis, InputActionType inputType)
		{
			return GetWrappedAxis(axis, false, inputType, defaultThreshold);
		}

		private static InputWrapper GetWrappedAxis(string axis, InputActionType inputType, float threshold)
		{
			return GetWrappedAxis(axis, false, inputType, threshold);
		}

		private static InputWrapper GetWrappedAxis(string axis, bool inverted, InputActionType inputType, float threshold)
		{
			switch (inputType)
			{
			case InputActionType.Down:
			case InputActionType.Up:
				return new AxisDownInputWrapper(axis, inverted, threshold);
			default:
				return new AxisInputWrapper(axis, inverted, threshold);
			}
		}

		public static InputWrapper GetWrappedButton(string button, InputActionType inputType)
		{
			return new ButtonInputWrapper(button, inputType);
		}

		private static InputWrapper GetWrappedKey(int keyID, InputActionType inputType, InputPlayer playerID, bool rawKeyCode = false)
		{
			return GetWrappedKey(GetKeyCode(keyID, playerID), inputType);
		}

		public static InputWrapper GetWrappedKey(KeyCode keyCode, InputActionType inputType)
		{
			return new KeyInputWrapper(keyCode, inputType);
		}

		private static InputWrapper GetWrappedKeyAxis(int keyAID, int keyBID, InputActionType inputType, InputPlayer playerID)
		{
			return GetWrappedKeyAxis(GetKeyCode(keyAID, playerID), GetKeyCode(keyBID, playerID), inputType);
		}

		private static InputWrapper GetWrappedKeyAxis(KeyCode keyCodeA, KeyCode keyCodeB, InputActionType inputType)
		{
			return new KeyAxisInputWrapper(keyCodeA, keyCodeB, inputType);
		}

		public static InputWrapper GetWrappedTouchButton(string buttonName, bool toggle = false, float delay = 0f, bool canHoldDown = false)
		{
			return new TouchButtonWrapper(buttonName, toggle, delay, canHoldDown);
		}

		public static InputWrapper GetWrappedVirtualStick(string stickObjectName)
		{
			return new VirtualStickWrapper(stickObjectName);
		}

		public static InputWrapper GetWrappedVirtualThresholdStick(string stickObjectName, string zoneName, float time = 0f, bool sticky = false)
		{
			return new VirtualStickThresholdWrapper(stickObjectName, zoneName, time, sticky);
		}

		public static InputWrapper GetWrappedVirtualThresholdStick(string stickObjectName, float minThreshold, float maxThreshold, float time = 0f, bool sticky = false)
		{
			return new VirtualStickThresholdWrapper(stickObjectName, minThreshold, maxThreshold, time, sticky);
		}

		public static InputWrapper GetWrappedAxisTouchButtons(string minButton, string maxButton)
		{
			return new AxisTouchButtonsWrapper(minButton, maxButton);
		}

		public static InputWrapper GetWrappedTapInput()
		{
			return new TapGestureWrapper();
		}

		public static InputWrapper GetWrappedSwipeInput(TKSwipeDirection swipeDirection, bool invert = false)
		{
			return new SwipeGestureWrapper(swipeDirection, invert);
		}

		public static InputWrapper GetWrappedPinchInput(int pinchDirection)
		{
			return new PinchGestureWrapper(pinchDirection);
		}

		public static InputWrapper GetWrappedDoubleTapInput(string gameObjectName)
		{
			return new DoubleTapInputWrapper(gameObjectName);
		}

		public static InputWrapper GetWrappedAnyKeyInput(InputActionType inputType)
		{
			return new AnyKeyInputWrapper(inputType);
		}

		public static InputWrapper GetWrappedKeyOrTouchButtonInput(KeyCode key, InputActionType type, string touchButtonName, bool touchButtonIsToggle = false, float touchPressDelay = 0f, bool canHoldDown = false)
		{
			return new KeyOrTouchButtonInputWrapper(key, type, touchButtonName, touchButtonIsToggle, touchPressDelay, canHoldDown);
		}

		private static InputWrapper GetWrappedKeyOrTouch(KeyCode key, InputActionType type, string touchButtonName, bool touchButtonIsToggle = false, float touchPressDelay = 0f)
		{
			return new KeyOrTouchButtonInputWrapper(key, type, touchButtonName, touchButtonIsToggle, touchPressDelay);
		}

		private static InputWrapper GetWrappedStickOrVirtualStick(string horizontalAxis, string verticalAxis, string virtualStickObjectName, bool invertHorizontal, bool invertVertical)
		{
			return new StickOrVirtualStickWrapper(horizontalAxis, verticalAxis, virtualStickObjectName, invertHorizontal, invertVertical);
		}

		private static InputWrapper GetWrappedVirtualStickOrMouseRotationWrapper(string stickObjectName)
		{
			return new VirtualStickOrMouseRotationWrapper(stickObjectName);
		}

		private static InputWrapper GetWrappedVirtualStickThresholdOrMouseButtonWrapper(string stickObjectName, string mouseButton, InputActionType inputType, string stickZone, float delay = 0f, bool sticky = false)
		{
			return new VirtualStickThresholdOrMouseButtonWrapper(stickObjectName, mouseButton, inputType, stickZone, delay, sticky);
		}

		private static InputWrapper GetWrappedDoubleTapOrKeyWrapper(string doubleTapObject, KeyCode key, InputActionType type)
		{
			return new DoubleTapOrKeyWrapper(doubleTapObject, key, type);
		}

		private static InputWrapper GetWrappedMouseRotation()
		{
			return new MouseRotationWrapper();
		}

		public static InputWrapper GetWrappedMultiInputWrapper(params InputWrapper[] wrappers)
		{
			return new MultipleInputsWrapper(wrappers);
		}

		private static string GetAxisName(int axisID, InputPlayer playerID)
		{
			return string.Format("Joy{0} Axis {1}", (int)(playerID + 1), axisID);
		}

		public static KeyCode GetKeyCode(int keyID, InputPlayer playerID)
		{
			return GetKeyCode(keyID, (int)(playerID + 1));
		}

		public static KeyCode GetKeyCode(int keyID, int playerID)
		{
			if (keyID < 20)
			{
				return (KeyCode)(330 + keyID + playerID * 20);
			}
			return (KeyCode)keyID;
		}

		private static void SetupThreshold()
		{
			thresholdOverrides = new Dictionary<ControllerType, float>();
			switch (platform)
			{
			case Platform.Android:
				thresholdOverrides.Add(ControllerType.XBOX360_Android, 0.15f);
				break;
			case Platform.Windows:
				thresholdOverrides.Add(ControllerType.XBOX360_Windows, 0.15f);
				break;
			}
		}

		private static float GetThreshold(ControllerType controllerType)
		{
			if (thresholdOverrides.ContainsKey(controllerType))
			{
				return thresholdOverrides[controllerType];
			}
			return defaultThreshold;
		}

		public static ControllerType ControllerNameToType(string controllerName)
		{
			if (KFCompat.IsVitaControls)
			{
				return ControllerType.PSVita;
			}
			controllerName = controllerName.ToUpper();
			ControllerType controllerType = ControllerType.Unknown;
			switch (platform)
			{
			case Platform.Android:
				switch (controllerName)
				{
				case "BROADCOM BLUETOOTH HID":
				case "NVIDIA CORPORATION NVIDIA CONTROLLER V01":
					controllerType = ControllerType.Shield_Android;
					break;
				case "MOGA PRO HID":
					controllerType = ControllerType.MogaPro_Android;
					break;
				case "OUYA GAME CONTROLLER":
					controllerType = ControllerType.Ouya_Android;
					break;
				case "AMAZON FIRE GAME CONTROLLER":
					controllerType = ControllerType.Fire_TV_Android;
					break;
				case "NYKO PLAYPAD PRO":
					controllerType = ControllerType.Nyko_Playpad_Pro_Android;
					break;
				case "GAMESTICK CONTROLLER 1":
				case "GAMESTICK CONTROLLER 2":
				case "GAMESTICK CONTROLLER 3":
				case "GAMESTICK CONTROLLER 4":
				case "BERWAY TECH LTD GAMESTICK CONTROLLER":
					controllerType = ControllerType.GameStick_Android;
					break;
				case "SONY PLAYSTATION(R)3 CONTROLLER":
				case "PLAYSTATION(R)3 CONTROLLER":
					controllerType = ControllerType.PS3_Android;
					break;
				case "XBOX 360 WIRELESS RECEIVER":
				case "CONTROLLER (XBOX 360 WIRELESS RECEIVER FOR Windows)":
				case "MICROSOFT WIRELESS 360 CONTROLLER":
					controllerType = ControllerType.XBOX360_Wireless_Android;
					break;
				case "CONTROLLER (AFTERGLOW GAMEPAD FOR XBOX 360)":
				case "CONTROLLER (ROCK CANDY GAMEPAD FOR XBOX 360)":
				case "MICROSOFT X-BOX 360 PAD":
				case "CONTROLLER (XBOX 360 FOR Windows)":
				case "CONTROLLER (XBOX ONE FOR WINDOWS)":
				case "CONTROLLER (XBOX360 GAMEPAD)":
				case "XBOX 360 FOR Windows (CONTROLLER)":
					controllerType = ControllerType.XBOX360_Windows;
					break;
				default:
					controllerType = ControllerType.Unknown;
					break;
				}
				break;
			case Platform.Windows:
				switch (controllerName)
				{
				case "OUYA GAME CONTROLLER":
					controllerType = ControllerType.Ouya_Windows;
					break;
				case "MOTIONINJOY VIRTUAL GAME CONTROLLER":
					controllerType = ControllerType.MotionInJoy_Windows;
					break;
				case "SONY PLAYSTATION(R)3 CONTROLLER":
				case "PLAYSTATION(R)3 CONTROLLER":
					controllerType = ControllerType.PS3_Windows;
					break;
				case "WIRELESS CONTROLLER":
					controllerType = ControllerType.PS4_Windows;
					break;
				case "MICROSOFT WIRELESS 360 CONTROLLER":
				case "XBOX 360 WIRELESS RECEIVER":
				case "CONTROLLER (AFTERGLOW GAMEPAD FOR XBOX 360)":
				case "CONTROLLER (ROCK CANDY GAMEPAD FOR XBOX 360)":
				case "CONTROLLER (XBOX 360 WIRELESS RECEIVER FOR WINDOWS)":
				case "MICROSOFT X-BOX 360 PAD":
				case "CONTROLLER (XBOX 360 FOR WINDOWS)":
				case "CONTROLLER (XBOX ONE FOR WINDOWS)":
				case "CONTROLLER (XBOX360 GAMEPAD)":
				case "XBOX 360 FOR WINDOWS (CONTROLLER)":
					controllerType = ControllerType.XBOX360_Windows;
					break;
				case "BERWAY TECH LTD GAMESTICK CONTROLLER":
					controllerType = ControllerType.GameStick_Windows;
					break;
				default:
					controllerType = ControllerType.Unknown;
					break;
				}
				break;
			case Platform.MacOS:
				switch (controllerName)
				{
				case "":
				case "PERFORMANCE DESIGNED PRODUCTS AFTERGLOW GAMEPAD FOR XBOX 360":
				case "MICROSOFT WIRELESS 360 CONTROLLER":
					controllerType = ControllerType.TattieBogle_OSX;
					break;
				case "SONY PLAYSTATION(R)3 CONTROLLER":
				case "PLAYSTATION(R)3 CONTROLLER":
					controllerType = ControllerType.PS3_OSX;
					break;
				case "AFTERGLOW WIRED USB XBOX360 CONTROLLER":
				case "BERWAY TECH LTD GAMESTICK CONTROLLER":
					controllerType = ControllerType.GameStick_OSX;
					break;
				default:
					controllerType = ControllerType.Unknown;
					break;
				}
				break;
			case Platform.iOS:
				controllerType = ControllerType.Unknown;
				break;
			}
			if (controllerType == ControllerType.Unknown)
			{
				if (controllerName.Contains("NVIDIA CORPORATION NVIDIA CONTROLLER V01"))
				{
					controllerType = ControllerType.Shield_Android;
				}
				else if (controllerName.Contains("NVIDIA CONTROLLER") && platform == Platform.Windows)
				{
					controllerType = ControllerType.ShieldController_Windows;
				}
			}
			return controllerType;
		}
	}
}
