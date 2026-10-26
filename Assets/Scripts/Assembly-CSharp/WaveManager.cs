using System.Collections.Generic;
using GameEvents;
using Spawning;
using UnityEngine;

public class WaveManager
{
	private static WaveManager instance;

	private EventSystem eventSystem;

	private Dictionary<string, Wave> templateWaves = new Dictionary<string, Wave>();

	private List<Wave> waves = new List<Wave>();

	private int waveCounter = -1;

	public static WaveManager Instance
	{
		get
		{
			return instance;
		}
	}

	public WaveManager(EventSystem eventSystem)
	{
		this.eventSystem = eventSystem;
		instance = this;
	}

	public void Shutdown()
	{
		instance = null;
	}

	public void SetupTemplateWaves(GameModeSetting gameMode)
	{
		switch (gameMode)
		{
		case GameModeSetting.Survival:
			AddTemplateWave("wave1", new Wave(new List<int> { 1, 1, 2, 2, 6 }, 20, 8));
			AddTemplateWave("wave2", new Wave(new List<int>
			{
				1, 1, 1, 1, 2, 2, 2, 2, 3, 3,
				4, 5, 5, 6, 7, 7
			}, 40, 10));
			AddTemplateWave("wave3", new Wave(new List<int>
			{
				1, 1, 1, 1, 2, 2, 2, 2, 3, 3,
				3, 3, 5, 5, 7, 7, 8, 8, 10, 10,
				11, 11
			}, 50, 10));
			AddTemplateWave("wave4", new Wave(new List<int>
			{
				2, 3, 3, 5, 5, 5, 5, 5, 5, 5,
				5, 7, 9, 9, 9, 9
			}, 40, 8));
			AddTemplateWave("wave5", new Wave(new List<int>
			{
				1, 1, 1, 1, 1, 1, 2, 2, 2, 2,
				3, 3, 3, 3, 5, 5, 10, 11, 12, 12,
				13, 13, 14, 14
			}, 50, 10));
			AddTemplateWave("wave6", new Wave(new List<int>
			{
				1, 1, 1, 1, 1, 2, 2, 2, 2, 2,
				3, 3, 3, 3, 5, 5, 5, 5, 7, 7,
				8, 8, 9, 9, 18, 18
			}, 60, 10));
			AddTemplateWave("wave7", new Wave(new List<int>
			{
				1, 1, 1, 2, 2, 2, 3, 3, 3, 4,
				6, 7, 7, 8, 9, 9, 10, 13, 14, 15,
				16, 18
			}, 60, 10));
			AddTemplateWave("wave8", new Wave(new List<int>
			{
				1, 1, 2, 2, 6, 6, 6, 6, 6, 6,
				6, 6, 6, 10, 13, 13, 14, 14, 14, 14,
				22, 22
			}, 50, 10));
			AddTemplateWave("wave9", new Wave(new List<int>
			{
				1, 1, 1, 2, 2, 2, 2, 7, 7, 7,
				7, 7, 7, 7, 7, 7, 7, 7, 7, 12,
				12, 13, 14, 17
			}, 80, 10));
			AddTemplateWave("wave10", new Wave(new List<int>
			{
				1, 1, 1, 1, 2, 2, 2, 3, 3, 5,
				5, 7, 8, 20
			}, 80, 10));
			AddTemplateWave("wave11", new Wave(new List<int>
			{
				3, 3, 3, 3, 5, 5, 5, 5, 5, 5,
				7, 7, 9, 9, 15, 16
			}, 60, 8));
			AddTemplateWave("wave12", new Wave(new List<int>
			{
				1, 1, 1, 1, 2, 2, 2, 3, 3, 4,
				4, 4, 5, 5, 6, 7, 7, 10, 11, 12,
				12, 19
			}, 100, 10));
			AddTemplateWave("wave13", new Wave(new List<int>
			{
				1, 1, 2, 2, 3, 5, 7, 17, 17, 18,
				19, 19
			}, 30, 6));
			AddTemplateWave("wave14", new Wave(new List<int>
			{
				1, 1, 1, 2, 2, 2, 3, 3, 3, 4,
				6, 7, 7, 8, 9, 9, 10, 13, 14, 16,
				18, 19
			}, 100, 10));
			AddTemplateWave("wave15", new Wave(new List<int>
			{
				1, 1, 1, 2, 2, 2, 3, 3, 3, 4,
				6, 7, 7, 8, 9, 9, 10, 13, 14, 16,
				18, 19, 1, 1, 1, 2, 2, 2, 3, 3,
				3, 4, 6, 7, 7, 8, 9, 9, 10, 13,
				14, 16, 18
			}, 100, 10));
			AddTemplateWave("wave16", new Wave(new List<int>
			{
				1, 1, 2, 3, 3, 4, 5, 6, 6, 6,
				7, 7, 8, 9, 10, 13, 14, 16, 18, 19
			}, 80, 10));
			AddTemplateWave("boss", new Wave(new List<int> { 21 }, 1, 1, true));
			break;
		case GameModeSetting.Tutorial:
			AddTemplateWave("wave1", new Wave(new List<int> { 1, 1, 1, 1, 1, 2, 2, 2, 6 }, 20, 10));
			AddTemplateWave("wave2", new Wave(new List<int>
			{
				1, 1, 1, 1, 1, 2, 2, 2, 3, 3,
				6
			}, 50, 14));
			break;
		}
		ValidateWaves();
	}

