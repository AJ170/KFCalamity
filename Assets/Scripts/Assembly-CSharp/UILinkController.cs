using System;
using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class UILinkController : MonoBehaviour
{
	public enum ControllerState
	{
		None = 0,
		Enabled = 1,
		Active = 2,
		Disabled = 3,
		PairDone = 4,
		Hidden = 5
	}

	[Serializable]
	public class Controller
	{
		public UISprite ControllerImage;

		public UISprite ButtonImage;

		public UILabel ControllerTitle;

		public UILabel ControllerStatus;

		public ControllerState State;

		public void Hide(bool hideLabel = false)
		{
			State = ControllerState.Hidden;
			ControllerStatus.enabled = false;
			ControllerImage.enabled = false;
			ButtonImage.enabled = false;
			if (hideLabel)
			{
				ControllerTitle.enabled = false;
			}
		}

		public void DisableController()
		{
			State = ControllerState.Disabled;
			ControllerImage.alpha = 0.15f;
			ControllerStatus.enabled = false;
			ControllerTitle.enabled = false;
			ControllerImage.enabled = false;
			ButtonImage.enabled = false;
		}

		public void EnableController()
		{
			State = ControllerState.Enabled;
			ControllerImage.alpha = 0.15f;
			ControllerTitle.enabled = true;
			ControllerImage.enabled = true;
			ControllerStatus.enabled = false;
			ButtonImage.enabled = false;
		}

		public void OnPairingDone()
		{
			State = ControllerState.PairDone;
			ControllerImage.alpha = 1f;
			ControllerStatus.text = Localizer.GetString("gui_controller_link_assigned");
			ControllerStatus.enabled = true;
			ControllerTitle.enabled = true;
			ControllerImage.enabled = true;
			ButtonImage.enabled = false;
		}

		public void OnActiveGain()
		{
			State = ControllerState.Active;
			ControllerImage.alpha = 0.15f;
			ControllerStatus.text = Localizer.GetString("gui_controller_link_press");
			ControllerStatus.enabled = true;
			ControllerTitle.enabled = true;
			ControllerImage.enabled = true;
			ButtonImage.enabled = true;
		}
	}

	public ControllerTypePopup Popup;

	public UILabel LabelTitle;

	public UILabel LabelPlayerPress;

	public UILabel LabelPressInfo;

	public UISprite OButtonSprite;

	public List<Controller> Controllers;

	public TweenPosition PositionTween;

	private int activeID = -1;

	private bool initialized;

	private bool isControllerPopupActive;

	private bool enabledPopupInput;

	private List<ControllerType> connectedControllerTypes;

	private RelinkControllerTypeCallback callback;

	private RelinkAxisInput axisInput;

	private RelinkConfirmInput confirmInput;

	private void Start()
	{
		if (!initialized)
		{
			Initialize();
		}
	}

	private void TweenOut(string callback)
	{
		PositionTween.enabled = true;
		PositionTween.eventReceiver = base.gameObject;
		PositionTween.callWhenFinished = callback;
		PositionTween.Play(false);
	}

	private void TweenIn(string callback)
	{
		PositionTween.enabled = true;
		PositionTween.eventReceiver = base.gameObject;
		PositionTween.callWhenFinished = callback;
		PositionTween.Play(true);
	}

	private void OnTweenInComplete()
	{
	}

	private void OnTweenOutComplete()
	{
		DisableControllers();
		base.gameObject.SetActive(false);
	}

	private void Initialize()
	{
		if (LabelPressInfo != null)
		{
			LabelPressInfo.text = Localizer.GetString("gui_controller_link_press_info");
		}
		if (LabelTitle != null)
		{
			LabelTitle.text = Localizer.GetString("gui_controller_link_title_info");
		}
		if (LabelPlayerPress != null)
		{
			LabelPlayerPress.text = Localizer.GetString("gui_player") + " " + 1 + " " + Localizer.GetString("gui_press").ToLower();
		}
		DisableControllers();
		initialized = true;
	}

	private void Open()
	{
		if (!initialized)
		{
			Initialize();
		}
		base.gameObject.SetActive(true);
		TweenIn("OnTweenInComplete");
		if (PlayerManager.Instance.MaxPlayers == 2)
		{
			Controllers[2].Hide(true);
			Controllers[3].Hide(true);
		}
	}

	private void Close()
	{
		TweenOut("OnTweenOutComplete");
	}

	private void OnAllControllersLinked()
	{
		if (OButtonSprite != null)
		{
			OButtonSprite.enabled = false;
		}
		if (LabelPressInfo != null)
		{
			LabelPressInfo.enabled = false;
		}
		if (LabelPlayerPress != null)
		{
			LabelPlayerPress.text = Localizer.GetString("gui_controller_link_all_done");
		}
	}

	private void ResetTitles()
	{
		if (OButtonSprite != null)
		{
			OButtonSprite.enabled = true;
		}
		if (LabelPressInfo != null)
		{
			LabelPressInfo.enabled = true;
		}
		if (LabelPlayerPress != null)
		{
			LabelPlayerPress.enabled = true;
			LabelPlayerPress.text = Localizer.GetString("gui_player") + " " + activeID + " " + Localizer.GetString("gui_press").ToLower();
		}
	}

	public void TriggerControllerLinkSelection(RelinkControllerTypeCallback callback, RelinkAxisInput axisInput, RelinkConfirmInput confirmInput, List<ControllerType> connectedControllerTypes)
	{
		this.callback = callback;
		this.axisInput = axisInput;
		this.confirmInput = confirmInput;
		this.connectedControllerTypes = connectedControllerTypes;
		Popup.SetActiveButtons(Localizer.GetString("gui_options_controller_type_popup_title"), connectedControllerTypes.Count);
		for (int i = 0; i < connectedControllerTypes.Count; i++)
		{
			Popup.SetButtonTextByIndex(i, GetNameBasedOnControllerType(connectedControllerTypes[i]));
		}
		OpenPopup();
	}

	private string GetNameBasedOnControllerType(ControllerType type)
	{
		switch (type)
		{
		case ControllerType.Broadcom_Android:
			return Localizer.GetString("gui_options_controller_type_broadcom");
		case ControllerType.GameStick_Android:
			return Localizer.GetString("gui_options_controller_type_gamestick");
		case ControllerType.GameStick_OSX:
			return Localizer.GetString("gui_options_controller_type_gamestick");
		case ControllerType.GameStick_Windows:
			return Localizer.GetString("gui_options_controller_type_gamestick");
		case ControllerType.Generic_Android:
			return Localizer.GetString("gui_options_controller_type_unknown");
		case ControllerType.Keyboard:
			return Localizer.GetString("gui_options_controller_type_keyboard");
		case ControllerType.MogaPro_Android:
			return Localizer.GetString("gui_options_controller_type_mogapro");
		case ControllerType.MotionInJoy_Windows:
			return Localizer.GetString("gui_options_controller_type_motion_in_joy");
		case ControllerType.Ouya_Android:
			return Localizer.GetString("gui_options_controller_type_ouya");
		case ControllerType.Ouya_Windows:
			return Localizer.GetString("gui_options_controller_type_ouya");
		case ControllerType.PS3_Android:
			return Localizer.GetString("gui_options_controller_type_ps3");
		case ControllerType.PS3_OSX:
			return Localizer.GetString("gui_options_controller_type_ps3");
		case ControllerType.PS3_Windows:
			return Localizer.GetString("gui_options_controller_type_ps3");
		case ControllerType.TattieBogle_OSX:
			return Localizer.GetString("gui_options_controller_type_unknown");
		case ControllerType.XBOX360_Android:
			return Localizer.GetString("gui_options_controller_type_xbox");
		case ControllerType.XBOX360_Windows:
			return Localizer.GetString("gui_options_controller_type_xbox");
		case ControllerType.XBOX360_Wireless_Android:
			return Localizer.GetString("gui_options_controller_type_xbow_wireless");
		case ControllerType.PSVita:
			return "PS Vita";
		case ControllerType.Unknown:
			return Localizer.GetString("gui_options_controller_type_unknown");
		case ControllerType.None:
			return Localizer.GetString("gui_options_controller_type_unknown");
		default:
			return string.Empty;
		}
	}

	private void OpenPopup()
	{
		float x = Controllers[activeID].ControllerImage.transform.position.x;
		Vector3 position = Popup.gameObject.transform.position;
		Popup.gameObject.transform.position = new Vector3(x, position.y, position.z);
		Popup.gameObject.SetActive(true);
		Popup.SelectReset();
		isControllerPopupActive = true;
		Controllers[activeID].Hide();
	}

	private void SendInputToPopup()
	{
		int axis = axisInput();
		Popup.HandleVerticalAxisInput(axis);
		if (confirmInput())
		{
			OnPopupActionMade(Popup.ButtonIndex);
		}
	}

	private void OnPopupActionMade(int index)
	{
		if (index == connectedControllerTypes.Count)
		{
			Controllers[activeID].OnActiveGain();
			callback(ControllerType.None);
		}
		else
		{
			callback(connectedControllerTypes[index]);
		}
		isControllerPopupActive = false;
		enabledPopupInput = false;
		Popup.Reset();
		Popup.gameObject.SetActive(false);
		callback = null;
		axisInput = null;
		confirmInput = null;
		connectedControllerTypes = null;
		TweenIn(string.Empty);
	}

	private void Update()
	{
		if (isControllerPopupActive)
		{
			if (enabledPopupInput)
			{
				SendInputToPopup();
			}
			else
			{
				enabledPopupInput = true;
			}
		}
	}

	private void OpenControllerTypePopup()
	{
	}

	private void CloseControllerTypePopup()
	{
	}

	private void ControllerEnabled(int ID)
	{
		Controller controllerByID = GetControllerByID(ID);
		if (controllerByID != null)
		{
			controllerByID.EnableController();
		}
		else
		{
			Debug.Log("me no find stuff " + Controllers.Count);
		}
	}

	private void ControllerDisable(int ID)
	{
		Controller controllerByID = GetControllerByID(ID);
		if (controllerByID != null)
		{
			controllerByID.DisableController();
		}
	}

	private void ControllerActive(int ID)
	{
		Controller controllerByID = GetControllerByID(ID);
		if (controllerByID != null)
		{
			if (activeID >= 0)
			{
				ControllerEnabled(activeID);
			}
			controllerByID.OnActiveGain();
			if (LabelPlayerPress != null)
			{
				LabelPlayerPress.text = Localizer.GetString("gui_player") + " " + (ID + 1) + " " + Localizer.GetString("gui_press").ToLower();
			}
			activeID = ID;
		}
		else
		{
			Debug.Log("me no find stuff " + Controllers.Count);
		}
	}

	private void ControllerLink(int ID)
	{
		if (ID == activeID)
		{
			activeID = -1;
		}
		Controller controllerByID = GetControllerByID(ID);
		if (controllerByID != null)
		{
			controllerByID.OnPairingDone();
		}
	}

	private void UndoControllerLink(int ID)
	{
		if (LabelPlayerPress != null)
		{
			LabelPlayerPress.text = string.Empty;
		}
		ResetTitles();
		ControllerEnabled(ID);
	}

	private void DisableControllers()
	{
		for (int i = 0; i < Controllers.Count; i++)
		{
			if (Controllers[i] != null)
			{
				Controllers[i].DisableController();
			}
		}
		if (LabelPlayerPress != null)
		{
			LabelPlayerPress.text = string.Empty;
		}
		ResetTitles();
	}

	public void PerformControllerLinkAction(ControllerLinkGUIAction type, int ID)
	{
		if (PlayerManager.Instance.MaxPlayers == 4 || ID <= 1)
		{
			switch (type)
			{
			case ControllerLinkGUIAction.Open:
				Open();
				break;
			case ControllerLinkGUIAction.Close:
				Close();
				break;
			case ControllerLinkGUIAction.PlayerActive:
				ControllerActive(ID);
				break;
			case ControllerLinkGUIAction.PlayerEnabled:
				ControllerEnabled(ID);
				break;
			case ControllerLinkGUIAction.PlayerLinked:
				ControllerLink(ID);
				break;
			case ControllerLinkGUIAction.PlayerUnlinked:
				UndoControllerLink(ID);
				break;
			case ControllerLinkGUIAction.PlayFailureSound:
				break;
			case ControllerLinkGUIAction.Done:
				OnAllControllersLinked();
				break;
			}
		}
	}

	private Controller GetControllerByID(int ID)
	{
		for (int i = 0; i < Controllers.Count; i++)
		{
			if (ID == i && Controllers[i] != null)
			{
				return Controllers[i];
			}
		}
		return null;
	}
}
