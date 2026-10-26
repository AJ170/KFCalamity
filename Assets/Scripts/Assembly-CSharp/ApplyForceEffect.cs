using EffectEvents;
using EventHelpers;
using Ragdoll;
using UnityEngine;

public class ApplyForceEffect : EffectBaseListener
{
	public enum ForceType
	{
		Direct = 0,
		Explosive = 1
	}

	public float MinForce = 50f;

	public float MaxForce = 100f;

	public Vector3 Offset;

	public bool RelativeOffset;

	public ForceType Type;

	private AttributeModifier forceAttribute;

	private void OnMounted(GameObject parent)
	{
		forceAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Force, parent);
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && (e.Id == TriggerId.Hit || e.Id == TriggerId.HitAOE))
		{
			HitBase hitBase = (HitBase)e;
			RagdollController component = hitBase.HitTransform.GetComponent<RagdollController>();
			if (component != null)
			{
				float multiplier = GetMultiplier();
				component.ApplyEffect(new AppliedRagdollEffect(Random.Range(MinForce * multiplier, MaxForce * multiplier), (!RelativeOffset) ? (base.transform.position + Offset) : base.transform.TransformPoint(Offset), ForceTypeToRagdollEffectType(Type)));
			}
		}
	}

	protected virtual float GetMultiplier()
	{
		float result = 1f;
		if (forceAttribute != null)
		{
			result = forceAttribute.Multiplier;
		}
		return result;
	}

	private RagdollEffectType ForceTypeToRagdollEffectType(ForceType force)
	{
		RagdollEffectType result = RagdollEffectType.DirectForce;
		switch (force)
		{
		case ForceType.Direct:
			result = RagdollEffectType.DirectForce;
			break;
		case ForceType.Explosive:
			result = RagdollEffectType.ExplosiveForce;
			break;
		}
		return result;
	}
}
