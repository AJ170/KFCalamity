using System;
using System.Collections.Generic;
using GameEvents;
using Spawning;

[Serializable]
public class BossMiniWave
{
	public List<int> Squads;

	public int TotalEnemeis;

	public int MaxEnemiesAtOnce;

	private Wave wave;

	public Wave Wave
	{
		get
		{
			return wave;
		}
	}

	public void Setup()
	{
		if (TotalEnemeis > 0 && MaxEnemiesAtOnce > 0 && Squads.Count > 0)
		{
			wave = new Wave(Squads, TotalEnemeis, MaxEnemiesAtOnce);
			if (EventSystem.Instance != null)
			{
				EventSystem.Instance.Trigger(new WaveCreate(wave));
			}
		}
	}
}
