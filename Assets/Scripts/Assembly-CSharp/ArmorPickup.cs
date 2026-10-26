using UnityEngine;

public class ArmorPickup : Pickup
{
	private void Awake()
	{
		type = PickupType.Armor;
	}

	protected override void OnRespawn()
	{
		ShowPickupObject();
	}

	protected override void OnCollidePlayer(Collider collider)
	{
		GameObject gameObject = collider.gameObject;
		Health health = HealthScript(gameObject);
		if (CanRefillArmor(health))
		{
			PlayPickupSound();
			PlayPickupParticle();
			RefillArmor(health);
			if (Respawn())
			{
				HidePickupObject();
			}
			else
			{
				ForceDelete();
			}
		}
		else
		{
			OnFailedPickup(gameObject);
		}
	}

	private Health HealthScript(GameObject go)
	{
		return go.GetComponent<Health>();
	}

	private bool CanRefillArmor(Health health)
	{
		return health.CurrentArmor < health.MaxArmor;
	}

	private void RefillArmor(Health healthScript)
	{
		if (healthScript != null)
		{
			healthScript.RefillArmor(100);
		}
	}
}
