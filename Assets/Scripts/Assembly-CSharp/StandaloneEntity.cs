using EffectEvents;
using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class StandaloneEntity : MonoBehaviour, IEffectTriggerListener
{
	public bool DelegateKills = true;

	public bool DelegateDamageDealt = true;

	public bool TriggerBeingKilledLogic;

	private GameObject owner;

	public GameObject Owner
	{
		get
		{
			return owner;
		}
		set
		{
			owner = value;
		}
	}

	void IEffectTriggerListener.OnEffectTrigger(TriggerBase e)
	{
		if (e.Id == TriggerId.DamageDealt || e.Id == TriggerId.HealingDealt || e.Id == TriggerId.Kill)
		{
			OnDelegateTrigger(e);
		}
		if (e.Id == TriggerId.BeingKilled)
		{
			BeingKilled beingKilled = e as BeingKilled;
			OnKilled(beingKilled.Attacker);
		}
	}

	private void Awake()
	{
		if (!GlobalHelpers.RegisterEffectEventListener(base.gameObject, this))
		{
			Debug.LogError(string.Format("StandaloneEntity:Awake() Missing Effect Event Delegator ({0})", base.transform.name));
		}
	}

	public static void OnCreated(GameObject standalone, GameObject owner)
	{
		if (!(standalone != null) || !(owner != null))
		{
			return;
		}
		StandaloneEntity component = standalone.GetComponent<StandaloneEntity>();
		if (component != null)
		{
			StandaloneEntity component2 = owner.GetComponent<StandaloneEntity>();
			if (component2 != null && component2.Owner != null)
			{
				component.Owner = component2.Owner;
			}
			else
			{
				component.Owner = owner;
			}
		}
	}

	private void OnDelegateTrigger(TriggerBase trigger)
	{
		if (Owner != null && trigger != null)
		{
			EffectEventDelegator component = Owner.GetComponent<EffectEventDelegator>();
			if (component != null)
			{
				component.ListenTrigger(trigger);
			}
		}
	}

	private void OnKilled(GameObject attacker)
	{
		if (Owner == null && attacker != null)
		{
			GameObject gameObject = Owner;
			StandaloneEntity component = attacker.GetComponent<StandaloneEntity>();
			if (component != null && component.Owner != null)
			{
				Owner = component.Owner;
			}
			else if (attacker.tag == "Soldier")
			{
				Owner = attacker;
			}
			if (gameObject != Owner)
			{
				BroadcastMessage("OnMounted", Owner, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public static GameObject Source(GameObject source)
	{
		if (source != null)
		{
			StandaloneEntity component = source.GetComponent<StandaloneEntity>();
			if (component != null && component.Owner != null)
			{
				return component.Owner;
			}
		}
		return source;
	}
}
