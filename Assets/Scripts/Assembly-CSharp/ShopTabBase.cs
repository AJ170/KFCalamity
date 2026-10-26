using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public abstract class ShopTabBase<T> : IShopTab where T : ShopTabBase<T>.TabButton
{
	public abstract class TabButton
	{
		public string Name;

		public Perk Perk;

		public GUIStyle Style;

		public bool Selected;

		public bool Disabled;

		public bool Locked;

		public Rect Rect;
	}

	protected Color backupColor;

	protected GUISkin skin;

	protected GUIStyle mainTitleStyle;

	protected GUIStyle subTitleStyle;

	protected string title;

	protected Rect tabRect;

	protected Rect titleRect;

	protected bool visible;

	protected bool enabled;

	protected bool shouldTint;

	protected PlayerShopWindow shop;

	protected int tintIndex;

	protected int inputID;

	private int currentSelected;

	protected List<T> buttons = new List<T>();

	protected bool buttonForwardEnabled = true;

	protected bool buttonBackwardEnabled = true;

	protected int CurrentSelected
	{
		get
		{
			return currentSelected;
		}
		set
		{
			SetSelectedButtonByIndex(value);
		}
	}

	public Rect TabRect
	{
		get
		{
			return tabRect;
		}
	}

	public ShopTabBase(PlayerShopWindow shop, Rect tabRect, GUISkin skin, int inputID)
	{
		this.tabRect = tabRect;
		this.shop = shop;
		this.skin = skin;
		mainTitleStyle = new GUIStyle(skin.label);
		mainTitleStyle.alignment = TextAnchor.MiddleLeft;
		subTitleStyle = new GUIStyle(skin.label);
		subTitleStyle.alignment = TextAnchor.MiddleLeft;
		this.inputID = inputID;
		visible = true;
	}

	public virtual void Update()
	{
		if (!enabled)
		{
		}
	}

	public virtual void Display()
	{
		GUI.color = backupColor;
	}

	public virtual void SetInitialSelection()
	{
		SetSelectedButtonByIndex(0);
	}

	public virtual void RemoveSelection()
	{
		foreach (T button in buttons)
		{
			button.Selected = false;
		}
		SetSelectedButtonByIndex(0);
	}

	public virtual void ResetUnableTint()
	{
	}

	protected void SetSelectedButtonByIndex(int index)
	{
		if (index >= buttons.Count)
		{
			Debug.LogError("Tried to set selection to an invalid index: " + index + " for ShopTab: " + GetType().Name);
		}
		else if (buttons.Count > 1)
		{
			if (currentSelected > -1 && currentSelected < buttons.Count)
			{
				buttons[currentSelected].Selected = false;
			}
			currentSelected = index;
			if (currentSelected > -1)
			{
				buttons[currentSelected].Selected = true;
			}
		}
		else if (buttons.Count == 1)
		{
			currentSelected = index;
			buttons[currentSelected].Selected = true;
		}
	}

	public virtual void SetSelectedButton(int direction)
	{
		if (!enabled || buttons.Count <= 1)
		{
			return;
		}
		ResetUnableTint();
		if (currentSelected > -1 && currentSelected < buttons.Count)
		{
			buttons[currentSelected].Selected = false;
		}
		if (direction == 0)
		{
			buttons[0].Selected = true;
			return;
		}
		currentSelected -= direction;
		if (currentSelected < 0)
		{
			currentSelected = buttons.Count - 1;
		}
		else if (currentSelected >= buttons.Count)
		{
			currentSelected = 0;
		}
		buttons[currentSelected].Selected = true;
	}

	public void SetTabPosX(float x)
	{
		tabRect.x = x;
	}

	public virtual void HandleButtonPressed()
	{
		if (enabled)
		{
			if (buttonForwardEnabled && InputManager.GetBool(InputAction.MenuForward, inputID))
			{
				OnForwardPressed();
			}
			if (buttonBackwardEnabled && InputManager.GetBool(InputAction.MenuBackward, inputID))
			{
				OnBackwardPressed();
			}
		}
	}

	protected virtual void ButtonPressed(T button)
	{
	}

	public virtual void HandleTouch(Vector2 location)
	{
		int num = 0;
		foreach (T button in buttons)
		{
			Rect rect = button.Rect;
			rect.x += shop.tabRect.x;
			if (rect.Contains(location))
			{
				SetSelectedButtonByIndex(num);
				ButtonPressed(button);
				break;
			}
			num++;
		}
	}

	public virtual void SetSizeBasedOnResolution()
	{
	}

	public virtual void OnForwardPressed()
	{
	}

	public virtual void OnBackwardPressed()
	{
	}

	public virtual void Start()
	{
		enabled = true;
	}

	public virtual void Stop()
	{
		enabled = false;
	}
}
