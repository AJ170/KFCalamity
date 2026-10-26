using System.Collections.Generic;
using UnityEngine;

public class PatriarchKilled : MonoBehaviour
{
	private void Start()
	{
		Health component = GetComponent<Health>();
		component.OnDeathSignal.AddReceiver(OnDeath);
	}

	private void MakePlayersInvulnerable()
	{
		List<Player> allPlayers = PlayerManager.Instance.GetAllPlayers();
		foreach (Player item in allPlayers)
		{
			if (item.HasSoldier())
			{
				GameObject soldier = item.GetSoldier();
				Health component = soldier.GetComponent<Health>();
				if (!component.Dead)
				{
					component.lesserInvulnerability = true;
				}
			}
		}
	}

	private void SaveProgress()
	{
		MetaManager instance = MetaManager.Instance;
		if (instance != null)
		{
			instance.SaveAllData();
		}
	}

	private void OnDeath(Health health)
	{
		MakePlayersInvulnerable();
		SaveProgress();
	}

	private void OnDestroy()
	{
		if (SpawnManager.Instance != null)
		{
			SpawnManager.Instance.AbortWave();
		}
	}

	private void Update()
	{
	}
}
