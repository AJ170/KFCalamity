using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class TutorialPlayerActionAttack : TutorialPlayerActionBase, IEffectTriggerListener
{
	private PlayerWeaponController weaponController;

	public TutorialPlayerActionAttack(GameObject player)
		: base(player)
	{
	}

	public override void OnStartEvent()
	{
		weaponController = Player.GetComponent<PlayerWeaponController>();
		weaponController.ActiveWeapon.EventDelegator.RegisterListener(this);
	}

	public override void OnEndEvent()
	{
		weaponController.ActiveWeapon.EventDelegator.UnregisterListener(this);
	}

	public void OnEffectTrigger(TriggerBase e)
	{
		if (e.Id == TriggerId.Attack)
		{
			OnComplete();
		}
	}
}
