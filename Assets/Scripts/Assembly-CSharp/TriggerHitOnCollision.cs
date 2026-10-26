using EffectEvents;
using UnityEngine;

public class TriggerHitOnCollision : MonoBehaviour
{
	public EffectSystem EffectSystem;

	public string Name;

	public bool TriggerOnce;

	private bool reset = true;

	private void OnMounted(GameObject parent)
	{
		reset = true;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (EffectSystem != null && reset)
		{
			TriggerHitEvents(collider.transform);
			if (TriggerOnce)
			{
				reset = false;
			}
		}
	}

	private void TriggerHitEvents(Transform hitTransform)
	{
		EffectSystem.TriggerInternal(new Hit(Name, hitTransform));
		if (EffectSystem.SystemType == EffectSystemType.Weapon)
		{
			EffectSystem.TriggerGlobal(new WeaponHit(hitTransform));
		}
		EffectSystem.TriggerRemote(hitTransform.gameObject, new BeingHit(base.transform.root.gameObject, hitTransform));
	}
}
