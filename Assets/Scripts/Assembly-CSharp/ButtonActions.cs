using System;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{
	private List<Action<ButtonActions>> onClick = new List<Action<ButtonActions>>();

	private List<Action<ButtonActions>> onRelease = new List<Action<ButtonActions>>();

	public object CustomData { get; set; }

	public void SetOnClickCallback(Action<ButtonActions> onClickCallback)
	{
		onClick.Add(onClickCallback);
	}

	public void RemoveOnClickCallback()
	{
		onClick = null;
	}

	public void SetOnReleaseCallback(Action<ButtonActions> onReleaseCallback)
	{
		onRelease.Add(onReleaseCallback);
	}

	public void RemoveOnReleaseCallback()
	{
		onRelease = null;
	}

	public void OnDestroy()
	{
		onClick = null;
		onRelease = null;
	}

	public void OnPress(bool isPressed)
	{
		if (!base.enabled)
		{
			return;
		}
		if (isPressed)
		{
			if (onClick == null)
			{
				return;
			}
			{
				foreach (Action<ButtonActions> item in onClick)
				{
					item(this);
				}
				return;
			}
		}
		if (onRelease == null)
		{
			return;
		}
		foreach (Action<ButtonActions> item2 in onRelease)
		{
			item2(this);
		}
	}
}
