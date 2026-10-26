using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
	public LegendBar InfoBar = new LegendBar();

	public UILabel Label;

	public UILabel Label1;

	public UILabel Label2;

	public UILabel Label3;

	public UISprite TimerSprite;

	public UISprite BGSprite;

	public TweenPosition InfoPositionTween;

	public GameObject BackButton;

	public GameObject LegendBar;

	private List<PlayerShopWindow> playerShopReferences = new List<PlayerShopWindow>();

	private float totalTime;

	private float timeLeft;

	private bool calculateTime;

	private bool allReady;

	private bool shopOpen;

	private bool shopPhaseFinished;

	private bool visible = true;

	private string getReadyText;

	private GameObject oldFallThrough;

	public static bool UseSelectionInShop = true;

	private void Start()
	{
		getReadyText = string.Empty;
		InfoBar.O_label.text = "- " + Localizer.GetString(InfoBar.Okey);
		InfoBar.U_label.text = "- " + Localizer.GetString(InfoBar.Ukey);
		InfoBar.Y_label.text = "- " + Localizer.GetString(InfoBar.Ykey);
		InfoBar.A_label.text = "- " + Localizer.GetString(InfoBar.Akey);
		InfoBar.Hide();
		if (PlayerManager.Instance.NumberOfPlayers == 2)
		{
			BackButton.SetActive(false);
		}
	}

	private void Update()
	{
		if (!shopOpen)
		{
			return;
		}
		if (!LegendBar.activeSelf && DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
		{
			LegendBar.SetActive(true);
		}
		else if (LegendBar.activeSelf && DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch)
		{
			LegendBar.SetActive(false);
		}
		for (int i = 0; i < playerShopReferences.Count; i++)
		{
			if (playerShopReferences[i] != null)
			{
				playerShopReferences[i].Update();
			}
		}
		if (!shopPhaseFinished)
		{
			allReady = true;
			for (int j = 0; j < playerShopReferences.Count; j++)
			{
				if (playerShopReferences[j] != null && !playerShopReferences[j].IsWaitingForPlayers())
				{
					allReady = false;
					if (calculateTime)
					{
						StopTimer();
					}
					break;
				}
			}
		}
		if (calculateTime)
		{
			OnTimerTick();
		}
		else if (!calculateTime && allReady)
		{
			OnAllShopWindowsClosed();
		}
	}

	public void AddShopWindow(PlayerShopWindow window)
	{
		window.SetBackButtonObject(BackButton);
		playerShopReferences.Add(window);
		if (HasMultiplePlayers())
		{
			InfoBar.U_label.color = Color.white;
		}
		else
		{
			InfoBar.U_label.color = Color.grey;
		}
	}

	public void SetShopState(bool isOpen)
	{
		shopOpen = isOpen;
	}

	public bool IsShopOpen()
	{
		return shopOpen;
	}

	public bool IsShopPhaseDone()
	{
		return shopPhaseFinished;
	}

	public void Show()
	{
		InfoBar.Show();
		visible = true;
	}

	public void Hide()
	{
		InfoBar.Hide();
		visible = false;
	}

	public void OpenShop()
	{
		base.gameObject.SetActive(true);
		shopOpen = true;
		OpenShopWindows();
		InfoBar.Show();
	}

	public void CloseShop()
	{
		shopOpen = false;
		shopPhaseFinished = false;
		Label.text = string.Empty;
		TimerSprite.fillAmount = 0f;
		BGSprite.gameObject.SetActive(false);
		CloseShopWindows();
		InfoBar.Hide();
		base.gameObject.SetActive(false);
	}

	private void OpenShopWindows()
	{
		for (int i = 0; i < playerShopReferences.Count; i++)
		{
			if (playerShopReferences[i] != null)
			{
				playerShopReferences[i].Open();
			}
		}
	}

	private void CloseShopWindows()
	{
		for (int i = 0; i < playerShopReferences.Count; i++)
		{
			if (playerShopReferences[i] != null)
			{
				playerShopReferences[i].Close();
			}
		}
	}

	public void Display()
	{
		if (!shopOpen || !visible)
		{
			return;
		}
		for (int i = 0; i < playerShopReferences.Count; i++)
		{
			if (playerShopReferences[i] != null)
			{
				playerShopReferences[i].Display();
			}
		}
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			playerShopReferences[0].OnForwardPressed();
		}
	}

	private void StartTimer(float duration)
	{
		totalTime = duration;
		timeLeft = duration;
		calculateTime = true;
		Label.text = getReadyText + "\n" + timeLeft;
		BGSprite.gameObject.SetActive(true);
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch)
		{
			oldFallThrough = UICamera.fallThrough;
			UICamera.fallThrough = base.gameObject;
		}
		if (PlayerManager.Instance.NumberOfPlayers == 1 && (bool)BackButton)
		{
			BackButton.SetActive(false);
		}
		playerShopReferences[0].TapRecognizer.enabled = false;
	}

	private void StopTimer()
	{
		calculateTime = false;
		Label.text = string.Empty;
		TimerSprite.fillAmount = 0f;
		BGSprite.gameObject.SetActive(false);
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch)
		{
			UICamera.fallThrough = oldFallThrough;
		}
		if (PlayerManager.Instance.NumberOfPlayers == 1 && (bool)BackButton)
		{
			BackButton.SetActive(true);
		}
		playerShopReferences[0].TapRecognizer.enabled = true;
	}

	private void OnTimerTick()
	{
		timeLeft -= Time.deltaTime;
		Label.text = getReadyText + "\n" + timeLeft;
		TimerSprite.fillAmount = (totalTime - timeLeft) / totalTime;
		if (timeLeft <= 0f)
		{
			UICamera.fallThrough = oldFallThrough;
			calculateTime = false;
			shopPhaseFinished = true;
			BGSprite.gameObject.SetActive(false);
		}
	}

	private void OnAllShopWindowsClosed()
	{
		StartTimer(3f);
	}

	public PlayerStats GetPlayerStatsByID(int playerID)
	{
		foreach (PlayerShopWindow playerShopReference in playerShopReferences)
		{
			PlayerGUI playerGUI = playerShopReference.PlayerGUI;
			if (playerGUI != null && playerGUI.GetID() == playerID && playerGUI.Stats != null)
			{
				return playerGUI.Stats;
			}
		}
		return null;
	}

	public bool HasMultiplePlayers()
	{
		if (playerShopReferences != null && playerShopReferences.Count > 1)
		{
			return true;
		}
		return false;
	}

	public int GetNumberOfPlayers()
	{
		int num = 0;
		foreach (PlayerShopWindow playerShopReference in playerShopReferences)
		{
			PlayerGUI playerGUI = playerShopReference.PlayerGUI;
			if (playerGUI != null)
			{
				num++;
			}
		}
		return num;
	}
}
