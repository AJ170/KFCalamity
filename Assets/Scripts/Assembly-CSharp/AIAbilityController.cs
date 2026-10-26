using UnityEngine;

public class AIAbilityController : AbilityController
{
	public bool UseGlobalCooldown;

	public float GlobalCooldown = 1f;

	private float globalCooldownCounter;

	private void Update()
	{
		if (UseGlobalCooldown && globalCooldownCounter > 0f)
		{
			globalCooldownCounter -= Time.deltaTime;
		}
	}

	public bool AnyCastableAbility()
	{
		bool result = false;
		foreach (Ability ability in abilities)
		{
			if (ability.Type == AbilityType.Activated)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public AIAbility CheckForCastableAbility()
	{
		AIAbility result = null;
		if (!UseGlobalCooldown || globalCooldownCounter <= 0f)
		{
			foreach (Ability ability in abilities)
			{
				AIAbility aIAbility = ability as AIAbility;
				if (aIAbility != null && aIAbility.Type == AbilityType.Activated && CanCast(aIAbility))
				{
					result = aIAbility;
					break;
				}
			}
		}
		return result;
	}

	public void Cast(Ability ability)
	{
		ability.Cast();
	}

	public bool CanCast(AIAbility ability, bool ignoreCriterion = false)
	{
		if (ability.CanBeCast)
		{
			return base.CanCast(ability, ignoreCriterion);
		}
		return false;
	}

	public override void OnCastCompleted(Ability ability)
	{
		if (UseGlobalCooldown)
		{
			globalCooldownCounter = GlobalCooldown;
		}
	}
}
