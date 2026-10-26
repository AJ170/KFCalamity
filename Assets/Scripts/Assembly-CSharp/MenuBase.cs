using UnityEngine;

public class MenuBase : MonoBehaviour
{
	protected GuiMenuManager manager;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public virtual void TweenButton(GameObject button, Vector3 position, TweenDirection direction, int tweenDistance, float toAlpha, float tweenSpeed)
	{
	}

	public virtual void SetManagerReference(GuiMenuManager manager)
	{
		this.manager = manager;
	}

	public virtual void HandleHorizontalAxisInput(int axis)
	{
	}

	public virtual void HandleVerticalAxisInput(int axis)
	{
	}

	public virtual void HandleButtonForwardPressed()
	{
	}

	public virtual void HandleButtonBackwardPressed()
	{
	}

	public virtual MenuStates OnButtonPressed()
	{
		return MenuStates.Main;
	}

	public virtual void OnTweenComplete()
	{
	}
}
