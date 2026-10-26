using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GA_ExitGameEvent : MonoBehaviour
{
	private const string gaDataFileName = "GAData";

	private bool firstTime;

	private uint wave;

	private string scene;

	private uint playTime;

	private void Start()
	{
		FileManager instance = FileManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		List<string> list = instance.Load(instance.RootPath + "GAData");
		if (list != null)
		{
			string empty = string.Empty;
			string value = string.Empty;
			foreach (string item in list)
			{
				if (DeserializeLine(item, out empty, out value))
				{
					OnDeserializeLine(empty, value);
				}
			}
			firstTime = false;
		}
		else
		{
			firstTime = true;
		}
		if (File.Exists(instance.RootPath + "GAData"))
		{
			File.Delete(instance.RootPath + "GAData");
		}
	}

	private void Update()
	{
	}

	private void OnApplicationQuit()
	{
		FileManager instance = FileManager.Instance;
		if (instance != null)
		{
			List<string> list = new List<string>();
			list.Add("firstTime:" + firstTime);
			list.Add("scene:" + Application.loadedLevelName.ToString());
			list.Add("playTime:" + (int)Time.time);
			instance.Save(instance.RootPath + "GAData", list);
		}
	}

	private void OnDeserializeLine(string name, string value)
	{
		switch (name)
		{
		case "firstTime":
			firstTime = Convert.ToBoolean(value);
			break;
		case "wave":
			wave = Convert.ToUInt32(value);
			break;
		case "scene":
			scene = Convert.ToString(value);
			break;
		case "playTime":
			playTime = Convert.ToUInt32(value);
			break;
		}
	}

	private bool DeserializeLine(string line, out string name, out string value)
	{
		name = string.Empty;
		value = string.Empty;
		string value2 = ":";
		int num = 0;
		num = line.LastIndexOf(value2);
		if (num != -1)
		{
			name = line.Substring(0, num);
			value = line.Substring(num + 1, line.Length - (num + 1));
			return true;
		}
		return false;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		Debug.Log("APPLICATION PAUSED: " + pauseStatus);
	}
}
