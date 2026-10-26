using Calamity.Input;
using UnityEngine;

public class ForceInputControl : MonoBehaviour
{
	public ControllerType Type = ControllerType.Keyboard;

	public InputPlayer ID;

	public InputMode Mode;

	private InputControl control;

	private void Start()
	{
		SoldierInputController component = GetComponent<SoldierInputController>();
		if (component != null)
		{
			control = InputControl.FromType(ID, Type, Mode);
			control.OnLevelLoaded();
			control.InitializeIngame();
			control.UpdateGameObject(base.gameObject);
			component.SetControl(control);
		}
	}

	private void Update()
	{
		if (control != null)
		{
			control.Update();
		}
	}
}
