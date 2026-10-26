using AIActionTransitions;
using UnityEngine;

internal class RampageAIAction : AIAction
{
	private AIAbilityController abilityController;

	private ZedAnimationController animationController;

	private RampageChargeAIAction chargeAction;

	private Transition enrageTransition;

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		if (triggeredEvent.ID == AIEventID.TargetLost || triggeredEvent.ID == AIEventID.TargetDied)
		{
			status = AIActionStatus.Complete;
		}
		return null;
	}

	public override void OnStart()
	{
		abilityController = controller.GetComponent<AIAbilityController>();
		animationController = controller.GetComponent<ZedAnimationController>();
		if (abilityController == null || animationController == null)
		{
			Debug.LogError("RampageAIAction:OnStart() No AbilityController or AnimationController on GameObject");
			status = AIActionStatus.Failed;
		}
		else
		{
			StartCharge();
			base.OnStart();
		}
	}

	public override Transition Update()
	{
		Transition result = null;
		if (IsDone())
		{
			result = ((enrageTransition == null) ? new Transition(TransitionType.Done) : enrageTransition);
		}
		return result;
	}

	private void StartCharge()
	{
		chargeAction = new RampageChargeAIAction(OnChargeEnd);
		actionController.Secondary.Push(chargeAction);
	}

	private void OnChargeEnd(bool success, bool completeCharge)
	{
		if (success && !completeCharge)
		{
			SetupEnrageTransition();
		}
		if (animationController != null)
		{
			animationController.TriggerAnimation(ZedAnimationTrigger.Rampage, false);
		}
		status = AIActionStatus.Complete;
	}

	private void SetupEnrageTransition()
	{
		UseAbilityAIAction useAbilityAIAction = null;
		if (abilityController != null)
		{
			Ability ability = abilityController.AbilityByName("FleshpoundEnrageAbility");
			if (ability != null)
			{
				useAbilityAIAction = new UseAbilityAIAction(ability as AIAbility);
			}
		}
		if (useAbilityAIAction != null)
		{
			enrageTransition = new Transition(TransitionType.ChangeTo, useAbilityAIAction);
		}
	}

	public override void OnEnd()
	{
		if (chargeAction != null && !chargeAction.IsDone())
		{
			chargeAction.Abort();
		}
	}
}
