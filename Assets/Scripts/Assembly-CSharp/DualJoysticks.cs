using UnityEngine;

public class DualJoysticks : MonoBehaviour
{
	public Vector2 JoystickOffset = Vector2.zero;

	private GameObject leftStick;

	private GameObject rightStick;

	public Joystick LeftJoystick { get; set; }

	public Joystick RightJoystick { get; set; }

	private void Start()
	{
		leftStick = base.transform.FindChild("LeftJoystick").gameObject;
		rightStick = base.transform.FindChild("RightJoystick").gameObject;
		leftStick.transform.position = Vector3.zero;
		rightStick.transform.position = Vector3.zero;
		LeftJoystick = leftStick.GetComponent<Joystick>();
		RightJoystick = rightStick.GetComponent<Joystick>();
		GUITexture gUITexture = leftStick.GetComponent<GUITexture>();
		gUITexture.pixelInset = new Rect((float)Screen.width * JoystickOffset.x, (float)Screen.height * JoystickOffset.y, gUITexture.pixelInset.width, gUITexture.pixelInset.height);
		GUITexture gUITexture2 = rightStick.GetComponent<GUITexture>();
		gUITexture2.pixelInset = new Rect((float)Screen.width - (float)Screen.width * JoystickOffset.x - gUITexture2.pixelInset.width, (float)Screen.height * JoystickOffset.y, gUITexture2.pixelInset.width, gUITexture2.pixelInset.height);
	}

	private void Update()
	{
	}
}
