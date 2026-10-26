using UnityEngine;

public class WeaponTrayDrag : MonoBehaviour
{
	private enum WeaponTrayState
	{
		Closing = 0,
		Closed = 1,
		Opening = 2,
		Open = 3
	}

	public float DragThreshold = 30f;

	public float OpenPositionY = 150f;

	public GameObject WeaponTrayObject;

	public UISprite Arrow;

	private Transform objectTransform;

	private float initialY;

	private bool hasTweenedIn;

	private WeaponTrayState currentState;

	private void Awake()
	{
		objectTransform = base.transform;
		currentState = WeaponTrayState.Closed;
	}

	private void Start()
	{
	}

	public void ToggleArrowBlinking(bool toggle)
	{
		TweenAlpha component = Arrow.GetComponent<TweenAlpha>();
		if (toggle)
		{
			component.enabled = true;
			component.Play(true);
		}
		else
		{
			component.enabled = false;
			Arrow.alpha = 0.5f;
		}
	}

	public void OpenWeaponTray()
	{
		CursorManager.SetCursorByType(CursorType.Menu);
		SetState(WeaponTrayState.Opening);
	}

	public void CloseWeaponTray()
	{
		CursorManager.SetCursorByType(CursorType.InGame);
		SetState(WeaponTrayState.Closing);
	}

	public bool IsOpen()
	{
		return currentState != WeaponTrayState.Closed;
	}

	private void OnClosing()
	{
		Vector3 localPosition = objectTransform.localPosition;
	}

	private void OnClosed()
	{
		if (WeaponTrayObject != null)
		{
			WeaponTrayObject.SetActive(false);
		}
		GuiInGameManager.Instance.UnPauseGame();
	}

	private void OnOpening()
	{
		Vector3 localPosition = objectTransform.localPosition;
		Arrow.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
		GuiInGameManager.Instance.PauseGame(true, true);
		if (WeaponTrayObject != null)
		{
			WeaponTrayObject.SetActive(true);
		}
	}

	private void OnOpen()
	{
	}

	private void SetState(WeaponTrayState newState)
	{
		currentState = newState;
		switch (newState)
		{
		case WeaponTrayState.Closing:
			OnClosing();
			break;
		case WeaponTrayState.Closed:
			OnClosed();
			break;
		case WeaponTrayState.Opening:
			OnOpening();
			break;
		case WeaponTrayState.Open:
			OnOpen();
			break;
		}
	}

	private void OnPress(bool isPressed)
	{
	}

	private void OnTweenComplete(UITweener tweener)
	{
		if (currentState == WeaponTrayState.Opening)
		{
			SetState(WeaponTrayState.Open);
		}
		else if (currentState == WeaponTrayState.Closing)
		{
			SetState(WeaponTrayState.Closed);
		}
		if (!hasTweenedIn)
		{
			hasTweenedIn = true;
			initialY = objectTransform.localPosition.y;
		}
		if (!GuiInGameManager.Instance.IsGamePaused && GuiInGameManager.Instance.IsShopOpen())
		{
		}
	}

	private void Update()
	{
	}
}
