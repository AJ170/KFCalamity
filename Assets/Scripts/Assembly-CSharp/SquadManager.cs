using System.Collections.Generic;
using Spawning;
using UnityEngine;

public class SquadManager
{
	private SquadDatabase database;

	private List<int> templateWaveSquads;

	private List<int> activeWaveSquads;

	private QueuedSquad activeSquad;

	private Queue<ZedType> unitQueue = new Queue<ZedType>();

	private Queue<int> squadQueue = new Queue<int>();

	public SquadManager()
	{
		database = new SquadDatabase();
	}

	public void SetupWaveSquads(Wave wave)
	{
		templateWaveSquads = wave.Squads;
	}

	public List<ZedType> GetNextSquad(int maxUnits)
	{
		List<ZedType> list = null;
		if (activeSquad != null)
		{
			list = GetUnitsFromActiveSquad(maxUnits);
		}
		if (list == null && AnyUnitsQueued())
		{
			list = GetUnitsFromQueue(maxUnits);
		}
		if (list == null)
		{
			list = GetUnitsFromWaveSquads(maxUnits);
		}
		if (list == null)
		{
			Debug.Log("[SquadManager:GetNextSquad] Failed to get squad!");
		}
		return list;
	}

	private List<ZedType> GetUnitsFromWaveSquads(int maxUnits)
	{
		if (!SquadsAvailable())
		{
			SetupSquads();
		}
		if (SquadsAvailable())
		{
			int index = Random.Range(0, activeWaveSquads.Count);
			int index2 = activeWaveSquads[index];
			activeWaveSquads.RemoveAt(index);
			WaveSquad waveSquad = database.SquadByIndex(index2);
			if (waveSquad != null)
			{
				List<ZedType> result;
				if (waveSquad.Units.Length <= maxUnits || waveSquad.SpawnAsWhole)
				{
					result = new List<ZedType>(waveSquad.Units);
				}
				else
				{
					QueuedSquad queueClone = waveSquad.GetQueueClone();
					result = queueClone.GetUnits(maxUnits);
					if (!waveSquad.DiscardOverflow)
					{
						activeSquad = queueClone;
					}
				}
				return result;
			}
		}
		return null;
	}

	private void SetupSquads()
	{
		activeWaveSquads = new List<int>(templateWaveSquads);
	}

	private bool SquadsAvailable()
	{
		return activeWaveSquads != null && activeWaveSquads.Count > 0;
	}

	private List<ZedType> GetUnitsFromActiveSquad(int maxUnits)
	{
		if (activeSquad != null)
		{
			List<ZedType> result;
			if (activeSquad.Units.Length <= maxUnits)
			{
				result = new List<ZedType>(activeSquad.Units);
				activeSquad = null;
			}
			else
			{
				result = activeSquad.GetUnits(maxUnits);
			}
			return result;
		}
		return null;
	}

	public void AddToUnitQueue(ZedType type)
	{
		unitQueue.Enqueue(type);
	}

	public void AddToSquadQueue(int squadID)
	{
		squadQueue.Enqueue(squadID);
	}

	private List<ZedType> GetUnitsFromQueue(int maxUnits)
	{
		List<ZedType> result = null;
		if (unitQueue.Count > 0)
		{
			result = GetUnitsFromUnitQueue(maxUnits);
		}
		else if (squadQueue.Count > 0)
		{
			result = GetUnitsFromSquadQueue(maxUnits);
		}
		return result;
	}

	private List<ZedType> GetUnitsFromUnitQueue(int maxUnits)
	{
		int num = 1;
		if (unitQueue.Count > 5)
		{
			num = 2;
		}
		num = Mathf.Min(num, unitQueue.Count, maxUnits);
		if (num > 0)
		{
			List<ZedType> list = new List<ZedType>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(unitQueue.Dequeue());
			}
			return list;
		}
		return null;
	}

	private List<ZedType> GetUnitsFromSquadQueue(int maxUnits)
	{
		if (squadQueue.Count > 0)
		{
			int index = squadQueue.Dequeue();
			WaveSquad waveSquad = database.SquadByIndex(index);
			if (waveSquad != null)
			{
				List<ZedType> result;
				if (waveSquad.Units.Length <= maxUnits || waveSquad.SpawnAsWhole)
				{
					result = new List<ZedType>(waveSquad.Units);
				}
				else
				{
					QueuedSquad queueClone = waveSquad.GetQueueClone();
					result = queueClone.GetUnits(maxUnits);
					if (!waveSquad.DiscardOverflow)
					{
						activeSquad = queueClone;
					}
				}
				return result;
			}
		}
		return null;
	}

	public bool AnyUnitsQueued()
	{
		return unitQueue.Count + squadQueue.Count > 0;
	}

	public void Clear()
	{
		activeSquad = null;
		templateWaveSquads = null;
		activeWaveSquads = null;
		unitQueue.Clear();
		squadQueue.Clear();
	}
}
