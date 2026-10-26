using System;
using System.Collections;
using EffectEvents;
using EventHelpers;
using UnityEngine;

public class ConeProjectileEffect : EffectBaseListener
{
	public string Name;

	public float Delay;

	public float Range = 5f;

	public float Angle = 60f;

	public bool UseTransformRoot = true;

	public bool IgnoreHeightDifference;

	public LayerMask Layer;

	public LayerMask HitLayer = 2816;

	protected override void Execute(TriggerBase e)
	{
		if (NoCustomEvents() || MatchCustomEvent(e.CustomName))
		{
			StartCoroutine(PerformCone());
		}
	}

	protected virtual IEnumerator PerformCone()
	{
		if (Delay > 0f)
		{
			yield return new WaitForSeconds(Delay);
		}
		Transform source = ((!UseTransformRoot) ? base.transform : base.transform.root);
		Vector3 position = source.position;
		Collider[] targets = Physics.OverlapSphere(position, Range, Layer);
		if (targets.Length <= 0)
		{
			yield break;
		}
		float cone = Mathf.Cos(Angle * ((float)Math.PI / 180f));
		Vector3 forward = source.forward;
		Collider[] array = targets;
		foreach (Collider target in array)
		{
			if (HitLayerMatch(target.gameObject.layer) && PositionIsInArc(position, target.transform.position, forward, cone))
			{
				TriggerHitEvents(target.transform);
			}
		}
	}

	private void DebugDraw(Transform source)
	{
		float num = Angle * 0.5f * ((float)Math.PI / 180f);
		Vector3 position = source.position;
		Vector3 forward = source.forward;
		Vector3 vector = new Vector3(forward.x, forward.y, forward.z).RotateY(0f - num);
		Vector3 vector2 = new Vector3(forward.x, forward.y, forward.z).RotateY(num);
		Debug.DrawLine(position, position + vector * Range, Color.red, 5f);
		Debug.DrawLine(position, position + vector2 * Range, Color.blue, 5f);
		Debug.DrawLine(position, position + forward * Range, Color.yellow, 5f);
	}

	protected bool PositionIsInArc(Vector3 source, Vector3 target, Vector3 direction, float cone)
	{
		if (IgnoreHeightDifference)
		{
			target.y = source.y;
		}
		Vector3 rhs = Vector3.Normalize(target - source);
		float num = Vector3.Dot(direction, rhs);
		return num > cone;
	}

	private void TriggerHitEvents(Transform hitTransform)
	{
		effectSystem.TriggerInternal(new Hit(Name, hitTransform));
		if (effectSystem.SystemType == EffectSystemType.Weapon)
		{
			effectSystem.TriggerGlobal(new WeaponHit(hitTransform));
		}
		effectSystem.TriggerRemote(hitTransform.gameObject, new BeingHit(base.transform.root.gameObject, hitTransform));
	}

	protected bool HitLayerMatch(int layer)
	{
		return (HitLayer.value & (1 << layer)) != 0;
	}
}
