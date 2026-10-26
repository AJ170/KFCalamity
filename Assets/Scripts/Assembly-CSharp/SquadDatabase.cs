using System.Collections.Generic;
using Spawning;
using UnityEngine;

public class SquadDatabase
{
	private List<WaveSquad> squadDatabase = new List<WaveSquad>();

	public SquadDatabase()
	{
		SetupDatabase();
	}

	private void SetupDatabase()
	{
		AddSquad(false, ZedType.Clot);
		AddSquad(true, ZedType.Clot, ZedType.Clot, ZedType.Clot);
		AddSquad(false, ZedType.Clot, ZedType.Clot, ZedType.Clot, ZedType.Clot);
		AddSquad(false, ZedType.Clot, ZedType.Clot, ZedType.Clot, ZedType.Gorefast);
		AddSquad(false, ZedType.Clot, ZedType.Gorefast, ZedType.Bloat);
		AddSquad(true, ZedType.Gorefast, ZedType.Gorefast);
		AddSquad(true, ZedType.Clot, ZedType.Clot, ZedType.Bloat);
		AddSquad(true, ZedType.Crawler, ZedType.Crawler);
		AddSquad(false, ZedType.Clot, ZedType.Gorefast, ZedType.Crawler);
		AddSquad(false, ZedType.Gorefast, ZedType.Gorefast, ZedType.Crawler);
		AddSquad(true, ZedType.Clot, ZedType.Siren);
		AddSquad(true, ZedType.Clot, ZedType.Gorefast, ZedType.Siren);
		AddSquad(true, ZedType.Clot, ZedType.Clot, ZedType.Stalker);
		AddSquad(true, ZedType.Stalker);
		AddSquad(true, ZedType.Stalker, ZedType.Stalker);
		AddSquad(true, ZedType.Husk);
		AddSquad(true, ZedType.Gorefast, ZedType.Husk);
		AddSquad(true, ZedType.Scrake);
		AddSquad(true, ZedType.Clot, ZedType.Scrake);
		AddSquad(true, ZedType.Fleshpound);
		AddSquad(false, ZedType.Clot, ZedType.Gorefast, ZedType.Fleshpound);
		AddSquad(true, ZedType.Patriarch);
		AddSquad(false, ZedType.Bloat, ZedType.Bloat, ZedType.Bloat);
	}

	private void AddSquad(bool spawnAsWhole, params ZedType[] zeds)
	{
		if (zeds.Length > 0)
		{
			squadDatabase.Add(new WaveSquad(zeds, spawnAsWhole));
		}
		else
		{
			Debug.LogError("SquadManager:ValidateDatabase() Squad does not contain any units!");
		}
	}

	private void AddSquad(bool spawnAsWhole, bool discardOverflow, params ZedType[] zeds)
	{
		if (zeds.Length > 0)
		{
			squadDatabase.Add(new WaveSquad(zeds, spawnAsWhole, discardOverflow));
		}
		else
		{
			Debug.LogError("SquadManager:ValidateDatabase() Squad does not contain any units!");
		}
	}

	public WaveSquad SquadByIndex(int index)
	{
		if (0 < index && index < squadDatabase.Count)
		{
			return squadDatabase[index];
		}
		return null;
	}
}
