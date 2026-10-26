using EffectEvents;
using EventHelpers;
using Ragdoll;
using UnityEngine;

public class DecapitationEffect : EffectBaseListener
{
	public float DecapitationChance = 0.5f;

	public float MinDecapitationForce = 5f;

	public float MaxDecapitationForce = 20f;

	public Vector3 ForceOffset = Vector3.zero;

	public bool RelativeForceOffset;

	private AttributeModifier forceAttribute;

	private void OnMounted(GameObject parent)
	{
		forceAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Force, parent);
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((!NoCustomEvents() && !flag) || (e.Id != TriggerId.Hit && e.Id != TriggerId.HitAOE))
		{
			return;
		}
		HitBase hitBase = (HitBase)e;
		RagdollController component = hitBase.HitTransform.GetComponent<RagdollController>();
		if (component != null && Random.value <= DecapitationChance)
		{
			float num = 1f;
			if (forceAttribute != null)
			{
				num = forceAttribute.Multiplier;
			}
			component.ApplyEffect(new AppliedRagdollEffect(Random.Range(MinDecapitationForce * num, MaxDecapitationForce * num), (!RelativeForceOffset) ? (base.transform.position + ForceOffset) : base.transform.TransformPoint(ForceOffset), RagdollEffectType.HeadshotDecapitate));
		}
	}
}
