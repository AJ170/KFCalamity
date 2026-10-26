using System.Collections.Generic;
using EventHelpers;
using UnityEngine;

public class EffectBaseListener : MonoBehaviour
{
	public TriggerId Trigger;

	public List<string> customEventListeners;

	protected EffectSystem effectSystem;

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
		if (effectSystem == null)
		{
			effectSystem = base.gameObject.GetComponentInParents<EffectSystem>();
		}
		if (effectSystem != null)
		{
			if (Trigger != 0)
			{
				effectSystem.EventSystem.Subscribe(Trigger, Execute, this);
			}
		}
		else
		{
			Debug.LogError(string.Format("EffectBase:Start() -> Didn't find EffectSystem in gameObject {0} ({1})", base.transform.root.name, base.transform.name));
		}
	}

	protected virtual void Shutdown()
	{
	}

	private void OnDestroy()
	{
		Shutdown();
		if (effectSystem != null)
		{
			effectSystem.EventSystem.Unsubscribe(Trigger, this);
		}
	}

	protected virtual void Execute(TriggerBase e)
	{
		Debug.LogError("EffectBaseListener:Execute() Base-method executed");
	}

	protected bool NoCustomEvents()
	{
		return customEventListeners.Count == 0;
	}

	protected bool MatchCustomEvent(string customName)
	{
		if (customEventListeners.Count > 0)
		{
			foreach (string customEventListener in customEventListeners)
			{
				if (customEventListener == customName)
				{
					return true;
				}
			}
		}
		return false;
	}
}