	private void AddTemplateWave(string waveName, Wave wave)
	{
		if (wave != null)
		{
			templateWaves.Add(waveName, wave);
		}
	}

	private void ValidateWaves()
	{
		foreach (KeyValuePair<string, Wave> templateWave in templateWaves)
		{
			Wave value = templateWave.Value;
			if (value.Squads != null && value.Squads.Count == 0)
			{
				Debug.LogError("WaveManager:ValidateWaves() Wave contains no squads");
				break;
			}
		}
		if (templateWaves.Count == 0)
		{
			Debug.LogError("WaveManager:ValidateWaves() No template waves present");
		}
	}

	public void SetupSurvivalWaves(GameLengthSetting gameLength)
	{
		if (waves.Count == 0)
		{
			switch (gameLength)
			{
			case GameLengthSetting.Short:
				AddWave(templateWaves["wave1"]);
				AddWave(templateWaves["wave3"]);
				AddWave(templateWaves["wave12"]);
				AddWave(templateWaves["boss"]);
				break;
			case GameLengthSetting.Normal:
				AddWave(templateWaves["wave1"]);
				AddWave(templateWaves["wave3"]);
				AddWave(templateWaves["wave7"]);
				AddWave(templateWaves["wave11"]);
				AddWave(templateWaves["wave14"]);
				AddWave(templateWaves["boss"]);
				break;
			case GameLengthSetting.Long:
				AddWave(templateWaves["wave1"]);
				AddWave(templateWaves["wave2"]);
				AddWave(templateWaves["wave3"]);
				AddWave(templateWaves["wave4"]);
				AddWave(templateWaves["wave7"]);
				AddWave(templateWaves["wave8"]);
				AddWave(templateWaves["wave9"]);
				AddWave(templateWaves["wave11"]);
				AddWave(templateWaves["wave12"]);
				AddWave(templateWaves["wave14"]);
				AddWave(templateWaves["boss"]);
				break;
			}
		}
	}

	public void SetupTutorialWaves()
	{
		waveCounter = -1;
		waves.Clear();
		AddWave(templateWaves["wave1"]);
		AddWave(templateWaves["wave2"]);
	}

	private void AddWave(Wave wave)
	{
		if (wave != null)
		{
			waves.Add(wave);
			eventSystem.Trigger(new WaveCreate(wave));
		}
	}

	public Wave WaveByIndex(int index)
	{
		if (index < waves.Count)
		{
			return waves[index];
		}
		Debug.LogError("WaveManager:WaveByIndex -> Index " + index + " not found.");
		return null;
	}

	public int NumWaves()
	{
		return waves.Count;
	}

	public int CurrentWaveIndex()
	{
		return waveCounter;
	}

	public Wave CurrentWave()
	{
		if (waves.Count > 0 && waves.Count > waveCounter)
		{
			return waves[waveCounter];
		}
		return null;
	}

	public int NextWaveIndex()
	{
		return Mathf.Min(waveCounter + 1, NumWaves() - 1);
	}

	public void NextWave()
	{
		if (waves.Count > 0)
		{
			waveCounter = NextWaveIndex();
		}
	}

	public bool AtLastWave()
	{
		return CurrentWaveIndex() + 1 == NumWaves();
	}

	public bool IsBossWave(int index)
	{
		Wave wave = WaveByIndex(index);
		if (wave != null)
		{
			return wave.BossWave;
		}
		return false;
	}
}
