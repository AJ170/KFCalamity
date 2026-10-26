using System;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
	[Serializable]
	public class EffectPoolEntry
	{
		public GameObject Prefab;

		public int Count;

		private GameObject[] pool;

		private int index;

		public void Initialize(Transform parent)
		{
			pool = new GameObject[Count];
			for (int i = 0; i < Count; i++)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Prefab, Vector3.zero, Quaternion.identity);
				gameObject.transform.parent = parent;
				gameObject.name = Prefab.name;
				gameObject.SetActive(false);
				pool[i] = gameObject;
			}
		}

		public GameObject GetNextObject()
		{
			GameObject result = null;
			if (index <= Count)
			{
				result = pool[index];
				index = (index + 1) % Count;
			}
			return result;
		}
	}

	public EffectPoolEntry[] Pools;

	private static EffectPool instance;

	private void Awake()
	{
		InitializePools();
		instance = this;
	}

	private void InitializePools()
	{
		EffectPoolEntry[] pools = Pools;
		foreach (EffectPoolEntry effectPoolEntry in pools)
		{
			if (effectPoolEntry.Prefab != null)
			{
				GameObject gameObject = new GameObject(effectPoolEntry.Prefab.name);
				gameObject.transform.parent = base.transform;
				effectPoolEntry.Initialize(gameObject.transform);
			}
		}
	}

	public static GameObject GetEffect(GameObject prefab)
	{
		GameObject result = null;
		if (instance != null)
		{
			for (int i = 0; i < instance.Pools.Length; i++)
			{
				EffectPoolEntry effectPoolEntry = instance.Pools[i];
				if (effectPoolEntry.Prefab == prefab)
				{
					result = effectPoolEntry.GetNextObject();
					break;
				}
			}
		}
		return result;
	}
}
