using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class MenuMain : MenuBase
{
	[Serializable]
	public class button
	{
		public string GuiName;

		public MenuStates StateOnPress;

		public Material ScreenMaterial;
	}

	public UILabel PurchaseButtonText;

	public UILabel BracketsLabel;

	public UIPanel MenuPanel;

	public TweenPosition PositionTween;

	public TweenPosition DemoTween;

	public TweenPosition PurchaseButtonTween;

	public TweenPosition InfoPositionTween;

	public GuiList ButtonList;

	public List<button> Data;

	public Material NoiseMaterial;

	public GameObject TutorialWindowPopup;

	public GameObject StickTypePopup;

	public float BracketSpacing = 1.4f;

	public string MonitorObjectName;

	public bool HideTutorial;

	private PopupWindow tutorialPopup;

	private PopupWindow stickTypePopup;

	private List<GuiList.ListButton> buttons = new List<GuiList.ListButton>();

	private bool handleInput;

	private bool isDemo = true;

	private bool hasEntered;

	private bool spawnTutorialPopup;

	private bool tutorialPopupConfirmed;

	private bool anyPopupSpawned;

	private OptionManager optionManager;

	private bool temp_weapons_unlocked;

	private bool temp_perks_unlocked;

	private static int selectedIndex = -1;

	private int cheatInputIndex;

	private AxisTouchButtonsWrapper horizontalButtonsWrapper;

	private void Start()
	{
		if (HideTutorial)
		{
			Data.RemoveAll((button a) => a.StateOnPress == MenuStates.Tutorial);
		}
		if (Data != null && Data.Count > 0)
		{
			PurchaseButtonText.text = " " + Localizer.GetString("gui_purchase_button");
			MetaManager instance = MetaManager.Instance;
			optionManager = OptionManager.Instance;
			if (optionManager.StickType == TouchStickType.Unknown)
			{
				optionManager.StickType = TouchStickType.Locked;
				optionManager.IsStickTypeTemporary = true;
			}
			if (instance != null)
			{
				if (optionManager != null && optionManager.TutorialPopup == 0)
				{
					if (instance.IsLevelCompleted("tutorial_scene", DifficultySetting.Normal))
					{
						selectedIndex = 0;
						optionManager.TutorialPopup = 1;
						optionManager.SetSaveData(optionManager.Serialize());
						spawnTutorialPopup = false;
					}
					else
					{
						selectedIndex = 0;
						spawnTutorialPopup = true;
					}
				}
				else
				{
					if (selectedIndex == -1)
					{
						selectedIndex = 0;
					}
					spawnTutorialPopup = false;
				}
			}
			else
			{
				selectedIndex = 0;
			}
			if (isDemo)
			{
				DemoTween.enabled = true;
				PurchaseButtonTween.enabled = true;
				PurchaseButtonTween.Play(true);
			}
			buttons = ButtonList.Create(2, Data.Count * 2 - 1, 0.33f);
			if (buttons != null)
			{
				SetButtonData(selectedIndex);
				ButtonList.AdjustButtonPositions();
				ButtonList.SetManagerReference(this);
				SetLevelListStartValues();
				OnTweenComplete();
			}
			else
			{
				Debug.Log("[MenuMain] - Start() buttons is null. selectedIndex == " + selectedIndex + " - Data.count == " + Data.Count);
			}
		}
		else
		{
			Debug.Log("[MenuMain] - Start() Data is null.");
			base.gameObject.SetActive(false);
		}
		if (PlayerManager.Instance.MaxPlayers == 2)
		{
			CheatManager instance2 = CheatManager.Instance;
			instance2.AddCheat("4PlayerUnlock", OnCheatEntered, new CheatDirectionEntry(CheatDirection.Up), new CheatDirectionEntry(CheatDirection.Up), new CheatDirectionEntry(CheatDirection.Down), new CheatDirectionEntry(CheatDirection.Down), new CheatDirectionEntry(CheatDirection.Left), new CheatDirectionEntry(CheatDirection.Right), new CheatButtonEntry(CheatButton.U), new CheatButtonEntry(CheatButton.A), new CheatButtonEntry(CheatButton.U), new CheatButtonEntry(CheatButton.A));
		}
		if (!string.IsNullOrEmpty(MonitorObjectName))
		{
			GameObject gameObject = GameObject.Find(MonitorObjectName);
			if ((bool)gameObject)
			{
				MainMenuScreenInputReceiver component = gameObject.GetComponent<MainMenuScreenInputReceiver>();
				if (component != null)
				{
					component.SetOnPressedCallback(OnMonitorScreenPressed);
				}
			}
		}
		UIAnchor[] componentsInChildren = GetComponentsInChildren<UIAnchor>();
		UIAnchor[] array = componentsInChildren;
		foreach (UIAnchor uIAnchor in array)
		{
			uIAnchor.uiCamera = manager.UICamera;
		}
		horizontalButtonsWrapper = new AxisTouchButtonsWrapper("button_left", "button_right");
		horizontalButtonsWrapper.InitializeIngame();
		UIStretch[] componentsInChildren2 = GetComponentsInChildren<UIStretch>();
		UIStretch[] array2 = componentsInChildren2;
		foreach (UIStretch uIStretch in array2)
		{
			uIStretch.uiCamera = manager.UICamera;
		}
	}

	private void OnDestroy()
	{
		CheatManager.Instance.Reset();
		if (string.IsNullOrEmpty(MonitorObjectName))
		{
			return;
		}
		GameObject gameObject = GameObject.Find(MonitorObjectName);
		if ((bool)gameObject)
		{
			MainMenuScreenInputReceiver component = gameObject.GetComponent<MainMenuScreenInputReceiver>();
			if (component != null)
			{
				component.RemoveOnPressedCallback();
			}
		}
	}

	private void OnCheatEntered(string cheatName)
	{
		Debug.Log("Cheat Entered!");
		PlayerManager.Instance.MaxPlayers = 4;
		manager.PlaySound(GuiMenuManager.Sound.CheatComplete);
	}

	private void Update()
	{
		if (tutorialPopup != null && tutorialPopup.Status != 0)
		{
			OnTutorialPopupPressed();
		}
		if (stickTypePopup != null && stickTypePopup.Status != 0)
		{
			OnStickTypePopupPressed();
		}
		HandleInput();
		horizontalButtonsWrapper.Update();
	}

	private void OnStickTypePopupPressed()
	{
		TouchStickType stickType = TouchStickType.Unknown;
		if (stickTypePopup.Status == PopupWindow.PopupState.Option1)
		{
			stickType = TouchStickType.Locked;
		}
		else if (stickTypePopup.Status == PopupWindow.PopupState.Option2)
		{
			stickType = TouchStickType.Dynamic;
			Debug.Log("Dynamic stick selected");
		}
		optionManager = OptionManager.Instance;
		optionManager.StickType = stickType;
		optionManager.IsStickTypeTemporary = false;
		optionManager.SetSaveData(optionManager.Serialize());
		optionManager.Save();
		anyPopupSpawned = false;
		UnityEngine.Object.Destroy(stickTypePopup.gameObject);
		stickTypePopup = null;
		TweenAlpha component = MenuPanel.GetComponent<TweenAlpha>();
		component.enabled = true;
		HandleButtonForwardPressed();
	}

	private void OnTutorialPopupPressed()
	{
		if (tutorialPopup.Status == PopupWindow.PopupState.Confirmed)
		{
			tutorialPopupConfirmed = true;
		}
		optionManager = OptionManager.Instance;
		optionManager.TutorialPopup = 1;
		optionManager.SetSaveData(optionManager.Serialize());
		optionManager.Save();
		anyPopupSpawned = false;
		UnityEngine.Object.Destroy(tutorialPopup.gameObject);
		tutorialPopup = null;
		if (!tutorialPopupConfirmed && DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch)
		{
			SpawnStickTypePopup();
		}
		else
		{
			HandleButtonForwardPressed();
		}
	}

	private void SpawnStickTypePopup()
	{
		if (StickTypePopup != null)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, StickTypePopup);
			stickTypePopup = gameObject.GetComponent<PopupWindow>();
			if (stickTypePopup != null)
			{
				stickTypePopup.SetButton1(Localizer.GetString("gui_popup_select"), PopupWindow.PopupState.Option1);
				stickTypePopup.SetButton2(Localizer.GetString("gui_popup_select"), PopupWindow.PopupState.Option2);
				stickTypePopup.SetMultiOptionPopupWindowInfo(Localizer.GetString("gui_popup_stick_type_header"), Localizer.GetString("gui_popup_stick_type_locked_header"), Localizer.GetString("gui_popup_stick_type_dynamic_header"), Localizer.GetString("gui_popup_stick_type_locked_message"), Localizer.GetString("gui_popup_stick_type_dynamic_message"));
				stickTypePopup.SetManagerReference(manager);
				anyPopupSpawned = true;
			}
			else
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}

	private void SpawnTutorialPopup()
	{
		if (TutorialWindowPopup != null)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, TutorialWindowPopup);
			tutorialPopup = gameObject.GetComponent<PopupWindow>();
			if (tutorialPopup != null)
			{
				tutorialPopup.SetButton1(Localizer.GetString("gui_popup_tutorial_accept"), PopupWindow.PopupState.Confirmed);
				tutorialPopup.SetButton2(Localizer.GetString("gui_popup_tutorial_deny"), PopupWindow.PopupState.Denied);
				tutorialPopup.SetPopupInfo(PopupType.Question, Localizer.GetString("gui_popup_tutorial_message"));
				tutorialPopup.SetManagerReference(manager);
				spawnTutorialPopup = false;
				anyPopupSpawned = true;
			}
			else
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}

	public void OnEnterComplete()
	{
		if (!hasEntered)
		{
			handleInput = true;
			hasEntered = true;
		}
		else if (tutorialPopupConfirmed)
		{
			manager.LoadState(MenuStates.Tutorial);
		}
		else
		{
			manager.LoadState(GetCurrentSelectedState());
		}
	}

	public void SetDemoLock(bool demo)
	{
		isDemo = demo;
	}

	public override void SetManagerReference(GuiMenuManager manager)
	{
		base.SetManagerReference(manager);
	}

	public void SetButtonData(int indexOffset)
	{
		int num = 0;
		int num2 = buttons.Count / 2;
		int num3 = indexOffset;
		int num4 = num2;
		for (int i = 0; i < buttons.Count; i++)
		{
			if (Data[num3] != null)
			{
				num = ((num != Data.Count - 1) ? (num + 1) : 0);
				if (!isDemo && Data[num3].StateOnPress == MenuStates.Purchase)
				{
					num = ((num != Data.Count - 1) ? (num + 1) : 0);
				}
				GuiListButtonMain component = buttons[num4].Object.GetComponent<GuiListButtonMain>();
				if (component != null)
				{
					component.CustomButtonData = num3;
					component.SetOnPressedCallback(HandleButtonPressed);
					component.SetText(Data[num3].GuiName);
					component.SetDepth(num4 + 1);
					component.SetButtonList(ButtonList);
				}
				else
				{
					Debug.Log("[MenuMain] - SetButtonData() button is null. writeIndex == " + num4 + " - buttons.count == " + buttons.Count);
				}
			}
			else
			{
				Debug.Log("[MenuMain] - SetButtonData() Data[readIndex] is null. readIndex == " + num3 + " - Data.count == " + Data.Count);
			}
			num3 = ++num3 % Data.Count;
			num4 = ++num4 % buttons.Count;
		}
	}

	private void SetLevelListStartValues()
	{
		List<GuiList.TweenValue> tweenValues = ButtonList.GetTweenValues();
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].Object.transform.localPosition = tweenValues[buttons[i].position].position;
			buttons[i].Object.GetComponent<GuiListButtonMain>().SetAlpha(tweenValues[buttons[i].position].Alpha);
		}
	}

	public override void TweenButton(GameObject button, Vector3 position, TweenDirection direction, int tweenDistance, float toAlpha, float tweenSpeed)
	{
		BracketsLabel.enabled = false;
		handleInput = false;
		if (toAlpha == 1f)
		{
			manager.PlaySound(GuiMenuManager.Sound.MenuButton);
		}
		button.GetComponent<GuiListButtonMain>().Tween(position, direction, tweenDistance, toAlpha, tweenSpeed);
		manager.ChangeScreenMaterial(NoiseMaterial);
		manager.SetScreenRandomUVEnableState(true);
	}

	private void HandleInput()
	{
		bool @bool = InputManager.GetBool(InputAction.MenuForward);
		bool bool2 = InputManager.GetBool(InputAction.MenuBackward);
		bool bool3 = InputManager.GetBool(InputAction.MenuY);
		int num = 0;
		int verticalAxis = 0;
		if (!anyPopupSpawned)
		{
			num = InputManager.GetInt(InputAction.MenuHorizontal, InputAction.MenuHorizontalAlternate);
			verticalAxis = InputManager.GetInt(InputAction.MenuVertical, InputAction.MenuVerticalAlternate);
		}
		if (num == 0)
		{
			num = horizontalButtonsWrapper.GetInt();
		}
		if (bool2 && anyPopupSpawned && isDemo)
		{
			UnityEngine.Object.Destroy(tutorialPopup.gameObject);
			spawnTutorialPopup = true;
			tutorialPopup = null;
			manager.PlaySound(GuiMenuManager.Sound.Backward);
		}
		if (!anyPopupSpawned)
		{
			if (@bool && handleInput)
			{
				HandleButtonForwardPressed();
				TweenOut();
			}
			if (handleInput)
			{
				HandleHorizontalAxisInput(num);
			}
			if (isDemo && bool3)
			{
				manager.OnPurchasePressed();
			}
		}
		CheatManager.Instance.HandleInput(num, verticalAxis);
	}

	public void TweenOutDemo()
	{
		PurchaseButtonTween.Play(false);
		DemoTween.Play(false);
	}

	public override void HandleHorizontalAxisInput(int axis)
	{
		if (axis != 0)
		{
			ButtonList.SetSelection(axis);
		}
	}

	private void HandleButtonPressed(ButtonActions sender)
	{
		if (!BuildRuntimeSettings.Current.SupportsTouch || !handleInput || anyPopupSpawned)
		{
			return;
		}
		MenuStates currentState = GetCurrentSelectedState();
		int num = (int)sender.CustomData;
		int num2 = Data.FindIndex((button a) => a.StateOnPress == currentState);
		int num3 = num;
		int num4 = num3 - num2;
		if (num4 != 0)
		{
			if (Mathf.Abs(num4) > Data.Count / 2)
			{
				num4 = ((num4 >= 0) ? (num3 - Data.Count - num2) : (Data.Count - Mathf.Abs(num4)));
			}
			ButtonList.SetSelection(num4);
		}
		else
		{
			SetState(currentState);
		}
	}

	private void SetState(MenuStates state)
	{
		if (spawnTutorialPopup && state == MenuStates.GameSetup)
		{
			manager.PlaySound(GuiMenuManager.Sound.Forward);
			SpawnTutorialPopup();
			return;
		}
		switch (state)
		{
		case MenuStates.UnlockWeapons:
			if (!temp_weapons_unlocked)
			{
				manager.PlaySound(GuiMenuManager.Sound.Forward);
				manager.LoadState(state);
			}
			return;
		case MenuStates.UnlockPerks:
			if (!temp_perks_unlocked)
			{
				manager.PlaySound(GuiMenuManager.Sound.Forward);
				manager.LoadState(state);
			}
			return;
		case MenuStates.Purchase:
			if (isDemo)
			{
				manager.PlaySound(GuiMenuManager.Sound.Forward);
				manager.LoadState(state);
			}
			else
			{
				manager.PlaySound(GuiMenuManager.Sound.UnConfim);
			}
			return;
		case MenuStates.Tutorial:
		{
			for (int i = 0; i < 4; i++)
			{
				if (InputManager.GetBool(InputAction.MenuForward, i))
				{
					manager.SetTutorialPlayerID(i);
					break;
				}
			}
			break;
		}
		}
		handleInput = false;
		manager.PlaySound(GuiMenuManager.Sound.Forward);
		PositionTween.Play(false);
		if (isDemo)
		{
			DemoTween.Play(false);
			PurchaseButtonTween.Play(false);
		}
	}

	public void OnMonitorScreenPressed()
	{
		if (BuildRuntimeSettings.Current.SupportsTouch && manager != null && handleInput && !anyPopupSpawned && manager.CurrentState == MenuStates.Main)
		{
			GuiList.ListButton listButton = ButtonList.CurrentSelected();
			GuiListButtonMain component = listButton.Object.GetComponent<GuiListButtonMain>();
			selectedIndex = component.Index;
			MenuStates currentSelectedState = GetCurrentSelectedState();
			SetState(currentSelectedState);
		}
	}

	public override void HandleButtonForwardPressed()
	{
		GuiList.ListButton listButton = ButtonList.CurrentSelected();
		GuiListButtonMain component = listButton.Object.GetComponent<GuiListButtonMain>();
		selectedIndex = component.Index;
		MenuStates currentSelectedState = GetCurrentSelectedState();
		SetState(currentSelectedState);
	}

	public void OnPurchaseSuccessful()
	{
		isDemo = false;
		DemoTween.Play(false);
		GuiList.ListButton listButton = ButtonList.CurrentSelected();
		GuiListButtonMain component = listButton.Object.GetComponent<GuiListButtonMain>();
		for (int i = 0; i < Data.Count; i++)
		{
			if (component.Name == Data[i].GuiName && Data[i].StateOnPress == MenuStates.Purchase)
			{
				component.SetText("gui_purchased");
				component.Name = Data[i].GuiName;
				break;
			}
		}
	}

	public MenuStates GetCurrentSelectedState()
	{
		GuiList.ListButton listButton = ButtonList.CurrentSelected();
		GuiListButtonMain component = listButton.Object.GetComponent<GuiListButtonMain>();
		for (int i = 0; i < Data.Count; i++)
		{
			if (component.Name == Data[i].GuiName)
			{
				return Data[i].StateOnPress;
			}
		}
		return MenuStates.Main;
	}

	public button GetCurrentButtonData()
	{
		GuiList.ListButton listButton = ButtonList.CurrentSelected();
		GuiListButtonMain component = listButton.Object.GetComponent<GuiListButtonMain>();
		for (int i = 0; i < Data.Count; i++)
		{
			if (component.Name == Data[i].GuiName)
			{
				return Data[i];
			}
		}
		return null;
	}

	public override void OnTweenComplete()
	{
		if (hasEntered)
		{
			handleInput = true;
		}
		MenuStates currentSelectedState = GetCurrentSelectedState();
		manager.SetScreenRandomUVEnableState(false);
		manager.ChangeScreenMaterial(GetStateScreenMaterial(currentSelectedState));
		button currentButtonData = GetCurrentButtonData();
		string @string = Localizer.GetString(currentButtonData.GuiName);
		int num = Mathf.CeilToInt((float)@string.Length * BracketSpacing);
		string text = "[BD0C0C][[-] ";
		if (currentSelectedState == MenuStates.Options)
		{
			text += " ";
		}
		for (int i = 0; i < num; i++)
		{
			text += " ";
		}
		text += " [BD0C0C]][-]";
		BracketsLabel.text = text;
		BracketsLabel.enabled = true;
	}

	public void TweenOut()
	{
	}

	private Material GetStateScreenMaterial(MenuStates state)
	{
		for (int i = 0; i < Data.Count; i++)
		{
			if (Data[i].StateOnPress == state)
			{
				return Data[i].ScreenMaterial;
			}
		}
		return null;
	}
}
