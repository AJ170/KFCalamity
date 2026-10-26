using UnityEngine;

public class DestroyPersistent : MonoBehaviour
{
	public string[] Levels;

	public string[] AllLevelsBut;

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnLevelWasLoaded(int level)
	{
		string loadedLevelName = Application.loadedLevelName;
		bool flag = false;
		if (DestroyOnSpecific(loadedLevelName))
		{
			flag = true;
		}
		if (!flag && DestroyOnAllButSpecific(loadedLevelName))
		{
			flag = true;
		}
		if (flag)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private bool DestroyOnSpecific(string levelName)
	{
		string[] levels = Levels;
		foreach (string text in levels)
		{
			if (text == levelName)
			{
				return true;
			}
		}
		return false;
	}

	private bool DestroyOnAllButSpecific(string levelName)
	{
		bool flag = false;
		string[] allLevelsBut = AllLevelsBut;
		foreach (string text in allLevelsBut)
		{
			if (text == levelName)
			{
				flag = true;
				break;
			}
		}
		return !flag && AllLevelsBut.Length > 0;
	}
}
