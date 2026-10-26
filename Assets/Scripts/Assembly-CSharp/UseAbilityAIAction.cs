using AIActionTransitions;
using EffectEvents;
using UnityEngine;

internal class UseAbilityAIAction : AIAction
{
	private enum UseAbilityState
	{
		Inactive = 0,
		ChargeUp = 1,
		WindDown = 2,
		Completed = 3
	}

	private AIAbility ability;

	private AIAbilityController abilityController;

	private UseAbilityState abilityState;

	private float counter;

	private bool firstUpdate = true;

	public UseAbilityAIAction(AIAbility ability)
	{
		this.ability = ability;
	}

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		Transition result = null;
		if (triggeredEvent.ID == AIEventID.Rampage)
		{
			result = new Transition(TransitionType.SuspendFor, new RampageAIAction());
		}
		else if (triggeredEvent.ID == AIEventID.Impale)
		{
			if (status == AIActionStatus.Active)
			{
				result = new Transition(TransitionType.SuspendFor, new ImpaleAIAction());
			}
		}
		else if (triggeredEvent.ID == AIEventID.Motion && status == AIActionStatus.Active)
		{
			MotionAIEvent motionAIEvent = triggeredEvent as MotionAIEvent;
			if (motionAIEvent != null)
			{
				result = new Transition(TransitionType.SuspendFor, new MotionAIAction(motionAIEvent.Agent, motionAIEvent.Link, motionAIEvent.Motion));
			}
		}
		return result;
	}

	public override void OnStart()
	{
		if (ability.CanDud && Random.Range(0f, 1f) <= ability.DudChance)
		{
			ability.TriggerDud();
			firstUpdate = false;
			status = AIActionStatus.Complete;
		}
		else
		{
			controller.TriggerGlobalEvent(new AIUseAbility());
			abilityController = controller.GetComponent<AIAbilityController>();
			base.OnStart();
		}
	}

	public override Transition Update()
	{
		if (firstUpdate)
		{
			if (ability.ChargeUp > 0f)
			{
				StartChargeUp();
			}
			else
			{
				Cast();
			}
			SetMovement();
			firstUpdate = false;
		}
		else
		{
			UpdateTimer();
		}
		return base.Update();
	}

	private void UpdateTimer()
	{
		if (!(counter > 0f))
		{
			return;
		}
		counter -= Time.deltaTime;
		if (counter <= 0f)
		{
			if (abilityState == UseAbilityState.ChargeUp)
			{
				Cast();
			}
			else
			{
				OnCompleted();
			}
		}
	}

	private void StartChargeUp()
	{
		ability.OnChargeUp();
		counter = ability.ChargeUp;
		abilityState = UseAbilityState.ChargeUp;
	}

	private void Cast()
	{
		ability.Cast();
		if (abilityController != null)
		{
			abilityController.OnCastCompleted(ability);
		}
		if (ability.WindDown > 0f)
		{
			StartWindDown();
		}
		else
		{
			OnCompleted();
		}
	}

	private void StartWindDown()
	{
		counter = ability.WindDown;
		abilityState = UseAbilityState.WindDown;
	}

	private void OnCompleted()
	{
		status = AIActionStatus.Complete;
	}

	private void SetMovement()
	{
		float num = ability.ChargeUp + ability.WindDown;
		if (num > 0.1f)
		{
			switch (ability.MoveAndUse)
			{
			case Ability.MoveAndUseMode.RotateFreely:
			case Ability.MoveAndUseMode.Freeze:
				actionController.Secondary.Push(new HoldGroundAIAction(num));
				break;
			case Ability.MoveAndUseMode.FaceTarget:
				actionController.Secondary.Push(new FaceTargetAIAction(FaceTargetAIAction.ExpirationMode.Timer, num));
				break;
			case Ability.MoveAndUseMode.FaceThenFreeze:
				actionController.Secondary.Push(new FaceTargetAIAction(FaceTargetAIAction.ExpirationMode.Timer, ability.ChargeUp));
				actionController.Secondary.Queue(new HoldGroundAIAction(ability.WindDown));
				break;
			}
		}
	}
}
