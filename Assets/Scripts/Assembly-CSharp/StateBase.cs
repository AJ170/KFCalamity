using UnityEngine;

public class StateBase
{
	public virtual void OnActive()
	{
	}

	public virtual void OnInactive()
	{
	}

	public virtual void Shutdown()
	{
	}

	public virtual void Update()
	{
		Debug.LogError("StateBase Update() called.");
	}
}
