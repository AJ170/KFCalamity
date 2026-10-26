using UnityEngine;

public abstract class GuiMenuBase
{
	public GUISkin mySkin;

	protected GuiMenuManager MenuManager;

	public GuiMenuBase(GuiMenuManager menu, GUISkin skin)
	{
		mySkin = skin;
		MenuManager = menu;
	}

	public virtual void Display()
	{
	}

	public virtual void Update()
	{
	}

	public virtual bool CheckAxis()
	{
		return false;
	}

	public virtual void CheckButtons()
	{
	}
}
