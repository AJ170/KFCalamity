using System.Collections.Generic;
using Calamity.Input;
using UnityEngine;

public class MenuCharacterSelect : MenuBase
{
	public GameObject WindowPrefab;

	public GameObject PreWindowPrefab;

	public Material GrayScaleMat;

	public GameObject NextButton;

	public GameObject BackButton;

	private PlayerManager playerManager;

	private List<MenuCharacterWindow> windows = new List<MenuCharacterWindow>();

	private List<MenuCharacterPreWindow> preWindows = new List<MenuCharacterPreWindow>();

	private List<Vector3> startPositions = new List<Vector3>();

	private List<PerkData> perkList = new List<PerkData>();

	private List<CharacterDisplayData> characterList = new List<CharacterDisplayData>();

	private List<GameObject> soldierSpawns;

	private List<GuiMenuManager.UnitSpawnData> SpawnDataList;

	private Dictionary<int, GameObject> spawnedUnits = new Dictionary<int, GameObject>();

	private bool[] playerReadyList = new bool[4];

	private float totalTime;

	private float timeLeft;

	private bool calculateTime;

	private bool isDemo;

	private TouchButtonWrapper nextButton;

	private TouchButtonWrapper prevButton;

	private void Awake()
	{
		if (!BuildRuntimeSettings.Current.SupportsTouch)
		{
			NextButton.SetActive(false);
			BackButton.SetActive(false);
		}
	}

	private void Start()
	{
		for (int i = 0; i < playerReadyList.Length; i++)
		{
			playerReadyList[i] = true;
		}
		startPositions.Add(new Vector3(-480f, 0f, 0f));
		startPositions.Add(new Vector3(-160f, 0f, 0f));
		startPositions.Add(new Vector3(160f, 0f, 0f));
		startPositions.Add(new Vector3(480f, 0f, 0f));
		startPositions.Add(new Vector3(0f, 0f, 0f));
		if (PlayerManager.Instance.MaxPlayers > 1)
		{
			AddPreWindows();
		}
		InitializePlayers();
		UIAnchor[] componentsInChildren = GetComponentsInChildren<UIAnchor>();
		UIAnchor[] array = componentsInChildren;
		foreach (UIAnchor uIAnchor in array)
		{
			uIAnchor.uiCamera = manager.UICamera;
		}
		if (BuildRuntimeSettings.Current.SupportsTouch)
		{
			nextButton = new TouchButtonWrapper(NextButton);
			prevButton = new TouchButtonWrapper(BackButton);
		}
	}

	private void Update()
	{
		if (BuildRuntimeSettings.Current.SupportsTouch)
		{
			prevButton.Update();
			nextButton.Update();
		}
		if (calculateTime)
		{
			OnTimerTick();
		}
		HandleInput();
	}

	private void HandleInput()
	{
		for (int i = 0; i < playerManager.MaxPlayers; i++)
		{
			bool @bool = InputManager.GetBool(InputAction.MenuForward, i);
			bool bool2 = InputManager.GetBool(InputAction.MenuBackward, i);
			if (@bool && !playerManager.DoesPlayerWithIDExist(i))
			{
				PlaySound(GuiMenuManager.Sound.Confirm);
				AddPlayer(i);
			}
			if (bool2 && (windows.Count == 0 || PlayerManager.Instance.MaxPlayers == 1))
			{
				TweenOutPreWindows();
				TweenOut();
			}
		}
		if (nextButton != null && nextButton.GetBool())
		{
			bool flag = true;
			foreach (MenuCharacterWindow window in windows)
			{
				if (!window.CanReady())
				{
					manager.PlaySound(GuiMenuManager.Sound.UnConfim);
					flag = false;
				}
			}
			if (flag)
			{
				foreach (MenuCharacterWindow window2 in windows)
				{
					window2.OnReady();
				}
			}
		}
		if (prevButton != null && prevButton.GetBool())
		{
			if (calculateTime)
			{
				UnreadyAllActivePlayers();
			}
			else
			{
				ReturnToLevelSelect();
			}
		}
	}

	private void InitializePlayers()
	{
		if (PlayerManager.Instance.MaxPlayers == 1 || (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Touch && PlayerManager.Instance.MaxPlayers > 1) || (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller && InputManager.Instance.NumberOfConnectedJoysticks <= 1))
		{
			AddPlayer(0);
			return;
		}
		List<Player> allPlayers = playerManager.GetAllPlayers();
		foreach (Player item in allPlayers)
		{
			AddPlayer(item.ID);
		}
	}

	private void OnPlayersReady()
	{
		if (BuildRuntimeSettings.Current.SupportsTouch)
		{
			NextButton.SetActive(false);
		}
		StartTimer(1f);
	}

