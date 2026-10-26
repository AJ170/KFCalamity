using System;
using System.Collections.Generic;
using EventHelpers;
using GameEvents;
using Spawning;
using UnityEngine;

[RequireComponent(typeof(SpawnPointManager))]
public class SpawnManager : MonoBehaviour
{
	private static SpawnManager instance;

	public float SpawnCooldownBase = 1f;

	public bool UseSineWaveScaling = true;

	public float SineWaveFrequency = 0.025f;

	public float SineWaveScale = 2f;

	public bool ResetWaveTimerOnNewWave = true;

	public bool UsePlayerNumberScaling = true;

	public bool UseDifficultyScaling = true;

	public bool DebugMode;

	private SpawnPointManager spawnPointManager;

	private UnitSpawnManager unitSpawnManager;

	private SquadManager squadManager;

	private SpawnedUnitTracker spawnedUnitTracker;

	private RelocateManager relocateManager;

	private EventSystem eventSystem;

	private Wave activeWave;

	private WaveInfo activeWaveInfo;

	private List<ZedType> activeSquad;

	private float cooldownDifficultyAndPlayerScaling = 1f;

	private float cooldownCounter;

	private float waveTimeCounter;

	public static SpawnManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		if (instance != null)
		{
			UnityEngine.Object.Destroy(instance.gameObject);
		}
		instance = this;
	}

	private void Start()
	{
		spawnPointManager = GetComponent<SpawnPointManager>();
		unitSpawnManager = GetComponent<UnitSpawnManager>();
		spawnedUnitTracker = GetComponent<SpawnedUnitTracker>();
		relocateManager = GetComponent<RelocateManager>();
		squadManager = new SquadManager();
		cooldownDifficultyAndPlayerScaling = ScalingByGameSettings.SpawnCooldownScaling(UseDifficultyScaling, UsePlayerNumberScaling);
		eventSystem = EventSystem.Instance;
		if (eventSystem != null)
		{
			eventSystem.Subscribe(EventId.WaveStart, OnGameEvent, this);
			eventSystem.Subscribe(EventId.DestroyUnit, OnGameEvent, this);
		}
		SetupSineWave();
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
		if (eventSystem != null)
		{
			eventSystem.Unsubscribe(EventId.WaveStart, this);
			eventSystem.Unsubscribe(EventId.DestroyUnit, this);
		}
	}

	private void Update()
	{
		if (activeWave == null)
		{
			return;
		}
		cooldownCounter -= Time.deltaTime;
		waveTimeCounter += Time.deltaTime;
		if (!(cooldownCounter <= 0f) || WaitingForRating())
		{
			return;
		}
		if (!SquadPrepared() && AnyUnitsLeftToSpawn() && CapacityForMoreUnits())
		{
			PrepareNextSquad();
		}
		if (SquadPrepared() && (CapacityForSquad() || CheckSplitActiveSquad()))
		{
			if (CheckSplitActiveSquad())
			{
				SplitActiveSquad();
			}
			PrepareNextSpawn();
		}
		else if (relocateManager.AnyUnitNeedsRelocating())
		{
			relocateManager.PrepareRelocation();
			TriggerCooldown();
		}
		else
		{
			TriggerSleep();
		}
	}

	public void OnGameEvent(EventBase e)
	{
		switch (e.Id)
		{
		case EventId.WaveStart:
		{
			WaveStart waveStart = (WaveStart)e;
			StartWave(waveStart.Wave);
			break;
		}
		case EventId.DestroyUnit:
		{
			DestroyUnit destroyUnit = (DestroyUnit)e;
			OnUnitDestroyed(destroyUnit.UnitObject);
			break;
		}
		}
	}

	private void StartWave(Wave wave)
	{
		if (activeWave != null)
		{
			Reset();
		}
		squadManager.SetupWaveSquads(wave);
		activeWaveInfo = new WaveInfo(wave);
		activeWave = wave;
	}

	private void Reset()
	{
		activeWave = null;
		activeWaveInfo = null;
		activeSquad = null;
		spawnedUnitTracker.Clear();
		squadManager.Clear();
		cooldownCounter = 0f;
		if (ResetWaveTimerOnNewWave)
		{
			waveTimeCounter = 0f;
		}
	}

	private void PrepareNextSpawn()
	{
		if (activeSquad != null)
		{
			RatingData ratingData = new RatingData(activeSquad, SpawnRatingCallback);
			spawnPointManager.StartRating(ratingData);
			TriggerCooldown();
		}
	}

	private void SpawnRatingCallback(SpawnPoint spawnPoint, Vector3 direction)
	{
		if (spawnPoint != null && activeSquad != null)
		{
			Quaternion rotation = Quaternion.LookRotation(direction);
			unitSpawnManager.AddSpawns(activeSquad, spawnPoint.Position, rotation);
		}
		activeSquad = null;
	}

	public void OnUnitSpawned(GameObject gameObject, ZedType type)
	{
		activeWaveInfo.UnitsSpawned++;
		activeWaveInfo.UnitsLeftToSpawn--;
		spawnedUnitTracker.AddUnit(gameObject, type);
	}

	private void OnUnitDestroyed(GameObject gameObject)
	{
		if (spawnedUnitTracker.RemoveUnit(gameObject))
		{
			activeWaveInfo.UnitsKilled++;
			if (GuiInGameManager.Instance != null)
			{
				GuiInGameManager.Instance.SetEnemiesLeft(activeWaveInfo.TotalUnitsInWave - activeWaveInfo.UnitsKilled);
			}
			CheckAllUnitsKilled();
		}
	}

	private bool PrepareNextSquad()
	{
		activeSquad = squadManager.GetNextSquad(UnitCapacity());
		return activeSquad != null;
	}

	private void SplitActiveSquad()
	{
		if (activeSquad != null)
		{
			int num = SplitThreshold();
			int num2 = activeSquad.Count - num;
			if (num2 > 0)
			{
				activeSquad.RemoveRange(num, num2);
			}
		}
	}

	private bool WaitingForRating()
	{
		return spawnPointManager.IsBusy();
	}

	private bool AnyUnitsLeftToSpawn()
	{
		return NumberOfUnitsLeftToSpawn() > 0;
	}

	private int NumberOfUnitsLeftToSpawn()
	{
		return activeWaveInfo.UnitsLeftToSpawn;
	}

	private bool CapacityForMoreUnits()
	{
		return spawnedUnitTracker.UnitsAlive < activeWaveInfo.MaxUnitsAtOnce;
	}

	private bool CapacityForSquad()
	{
		return activeSquad.Count <= UnitCapacity();
	}

	private int UnitCapacity()
	{
		return Mathf.Min(activeWaveInfo.MaxUnitsAtOnce - spawnedUnitTracker.UnitsAlive, NumberOfUnitsLeftToSpawn());
	}

	private bool SquadPrepared()
	{
		return activeSquad != null;
	}

	private bool CheckSplitActiveSquad()
	{
		if (activeSquad != null && activeSquad.Count > SplitThreshold())
		{
			return true;
		}
		return false;
	}

	private int SplitThreshold()
	{
		if (activeSquad != null)
		{
			return Mathf.Min(NumberOfUnitsLeftToSpawn(), activeWaveInfo.MaxUnitsAtOnce);
		}
		return 1;
	}

	private void TriggerCooldown()
	{
		float spawnCooldownBase = SpawnCooldownBase;
		float num = cooldownDifficultyAndPlayerScaling;
		float waveNumberCooldownModifier = GetWaveNumberCooldownModifier();
		spawnCooldownBase *= num * waveNumberCooldownModifier;
		if (UseSineWaveScaling)
		{
			float sineWaveCooldownModifier = GetSineWaveCooldownModifier();
			spawnCooldownBase += sineWaveCooldownModifier * (spawnCooldownBase * SineWaveScale);
		}
		cooldownCounter = spawnCooldownBase;
	}

	private void TriggerSleep()
	{
		cooldownCounter = 0.25f;
	}

	private float GetSineWaveCooldownModifier()
	{
		return 1f - Mathf.Abs(Mathf.Sin(waveTimeCounter * SineWaveFrequency));
	}

	private float GetWaveNumberCooldownModifier()
	{
		return 1f;
	}

	private void SetupSineWave()
	{
		SineWaveFrequency *= (float)Math.PI / 2f;
	}

	public List<SpawnedUnit> GetSpawnedObjectes()
	{
		return spawnedUnitTracker.GetTrackedUnits();
	}

	public void AddUnitToQueue(ZedType unit)
	{
		squadManager.AddToUnitQueue(unit);
	}

	public void AddSquadToQueue(int squad)
	{
		squadManager.AddToSquadQueue(squad);
	}

	private void CheckAllUnitsKilled()
	{
		if (spawnedUnitTracker.UnitsAlive == 0 && !AnyUnitsLeftToSpawn())
		{
			Reset();
			if (eventSystem != null)
			{
				eventSystem.Trigger(new WaveUnitsKilled());
			}
		}
	}

	public void AddBossMiniWave(BossMiniWave bossWave)
	{
		if (activeWave != null && activeWaveInfo != null)
		{
			Wave wave = bossWave.Wave;
			if (wave != null)
			{
				activeWaveInfo.UnitsLeftToSpawn += wave.TotalEnemies;
				activeWaveInfo.MaxUnitsAtOnce = activeWaveInfo.Wave.MaxEnemiesAtOnce + wave.MaxEnemiesAtOnce;
				squadManager.SetupWaveSquads(wave);
			}
		}
	}

	public void AbortWave()
	{
		if (activeWave == null || activeWaveInfo == null)
		{
			return;
		}
		if (spawnPointManager.IsBusy())
		{
			spawnPointManager.AbortRating();
		}
		squadManager.Clear();
		activeWaveInfo.UnitsLeftToSpawn = 0;
		spawnedUnitTracker.DestroyAllUnits();
		if (activeWave != null)
		{
			Reset();
			if (eventSystem != null)
			{
				eventSystem.Trigger(new WaveUnitsKilled());
			}
		}
	}
}
