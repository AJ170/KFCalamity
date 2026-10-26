using System.Collections.Generic;
using EffectEvents;
using UnityEngine;

public class AdvancedRaycastProjectileEffect : RaycastProjectileEffect
{
	private class QueuedAttack
	{
		public Vector3 Source;

		public Vector3 Direction;

		public Attack Event;

		public QueuedAttack(Vector3 source, Vector3 direction, Attack attackEvent = null)
		{
			Source = source;
			Direction = direction;
			Event = attackEvent;
		}
	}

	public enum ProjectileBoneOffset
	{
		None = 0,
		RightMuzzle = 1,
		LeftMuzzle = 2
	}

	public float HorizontalSpread;

	public float VerticalSpread;

	public int NumberOfProjectiles = 1;

	public bool UsePiercing;

	public float PierceStrength = 1f;

	public float RaycastOffset = -0.3f;

	public ProjectileBoneOffset PresetOffset;

	private Queue<QueuedAttack> queuedAttacks;

	private GameObject parent;

	private AttributeModifier spreadModifier;

	protected override void Start()
	{
		base.Start();
		queuedAttacks = new Queue<QueuedAttack>(NumberOfProjectiles);
		spreadModifier = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Spread, base.gameObject);
	}

	protected virtual void OnMounted(GameObject parent)
	{
		this.parent = parent;
	}

	private void Update()
	{
		if (queuedAttacks.Count <= 0)
		{
			return;
		}
		int num = Mathf.Min(queuedAttacks.Count, Mathf.CeilToInt((float)NumberOfProjectiles / 2f));
		for (int i = 0; i < num; i++)
		{
			QueuedAttack queuedAttack = queuedAttacks.Dequeue();
			if (UsePiercing)
			{
				FirePiercingRaycast(queuedAttack.Source, queuedAttack.Direction, queuedAttack.Event);
			}
			else
			{
				FireRaycast(queuedAttack.Source, queuedAttack.Direction, queuedAttack.Event);
			}
		}
	}

	protected override void PerformRaycast(Attack attackEvent)
	{
		if (BoneOffset == null)
		{
			FindMuzzle();
		}
		Vector3 position = BoneOffset.position;
		for (int i = 0; i < NumberOfProjectiles; i++)
		{
			Vector3 vector;
			if (HorizontalSpread > 0f || VerticalSpread > 0f)
			{
				Quaternion quaternion = Quaternion.Euler(GetSpread(VerticalSpread), GetSpread(HorizontalSpread), 0f);
				vector = BoneOffset.rotation * quaternion * Vector3.forward;
			}
			else
			{
				vector = BoneOffset.forward;
			}
			position = BoneOffset.position + vector * RaycastOffset;
			if (NumberOfProjectiles > 1)
			{
				queuedAttacks.Enqueue(new QueuedAttack(position, vector, attackEvent));
			}
			else if (UsePiercing)
			{
				FirePiercingRaycast(position, vector, attackEvent);
			}
			else
			{
				FireRaycast(position, vector, attackEvent);
			}
		}
	}

	protected void FirePiercingRaycast(Vector3 position, Vector3 direction, Attack attackEvent)
	{
		RaycastHit[] hits = Physics.RaycastAll(position, direction, Range, Layer.value);
		bool flag = false;
		if (hits.Length > 0)
		{
			SortHits(ref hits);
			float num = 0f;
			RaycastHit[] array = hits;
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit = array[i];
				Transform transform = raycastHit.transform;
				if (transform != base.transform && HitLayerMatch(transform.gameObject.layer))
				{
					TriggerHitEvents(transform, direction, attackEvent);
				}
				Health component = transform.GetComponent<Health>();
				num = ((!(component != null)) ? float.MaxValue : (num + component.PiercingResistance));
				if (num >= PierceStrength)
				{
					TriggerProjectileEvent(direction, raycastHit.point, raycastHit.collider);
					TriggerImpactEvent(new Impact(Name, raycastHit.point, raycastHit.collider, -direction));
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			TriggerProjectileEvent(direction, position + direction * Range);
			TriggerImpactEvent(new Impact(Name, position + direction * Range));
		}
	}

	private void SortHits(ref RaycastHit[] hits)
	{
		if (hits.Length > 1)
		{
			InsertSort(ref hits);
		}
	}

	private void InsertSort(ref RaycastHit[] hits)
	{
		for (int i = 1; i < hits.Length; i++)
		{
			RaycastHit raycastHit = hits[i];
			int num = i;
			while (num > 0 && raycastHit.distance < hits[num - 1].distance)
			{
				hits[num] = hits[num - 1];
				num--;
			}
			hits[num] = raycastHit;
		}
	}

	private void InsertSortT(ref int[] hits)
	{
		for (int i = 1; i < hits.Length; i++)
		{
			int num = hits[i];
			int num2 = i;
			while (num2 > 0 && num < hits[num2 - 1])
			{
				hits[num2] = hits[num2 - 1];
				num2--;
			}
			hits[num2] = num;
		}
	}

	private void Quicksort(ref RaycastHit[] hits, int left, int right)
	{
		if (hits.Length > 1)
		{
			int pivot = hits.Length / 2;
			int num = Partition(ref hits, 0, hits.Length - 1, pivot);
			Quicksort(ref hits, left, num - 1);
			Quicksort(ref hits, num + 1, right);
		}
	}

	private int Partition(ref RaycastHit[] hits, int left, int right, int pivot)
	{
		RaycastHit raycastHit = hits[pivot];
		float distance = raycastHit.distance;
		hits[pivot] = hits[right];
		hits[right] = raycastHit;
		int num = left;
		for (int i = left; i < right; i++)
		{
			if (hits[i].distance <= distance)
			{
				RaycastHit raycastHit2 = hits[i];
				hits[i] = hits[num];
				hits[num] = raycastHit2;
				num++;
			}
		}
		hits[right] = hits[num];
		hits[num] = raycastHit;
		return num;
	}

	protected float GetSpread(float spread)
	{
		spread *= spreadModifier.Multiplier;
		float num = 0f;
		int num2 = 2;
		for (int i = 0; i < num2; i++)
		{
			num += Random.Range(0f - spread, spread);
		}
		return num / (float)num2;
	}

	protected override void SetMuzzle()
	{
	}

	protected void FindMuzzle()
	{
		WeaponController component = parent.GetComponent<WeaponController>();
		if (component != null)
		{
			Weapon activeWeapon = component.ActiveWeapon;
			if (activeWeapon != null)
			{
				if (PresetOffset == ProjectileBoneOffset.RightMuzzle)
				{
					BoneOffset = activeWeapon.PrimaryMuzzleBone;
				}
				else if (PresetOffset == ProjectileBoneOffset.LeftMuzzle)
				{
					BoneOffset = activeWeapon.SecondaryMuzzleBone;
				}
			}
		}
		if (BoneOffset == null)
		{
			BoneOffset = base.transform;
		}
	}
}
