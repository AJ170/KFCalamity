using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class TutorialMessage : MonoBehaviour
{
	public class SenderMessage
	{
		public bool FixedDuration;

		public float Duration;

		public string Message;
	}

	public UIAnchor Anchor;

	public TweenAlpha[] Alphas;

	public UILabel Label;

	public UILabel PressLabel;

	public UISprite Portrait;

	public UISprite TextField;

	public UISprite Obutton;

	private TimePrint printTimer = new TimePrint();

	private string message = string.Empty;

	private bool blendingDone;

	private bool waitingForMessage = true;

	private bool messageEntered;

	private SenderMessage mSend;

	private List<Action> onAlphaCompleteCallbacks = new List<Action>();

	private List<Action> onMessageDestroyCallbacks = new List<Action>();

	private int playerInputID;

	private bool hasInputID;

	private float timeSinceCompleted;

	private bool requestClose;

	private float TIME_BEFORE_AUTO_CLOSE = 10f;

	private void Start()
	{
	}

	public void SetInputID(int id)
	{
		playerInputID = id;
		hasInputID = true;
	}

	public void AddAlphaCompleteListener(Action callback)
	{
		onAlphaCompleteCallbacks.Add(callback);
	}

	public void AddMessageDestroyListener(Action callback)
	{
		onMessageDestroyCallbacks.Add(callback);
	}

	private void RemoveAlphaCompleteListener(Action callback)
	{
		onAlphaCompleteCallbacks.Remove(callback);
	}

	private void RemoveMessageDestroyListener(Action callback)
	{
		onMessageDestroyCallbacks.Remove(callback);
	}

	private void callAlphaCompleteCallbacks()
	{
		foreach (Action onAlphaCompleteCallback in onAlphaCompleteCallbacks)
		{
			onAlphaCompleteCallback();
		}
	}

	private void callMessageDestroyCallbacks()
	{
		foreach (Action onMessageDestroyCallback in onMessageDestroyCallbacks)
		{
			onMessageDestroyCallback();
		}
	}

	public void SetMessage(MessageSender sender, string message, float duration, bool fixedDuration)
	{
		initializeSenderSettings(sender);
		mSend = new SenderMessage();
		mSend.Duration = duration;
		mSend.FixedDuration = fixedDuration;
		mSend.Message = message;
	}

	private void EnterMessage(string message, float duration, bool fixedDuration)
	{
		if (printTimer == null)
		{
			printTimer = new TimePrint(message, duration, fixedDuration);
		}
		else
		{
			printTimer.AddMessage(message, duration, fixedDuration);
		}
	}

	private void initializeSenderSettings(MessageSender sender)
	{
		switch (sender)
		{
		case MessageSender.Player:
			Anchor.side = UIAnchor.Side.BottomLeft;
			Anchor.relativeOffset.Set(0.02f, 0.08f);
			Portrait.pivot = UIWidget.Pivot.BottomLeft;
			Portrait.transform.localPosition = Vector3.zero;
			Portrait.spriteName = "PlayerPortrait";
			TextField.pivot = UIWidget.Pivot.BottomLeft;
			TextField.transform.localPosition = new Vector3(Portrait.transform.localScale.x, 0f, 0f);
			Label.transform.localPosition = new Vector3(Portrait.transform.localScale.x + 10f, TextField.transform.localScale.y - 6f, 0f);
			PressLabel.pivot = UIWidget.Pivot.Left;
			PressLabel.transform.localPosition = new Vector3(169f, 18f, 0f);
			Obutton.pivot = UIWidget.Pivot.Left;
			Obutton.transform.localPosition = new Vector3(114f, 18f, 0f);
			break;
		}
		Label.lineWidth = 520;
		Label.maxLineCount = 5;
	}

	private void Update()
	{
		HandleInput();
		if (printTimer != null)
		{
			printTimer.Update();
			message = printTimer.GetCurrentMessage();
			if (messageEntered)
			{
				if (!waitingForMessage)
				{
					string empty = string.Empty;
					empty = ((DeviceSwitchController.CurrentDeviceMode != DeviceMode.Touch) ? "gui_close" : "gui_tap_to_close");
					PressLabel.text = Localizer.GetString(empty);
				}
				waitingForMessage = printTimer.isTyping();
			}
			if (!waitingForMessage && !requestClose)
			{
				timeSinceCompleted += Time.deltaTime;
				if (timeSinceCompleted > TIME_BEFORE_AUTO_CLOSE)
				{
					requestClose = true;
				}
			}
		}
		SetDisplayText();
	}

	public void SetDisplayText()
	{
		Label.text = message;
	}

	private void HandleInput()
	{
		bool flag = false;
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.MouseKeyboard || DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
		{
			flag = ((!hasInputID) ? InputManager.GetBool(InputAction.MenuBackward) : InputManager.GetBool(InputAction.MenuBackward, playerInputID));
		}
		else if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch)
		{
			flag = InputManager.GetBool(InputAction.SkipSplashScreen);
		}
		if (flag || requestClose)
		{
			if (waitingForMessage)
			{
				printTimer.Stop();
				PressLabel.text = Localizer.GetString("gui_close");
			}
			else
			{
				OnMessageDestroy();
			}
		}
	}

	private void OnMessageDestroy()
	{
		callMessageDestroyCallbacks();
		requestClose = false;
		timeSinceCompleted = 0f;
		printTimer = null;
		TweenAlpha[] alphas = Alphas;
		foreach (TweenAlpha tweenAlpha in alphas)
		{
			tweenAlpha.Play(false);
		}
	}

	public void OnAlphaComplete()
	{
		if (blendingDone)
		{
			callAlphaCompleteCallbacks();
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			EnterMessage(mSend.Message, mSend.Duration, mSend.FixedDuration);
			blendingDone = true;
			messageEntered = true;
		}
	}

	private void OnDestroy()
	{
		while (onAlphaCompleteCallbacks.Count > 0)
		{
			onAlphaCompleteCallbacks.RemoveAt(0);
		}
		while (onMessageDestroyCallbacks.Count > 0)
		{
			onMessageDestroyCallbacks.RemoveAt(0);
		}
	}
}
