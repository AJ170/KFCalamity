using UnityEngine;

public class DemoTwo : MonoBehaviour
{
	private VirtualControls _controls;

	private void Start()
	{
		_controls = new VirtualControls();
		_controls.createDebugQuads();
	}

	private void Update()
	{
		_controls.update();
	}

	private void OnGUI()
	{
		showLabelAndValue("Left: ", _controls.leftDown.ToString());
		showLabelAndValue("Right: ", _controls.rightDown.ToString());
		showLabelAndValue("Up: ", _controls.upDown.ToString());
		GUILayout.Space(4f);
		showLabelAndValue("Attack: ", _controls.attackDown.ToString());
		showLabelAndValue("Jump: ", _controls.jumpDown.ToString());
	}

	private void showLabelAndValue(string label, string value)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.Width(50f));
		GUILayout.Label(value);
		GUILayout.EndHorizontal();
	}
}
