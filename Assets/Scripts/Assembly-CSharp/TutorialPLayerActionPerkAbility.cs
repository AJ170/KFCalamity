using Calamity.Input;
using UnityEngine;

public class TutorialPLayerActionPerkAbility : TutorialPlayerActionBase
{
	private Ability perkAbility;

	private SoldierInputController input;

	public TutorialPLayerActionPerkAbility(GameObject player)
		: base(player)
	{
		PlayerAbilityController component = player.GetComponent<PlayerAbilityController>();
		perkAbility = component.Perk.GetComponent<Ability>();
		input = player.GetComponent<SoldierInputController>();
	}

	public override void Update()
	{
		if (perkAbility.CanCast() && input.Control.GetBool(InputAction.Ability))
		{
			OnComplete();
		}
	}
}
