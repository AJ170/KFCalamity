using System;

public class ActionToggleAbilities : TutorialSubStepBase
{
	[Serializable]
	public class AbilityToggle
	{
		public AbilitySlot Ability;

		public bool Toggle;
	}

	public AbilityToggle[] Abilities;

	public override void OnStart()
	{
		PlayerAbilityController component = Player.GetComponent<PlayerAbilityController>();
		AbilityToggle[] abilities = Abilities;
		foreach (AbilityToggle abilityToggle in abilities)
		{
			Ability ability = component.AbilityBySlot(abilityToggle.Ability);
			if (ability != null)
			{
				ability.enabled = abilityToggle.Toggle;
			}
		}
		SetComplete(true);
	}
}
