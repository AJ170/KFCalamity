using System;
using System.Collections.Generic;

public class CheatManager
{
	private List<Cheat> cheats = new List<Cheat>();

	private List<Cheat> removeList = new List<Cheat>();

	private Dictionary<string, Action<string>> onCheatCompleteCallbacks = new Dictionary<string, Action<string>>();

	private static CheatManager instance;

	public static CheatManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new CheatManager();
			}
			return instance;
		}
	}

	public void HandleInput(int horizontalAxis, int verticalAxis)
	{
		foreach (Cheat cheat in cheats)
		{
			if (!cheat.Complete && cheat.HandleInput(horizontalAxis, verticalAxis))
			{
				OnCheatComplete(cheat.Name);
				removeList.Add(cheat);
			}
		}
		if (removeList.Count <= 0)
		{
			return;
		}
		foreach (Cheat remove in removeList)
		{
			cheats.Remove(remove);
		}
		removeList.Clear();
	}

	public void Reset()
	{
		foreach (Cheat cheat in cheats)
		{
			cheat.Reset();
		}
		onCheatCompleteCallbacks.Clear();
	}

	public void AddCheat(string name, List<CheatEntryBase> inputActions, Action<string> completeCallback)
	{
		if (!cheats.Exists((Cheat a) => a.Name == name))
		{
			cheats.Add(new Cheat(name, inputActions));
			onCheatCompleteCallbacks.Add(name, completeCallback);
		}
	}

	public void AddCheat(string name, Action<string> completeCallback, params CheatEntryBase[] inputActions)
	{
		AddCheat(name, new List<CheatEntryBase>(inputActions), completeCallback);
	}

	private void OnCheatComplete(string cheatName)
	{
		if (onCheatCompleteCallbacks.ContainsKey(cheatName))
		{
			onCheatCompleteCallbacks[cheatName](cheatName);
		}
	}
}