	private void UnreadyAllActivePlayers()
	{
		for (int i = 0; i < windows.Count; i++)
		{
			if (windows[i] != null)
			{
				windows[i].OnUnReady();
			}
		}
		calculateTime = false;
	}

	public void SetDemoLock(bool demo)
	{
		isDemo = demo;
	}

	public void PlaySound(GuiMenuManager.Sound sound)
	{
		manager.PlaySound(sound);
	}

	public void SetCharacterReadyStatus(int playerId, bool isReady)
	{
		if (!isReady)
		{
			StopTimer();
		}
		playerReadyList[playerId] = isReady;
		bool flag = true;
		for (int i = 0; i < playerReadyList.Length; i++)
		{
			if (!playerReadyList[i])
			{
				flag = false;
			}
		}
		if (flag)
		{
			if (BuildRuntimeSettings.Current.SupportsTouch)
			{
				NextButton.SetActive(false);
			}
			OnPlayersReady();
		}
		else if (BuildRuntimeSettings.Current.SupportsTouch)
		{
			NextButton.SetActive(true);
		}
	}

	public void SpawnSoldierPrefab(int playerId, string UnitName, bool isLocked)
	{
		if (spawnedUnits.ContainsKey(playerId))
		{
			Object.Destroy(spawnedUnits[playerId]);
			spawnedUnits.Remove(playerId);
		}
		for (int i = 0; i < SpawnDataList.Count; i++)
		{
			if (SpawnDataList[i].Name == UnitName)
			{
				int num = 0;
				num = ((PlayerManager.Instance.MaxPlayers == 2) ? (playerId + 1) : ((PlayerManager.Instance.MaxPlayers != 1) ? playerId : 4));
				GameObject gameObject = Object.Instantiate(SpawnDataList[i].Unit, soldierSpawns[num].transform.position, soldierSpawns[playerId].transform.rotation) as GameObject;
				SetMaterialForCharacterByStatus(gameObject, isLocked);
				spawnedUnits.Add(playerId, gameObject);
				break;
			}
		}
	}

	public void SetMaterialForCharacterByStatusAndID(int ID, bool status)
	{
		if (spawnedUnits[ID] != null)
		{
			SetMaterialForCharacterByStatus(spawnedUnits[ID], status);
		}
	}

	private void SetMaterialForCharacterByStatus(GameObject unit, bool status)
	{
		MeshRenderReference component = unit.gameObject.GetComponent<MeshRenderReference>();
		if (!(component != null) || !(GrayScaleMat != null))
		{
			return;
		}
		List<MeshRenderReference.MeshAndMaterial> references = component.References;
		if (references == null)
		{
			return;
		}
		for (int i = 0; i < references.Count; i++)
		{
			if (references[i].DefaultMaterial != null && references[i].Renderer != null)
			{
				if (status)
				{
					references[i].Renderer.material = GrayScaleMat;
					references[i].Renderer.material.mainTexture = references[i].DefaultMaterial.mainTexture;
				}
				else
				{
					references[i].Renderer.material = references[i].DefaultMaterial;
					references[i].Renderer.material.mainTexture = references[i].DefaultMaterial.mainTexture;
				}
			}
		}
	}

	public void SetPerkPickedStatus(Perk perk, bool status)
	{
		for (int i = 0; i < perkList.Count; i++)
		{
			if (perkList[i].Perk == perk)
			{
				perkList[i].IsAlreadyPicked = status;
				break;
			}
		}
		for (int j = 0; j < windows.Count; j++)
		{
			if (windows[j] != null)
			{
				windows[j].UpdateMaterialForCharacterByPerk(perk, status);
			}
		}
	}

	private void AddPreWindows()
	{
		string text = Localizer.GetString("gui_press") + "     " + Localizer.GetString("gui_to_join");
		int num = ((playerManager.MaxPlayers == 2) ? 1 : 4);
		for (int i = num; i < num + playerManager.MaxPlayers; i++)
		{
			GameObject gameObject = NGUITools.AddChild(base.gameObject, PreWindowPrefab);
			gameObject.transform.localPosition = startPositions[i];
			preWindows.Add(gameObject.GetComponent<MenuCharacterPreWindow>());
			preWindows[preWindows.Count - 1].SetText(text);
		}
	}

	public void RemovePlayer(int playerId)
	{
		if (spawnedUnits[playerId] != null)
		{
			Object.Destroy(spawnedUnits[playerId]);
			spawnedUnits.Remove(playerId);
		}
		int num = -1;
		for (int i = 0; i < windows.Count; i++)
		{
			if (windows[i].GetId() == playerId)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			playerManager.RemovePlayer(playerId);
			Object.Destroy(windows[num].gameObject);
			windows.RemoveAt(num);
			playerReadyList[playerId] = true;
			UnreadyAllActivePlayers();
			if (preWindows != null && preWindows.Count > playerId)
			{
				preWindows[playerId].TweenIn();
			}
			if (windows.Count == 0 && NextButton.activeSelf)
			{
				NextButton.SetActive(false);
			}
		}
		else
		{
			Debug.Log("[MenuCharacterSelect] Player window with id: " + playerId + " could not be found and therfor not removed.");
		}
	}

