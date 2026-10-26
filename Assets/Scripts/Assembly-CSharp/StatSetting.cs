using System;
using UnityEngine;

[Serializable]
public class StatSetting
{
	public string DisplayName;

	public MenuStatistics.Categories Setting;

	public GameObject Prefab;

	private GameObject loadedPrefab;

	public GameObject LoadedPrefab
	{
		get
		{
			return loadedPrefab;
		}
		set
		{
			loadedPrefab = value;
		}
	}
}
