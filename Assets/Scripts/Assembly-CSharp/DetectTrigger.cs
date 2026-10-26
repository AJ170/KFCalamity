using EffectEvents;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DetectTrigger : MonoBehaviour
{
	public Collider Collider;

	public bool TriggerOnce = true;

	private bool triggered;

	private EffectEventDelegator effectDelegator;

	private void Start()
	{
		if (Collider == null)
		{
			Collider = GetComponent<Collider>();
		}
		if (Collider == null || !Collider.isTrigger)
		{
			base.enabled = false;
		}
		effectDelegator = base.transform.root.GetComponent<EffectEventDelegator>();
		if (effectDelegator == null)
		{
			if (Collider != null)
			{
				Collider.enabled = false;
			}
			base.enabled = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!triggered || !TriggerOnce)
		{
			TriggerEvent(other.gameObject);
			triggered = true;
		}
		if (TriggerOnce)
		{
			Collider.enabled = false;
			base.enabled = false;
		}
	}

	private void TriggerEvent(GameObject target)
	{
		if (effectDelegator != null)
		{
			effectDelegator.TriggerInternal(new Detected(target));
		}
	}
}
