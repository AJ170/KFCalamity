using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioSourcePool
{
	public GameObject AudioSourcePrefab;

	public int Count;

	private Transform parent;

	private List<GameObject> pool;

	private List<GameObject> busy;

	public void Initialize(Transform parent)
	{
		this.parent = parent;
		pool = new List<GameObject>();
		busy = new List<GameObject>();
		if (AudioSourcePrefab == null)
		{
			Debug.LogError("AudioSourcePool:Initialize() AudioSourcePrefab is null");
			return;
		}
		if (AudioSourcePrefab != null && AudioSourcePrefab.GetComponent<AudioSource>() == null)
		{
			Debug.LogError("AudioSourcePool:Initialize() AudioSourcePrefab is missing AudioSource");
			return;
		}
		for (int i = 0; i < Count; i++)
		{
			CreateNewInstance();
		}
	}

	private GameObject CreateNewInstance()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(AudioSourcePrefab, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.parent = parent;
		gameObject.name = AudioSourcePrefab.name;
		gameObject.SetActive(false);
		pool.Add(gameObject);
		return gameObject;
	}

	public GameObject GetPoolObject()
	{
		if (pool.Count == 0)
		{
			CreateNewInstance();
		}
		GameObject gameObject = pool[0];
		pool.RemoveAt(0);
		busy.Add(gameObject);
		return gameObject;
	}

	public void StopBusyWithClip(AudioClip clip)
	{
		foreach (GameObject item in busy)
		{
			if (item.GetComponent<AudioSource>().clip == clip)
			{
				item.GetComponent<AudioSource>().Stop();
			}
		}
	}

	public void Update()
	{
		if (busy.Count <= 0)
		{
			return;
		}
		for (int num = busy.Count - 1; num >= 0; num--)
		{
			GameObject gameObject = busy[num];
			if (!gameObject.GetComponent<AudioSource>().isPlaying)
			{
				gameObject.SetActive(false);
				busy.RemoveAt(num);
				pool.Add(gameObject);
			}
		}
	}
}
