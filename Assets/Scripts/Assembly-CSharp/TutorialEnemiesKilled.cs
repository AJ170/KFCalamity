using UnityEngine;

public class TutorialEnemiesKilled : TutorialEventBase, IMissionCounter
{
	public int EnemiesToKill;

	public GameObject[] Enemies;

	public bool MustAim;

	public bool RespawnEnemiesIfNotAiming;

	public GameObject RespawnSpawnPoint;

	public bool ShowInQuestLog = true;

	public bool GotoKilledUnitPosition;

	public bool InvulnerableOnSpawn;

	public string FailMessageKey;

	public TutorialSpeechLine FailMessageSpeechLine = TutorialSpeechLine.None;

	public float InvulnerableTimer;

	public Vector3 LastKilledOffset;

	public QuestEntry.EntryType QuestDisplayType;

	private bool needsUpdate;

	private bool hasStarted;

	private int enemiesKilled;

	private int kills;

	private GameUnitManager gameUnitManager;

	private Vector3 lastDied;

	public bool NeedsUpdate
	{
		get
		{
			return needsUpdate;
		}
		set
		{
			needsUpdate = value;
		}
	}

	public bool ShowCounter
	{
		get
		{
			return ShowInQuestLog;
		}
	}

	public QuestEntry.EntryType DisplayType
	{
		get
		{
			return QuestEntry.EntryType.Counter;
		}
	}

	private void Start()
	{
		GameObject[] enemies = Enemies;
		foreach (GameObject gameObject in enemies)
		{
			if (gameObject != null)
			{
				Health component = gameObject.GetComponent<Health>();
				if ((bool)component)
				{
					component.OnDeathSignal.AddReceiver(OnEnemyDeath);
				}
			}
		}
		gameUnitManager = GameObject.Find("GameUnitManager").GetComponent<GameUnitManager>();
	}

	public override void OnStart()
	{
		if (EnemiesToKill == 0)
		{
			EnemiesToKill = Enemies.Length;
		}
		GameObject[] enemies = Enemies;
		foreach (GameObject gameObject in enemies)
		{
			if (gameObject != null)
			{
				Health component = gameObject.GetComponent<Health>();
				component.ResetHealth();
			}
		}
		enemiesKilled = 0;
		hasStarted = true;
	}

	private void OnEnemyDeath(Health health)
	{
		lastDied = health.transform.position;
		kills++;
		if (!hasStarted)
		{
			SpawnNewEnemy();
			return;
		}
		if (MustAim)
		{
			AimController aimController = GetAimController();
			if (aimController.IsAiming())
			{
				enemiesKilled++;
			}
			else if (!CheckComplete())
			{
				if (kills % 2 == 0)
				{
					TutorialManager.RunSpeech(Localizer.GetString(FailMessageKey), 0.02f, null);
				}
				if (FailMessageSpeechLine != TutorialSpeechLine.None)
				{
					TutorialSpeechManager.Play(FailMessageSpeechLine);
				}
				SpawnNewEnemy();
			}
		}
		else
		{
			enemiesKilled++;
		}
		needsUpdate = true;
	}

	private bool CheckComplete()
	{
		return enemiesKilled >= EnemiesToKill;
	}

	private void Update()
	{
		if (CheckComplete())
		{
			SetComplete(true);
		}
	}

	private void SpawnNewEnemy()
	{
		if (RespawnEnemiesIfNotAiming)
		{
			GameObject gameObject = gameUnitManager.CreateUnit("Clot", RespawnSpawnPoint.transform.position, Quaternion.identity);
			gameObject.GetComponent<AITargetHandler>().TargetTags = new string[1] { string.Empty };
			Health component = gameObject.GetComponent<Health>();
			component.OnDeathSignal.AddReceiver(OnEnemyDeath);
			TutorialInvisibility tutorialInvisibility = gameObject.AddComponent<TutorialInvisibility>();
			tutorialInvisibility.InvisibilityTimer = 1.5f;
			if (GotoKilledUnitPosition)
			{
				AIMovementMotor component2 = gameObject.GetComponent<AIMovementMotor>();
				component2.StoppingDistance = 0.1f;
				component2.MoveToTargetPosition(lastDied + LastKilledOffset);
			}
			if (InvulnerableOnSpawn)
			{
				component.Invulnerable = true;
			}
		}
	}

	private AimController GetAimController()
	{
		return TutorialManager.Soldier.GetComponent<AimController>();
	}

	public int GetCurrentCount()
	{
		return enemiesKilled;
	}

	public int GetMaxCount()
	{
		return EnemiesToKill;
	}

	bool IMissionCounter.IsComplete
	{
		get
		{
			return base.IsComplete;
		}
	}
}
