using System.Collections.Generic;
using UnityEngine;

public class Cheat
{
	private string name;

	private List<CheatEntryBase> inputEntries = new List<CheatEntryBase>();

	private int entryIndex;

	public string Name
	{
		get
		{
			return name;
		}
	}

	public bool Complete
	{
		get
		{
			return entryIndex == -1;
		}
	}

	public Cheat(string name, List<CheatEntryBase> inputEntries)
	{
		this.name = name;
		this.inputEntries = inputEntries;
	}

	public Cheat(string name, params CheatEntryBase[] inputEntries)
		: this(name, new List<CheatEntryBase>(inputEntries))
	{
	}

	public void Reset()
	{
		entryIndex = 0;
	}

	public bool HandleInput(int horizontalAxis, int verticalAxis)
	{
		if (entryIndex > -1)
		{
			CheatEntryBase cheatEntryBase = inputEntries[entryIndex];
			switch (cheatEntryBase.Check(horizontalAxis, verticalAxis))
			{
			case CheatEntryReturnStatus.Success:
				Debug.Log("Right!");
				if (++entryIndex == inputEntries.Count)
				{
					entryIndex = -1;
					return true;
				}
				break;
			case CheatEntryReturnStatus.WrongInput:
				Reset();
				break;
			}
		}
		return false;
	}
}
