using EventHelpers;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
	protected EffectSystemType systemType;

	private EffectEventSystem eventSystem = new EffectEventSystem();

	protected EffectEventDelegator eventDelegator;

	public EffectEventSystem EventSystem
	{
		get
		{
			return eventSystem;
		}
	}

	public EffectEventDelegator EventDelegator
	{
		get
		{
			return eventDelegator;
		}
	}

	public EffectSystemType SystemType
	{
		get
		{
			return systemType;
		}
	}

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
		eventDelegator = base.transform.root.GetComponent<EffectEventDelegator>();
		if (eventDelegator != null)
		{
			eventDelegator.RegisterSystem(this);
		}
		else if (systemType != 0)
		{
			Debug.LogError(string.Format("EffectSystem:Start() -> Didn't find 'EffectEventDelegator' in {0}", base.transform.root.name));
		}
	}

	protected virtual void OnDestroy()
	{
		if (eventDelegator != null)
		{
			eventDelegator.UnregisterSystem(this);
		}
	}

	public void TriggerInternal(TriggerBase e, bool delegateToListeners)
	{
		eventSystem.Trigger(e);
		if (delegateToListeners && eventDelegator != null)
		{
			eventDelegator.ListenTrigger(e);
		}
	}

	public void TriggerInternal(TriggerBase e)
	{
		TriggerInternal(e, true);
	}

	public void TriggerGlobal(TriggerBase e)
	{
		if (eventDelegator != null)
		{
			eventDelegator.TriggerInternal(this, e);
		}
		else
		{
			Debug.LogError(string.Format("EffectSystem:TriggerGlobal() No delegator found for {0}", ToString()));
		}
	}

	public void TriggerRemote(GameObject target, TriggerBase e)
	{
		EffectEventDelegator effectEventDelegator = RemoteEventDelegator(target);
		if (effectEventDelegator != null)
		{
			bool flag = base.transform.root.gameObject != target;
			effectEventDelegator.TriggerInternal((!flag) ? null : this, e);
		}
	}

	private EffectEventDelegator RemoteEventDelegator(GameObject target)
	{
		return target.transform.root.GetComponent<EffectEventDelegator>();
	}
}
