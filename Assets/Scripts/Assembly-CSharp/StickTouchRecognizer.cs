using Calamity.Input;
using UnityEngine;

public class StickTouchRecognizer : MonoBehaviour
{
	private const int NO_TOUCH_ID = int.MinValue;

	private const float RADIUS_TWEAK = 0.8f;

	public GameObject LeftStick;

	public GameObject RightStick;

	public UIPanel ParentPanel;

	public GameObject LeftButtons;

	public GameObject RightButtons;

	public ButtonBarDrag ButtonBar;

	public float ButtonOffset = 0.2f;

	public float RadiusScreenOffset = 0.8f;

	private int activeTouches;

	private bool enableSticks = true;

	private float posMultiplier;

	private TouchStickType stickType;

	private VirtualJoystick leftJoystick;

	private VirtualJoystick rightJoystick;

	private int leftStickTouchID = int.MinValue;

	private int rightStickTouchID = int.MinValue;

	private DoubleTapInterval leftStickDoubleTap;

	private DoubleTapInterval rightStickDoubleTap;

	private static StickTouchRecognizer instance;

	public bool IsSticksEnabled
	{
		get
		{
			return enableSticks;
		}
	}

	public bool IsRightSideTouch
	{
		get
		{
			return rightStickTouchID != int.MinValue;
		}
	}

	public bool IsLeftSideTouch
	{
		get
		{
			return leftStickTouchID != int.MinValue;
		}
	}

	public static StickTouchRecognizer Instance
	{
		get
		{
			return instance;
		}
	}

	public void ToggleSticksEnabled(bool enabled)
	{
		enableSticks = enabled;
		if (!enabled && (IsRightSideTouch || IsLeftSideTouch))
		{
			HideAllSticks();
		}
	}

	private void Awake()
	{
		instance = this;
		leftJoystick = LeftStick.GetComponent<VirtualJoystick>();
		rightJoystick = RightStick.GetComponent<VirtualJoystick>();
		leftStickDoubleTap = LeftStick.GetComponent<DoubleTapInterval>();
		rightStickDoubleTap = RightStick.GetComponent<DoubleTapInterval>();
		stickType = OptionManager.Instance.StickType;
		if (stickType == TouchStickType.Locked)
		{
			if (leftStickDoubleTap != null)
			{
				leftStickDoubleTap.EnablingCountsAsPress = false;
				leftStickDoubleTap.enabled = true;
			}
			if (rightStickDoubleTap != null)
			{
				rightStickDoubleTap.EnablingCountsAsPress = false;
				rightStickDoubleTap.enabled = true;
			}
		}
	}

	private void Start()
	{
		if (InputManager.Instance.StickType == TouchStickType.Dynamic)
		{
			UICamera.fallThrough = base.gameObject;
		}
		posMultiplier = (float)UIRoot.list[0].activeHeight / (float)Screen.height;
	}

	private void Update()
	{
		if (GuiInGameManager.Instance != null && (GuiInGameManager.Instance.IsShopOpen() || GuiInGameManager.Instance.IsEndScreenVisible || GuiInGameManager.Instance.IsGamePaused) && activeTouches > 0)
		{
			if (leftStickTouchID != int.MinValue)
			{
				DisableStick(Stick.Left);
			}
			if (rightStickTouchID != int.MinValue)
			{
				DisableStick(Stick.Right);
			}
		}
	}

	private void OnDestroy()
	{
		UICamera.fallThrough = null;
	}

	private bool IsInsideScreen(Vector2 position, float radius)
	{
		float num = (float)UIRoot.list[0].activeHeight / (float)Screen.height;
		float num2 = UIRoot.list[0].activeHeight;
		float num3 = (float)Screen.width * num;
		position *= num;
		if (position.x - radius < 0f)
		{
			return false;
		}
		if (position.y - radius < 0f)
		{
			return false;
		}
		if (position.x + radius > num3)
		{
			return false;
		}
		if (position.y + radius > num2)
		{
			return false;
		}
		return true;
	}

