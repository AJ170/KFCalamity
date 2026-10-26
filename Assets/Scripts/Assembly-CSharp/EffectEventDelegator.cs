using System.Collections.Generic;
using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class EffectEventDelegator : MonoBehaviour
{
	private List<EffectSystem> systems = new List<EffectSystem>();

	private List<IEffectTriggerListener> listeners = new List<IEffectTriggerListener>();

	private void OnDestroy()
	{
		systems.Clear();
		listeners.Clear();
	}

	public void RegisterSystem(EffectSystem system)
	{
		systems.Add(system);
	}

	public void UnregisterSystem(EffectSystem system)
	{
		systems.Remove(system);
	}

	public void RegisterListener(IEffectTriggerListener listener)
	{
		listeners.Add(listener);
	}

	public void UnregisterListener(IEffectTriggerListener listener)
	{
		listeners.Remove(listener);
	}

	public void TriggerInternal(EffectSystem ignore, TriggerBase e)
	{
		foreach (EffectSystem system in systems)
		{
			if (system != ignore)
			{
				system.TriggerInternal(e, false);
			}
		}
		ListenTrigger(e);
	}

	public void TriggerInternal(TriggerBase e)
	{
		TriggerInternal(null, e);
	}

	public void ListenTrigger(TriggerBase e)
	{
		foreach (IEffectTriggerListener listener in listeners)
		{
			listener.OnEffectTrigger(e);
		}
	}
}
