using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class GuiStartUpManager : MonoBehaviour
{
	[Serializable]
	public class SplashScreen
	{
		public string SplashName;

		public float Duration;

		public bool finished;

		public bool isMovie;

		public Vector2 ImageOffset;
	}

	public List<SplashScreen> SplashList;

	public GameObject StartScreen;

	public GameObject Splash;

	public UILabel startLabel;

	public UISprite AButtonSprite;

	public GameObject MovieObject;

	private UISprite splashSprite;

	private TweenAlpha splashTween;

	private StartUpState startState;

	private bool inStartScreen;

	private bool playingMovie = true;

	private int frameCount;

	private bool prevKeyDown;

	private void Start()
	{
		Resources.UnloadUnusedAssets();
		if (Splash != null)
		{
			splashSprite = Splash.GetComponent<UISprite>();
			splashTween = Splash.GetComponent<TweenAlpha>();
			Splash.SetActive(false);
		}
		DeviceSwitchController.SubscribeToSwitch(this, OnDeviceSwitch);
		StartSplashScreens();
	}

	private void OnDeviceSwitch(DeviceSwitchController.SwitchArgs args)
	{
		SetStartLabelText();
	}

	private void OnDestroy()
	{
		DeviceSwitchController.UnsubscribeToSwitch(this);
	}

	private void Update()
	{
		HandleButtonPressed();
		if (!playingMovie)
		{
			return;
		}
		frameCount++;
		if (frameCount == 10)
		{
			frameCount = 0;
			if (!OnSplashEnded())
			{
				StartScreenOpen();
			}
		}
	}

	private void HandleButtonPressed()
	{
		if (!InputManager.GetBool(InputAction.SkipSplashScreen))
		{
			return;
		}
		if (!inStartScreen)
		{
			if (!OnSplashEnded())
			{
				StartScreenOpen();
			}
		}
		else
		{
			LoadNextState();
		}
	}

	private void SetStartLabelText()
	{
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
		{
			startLabel.text = Localizer.GetString("gui_press_controller_to_start");
			AButtonSprite.enabled = true;
		}
		else if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch)
		{
			startLabel.text = Localizer.GetString("gui_tap_to_start");
			AButtonSprite.enabled = false;
		}
	}

	private void StartScreenOpen()
	{
		Splash.SetActive(false);
		inStartScreen = true;
		SetStartLabelText();
		StartScreen.SetActive(true);
	}

	private void PlayMovie()
	{
		PlayFullScreenMovie(SplashList[0].SplashName);
		playingMovie = true;
	}

	// (PS Vita port) Handheld exists only in the mobile players. It blocks until the movie ends, and Update moves on to
	// the next splash 10 frames after playingMovie is set, so without it the movie splash is simply skipped (the intro
	// movie is not in the export either).
	private static void PlayFullScreenMovie(string path)
	{
#if UNITY_ANDROID || UNITY_IOS
		Handheld.PlayFullScreenMovie(path, Color.black, FullScreenMovieControlMode.CancelOnInput);
#endif
	}

	private void StartSplashScreens()
	{
		if (SplashList.Count > 0)
		{
			if (SplashList[0].isMovie)
			{
				PlayMovie();
			}
			SplashList[0].finished = true;
		}
	}

	public bool OnSplashEnded()
	{
		if (splashSprite == null && Splash != null)
		{
			splashSprite = Splash.GetComponent<UISprite>();
			splashTween = Splash.GetComponent<TweenAlpha>();
			Splash.SetActive(false);
		}
		playingMovie = false;
		foreach (SplashScreen splash in SplashList)
		{
			if (!splash.finished)
			{
				if (splash.isMovie)
				{
					PlayMovie();
				}
				else
				{
					splashSprite.spriteName = splash.SplashName;
					splashSprite.MakePixelPerfect();
					Splash.SetActive(true);
					splashTween.Reset();
					splashTween.duration = splash.Duration;
					Splash.transform.localPosition = splash.ImageOffset;
					splashTween.Play(true);
				}
				splash.finished = true;
				return true;
			}
		}
		StartScreenOpen();
		return false;
	}

	private void OnSplashCancelled()
	{
		playingMovie = false;
		bool flag = true;
		foreach (SplashScreen splash in SplashList)
		{
			if (!splash.finished)
			{
				if (splash.isMovie)
				{
					PlayFullScreenMovie(splash.SplashName);
					playingMovie = true;
				}
				else
				{
					splashSprite.spriteName = splash.SplashName;
					splashSprite.MakePixelPerfect();
					Splash.SetActive(true);
					splashTween.Reset();
					splashTween.duration = splash.Duration;
					splashTween.Play(true);
				}
				splash.finished = true;
				flag = false;
				break;
			}
		}
		if (flag)
		{
			StartScreenOpen();
		}
	}

	public void LoadNextState()
	{
		base.gameObject.SetActive(false);
		startState.OnMenuStart();
	}

	public void Setup(StartUpState startup)
	{
		startState = startup;
	}
}
