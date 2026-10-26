using UnityEngine;

public class TouchscreenControls
{
	private const string joystickResourcePath = "Prefabs/Misc/Joystick";

	private static TouchscreenControls instance;

	private GameObject leftGameObject;

	private GameObject rightGameObject;

	private Joystick leftJoystick;

	private Joystick rightJoystick;

	public static bool Instantiated
	{
		get
		{
			return instance != null;
		}
	}

	public static GameObject LeftGameObject
	{
		get
		{
			if (Instantiated)
			{
				return instance.leftGameObject;
			}
			return null;
		}
	}

	public static GameObject RightGameObject
	{
		get
		{
			if (Instantiated)
			{
				return instance.rightGameObject;
			}
			return null;
		}
	}

	public static Joystick LeftJoystick
	{
		get
		{
			if (Instantiated)
			{
				return instance.leftJoystick;
			}
			return null;
		}
	}

	public static Joystick RightJoystick
	{
		get
		{
			if (Instantiated)
			{
				return instance.rightJoystick;
			}
			return null;
		}
	}

	private TouchscreenControls()
	{
		Object original = Resources.Load("Prefabs/Misc/Joystick");
		leftGameObject = (GameObject)Object.Instantiate(original);
		leftGameObject.name = "Left stick";
		leftJoystick = leftGameObject.GetComponent<Joystick>();
		Object.DontDestroyOnLoad(leftGameObject);
		rightGameObject = (GameObject)Object.Instantiate(original);
		rightGameObject.name = "Right stick";
		rightJoystick = rightGameObject.GetComponent<Joystick>();
		GUITexture component = rightGameObject.GetComponent<GUITexture>();
		Rect pixelInset = component.pixelInset;
		component.pixelInset = new Rect((float)Screen.width - pixelInset.x - pixelInset.width, pixelInset.y, pixelInset.width, pixelInset.height);
		Object.DontDestroyOnLoad(rightGameObject);
	}

	public static void Instantiate()
	{
		if (!Instantiated)
		{
			instance = new TouchscreenControls();
		}
	}

	public static void Destroy()
	{
		if (Instantiated)
		{
			instance.DestroyObjects();
			instance = null;
		}
	}

	private void DestroyObjects()
	{
		Object.Destroy(leftGameObject);
		Object.Destroy(rightGameObject);
	}
}
