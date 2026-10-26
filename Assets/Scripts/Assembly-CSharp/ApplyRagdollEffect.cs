using EffectEvents;
using EventHelpers;
using Ragdoll;
using UnityEngine;

public class ApplyRagdollEffect : EffectBaseListener
{
	public enum RagdollEffect
	{
		Burning = 0
	}

	public RagdollEffect Type;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && (e.Id == TriggerId.Hit || e.Id == TriggerId.HitAOE))
		{
			HitBase hitBase = (HitBase)e;
			GameObject target = hitBase.HitTransform.gameObject;
			ApplyEffect(target);
		}
	}

	private void ApplyEffect(GameObject target)
	{
		RagdollController component = target.GetComponent<RagdollController>();
		if (component != null)
		{
			component.ApplyEffect(new AppliedRagdollEffect(RagdollEffectType.Burning));
		}
	}
}