	public void AddPlayer(int playerId)
	{
		bool flag = false;
		for (int i = 0; i < windows.Count; i++)
		{
			if (windows[i].GetId() == playerId)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			Player playerReference = ((!playerManager.DoesPlayerWithIDExist(playerId)) ? playerManager.AddPlayer(playerId) : playerManager.GetPlayerByID(0));
			int index = ((playerManager.MaxPlayers != 2) ? 4 : (playerId + 1));
			GameObject gameObject = NGUITools.AddChild(base.gameObject, WindowPrefab);
			gameObject.transform.localPosition = startPositions[index];
			MenuCharacterWindow component = gameObject.GetComponent<MenuCharacterWindow>();
			component.SetPlayerId(playerId);
			component.SetManagerReference(this);
			component.SetPlayerReference(playerReference);
			component.SetDemoLock(isDemo);
			windows.Add(component);
			playerReadyList[playerId] = false;
			if (preWindows != null && preWindows.Count > playerId)
			{
				preWindows[playerId].TweenOut();
			}
			if (!NextButton.activeSelf && BuildRuntimeSettings.Current.SupportsTouch)
			{
				NextButton.SetActive(true);
			}
		}
		SetCharacterReadyStatus(playerId, false);
	}

	public void PopulateCharacterList(SoldierMenuDefinitions soldierDefines, PerkMenuDefinitions perkDefines)
	{
		PerkData[] perkData = perkDefines.PerkData;
		CharacterDisplayData[] soldierData = soldierDefines.SoldierData;
		for (int i = 0; i < perkData.Length; i++)
		{
			perkList.Add(perkData[i]);
		}
		for (int j = 0; j < soldierData.Length; j++)
		{
			characterList.Add(soldierData[j]);
		}
	}

	public void StartTimer(float duration)
	{
		totalTime = 1f;
		timeLeft = duration;
		calculateTime = true;
	}

	private void OnTimerTick()
	{
		timeLeft -= Time.deltaTime / totalTime;
		if (!(timeLeft <= 0f))
		{
			return;
		}
		calculateTime = false;
		for (int i = 0; i < windows.Count; i++)
		{
			windows[i].TweenOut();
		}
		for (int j = 0; j < preWindows.Count; j++)
		{
			if (!preWindows[j].OutTweened)
			{
				preWindows[j].TweenOut();
			}
		}
		Invoke("StartGame", 0.2f);
	}

	private void TweenOutPreWindows()
	{
		for (int i = 0; i < preWindows.Count; i++)
		{
			preWindows[i].TweenOut();
		}
		Invoke("ReturnToLevelSelect", 0.2f);
	}

	private void StartGame()
	{
		List<Player> allPlayers = playerManager.GetAllPlayers();
		for (int i = 0; i < allPlayers.Count; i++)
		{
			GA.API.Design.NewEvent(string.Concat("Perk:", allPlayers[i].SelectedPerk, ":", Localizer.GetString(allPlayers[i].SelectedCharacter)));
		}
		manager.LoadState(MenuStates.GameStart);
	}

	public void ReturnToLevelSelect()
	{
		int num = windows.Count - 1;
		for (int num2 = num; num2 > -1; num2--)
		{
			RemovePlayer(windows[num2].GetId());
		}
		manager.PlaySound(GuiMenuManager.Sound.Backward);
		manager.LoadState(MenuStates.GameSetup);
	}

	public void TweenOut()
	{
	}

	public void StopTimer()
	{
		calculateTime = false;
	}

	public List<PerkData> Perks()
	{
		return perkList;
	}

	public List<CharacterDisplayData> Characters()
	{
		return characterList;
	}

	public override void HandleButtonForwardPressed()
	{
	}

	public override void HandleButtonBackwardPressed()
	{
	}

	public override void HandleHorizontalAxisInput(int axis)
	{
	}

	public override void SetManagerReference(GuiMenuManager manager)
	{
		base.SetManagerReference(manager);
	}

	public void SetPlayerManagerReference(PlayerManager manager)
	{
		playerManager = manager;
	}

	public void SetSoldierSpawnData(List<GameObject> spawns, List<GuiMenuManager.UnitSpawnData> data)
	{
		soldierSpawns = spawns;
		SpawnDataList = data;
	}
}
