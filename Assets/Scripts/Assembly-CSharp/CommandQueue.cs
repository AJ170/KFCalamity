using System;
using System.Collections.Generic;
using UnityEngine;

public class CommandQueue
{
	private class CommandInfo
	{
		public Action action;

		public float time;

		public CommandInfo(Action action, float time)
		{
			this.action = action;
			this.time = time;
		}
	}

	private List<CommandInfo> commands = new List<CommandInfo>();

	public void Update()
	{
		if (commands.Count <= 0)
		{
			return;
		}
		CommandInfo commandInfo = commands[0];
		commandInfo.time = Mathf.Max(commandInfo.time - Time.deltaTime, 0f);
		if (commandInfo.time == 0f)
		{
			if (commandInfo.action != null)
			{
				commandInfo.action();
			}
			commands.RemoveAt(0);
		}
	}

	public void Add(Action action, float time)
	{
		commands.Add(new CommandInfo(action, time));
	}

	public void Add(Action action)
	{
		Add(action, 0f);
	}

	public void Insert(Action action, float time, int index)
	{
		commands.Insert(index, new CommandInfo(action, time));
	}

	public void Clear()
	{
		commands.Clear();
	}
}
