using System;
using System.Collections.Generic;
using Calamity.Input;
using EventHelpers;
using GameEvents;
using Tutorial;
using TutorialHelpers;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	private static TutorialManager instance;

	public GameObject StartingStep;

	public AbilitySlot[] DisabledAbilities;

	public EventObject[] Objects;

	private GameObject soldier;

	private List<GameObject> tutorialSteps = new List<GameObject>();

	private List<GameObject> tutorialSubStepObjects = new List<GameObject>();

	private TutorialSubStepBase[] tutorialSubSteps;

	private GameModeTutorial gameMode;

	private GuiInGameManager guiManager;

	private GameUnitManager unitManager;

	private PickupManager pickupManager;

	private QuestLog questLog;

	private TutorialMessage tutorialMessage;

	private RunDialogScript currentDialog;

	private int stepIndex;

	private int subStepIndex = -1;

	private int objectPlayerID;

	private float searchTimer = 2f;

	private bool isTutorialStarted;

	private Dictionary<GameObject, TutorialQuest> questEntries = new Dictionary<GameObject, TutorialQuest>();

	private Dictionary<string, GameObject> eventObjects = new Dictionary<string, GameObject>();

	private Dictionary<string, UnitBar> unitBars = new Dictionary<string, UnitBar>();

	private Ability syringeAbilitySelf;

	private Ability syringeAbilityTarget;

	private TouchStickType oldStickType;

	private CommandQueue commandQueue = new CommandQueue();

	public static TutorialManager Instance
	{
		get
		{
			return instance;
		}
	}

	public TouchStickType OldStickType
	{
		get
		{
			return oldStickType;
		}
		set
		{
			oldStickType = value;
		}
	}

	public GameObject Soldier
	{
		get
		{
			return soldier;
		}
	}

	public GuiInGameManager GuiManager
	{
		get
		{
			return guiManager;
		}
	}

	public bool IsTutorialStarted
	{
		get
		{
			return isTutorialStarted;
		}
	}

	public GameModeTutorial GameModeRef()
	{
		return gameMode;
	}

	public void SetGameMode(GameModeTutorial gameMode)
	{
		this.gameMode = gameMode;
	}

	private void DisableAllSteps()
	{
		TutorialSubStepBase[] componentsInChildren = GetComponentsInChildren<TutorialSubStepBase>();
		TutorialSubStepBase[] array = componentsInChildren;
		foreach (TutorialSubStepBase tutorialSubStepBase in array)
		{
			tutorialSubStepBase.enabled = false;
		}
	}

	public void DisablePlayerAbilities()
	{
		PlayerAbilityController component = soldier.GetComponent<PlayerAbilityController>();
		AbilitySlot[] disabledAbilities = DisabledAbilities;
		foreach (AbilitySlot abilitySlot in disabledAbilities)
		{
			if (abilitySlot != 0)
			{
				Ability ability = component.AbilityBySlot(abilitySlot);
				ability.enabled = false;
				ability.SetCharges(0);
			}
		}
	}

	private void Awake()
	{
		instance = this;
		if (OptionManager.Instance != null && OptionManager.Instance.IsStickTypeTemporary)
		{
			oldStickType = OptionManager.Instance.StickType;
			OptionManager.Instance.StickType = TouchStickType.Locked;
		}
	}

	private void Start()
	{
		TutorialStep[] componentsInChildren = GetComponentsInChildren<TutorialStep>();
		if (componentsInChildren != null)
		{
			tutorialSteps = new List<GameObject>(Array.ConvertAll(componentsInChildren, (TutorialStep a) => a.gameObject));
			tutorialSteps.Sort((GameObject a, GameObject b) => a.name.CompareTo(b.name));
			DisableAllSteps();
		}
		else
		{
			Debug.LogError("TutorialManager:Start() - No Tutorial Steps Found!");
		}
		GameObject gameObject = GameObject.Find("GuiIngameManager");
		PlayerManager playerManager = PlayerManager.Instance;
		if (gameObject != null && playerManager != null)
		{
			int tutorialPlayerInputID = 0;
			if (playerManager.DoesPlayerWithIDExist(0))
			{
				Player playerByID = playerManager.GetPlayerByID(0);
				tutorialPlayerInputID = playerByID.InputID;
			}
			else
			{
				List<Player> allPlayers = playerManager.GetAllPlayers();
				if (allPlayers.Count > 0)
				{
					Player playerByID = allPlayers[0];
					tutorialPlayerInputID = playerByID.InputID;
				}
			}
			guiManager = gameObject.GetComponent<GuiInGameManager>();
			guiManager.SetTutorialPlayerInputID(tutorialPlayerInputID);
			questLog = guiManager.AddQuestLog();
		}
		SetupObjects();
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
		if (OptionManager.Instance != null && OptionManager.Instance.IsStickTypeTemporary)
		{
			OptionManager.Instance.StickType = oldStickType;
		}
	}

	private void SetupObjects()
	{
		EventObject[] objects = Objects;
		foreach (EventObject eventObject in objects)
		{
			eventObjects.Add(eventObject.Name, eventObject.Object);
			if (eventObject.Type == ObjectType.Pickup)
			{
				Pickup component = eventObject.Object.GetComponent<Pickup>();
				if (component != null)
				{
					PickupManager.Instance.AddPickup(component);
				}
			}
			else if (eventObject.Type != ObjectType.Npc)
			{
			}
			if (eventObject.HasHealthbar)
			{
				CreateHealthBarForObject(eventObject.Name, eventObject.Object);
			}
		}
	}

	public void DestroyTutorialMessage()
	{
		if (tutorialMessage != null)
		{
			UnityEngine.Object.Destroy(tutorialMessage.gameObject);
			tutorialMessage = null;
		}
	}

	public void ToggleTutorialMessage(bool visible)
	{
		if (tutorialMessage != null)
		{
			tutorialMessage.gameObject.SetActive(visible);
		}
	}

	public void ToggleQuestLog(bool visible)
	{
		questLog.gameObject.SetActive(visible);
	}

	public static void HideQuestLog()
	{
		if (instance != null)
		{
			instance.ToggleQuestLog(false);
		}
	}

	private void UpdateQuestLog()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (KeyValuePair<GameObject, TutorialQuest> questEntry in questEntries)
		{
			GameObject key = questEntry.Key;
			TutorialQuest value = questEntry.Value;
			if (value.HasMissionCounter())
			{
				IMissionCounter missionCounter = value.GetMissionCounter();
				if (missionCounter.NeedsUpdate)
				{
					if (missionCounter.DisplayType == QuestEntry.EntryType.Counter)
					{
						questLog.SetEntryCounter(key, missionCounter.GetCurrentCount());
					}
					else if (missionCounter.DisplayType == QuestEntry.EntryType.Timer)
					{
						questLog.SetEntryTimer(key, missionCounter.GetCurrentCount());
					}
					missionCounter.NeedsUpdate = false;
				}
			}
			if (value.IsComplete())
			{
				questLog.SetEntryDone(key);
				list.Add(key);
			}
		}
		foreach (GameObject item in list)
		{
			questEntries.Remove(item);
		}
		list.Clear();
	}

	private List<GameObject> GetChildsSorted(GameObject gameObject)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Transform item in gameObject.transform)
		{
			list.Add(item.gameObject);
		}
		list.Sort((GameObject a, GameObject b) => a.name.CompareTo(b.name));
		return list;
	}

	public void GotoStep(GameObject tutorialStep)
	{
		tutorialSubStepObjects = GetChildsSorted(tutorialStep);
		subStepIndex = -1;
		if (questLog != null)
		{
			questLog.ClearLog();
			questEntries.Clear();
		}
		if (tutorialSteps.IndexOf(StartingStep) > 0)
		{
			TutoriaTriggerEvent component = tutorialSubStepObjects[0].GetComponent<TutoriaTriggerEvent>();
			stepIndex = tutorialSteps.IndexOf(StartingStep);
			if (component != null && component.WaypointTrigger != null)
			{
				GameObject eventObject = GetEventObject("Commander");
				eventObject.GetComponent<AIMovementMotor>().Speed = 20f;
				eventObject.GetComponent<AIMovementMotor>().MoveToTargetPosition(component.WaypointTrigger.transform.position);
				component.WaypointTrigger.GetComponent<ObjectWaypoint>().HasBeenReached = true;
			}
		}
		NextSubStep();
	}

	public bool NextSubStep()
	{
		if (subStepIndex + 1 >= tutorialSubStepObjects.Count)
		{
			return false;
		}
		GameObject gameObject = tutorialSubStepObjects[++subStepIndex];
		RunDialogScript component = gameObject.GetComponent<RunDialogScript>();
		if (component != null)
		{
			component.StartDialog();
		}
		currentDialog = component;
		TutorialQuest component2 = gameObject.GetComponent<TutorialQuest>();
		tutorialSubSteps = tutorialSubStepObjects[subStepIndex].GetComponents<TutorialSubStepBase>();
		TutorialSubStepBase[] array = tutorialSubSteps;
		foreach (TutorialSubStepBase tutorialSubStepBase in array)
		{
			tutorialSubStepBase.enabled = true;
			tutorialSubStepBase.SetPlayer(soldier);
			tutorialSubStepBase.SetTutorialManager(this);
			tutorialSubStepBase.OnStart();
		}
		if (component2 != null)
		{
			if (questLog != null)
			{
				questLog.ClearLog();
				questEntries.Clear();
			}
			GameObject gameObject3 = (component2.QuestID = questLog.NewEntry(component2.QuestTextKey, component2.QuestSymbolEntry));
			if (component2.HasMissionCounter())
			{
				IMissionCounter missionCounter = component2.GetMissionCounter();
				if (missionCounter.ShowCounter)
				{
					questLog.SetEntryMaxCounter(gameObject3, component2.GetMissionCounter().GetMaxCount());
					questLog.SetEntryCounter(gameObject3, 0);
				}
			}
			questEntries.Add(gameObject3, component2);
		}
		return tutorialSubSteps.Length > 0 || component != null;
	}

	private void StartTutorial()
	{
		if (!isTutorialStarted)
		{
			TutorialDebug component = GetComponent<TutorialDebug>();
			if (component != null && component.enabled)
			{
				component.Run();
			}
			objectPlayerID = gameMode.GameStateRef().Context.PlayerManager.NumberOfPlayers;
			isTutorialStarted = true;
			GotoStep(StartingStep);
		}
	}

	private bool NextStep()
	{
		if (stepIndex + 1 >= tutorialSteps.Count)
		{
			return false;
		}
		GotoStep(tutorialSteps[++stepIndex]);
		return true;
	}

	private bool IsStepComplete()
	{
		TutorialSubStepBase[] array = tutorialSubSteps;
		foreach (TutorialSubStepBase tutorialSubStepBase in array)
		{
			if (!tutorialSubStepBase.IsComplete)
			{
				return false;
			}
		}
		if (currentDialog != null && currentDialog.CompletesOnClose)
		{
			return currentDialog.CheckComplete();
		}
		return true;
	}

	private void DoStep()
	{
		if (isTutorialStarted && IsStepComplete() && !NextSubStep() && !NextStep())
		{
			isTutorialStarted = false;
		}
	}

	private void Update()
	{
		commandQueue.Update();
		if (!isTutorialStarted)
		{
			SearchForPlayer();
		}
		if (Input.GetKeyDown(KeyCode.U))
		{
			Game.Context context = GameModeRef().GameStateRef().Context;
			GamePhaseManager gamePhaseManager = GameModeRef().GamePhaseManagerRef();
			gamePhaseManager.SetPhase(new Tutorial.GameOverGamePhase(context, true));
		}
		UpdateQuestLog();
		DoStep();
	}

	private void OnCommanderSpeechDestroy()
	{
		commandQueue.Add(RestoreSyringeAbilitiesType, 0.5f);
	}

	private void RestoreSyringeAbilitiesType()
	{
		if (tutorialMessage == null && DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
		{
			syringeAbilityTarget.enabled = true;
			syringeAbilitySelf.enabled = true;
		}
	}

	public TutorialMessage RunSpeech(string speechText, float speechTime, Action endMessageCallback)
	{
		if (DeviceSwitchController.CurrentDeviceMode == DeviceMode.Controller)
		{
			syringeAbilityTarget.enabled = false;
			syringeAbilitySelf.enabled = false;
		}
		tutorialMessage = guiManager.DisplayTutorialMessage(endMessageCallback, MessageSender.Commander, speechText, 0.02f, true);
		tutorialMessage.AddMessageDestroyListener(OnCommanderSpeechDestroy);
		return tutorialMessage;
	}

	private void OnCreateSoldier(GameObject soldier)
	{
		ObjectStartHealth objectStartHealth = soldier.AddComponent<ObjectStartHealth>();
		Health component = soldier.GetComponent<Health>();
		objectStartHealth.StartHealth = component.MaxHealth / 2;
		PlayerWeaponController component2 = soldier.GetComponent<PlayerWeaponController>();
		component2.StartingPistol = null;
		component2.StartingMelee = null;
	}

	public void OnEvent(EventBase evnt)
	{
		if (evnt.Id == EventId.CreateSoldier)
		{
			CreateSoldier createSoldier = (CreateSoldier)evnt;
			OnCreateSoldier(createSoldier.Soldier);
		}
	}

	private void SetupPlayer()
	{
		DisablePlayerAbilities();
		PlayerAbilityController component = soldier.GetComponent<PlayerAbilityController>();
		syringeAbilitySelf = component.AbilityBySlot(AbilitySlot.SyringeSelf);
		syringeAbilityTarget = component.AbilityBySlot(AbilitySlot.SyringeTarget);
		Ability ability = component.AbilityBySlot(AbilitySlot.Perk);
		ForwardImpactEffect componentInChildren = ability.GetComponentInChildren<ForwardImpactEffect>();
		componentInChildren.Offset.y = 1.5f;
		Health component2 = soldier.GetComponent<Health>();
		component2.LesserInvulnerability = true;
	}

	private void CreateHealthBarForObject(string objectName, GameObject gameObject)
	{
		if (gameObject.GetComponent<Health>() == null)
		{
			Debug.LogWarning("TutorialManager:CreateHealthBarForObject() - Tried to create a unitBar for an object without a Health component!");
			return;
		}
		GameObject gameObject2 = GuiManager.CreateUnitBar(++objectPlayerID);
		UnitBar component = gameObject2.GetComponent<UnitBar>();
		component.SetTarget(gameObject);
		component.Show();
		unitBars.Add(objectName, component);
	}

	private void SearchForPlayer()
	{
		searchTimer = Mathf.Max(0f, searchTimer - Time.deltaTime);
		if (searchTimer == 0f)
		{
			soldier = FindPlayer();
			if (soldier != null)
			{
				SetupPlayer();
				StartTutorial();
			}
			else
			{
				searchTimer = 2f;
			}
		}
	}

	private GameObject FindPlayer()
	{
		return GameObject.FindGameObjectWithTag("Soldier");
	}

	public GameObject GetEventObject(string objectName)
	{
		if (eventObjects.ContainsKey(objectName))
		{
			return eventObjects[objectName];
		}
		Debug.Log("TutorialManager:GetEventObjct() - Object with name: '" + objectName + "' does doesn't exist!");
		return null;
	}

	public bool EventObjectExists(string keyName)
	{
		return eventObjects.ContainsKey(keyName);
	}

	public void AddEventObject(string keyName, GameObject gameObject)
	{
		if (keyName.Contains("(Clone"))
		{
			keyName = keyName.Remove(keyName.IndexOf("(Clone)"));
		}
		if (!eventObjects.ContainsKey(gameObject.name))
		{
			eventObjects.Add(keyName, gameObject);
		}
		else
		{
			Debug.Log("TutorialManager:AddEventObject() - An event object with the name: '" + keyName + "' already exists");
		}
	}

	public UnitBar GetUnitBar(string objectName)
	{
		if (!unitBars.ContainsKey(objectName))
		{
			return null;
		}
		return unitBars[objectName];
	}
}
