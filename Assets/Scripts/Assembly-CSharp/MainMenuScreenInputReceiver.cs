using System;
using UnityEngine;

public class MainMenuScreenInputReceiver : MonoBehaviour
{
	private Action onPressedCallback;

	public void SetOnPressedCallback(Action onPressedCallback)
	{
		this.onPressedCallback = onPressedCallback;
	}

	public void RemoveOnPressedCallback()
	{
		onPressedCallback = null;
	}

	public void OnPress(bool isDown)
	{
		if (isDown && onPressedCallback != null)
		{
			onPressedCallback();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
