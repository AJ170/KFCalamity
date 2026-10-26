using System;
using UnityEngine;

public class GuiButton : MonoBehaviour
{
	public UILabel Label;

	public UISprite Sprite;

	public Color LabelColor = Color.white;

	public string SelectSpriteName = "scoreboard_button_white";

	public string DeselectSpriteName = "scoreboard_button_black";

	private ButtonActions callbacks;

	public object CustomButtonData
	{
		get
		{
			return (!(callbacks == null)) ? callbacks.CustomData : null;
		}
		set
		{
			if (callbacks != null)
			{
				callbacks.CustomData = value;
			}
			else
			{
				Debug.LogError("Tried to set custom button data on a button that has no ButtonActions component", base.gameObject);
			}
		}
	}

	private void Awake()
	{
		callbacks = GetComponent<ButtonActions>();
		if (callbacks == null)
		{
			callbacks = GetComponentInChildren<ButtonActions>();
		}
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
		{
			Sprite.color = Color.black;
			LabelColor = Color.white;
		}
	}

	private void Start()
	{
	}

	public void SetOnClickCallback(Action<ButtonActions> callback)
	{
		if (callbacks != null)
		{
			callbacks.SetOnClickCallback(callback);
		}
		else
		{
			Debug.LogError("Tried to set click callback on button that has no ButtonActions component", base.gameObject);
		}
	}

	public void SetOnReleaseCallback(Action<ButtonActions> callback)
	{
		if (callbacks != null)
		{
			callbacks.SetOnReleaseCallback(callback);
		}
		else
		{
			Debug.LogError("Tried to set release callback on button that has no ButtonActions component", base.gameObject);
		}
	}

	public void SetText(string text)
	{
		Label.text = text;
		Label.color = LabelColor;
	}

	public void OnSelect()
	{
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
		{
			Sprite.spriteName = SelectSpriteName;
			Label.color = Color.black;
			Sprite.color = Color.white;
		}
	}

	public void OnDeselect()
	{
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
		{
			Sprite.spriteName = DeselectSpriteName;
			Label.color = Color.white;
			Sprite.color = Color.black;
		}
	}
}
