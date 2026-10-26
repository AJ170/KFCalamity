using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager : MonoBehaviour
{
	private static FileManager instance;

	private string rootPath;

	private string perkPath;

	private string weaponPath;

	private string levelPath;

	private string statsPath;

	public static FileManager Instance
	{
		get
		{
			return instance;
		}
	}

	public string RootPath
	{
		get
		{
			return rootPath;
		}
	}

	public string PerkPath
	{
		get
		{
			return perkPath;
		}
	}

	public string WeaponPath
	{
		get
		{
			return weaponPath;
		}
	}

	public string LevelPath
	{
		get
		{
			return levelPath;
		}
	}

	public string StatsPath
	{
		get
		{
			return statsPath;
		}
	}

	private void Awake()
	{
		instance = this;
		rootPath = Application.persistentDataPath + "/";
		perkPath = rootPath + "Perks/";
		weaponPath = rootPath + "Weapons/";
		levelPath = rootPath + "Levels/";
		statsPath = rootPath + "Stats/";
		Directory.CreateDirectory(perkPath);
		Directory.CreateDirectory(weaponPath);
		Directory.CreateDirectory(levelPath);
		Directory.CreateDirectory(statsPath);
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public bool Save(string fileName, List<string> lines)
	{
		if (File.Exists(fileName))
		{
			File.Delete(fileName);
		}
		StreamWriter streamWriter = File.CreateText(fileName);
		if (streamWriter != null)
		{
			foreach (string line in lines)
			{
				streamWriter.WriteLine(line);
			}
			streamWriter.Close();
			return true;
		}
		return false;
	}

	public List<string> Load(string fileName)
	{
		if (File.Exists(fileName))
		{
			StreamReader streamReader = File.OpenText(fileName);
			if (streamReader != null)
			{
				List<string> list = new List<string>();
				string empty = string.Empty;
				while ((empty = streamReader.ReadLine()) != null)
				{
					list.Add(empty);
				}
				streamReader.Close();
				return list;
			}
		}
		return null;
	}

	public List<string> GetFilesByDirectory(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			FileInfo[] files = directoryInfo.GetFiles();
			List<string> list = new List<string>();
			FileInfo[] array = files;
			foreach (FileInfo fileInfo in array)
			{
				list.Add(fileInfo.FullName);
			}
			return list;
		}
		return null;
	}
}
