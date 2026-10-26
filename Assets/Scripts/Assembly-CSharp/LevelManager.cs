using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
	private struct LoadInfo
	{
		private int id;

		private AsyncOperation async;

		private Action<int> callback;

		public int ID
		{
			get
			{
				return id;
			}
		}

		public bool AllowSceneActivation
		{
			get
			{
				return async.allowSceneActivation;
			}
			set
			{
				async.allowSceneActivation = value;
			}
		}

		public LoadInfo(int asyncId, AsyncOperation asyncOperation, Action<int> callbackMethod)
		{
			id = asyncId;
			async = asyncOperation;
			callback = callbackMethod;
		}

		public float Progress()
		{
			return async.progress;
		}

		public bool Done()
		{
			return async.isDone;
		}

		public void Callback()
		{
			callback(id);
		}
	}

	private List<LoadInfo> asyncs = new List<LoadInfo>();

	private int ELAPSED_ASYNC_IDS;

	public int LoadLevelAsync(string levelName, Action<int> callback, bool allowSceneActivation = true)
	{
		return TrackAsyncLoad(Application.LoadLevelAsync(levelName), callback, allowSceneActivation);
	}

	public int LoadLevelAdditiveAsync(string levelName, Action<int> callback)
	{
		return TrackAsyncLoad(Application.LoadLevelAdditiveAsync(levelName), callback, true);
	}

	public void ActivateLoadedScene(int asyncId)
	{
		int num = asyncs.FindIndex((LoadInfo a) => a.ID == asyncId);
		if (num > -1)
		{
			LoadInfo loadInfo = asyncs[num];
			loadInfo.AllowSceneActivation = true;
		}
		else
		{
			Debug.LogWarning("LevelManager:ActivateLoadedScene() - No LoadInfo found with AsyncId: " + asyncId);
		}
	}

	public void Update()
	{
		for (int num = asyncs.Count - 1; num >= 0; num--)
		{
			LoadInfo loadInfo = asyncs[num];
			if (loadInfo.Done() && loadInfo.AllowSceneActivation)
			{
				loadInfo.Callback();
				asyncs.RemoveAt(num);
			}
		}
	}

	private int TrackAsyncLoad(AsyncOperation async, Action<int> callback, bool allowSceneActivation)
	{
		int num = NewAsyncId();
		async.allowSceneActivation = allowSceneActivation;
		asyncs.Add(new LoadInfo(num, async, callback));
		return num;
	}

	private int NewAsyncId()
	{
		return ++ELAPSED_ASYNC_IDS;
	}
}
