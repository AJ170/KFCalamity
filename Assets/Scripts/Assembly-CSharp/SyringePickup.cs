using UnityEngine;

public class SyringePickup : Pickup
{
	private void Awake()
	{
		type = PickupType.Syringe;
	}

	protected override void OnRespawn()
	{
		ShowPickupObject();
	}

	protected override void OnCollidePlayer(Collider collider)
	{
		GameObject gameObject = collider.gameObject;
		PlayerAbilityController controller = AbilityController(gameObject);
		if (SyringeNeedRefill(controller))
		{
			PlayPickupSound();
			PlayPickupParticle();
			RefillSyringe(controller);
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

	private PlayerAbilityController AbilityController(GameObject go)
	{
		return go.GetComponent<PlayerAbilityController>();
	}

	private bool SyringeNeedRefill(PlayerAbilityController controller)
	{
		if (controller != null)
		{
			Ability ability = controller.AbilityBySlot(AbilitySlot.SyringeSelf);
			if (ability != null)
			{
				return !ability.AllChargesLeft();
			}
		}
		return false;
	}

	private void RefillSyringe(PlayerAbilityController controller)
	{
		if (!(controller != null))
		{
			return;
		}
		Ability ability = controller.AbilityBySlot(AbilitySlot.SyringeSelf);
		if (ability != null)
		{
			int charges = 1;
			PlayerManager instance = PlayerManager.Instance;
			if (instance != null && instance.PlayerCount() > 2)
			{
				charges = 2;
			}
			ability.RefillCharges(charges);
		}
	}
}
