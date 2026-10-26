using UnityEngine.AI;
using Calamity.Input;
using EffectHelpers;
using UnityEngine;

public class RampageKnockbackEffect : EffectBaseTarget
{
	private SoldierInputController inputController;

	private MovementMotor movementMotor;

	private EffectResistance constraint;

	protected float duration = 0.15f;

	protected float knockbackForce = 20f;

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
				Vector3 vector = ((Random.Range(0, 2) != 0) ? (-transform.right) : transform.right);
				Vector3 normalized = (transform.forward + vector).normalized;
				movementMotor.SetForcedVelocity(normalized * knockbackForce * constraint.Multiplier);
			}
			else
			{
				Debug.LogError("RampageKnockbackEffect:OnActivated() Missing MovementMotor");
				ForceDelete();
			}
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
		Transform transform = gameObject.transform;
		NavMeshHit hit;
		if (NavMesh.SamplePosition(transform.position, out hit, 4f, 1))
		{
			transform.position = hit.position;
		}
	}
}
