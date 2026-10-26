using UnityEngine;

public class TakeDamageMaterialTint : MaterialPulse
{
	public Transform Target;

	protected override void Start()
	{
		if (Target == null)
		{
			Target = base.transform.root;
		}
		FindMaterials(Target);
		GlobalHelpers.AddDamageTakenReceiver(base.gameObject, OnDamageTaken);
	}

	protected virtual void OnDamageTaken(Health health, int damageTaken, int rawDamage, float armorTypeMultiplier)
	{
		Pulse();
	}
}
