using UnityEngine;

public class AIAbility : Ability
{
	public bool UseRandomCooldown;

	public float MinCooldown = 1f;

	public float MaxCooldown = 1f;

	public bool CanDud;

	public float DudChance = 0.1f;

	public float DudMinCooldown = 1f;

	public float DudMaxCooldown = 1f;

	public bool CanBeCast = true;

	public override void TriggerCooldown()
	{
		if (UseRandomCooldown)
		{
			activeCooldown = Random.Range(MinCooldown, MaxCooldown);
			if (abilityController != null)
			{
				abilityController.OnTriggerCooldown(this);
			}
		}
		else
		{
			base.TriggerCooldown();
		}
	}

	public void TriggerDud()
	{
		activeCooldown = Random.Range(DudMinCooldown, DudMaxCooldown);
		if (abilityController != null)
		{
			abilityController.OnTriggerCooldown(this);
		}
	}
}
