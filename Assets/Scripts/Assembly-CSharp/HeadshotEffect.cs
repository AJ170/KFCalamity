using EffectEvents;
using EventHelpers;
using Ragdoll;
using UnityEngine;

public class HeadshotEffect : EffectBaseListener
{
	public bool CanDecapitate;

	public float DecapitationChance = 0.5f;

	public float MinDecapitationForce = 5f;

	public float MaxDecapitationForce = 20f;

	public Vector3 ForceOffset = Vector3.zero;

	public bool RelativeForceOffset;

	public bool CanGib;

	public float GibChance = 0.5f;

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
		if (!hitBase.Headshot)
		{
			return;
		}
		RagdollController component = hitBase.HitTransform.GetComponent<RagdollController>();
		if (!(component != null))
		{
			return;
		}
		float action = GetAction();
		if (CanDecapitate && action <= DecapitationChance)
		{
			float num = 1f;
			if (forceAttribute != null)
			{
				num = forceAttribute.Multiplier;
			}
			component.ApplyEffect(new AppliedRagdollEffect(Random.Range(MinDecapitationForce * num, MaxDecapitationForce * num), (!RelativeForceOffset) ? (base.transform.position + ForceOffset) : base.transform.TransformPoint(ForceOffset), RagdollEffectType.HeadshotDecapitate));
		}
		else if (CanGib && action <= ((!CanDecapitate) ? 0f : DecapitationChance) + GibChance)
		{
			component.ApplyEffect(new AppliedRagdollEffect(RagdollEffectType.HeadshotGib));
		}
	}

	private float GetAction()
	{
		float num = 0f;
		if (CanDecapitate)
		{
			num += DecapitationChance;
		}
		if (CanGib)
		{
			num += GibChance;
		}
		num = Mathf.Max(num, 1f);
		return Random.value * num;
	}
}
