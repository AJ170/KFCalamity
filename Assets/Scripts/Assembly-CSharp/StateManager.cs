using System.Collections.Generic;

public class StateManager
{
	private Dictionary<string, StateBase> states = new Dictionary<string, StateBase>();

	private StateBase activeState;

	public void AddState(string stateName, StateBase state)
	{
		RemoveState(stateName);
		states.Add(stateName, state);
		if (activeState == null)
		{
			ChangeState(stateName);
		}
	}

	public bool ChangeState(string stateName)
	{
		if (states.ContainsKey(stateName))
		{
			if (activeState != null)
			{
				activeState.OnInactive();
			}
			activeState = states[stateName];
			activeState.OnActive();
			return true;
		}
		return false;
	}

	public bool RemoveState(string stateName)
	{
		if (states.ContainsKey(stateName))
		{
			StateBase stateBase = states[stateName];
			stateBase.Shutdown();
			states.Remove(stateName);
			return true;
		}
		return false;
	}

	public void Update()
	{
		if (activeState != null)
		{
			activeState.Update();
		}
	}
}
