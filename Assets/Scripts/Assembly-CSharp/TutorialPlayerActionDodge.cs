using Calamity.Input;
using UnityEngine;

public class TutorialPlayerActionDodge : TutorialPlayerActionBase
{
	private SoldierInputController inputController;

	private PlayerAbilityController abilityController;

	private Ability dodgeAbility;

	public TutorialPlayerActionDodge(GameObject player)
		: base(player)
	{
	}

	public void SetComplete()
	{
		OnComplete();
	}

	public override void OnStartEvent()
	{
		PlayerActionController component = Player.GetComponent<PlayerActionController>();
		component.AddAction(new TutorialPlayerActionStuff(this, TutorialActionType.Dodge));
	}

	public override void OnEndEvent()
	{
	}

	public override void Update()
	{
	}
}
