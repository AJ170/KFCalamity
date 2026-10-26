using UnityEngine.AI;
using System;
using EffectEvents;
using UnityEngine;

public class RocketRainProjectile : PhysicalProjectile
{
	public string TargetTag = "Soldier";

	public float Height = 10f;

	public float ImpactTime = 2f;

	public float TargetRadius = 10f;

	public float TargetOffsetRadius = 3f;

	public bool LimitToNavmesh = true;

	private GameObject target;

	private Vector3 targetPosition;

	private GameObject parent;

	private void OnMounted(GameObject parent)
	{
		this.parent = parent;
	}

	public override void Fire()
	{
		base.Fire();
		FindTargetPosition();
	}

	protected override void UpdateMovement()
	{
		if (UseRigidbody)
		{
			return;
		}
		Vector3 position = transform.position;
		float num = timeSinceSpawn / ImpactTime;
		if (num < 1f)
		{
			Vector3 vector = Vector3.Lerp(startPosition, targetPosition, num);
			vector.y += Height * Mathf.Sin((float)Math.PI * num);
			transform.position = vector;
			Vector3 forward = Vector3.Normalize(vector - position);
			transform.rotation = Quaternion.LookRotation(forward);
		}
		else
		{
			if (!impacted)
			{
				TriggerImpactEvent(new Impact("Projectile", position, null, -transform.forward));
				OnImpact(null);
			}
			base.UpdateMovement();
		}
	}

	private void FindTargetPosition()
	{
		Transform transform = parent.transform;
		Vector3 position = transform.position;
		GameObject[] array = GameObject.FindGameObjectsWithTag(TargetTag);
		int num = array.Length;
		float num2 = TargetRadius * TargetRadius;
		for (int i = 0; i < array.Length; i++)
		{
			if ((array[i].transform.position - position).sqrMagnitude > num2)
			{
				array[i] = null;
				num--;
			}
		}
		if (num > 0)
		{
			int num3 = UnityEngine.Random.Range(0, num);
			int num4 = 0;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] != null)
				{
					if (num4 == num3)
					{
						target = array[j];
					}
					else
					{
						num4++;
					}
				}
			}
		}
		if (target == null)
		{
			target = parent;
		}
		if (target != null)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * TargetOffsetRadius;
			targetPosition = target.transform.position + new Vector3(vector.x, 0f, vector.y);
			NavMeshHit hit;
			if (LimitToNavmesh && NavMesh.SamplePosition(targetPosition, out hit, TargetOffsetRadius, -1))
			{
				targetPosition = hit.position;
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}
}
