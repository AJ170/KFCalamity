using EffectEvents;
using EventHelpers;
using UnityEngine;

public class SphereCollisionHitEffect : SphereCollisionBase
{
	public bool TriggerAOEHit = true;

	public bool TriggerRemoteHit = true;

	public bool TriggerImpact = true;

	public bool MatchTag;

	public string[] Tags;

	protected override void Execute(TriggerBase e)
	{
		if (!NoCustomEvents() && !MatchCustomEvent(e.CustomName))
		{
			return;
		}
		Vector3 position;
		if (e.Id == TriggerId.Impact)
		{
			Impact impact = (Impact)e;
			position = impact.Position;
		}
		else
		{
			position = base.transform.position;
		}
		float distanceFromImpact = 0f;
		float num = Radius * GetMultiplier();
		Collider[] array = Physics.OverlapSphere(position, num, Layer);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if ((HitLayerMatch(collider.gameObject.layer) || (MatchTag && HitTagMatch(collider.tag))) && IsColliderAccepted(collider))
			{
				if (TriggerAOEHit)
				{
					distanceFromImpact = Mathf.Min(CalculateDistance2D(position, collider.transform.position) - ColliderRadius2D(collider, position), num);
					distanceFromImpact = Mathf.Max(distanceFromImpact, 0f);
				}
				TriggerHitEvents(collider.transform, num, distanceFromImpact);
			}
		}
		if (TriggerImpact)
		{
			TriggerImpactEvent(position);
		}
	}

	private void TriggerHitEvents(Transform hitTransform, float radius, float distanceFromImpact)
	{
		effectSystem.TriggerInternal(new Hit(Name, hitTransform));
		if (TriggerAOEHit)
		{
			effectSystem.TriggerInternal(new HitAOE(Name, hitTransform, radius, distanceFromImpact));
		}
		if (effectSystem.SystemType == EffectSystemType.Weapon)
		{
			effectSystem.TriggerGlobal(new WeaponHit(hitTransform));
		}
		if (TriggerRemoteHit)
		{
			effectSystem.TriggerRemote(hitTransform.gameObject, new BeingHit(base.transform.root.gameObject, hitTransform));
		}
	}

	private void TriggerImpactEvent(Vector3 position)
	{
		effectSystem.TriggerInternal(new Impact(Name, position));
	}

	protected virtual float GetMultiplier()
	{
		return 1f;
	}

	private float CalculateDistance2D(Vector3 source, Vector3 target)
	{
		source.y = 0f;
		target.y = 0f;
		return Vector3.Distance(source, target);
	}

	private float ColliderRadius2D(Collider target, Vector3 impactPosition)
	{
		float num = 0f;
		if (target is CapsuleCollider)
		{
			return ((CapsuleCollider)target).radius;
		}
		if (target is CharacterController)
		{
			return ((CharacterController)target).radius;
		}
		if (target is SphereCollider)
		{
			return ((SphereCollider)target).radius;
		}
		Vector3 a = target.ClosestPointOnBounds(impactPosition);
		a.y = target.transform.position.y;
		return Vector3.Distance(a, target.transform.position);
	}

	protected bool HitTagMatch(string tag)
	{
		string[] tags = Tags;
		foreach (string text in tags)
		{
			if (text == tag)
			{
				return true;
			}
		}
		return false;
	}
}
