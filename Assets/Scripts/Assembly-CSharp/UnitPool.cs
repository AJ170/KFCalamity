using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitPool : MonoBehaviour
{
	[Serializable]
	public class UnitPoolEntry
	{
		public GameObject Prefab;

		public int Count;
	}

	public UnitPoolEntry[] Entries = new UnitPoolEntry[0];

	private List<GameObject>[] Pool;

	private void Start()
	{
		Pool = new List<GameObject>[Entries.Length];
		for (int i = 0; i < Entries.Length; i++)
		{
			UnitPoolEntry unitPoolEntry = Entries[i];
			GameObject prefab = unitPoolEntry.Prefab;
			int count = unitPoolEntry.Count;
			Pool[i] = new List<GameObject>(count);
			for (int j = 0; j < count; j++)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(prefab);
				gameObject.name = prefab.name;
				AddToPool(gameObject);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if (Pool == null)
		{
			return;
		}
		for (int num = Pool.Length - 1; num >= 0; num--)
		{
			List<GameObject> list = Pool[num];
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				UnityEngine.Object.Destroy(list[num2]);
			}
			list.Clear();
		}
	}

	public GameObject GetUnit(string typeName)
	{
		return GetUnit(typeName, true);
	}

	public GameObject GetUnit(string typeName, bool onlyPooled)
	{
		for (int i = 0; i < Entries.Length; i++)
		{
			GameObject prefab = Entries[i].Prefab;
			if (prefab.name == typeName)
			{
				if (Pool[i].Count > 0)
				{
					List<GameObject> list = Pool[i];
					int index = list.Count - 1;
					GameObject gameObject = list[index];
					list.RemoveAt(index);
					gameObject.transform.parent = null;
					gameObject.SetActive(true);
					return gameObject;
				}
				if (!onlyPooled)
				{
					return (GameObject)UnityEngine.Object.Instantiate(prefab);
				}
			}
		}
		Debug.LogWarning(string.Format("[UnitPool:GetUnit] Failed to locate prefab {0}.", typeName));
		return null;
	}

	public void AddToPool(GameObject instance)
	{
		for (int i = 0; i < Entries.Length; i++)
		{
			if (Entries[i].Prefab.name == instance.name)
			{
				instance.SetActive(false);
				instance.transform.parent = base.gameObject.transform;
				Pool[i].Add(instance);
				return;
			}
		}
		Debug.LogWarning(string.Format("[UnitPool:AddToPool] Failed to add unit {0} to pool: prefab not found.", instance.name));
	}
}
