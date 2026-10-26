using AIActionTransitions;

internal class PatriarchCombatAIAction : CombatAIAction
{
	public PatriarchCombatAIAction()
	{
		attackRange = 3f;
	}

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		Transition transition = base.OnAIEvent(triggeredEvent);
		if (transition == null)
		{
			if (triggeredEvent.ID == AIEventID.Patriarch)
			{
				PatriarchAIEvent patriarchAIEvent = (PatriarchAIEvent)triggeredEvent;
				if (patriarchAIEvent.Type == PatriarchAIEvent.EventType.Charge)
				{
					transition = new Transition(TransitionType.SuspendFor, new PatriarchChargeAIAction());
				}
				else
				{
					UseAbilityAIAction useAbilityAIAction = null;
					if (abilityController != null)
					{
						Ability ability = abilityController.AbilityByName("PatriarchHeal");
						if (ability != null)
						{
							useAbilityAIAction = new UseAbilityAIAction((AIAbility)ability);
						}
					}
					if (useAbilityAIAction != null)
					{
						PatriarchChargeAIAction patriarchChargeAIAction = null;
						if (targetHandler != null && targetHandler.HasTarget)
						{
							patriarchChargeAIAction = new PatriarchChargeAIAction();
						}
						if (patriarchChargeAIAction != null)
						{
							actionController.Primary.Queue(useAbilityAIAction);
							transition = new Transition(TransitionType.ChangeTo, patriarchChargeAIAction);
						}
						else
						{
							transition = new Transition(TransitionType.ChangeTo, useAbilityAIAction);
						}
					}
				}
			}
			else if (triggeredEvent.ID == AIEventID.IncomingProjectile && abilityController != null)
			{
				AIAbility aIAbility = abilityController.AbilityByName("PatriarchLastStand") as AIAbility;
				if (aIAbility != null && aIAbility.enabled && aIAbility.IsCooldownReady())
				{
					aIAbility.Cast();
					abilityController.OnCastCompleted(aIAbility);
				}
			}
		}
		return transition;
	}
}