	public void EnableStick(Stick stick)
	{
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		VirtualJoystick virtualJoystick = null;
		DoubleTapInterval doubleTapInterval = null;
		Vector2 vector = UICamera.lastTouchPosition * posMultiplier;
		switch (stick)
		{
		case Stick.Left:
			gameObject = LeftStick;
			gameObject2 = LeftButtons;
			virtualJoystick = leftJoystick;
			doubleTapInterval = leftStickDoubleTap;
			if (!IsInsideScreen(UICamera.lastTouchPosition, virtualJoystick.maxDragLength * RadiusScreenOffset))
			{
				return;
			}
			leftStickTouchID = UICamera.currentTouchID;
			break;
		case Stick.Right:
			gameObject = RightStick;
			gameObject2 = RightButtons;
			virtualJoystick = rightJoystick;
			doubleTapInterval = rightStickDoubleTap;
			if (!IsInsideScreen(UICamera.lastTouchPosition, virtualJoystick.maxDragLength * RadiusScreenOffset))
			{
				return;
			}
			rightStickTouchID = UICamera.currentTouchID;
			break;
		default:
			Debug.LogError("[StickTouchRecognizer:EnableStick() got an invalid stick: " + stick);
			return;
		}
		Transform transform = gameObject.transform;
		Transform transform2 = gameObject2.transform;
		if (stickType == TouchStickType.Locked)
		{
			Vector3 staticStickOffset = (Vector2)transform.localPosition - vector;
			virtualJoystick.SetStaticStickOffset(staticStickOffset);
		}
		transform.localPosition = vector;
		transform2.localPosition = vector;
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}
		if (!gameObject2.activeSelf)
		{
			gameObject2.gameObject.SetActive(true);
		}
		if (doubleTapInterval != null && stickType == TouchStickType.Dynamic)
		{
			doubleTapInterval.Enable();
		}
		virtualJoystick.touchID = UICamera.currentTouchID;
		virtualJoystick.EnableStick();
		activeTouches++;
	}

	public void DisableStick(Stick stick)
	{
		switch (stick)
		{
		case Stick.Left:
			leftJoystick.DisableStick();
			leftStickTouchID = int.MinValue;
			break;
		case Stick.Right:
			rightJoystick.DisableStick();
			rightStickTouchID = int.MinValue;
			break;
		}
		activeTouches--;
	}

	public void HideAllSticks()
	{
		DisableStick(Stick.Left);
		DisableStick(Stick.Right);
		leftJoystick.HideJoystick();
		rightJoystick.HideJoystick();
		LeftButtons.gameObject.SetActive(false);
		RightButtons.gameObject.SetActive(false);
	}

	public void ShowAllSticks()
	{
		leftJoystick.ShowJoystick();
		rightJoystick.ShowJoystick();
	}

	public void SetJoystickPosition(Stick stick, Vector3 newPosition)
	{
		VirtualJoystick virtualJoystick = ((stick != 0) ? rightJoystick : leftJoystick);
		virtualJoystick.SetStickPosition(newPosition);
	}

	public void OnPress(bool isPressed)
	{
		if (isPressed)
		{
			if (DeviceSwitchController.CurrentDeviceMode != DeviceMode.Touch || GuiInGameManager.Instance.IsShopOpen() || GuiInGameManager.Instance.IsEndScreenVisible || GuiInGameManager.Instance.IsGamePaused || !enableSticks || activeTouches >= 2)
			{
				return;
			}
			if (UICamera.lastTouchPosition.x < (float)(Screen.width / 2))
			{
				if (leftStickTouchID == int.MinValue)
				{
					EnableStick(Stick.Left);
				}
			}
			else if (rightStickTouchID == int.MinValue)
			{
				EnableStick(Stick.Right);
			}
		}
		else if (UICamera.currentTouchID == leftStickTouchID)
		{
			DisableStick(Stick.Left);
		}
		else if (UICamera.currentTouchID == rightStickTouchID)
		{
			DisableStick(Stick.Right);
		}
	}

	public void OnClick()
	{
	}
}
