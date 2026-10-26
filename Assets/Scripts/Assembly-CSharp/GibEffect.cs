using EffectEvents;
using EventHelpers;
using Ragdoll;
using UnityEngine;

public class GibEffect : EffectBaseListener
{
	public GibType Type = GibType.Lesser;

	public float GibChance = 1f;

	public bool CheckRange;

	public float MaxRange = 5f;

	public bool IgnoreHeight;

	private GameObject parent;

	private void OnMounted(GameObject parent)
	{
		this.parent = parent;
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && Type != 0)
		{
			GameObject gameObject = null;
			if (e.Id == TriggerId.Hit)
			{
				HitBase hitBase = (HitBase)e;
				gameObject = hitBase.HitTransform.gameObject;
			}
			else if (e.Id == TriggerId.Cast)
			{
				gameObject = parent;
			}
			if (gameObject != null && ShouldTrigger(e))
			{
				ApplyGib(gameObject);
			}
		}
	}

	private void ApplyGib(GameObject target)
	{
		if (target != null)
		{
			RagdollController component = target.GetComponent<RagdollController>();
			if (component != null)
			{
				component.ApplyEffect(new AppliedRagdollEffect(ForceTypeToRagdollEffectType(Type)));
			}
		}
	}

	private bool PerformRangeCheck(TriggerBase e)
	{
		float num = 0f;
		if (e.Id == TriggerId.Hit)
		{
			HitBase hitBase = (HitBase)e;
			Transform hitTransform = hitBase.HitTransform;
			Vector3 position = hitTransform.position;
			if (IgnoreHeight)
			{
				position.y = base.transform.position.y;
			}
			num = Vector3.Distance(base.transform.position, position);
		}
		else if (e.Id == TriggerId.HitAOE)
		{
			HitAOE hitAOE = (HitAOE)e;
			num = hitAOE.DistanceFromImpact;
		}
		return num < MaxRange;
	}

	protected virtual bool ShouldTrigger(TriggerBase e)
	{
		bool flag = Random.value < GibChance;
		if (flag && CheckRange)
		{
			flag = PerformRangeCheck(e);
		}
		return flag;
	}

	private RagdollEffectType ForceTypeToRagdollEffectType(GibType gibType)
	{
		RagdollEffectType result = RagdollEffectType.LesserGib;
		switch (gibType)
		{
		case GibType.Lesser:
			result = RagdollEffectType.LesserGib;
			break;
		case GibType.Greater:
			result = RagdollEffectType.GreaterGib;
			break;
		case GibType.UpperBody:
			result = RagdollEffectType.UpperBodyGib;
			break;
		}
		return result;
	}
}
