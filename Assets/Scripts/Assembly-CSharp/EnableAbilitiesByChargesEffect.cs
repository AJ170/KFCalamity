using System;
using System.Collections.Generic;
using EventHelpers;
using UnityEngine;

public class EnableAbilitiesByChargesEffect : EffectBaseListener
{
	[Serializable]
	public class AbilityToEnable
	{
		public int Charges;

		public string Name;
	}

	public Ability Ability;

	public List<AbilityToEnable> AbilitiesToEnable;

	private AbilityController abilityController;

	protected override void Start()
	{
		base.Start();
		DisableAbilities();
	}

	private void OnMounted(GameObject parent)
	{
		abilityController = parent.GetComponent<AbilityController>();
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((!NoCustomEvents() && !flag) || !(Ability != null) || !(abilityController != null))
		{
			return;
		}
		int num = Ability.ChargesLeft();
		for (int i = 0; i < AbilitiesToEnable.Count; i++)
		{
			if (AbilitiesToEnable[i].Charges == num)
			{
				Ability ability = abilityController.AbilityByName(AbilitiesToEnable[i].Name);
				if (ability != null)
				{
					ability.enabled = true;
				}
			}
		}
	}

	private void DisableAbilities()
	{
		if (!(abilityController != null))
		{
			return;
		}
		for (int i = 0; i < AbilitiesToEnable.Count; i++)
		{
			Ability ability = abilityController.AbilityByName(AbilitiesToEnable[i].Name);
			if (ability != null)
			{
				ability.enabled = false;
			}
		}
	}
}
