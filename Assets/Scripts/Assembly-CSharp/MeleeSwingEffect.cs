using System;
using System.Collections;
using System.Collections.Generic;
using EffectEvents;
using EventHelpers;
using UnityEngine;

public class MeleeSwingEffect : EffectBaseListener
{
	private class ActualHits
	{
		public Collider collider;

		public float angle;

		public ActualHits(Collider collider, float angle)
		{
			this.collider = collider;
			this.angle = angle;
		}
	}

	public string Name;

	public float Delay;

	public float Range = 5f;

	public float Angle = 60f;

	public float ConeOffset = -0.5f;

	public float HeightThreshold = 1.5f;

	public int Hits = 1;

	public LayerMask Layer;

	public LayerMask HitLayer = 2816;

	private List<ActualHits> actualHits = new List<ActualHits>();

	protected override void Execute(TriggerBase e)
	{
		actualHits.Clear();
		if (NoCustomEvents() || MatchCustomEvent(e.CustomName))
		{
			if (Delay > 0f)
			{
				StartCoroutine(WaitForDelayBeforeFire());
			}
			else
			{
				PerformCone();
			}
		}
	}

	private IEnumerator WaitForDelayBeforeFire()
	{
		yield return new WaitForSeconds(Delay);
		PerformCone();
	}

	protected virtual void PerformCone()
	{
		Transform root = base.transform.root;
		Vector3 up = Vector3.up;
		Vector3 vector = root.forward * ConeOffset;
		Vector3 eulerAngles = root.rotation.eulerAngles;
		Vector3 vector2 = root.position + up;
		Vector3 source = vector2 + vector;
		Vector3 zero = Vector3.zero;
		Collider[] array = Physics.OverlapSphere(vector2, Range, Layer);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (!HitLayerMatch(collider.gameObject.layer))
			{
				continue;
			}
			zero = collider.transform.root.position + up;
			if (AccetpedHeightDifference(vector2, zero) && AcceptedRange(source, zero))
			{
				float angle = AngleToTarget(source, eulerAngles.y, zero);
				if (IsInsideCone(angle))
				{
					AddToActualHits(collider, angle);
				}
			}
		}
		int num = Mathf.Min(actualHits.Count, Hits);
		for (int j = 0; j < num; j++)
		{
			TriggerHitEvents(actualHits[j].collider.transform);
		}
	}

	private void DebugDraw()
	{
		float num = Angle * 0.5f * ((float)Math.PI / 180f);
		Transform root = base.transform.root;
		Vector3 position = root.position;
		Vector3 forward = root.forward;
		Vector3 vector = new Vector3(forward.x, forward.y, forward.z).RotateY(0f - num);
		Vector3 vector2 = new Vector3(forward.x, forward.y, forward.z).RotateY(num);
		float duration = 2f;
		Debug.DrawLine(position, position + vector * Range, Color.red, duration);
		Debug.DrawLine(position, position + vector2 * Range, Color.blue, duration);
		Debug.DrawLine(position, position + forward * Range, Color.yellow, duration);
	}

	private bool AccetpedHeightDifference(Vector3 source, Vector3 target)
	{
		float num = Mathf.Abs(source.y - target.y);
		return num <= HeightThreshold;
	}

	private bool AcceptedRange(Vector3 source, Vector3 target)
	{
		float num = Mathf.Abs(ConeOffset) * 0.5f;
		float num2 = Vector3.Distance(source, target);
		return num2 >= num;
	}

	private float AngleToTarget(Vector3 source, float sourceAngle, Vector3 target)
	{
		float num = (Mathf.Atan2(source.x - target.x, source.z - target.z) + (float)Math.PI) * 57.29578f;
		return (num - sourceAngle) * ((float)Math.PI / 180f);
	}

	private bool IsInsideCone(float angle)
	{
		float num = Angle * 0.5f * ((float)Math.PI / 180f);
		return Mathf.Abs(angle) <= num;
	}

	private void AddToActualHits(Collider collider, float angle)
	{
		ActualHits item = new ActualHits(collider, angle);
		if (actualHits.Count == 0)
		{
			actualHits.Add(item);
			return;
		}
		for (int i = 0; i < actualHits.Count; i++)
		{
			if (actualHits[i].angle > angle)
			{
				actualHits.Insert(i, item);
				break;
			}
			if (i == actualHits.Count - 1)
			{
				actualHits.Add(item);
				break;
			}
		}
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

	private bool HitLayerMatch(int layer)
	{
		return (HitLayer.value & (1 << layer)) != 0;
	}
}
