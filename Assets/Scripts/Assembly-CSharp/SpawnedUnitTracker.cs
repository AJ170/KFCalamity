using System.Collections.Generic;
using Spawning;
using UnityEngine;

public class SpawnedUnitTracker : MonoBehaviour
{
	private List<SpawnedUnit> trackedUnits = new List<SpawnedUnit>();

	public int UnitsAlive
	{
		get
		{
			return trackedUnits.Count;
		}
	}

	public void AddUnit(GameObject gameObject, ZedType type)
	{
		trackedUnits.Add(new SpawnedUnit(gameObject, type));
	}

	public bool RemoveUnit(GameObject gameObject)
	{
		for (int i = 0; i < trackedUnits.Count; i++)
		{
			if (trackedUnits[i].GameObject == gameObject)
			{
				trackedUnits.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public List<SpawnedUnit> GetTrackedUnits()
	{
		return trackedUnits;
	}

	public SpawnedUnit GetTrackedUnit(GameObject gameObject)
	{
		SpawnedUnit spawnedUnit = null;
		for (int i = 0; i < trackedUnits.Count; i++)
		{
			spawnedUnit = trackedUnits[i];
			if (spawnedUnit.GameObject == gameObject)
			{
				return spawnedUnit;
			}
		}
		return null;
	}

	public void DestroyAllUnits()
	{
		GameUnitManager instance = GameUnitManager.Instance;
		if (instance != null)
		{
			for (int num = trackedUnits.Count - 1; num >= 0; num--)
			{
				SpawnedUnit spawnedUnit = trackedUnits[num];
				if (spawnedUnit.GameObject != null)
				{
					instance.DestroyUnit(spawnedUnit.GameObject);
				}
			}
		}
		Clear();
	}

	public void Clear()
	{
		trackedUnits.Clear();
	}
}
