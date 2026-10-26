using Calamity.Input;
using EffectHelpers;
using UnityEngine;

public class StompKnockbackEffect : EffectBaseTarget
{
	private SoldierInputController inputController;

	private MovementMotor movementMotor;

	private EffectResistance constraint;

	protected float duration = 0.15f;

	protected float knockbackForce = 20f;

	protected float offset = 0.5f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Debuff;
		buffClass = BuffClass.Constraint;
		repeatInterval = duration;
		maxRepeats = 1;
		inputController = gameObject.GetComponent<SoldierInputController>();
		movementMotor = gameObject.GetComponent<MovementMotor>();
		constraint = EffectResistanceController.CreateResistance(BuffClass.Constraint, gameObject);
	}

	protected override void OnActivated()
	{
		if (inputController != null)
		{
			inputController.SetMovementLock(true, true, this);
			if (movementMotor != null)
			{
				Transform transform = base.Source.transform;
				Transform transform2 = gameObject.transform;
				Vector3 vector = Vector3.Normalize(transform2.position - (transform.position + transform.forward * offset));
				movementMotor.SetForcedVelocity(vector * knockbackForce * constraint.Multiplier);
			}
			else
			{
				Debug.LogError("StompKnockbackEffect:OnActivated() Missing MovementMotor");
				ForceDelete();
			}
		}
		else
		{
			Debug.LogError("StompKnockbackEffect:OnActivated() Missing PlayerInputController");
			ForceDelete();
		}
	}

	public override void Shutdown()
	{
		base.Shutdown();
		if (movementMotor != null)
		{
			movementMotor.SetForcedVelocity(Vector3.zero);
		}
		if (inputController != null)
		{
			inputController.SetMovementLock(false, this);
		}
	}
}
