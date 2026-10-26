using EventHelpers;
using GameEvents;
using Spawning;
using UnityEngine;

public class ScalingByGameSettings
{
	private static ScalingByGameSettings instance;

	private GameSettings gameSettings;

	private EventSystem eventSystem;

	public ScalingByGameSettings(GameSettings gameSettings, EventSystem eventSystem)
	{
		this.gameSettings = gameSettings;
		this.eventSystem = eventSystem;
		eventSystem.Subscribe(EventId.CreateUnit, OnGameEvent, this);
		eventSystem.Subscribe(EventId.WaveCreate, OnGameEvent, this);
		instance = this;
	}

	public void Shutdown()
	{
		eventSystem.Unsubscribe(this);
		instance = null;
	}

	private void OnGameEvent(EventBase e)
	{
		if (e.Id == EventId.CreateUnit)
		{
			CreateUnit createUnit = e as CreateUnit;
			GameObject unitObject = createUnit.UnitObject;
			if (unitObject.tag == "Soldier")
			{
				ScaleSoldierAttributes(unitObject);
			}
			else if (unitObject.tag == "Zed")
			{
				ScaleZedAttributes(unitObject);
			}
		}
		else if (e.Id == EventId.WaveCreate)
		{
			WaveCreate waveCreate = e as WaveCreate;
			ScaleWaveAttributes(waveCreate.Wave);
		}
	}

