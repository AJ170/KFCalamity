using UnityEngine.AI;
using System.Collections.Generic;
using Spawning;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	private const float invalidRating = -1f;

	public float Desirability = 1f;

	public ZedType AllowedUnits = (ZedType)(-1);

	public float Cooldown = 20f;

	private float cooldownCounter;

	private Vector3 targetPosition = Vector3.zero;

	public float Rating
	{
		get
		{
			return 1f;
		}
	}

	public Vector3 Position
	{
		get
		{
			return targetPosition;
		}
	}

	private void Start()
	{
		NavMeshHit hit;
		if (NavMesh.SamplePosition(base.transform.position, out hit, 2f, -1))
		{
			targetPosition = hit.position;
			base.transform.position = hit.position;
		}
		else
		{
			targetPosition = base.transform.position;
			Debug.Log("Spawn point failed to sample target position", base.gameObject);
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (cooldownCounter != 0f)
		{
			cooldownCounter = Mathf.Max(cooldownCounter - Time.deltaTime, 0f);
		}
	}

	public float Rate(RatingData ratingData)
	{
		if (base.enabled)
		{
			if (ratingData.Squad != null && !IsSquadCompatible(ratingData.Squad))
			{
				return -1f;
			}
			float num = ratingData.MaxRange * ratingData.MaxRange;
			Vector3 vector = Position - ratingData.Position;
			float num2 = 1f - vector.sqrMagnitude / num;
			float num3 = 1f;
			if (ratingData.Direction != Vector3.zero)
			{
				float num4 = Vector3.Angle(ratingData.Direction, vector.normalized);
				num3 = 1f - num4 / 180f;
			}
			float num5 = 1f;
			if (cooldownCounter > 0f)
			{
				num5 = 1f - cooldownCounter / Cooldown;
			}
			float value = Random.value;
			return Desirability * RatingWeight.DESIRABILITY + num2 * RatingWeight.DISTANCE + num3 * RatingWeight.DIRECTION + num5 * RatingWeight.COOLDOWN + value * RatingWeight.RANDOM;
		}
		return -1f;
	}

	private bool IsSquadCompatible(List<ZedType> squad)
	{
		int allowedUnits = (int)AllowedUnits;
		foreach (ZedType item in squad)
		{
			if (((uint)allowedUnits & (uint)item) == 0)
			{
				return false;
			}
		}
		return true;
	}

	public void TriggerCooldown()
	{
		cooldownCounter = Cooldown;
	}
}
