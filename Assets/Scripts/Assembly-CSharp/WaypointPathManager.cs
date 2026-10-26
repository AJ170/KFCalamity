using System;
using UnityEngine;

public class WaypointPathManager : MonoBehaviour
{
	[Serializable]
	public class WaypointPath
	{
		public string Name;

		public ObjectWaypoint[] Waypoints;
	}

	public WaypointPath[] Paths;

	private static WaypointPathManager instance;

	public static WaypointPathManager Instance
	{
		get
		{
			return instance;
		}
	}

	public void Awake()
	{
		instance = this;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public WaypointPath GetPath(string pathName)
	{
		return Array.Find(Paths, (WaypointPath a) => a.Name == pathName);
	}
}