	private void ScaleSoldierAttributes(GameObject soldier)
	{
		AttributeModifier attributeModifier = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.MoneyReceived, soldier);
		if (attributeModifier != null)
		{
			attributeModifier.Alter(this, PlayerIncomeByDifficulty(gameSettings.Difficulty) * PlayerIncomeByNumPlayers(gameSettings.NumPlayers));
		}
		AttributeModifier attributeModifier2 = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.ExperienceReceived, soldier);
		if (attributeModifier2 != null)
		{
			attributeModifier2.Alter(this, PlayerExperienceByDifficulty(gameSettings.Difficulty));
		}
	}

	private void ScaleZedAttributes(GameObject zed)
	{
		AttributeModifier attributeModifier = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Health, zed);
		if (attributeModifier != null)
		{
			attributeModifier.Alter(this, ZedHealthByDifficulty(gameSettings.Difficulty) * ZedHealthByNumPlayers(gameSettings.NumPlayers));
		}
		AttributeModifier attributeModifier2 = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageDealt, zed);
		if (attributeModifier2 != null)
		{
			attributeModifier2.Alter(this, ZedDamageByDifficulty(gameSettings.Difficulty));
		}
		AttributeModifier attributeModifier3 = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, zed);
		if (attributeModifier3 != null)
		{
			attributeModifier3.Alter(this, ZedSpeedByDifficulty(gameSettings.Difficulty));
		}
	}

	private void ScaleWaveAttributes(Wave wave)
	{
		float scale = WaveUnitsAtOnceByDifficulty(gameSettings.Difficulty) * WaveUnitsAtOnceByNumPlayers(gameSettings.NumPlayers);
		wave.ScaleEnemiesAtOnce(scale);
		float scale2 = WaveLengthByNumPlayers(gameSettings.NumPlayers);
		wave.ScaleTotalEnemies(scale2);
	}

	public static float SpawnCooldownScaling(bool scaleByDifficulty, bool scaleByNumPlayers)
	{
		float num = ((!scaleByDifficulty) ? 1f : SpawnCooldownByDifficulty());
		float num2 = ((!scaleByNumPlayers) ? 1f : SpawnCooldownByNumPlayers());
		return num * num2;
	}

	public static float ZedHealthByDifficulty(DifficultySetting difficulty)
	{
		switch (difficulty)
		{
		case DifficultySetting.Beginner:
			return 1f;
		case DifficultySetting.Hard:
			return 5f;
		case DifficultySetting.Hell:
			return 10f;
		default:
			return 1.5f;
		}
	}

	public static float ZedDamageByDifficulty(DifficultySetting difficulty)
	{
		switch (difficulty)
		{
		case DifficultySetting.Beginner:
			return 0.5f;
		case DifficultySetting.Hard:
			return 2f;
		case DifficultySetting.Hell:
			return 3f;
		default:
			return 1f;
		}
	}

	public static float ZedSpeedByDifficulty(DifficultySetting difficulty)
	{
		switch (difficulty)
		{
		case DifficultySetting.Beginner:
			return 0.85f;
		case DifficultySetting.Hard:
			return 1.2f;
		case DifficultySetting.Hell:
			return 1.35f;
		default:
			return 1f;
		}
	}

	public static float PlayerIncomeByDifficulty(DifficultySetting difficulty)
	{
		switch (difficulty)
		{
		case DifficultySetting.Beginner:
			return 1f;
		case DifficultySetting.Hard:
			return 0.6f;
		case DifficultySetting.Hell:
			return 0.4f;
		default:
			return 1f;
		}
	}

	public static float PlayerExperienceByDifficulty(DifficultySetting difficulty)
	{
		switch (difficulty)
		{
		case DifficultySetting.Beginner:
			return 0.5f;
		case DifficultySetting.Hard:
			return 1.75f;
		case DifficultySetting.Hell:
			return 2.25f;
		default:
			return 1f;
		}
	}

	public static float WaveUnitsAtOnceByDifficulty(DifficultySetting difficulty)
	{
		switch (difficulty)
		{
		case DifficultySetting.Beginner:
			return 1f;
		case DifficultySetting.Hard:
			return 1f;
		case DifficultySetting.Hell:
			return 1f;
		default:
			return 1f;
		}
	}

	public static float ProjectileSpeedByDifficulty()
	{
		switch (Difficulty())
		{
		case DifficultySetting.Hard:
			return 1.25f;
		case DifficultySetting.Hell:
			return 1.5f;
		default:
			return 1f;
		}
	}

	public static float SpawnCooldownByDifficulty()
	{
		switch (Difficulty())
		{
		case DifficultySetting.Beginner:
			return 1.5f;
		case DifficultySetting.Hard:
			return 0.8f;
		case DifficultySetting.Hell:
			return 0.6f;
		default:
			return 1f;
		}
	}

	public static float SelfDamageByDifficulty()
	{
		switch (Difficulty())
		{
		case DifficultySetting.Beginner:
			return 0.1f;
		case DifficultySetting.Hard:
			return 0.5f;
		case DifficultySetting.Hell:
			return 1f;
		default:
			return 0.25f;
		}
	}

	public static int SyringeModifierByDifficulty()
	{
		switch (Difficulty())
		{
		case DifficultySetting.Hard:
			return -1;
		case DifficultySetting.Hell:
			return -2;
		default:
			return 0;
		}
	}

	public static float GorefastChargeSpeedModifierByDifficuity()
	{
		switch (Difficulty())
		{
		case DifficultySetting.Hard:
			return 1.4f;
		case DifficultySetting.Hell:
			return 1.5f;
		default:
			return 1.3f;
		}
	}

	public static float GorefastChargeDamageModifierByDifficuity()
	{
		switch (Difficulty())
		{
		case DifficultySetting.Hard:
			return 0.5f;
		case DifficultySetting.Hell:
			return 0.4f;
		default:
			return 0.6f;
		}
	}

	public static float BloatExplodeRadiusModifierByDifficulty()
	{
		switch (Difficulty())
		{
		case DifficultySetting.Hard:
			return 1.1f;
		case DifficultySetting.Hell:
			return 1.2f;
		default:
			return 1f;
		}
	}

	public static float WaveLengthByGameLength(GameLengthSetting gameLength)
	{
		switch (gameLength)
		{
		case GameLengthSetting.Short:
			return 1f;
		case GameLengthSetting.Long:
			return 1f;
		default:
			return 1f;
		}
	}

	public static float WaveUnitsAtOnceByNumPlayers(int numPlayers)
	{
		switch (numPlayers)
		{
		case 2:
			return 1f;
		case 3:
			return 1f;
		case 4:
			return 1f;
		default:
			return 1f;
		}
	}

	public static float WaveLengthByNumPlayers(int numPlayers)
	{
		switch (numPlayers)
		{
		case 2:
			return 1.2f;
		case 3:
			return 1.35f;
		case 4:
			return 1.5f;
		default:
			return 1f;
		}
	}

	public static float ZedHealthByNumPlayers(int numPlayers)
	{
		switch (numPlayers)
		{
		case 2:
			return 1.4f;
		case 3:
			return 1.6f;
		case 4:
			return 1.8f;
		default:
			return 1f;
		}
	}

	public static float PlayerIncomeByNumPlayers(int numPlayers)
	{
		switch (numPlayers)
		{
		case 2:
			return 1.65f;
		case 3:
			return 2f;
		case 4:
			return 2.25f;
		default:
			return 1f;
		}
	}

	public static float SpawnCooldownByNumPlayers()
	{
		switch (NumPlayers())
		{
		case 2:
			return 0.9f;
		case 3:
			return 0.8f;
		case 4:
			return 0.7f;
		default:
			return 1f;
		}
	}

	public static float ZedTimeMeterThresholdByGameMode()
	{
		GameModeSetting gameModeSetting = GameMode();
		if (gameModeSetting == GameModeSetting.Tutorial)
		{
			return 0.43f;
		}
		return 1f;
	}

	private static DifficultySetting Difficulty()
	{
		DifficultySetting result = DifficultySetting.Normal;
		if (instance != null && instance.gameSettings != null)
		{
			result = instance.gameSettings.Difficulty;
		}
		return result;
	}

	private static int NumPlayers()
	{
		int result = 1;
		if (instance != null && instance.gameSettings != null)
		{
			result = instance.gameSettings.NumPlayers;
		}
		return result;
	}

	private static GameModeSetting GameMode()
	{
		GameModeSetting result = GameModeSetting.None;
		if (instance != null && instance.gameSettings != null)
		{
			result = instance.gameSettings.GameMode;
		}
		return result;
	}
}
