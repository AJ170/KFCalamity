using EffectEvents;
using EventHelpers;
using UnityEngine;

public class ProjectileTriggerDelay : MonoBehaviour
{
	public float Delay = 0.5f;

	public TriggerId Event;

	public string Name = string.Empty;

	protected EffectSystem effectSystem;

	private float counter;

	private void Awake()
	{
		effectSystem = base.gameObject.GetComponentInParents<EffectSystem>();
	}

	private void OnMounted(GameObject parent)
	{
		counter = Delay;
	}

	private void Update()
	{
		if (counter > 0f)
		{
			counter -= Time.deltaTime;
			if (counter <= 0f)
			{
				TriggerEvent();
			}
		}
	}

	private void TriggerEvent()
	{
		if (effectSystem != null && Event != 0)
		{
			TriggerBase @event = GetEvent(Event);
			if (@event != null)
			{
				effectSystem.TriggerInternal(@event);
			}
		}
	}

	private TriggerBase GetEvent(TriggerId id)
	{
		TriggerBase result = null;
		switch (id)
		{
		case TriggerId.Attack:
			result = new Attack(Name);
			break;
		case TriggerId.Cast:
			result = new Cast(Name);
			break;
		}
		return result;
	}
}
