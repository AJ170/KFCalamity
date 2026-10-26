using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine;

public class PathingTargets : MonoBehaviour
{
	private struct SortTarget
	{
		public PathTarget target;

		public float distance;

		public SortTarget(PathTarget target, float distance)
		{
			this.target = target;
			this.distance = distance;
		}
	}

	private int NumFarTargets = 5;

	private float FarRadius = 4f;

	private int NumNearTargets = 6;

	private float NearRadius = 2f;

	private float NearDistanceThreshold = 5f;

	private Transform source;

	private MovementMotor motor;

	private List<Vector3> largeCircle;

	private List<Vector3> smallCircle;

	private List<PathTarget> farTargets = new List<PathTarget>();

	private List<PathTarget> nearTargets = new List<PathTarget>();

	private bool needUpdateFarTargets = true;

	private bool needUpdateNearTargets = true;

	private float updateCooldown = 0.5f;

	private float farTimer;

	private float nearTimer;

	private int Comparison(SortTarget a, SortTarget b)
	{
		if (a.distance < b.distance)
		{
			return -1;
		}
		if (a.distance > b.distance)
		{
			return 1;
		}
		return 0;
	}

	private void Awake()
	{
		motor = GetComponent<MovementMotor>();
		source = base.transform;
		largeCircle = GlobalHelpers.CircleVectors(new Vector3(0f, 0f, 0f), NumFarTargets, FarRadius);
		smallCircle = GlobalHelpers.CircleVectors(new Vector3(0f, 0f, 0f), NumNearTargets, NearRadius);
	}

	private void Update()
	{
		if (farTimer > 0f)
		{
			farTimer = Mathf.Max(farTimer - Time.deltaTime, 0f);
			if (farTimer == 0f)
			{
				needUpdateFarTargets = true;
			}
		}
		if (nearTimer > 0f)
		{
			nearTimer = Mathf.Max(nearTimer - Time.deltaTime, 0f);
			if (nearTimer == 0f)
			{
				needUpdateNearTargets = true;
			}
		}
	}

	private void UpdateFarTargets()
	{
		if (needUpdateFarTargets)
		{
			farTargets = FetchTargetsByCircle(largeCircle);
			needUpdateFarTargets = false;
			farTimer = updateCooldown;
		}
	}

	private void UpdateNearTargets()
	{
		if (needUpdateNearTargets)
		{
			nearTargets = FetchTargetsByCircle(smallCircle);
			needUpdateNearTargets = false;
			nearTimer = updateCooldown;
		}
	}

	private List<PathTarget> FetchTargetsByCircle(List<Vector3> circle)
	{
		List<PathTarget> list = new List<PathTarget>();
		Vector3 position = source.position;
		Vector3 vector = motor.MovementDirection * motor.CurrentMovementSpeed();
		Vector3 up = Vector3.up;
		foreach (Vector3 item in circle)
		{
			Vector3 vector2 = position + item + vector;
			NavMeshHit hit;
			NavMesh.SamplePosition(vector2, out hit, 0.5f, 1);
			if (hit.hit)
			{
				Vector3 vector3 = vector2 - position;
				float num = Vector3.Magnitude(vector3);
				vector3.Normalize();
				if (!Physics.Raycast(position + up, vector3, num))
				{
					list.Add(new PathTarget(hit.position, num));
				}
			}
		}
		if (list.Count == 0)
		{
			list.Add(new PathTarget(position, 0f));
		}
		return list;
	}

	private List<PathTarget> FetchTargetsByDistance(float distance)
	{
		List<PathTarget> list = null;
		if (distance > NearDistanceThreshold)
		{
			UpdateFarTargets();
			return new List<PathTarget>(farTargets);
		}
		UpdateNearTargets();
		return new List<PathTarget>(nearTargets);
	}

	public PathTarget? GetRandomTarget()
	{
		return null;
	}

	public PathTarget? GetSmartZedTarget(Vector3 zedPosition)
	{
		PathTarget? pathTarget = null;
		float distance = Vector3.Distance(source.position, zedPosition);
		List<PathTarget> targets = FetchTargetsByDistance(distance);
		targets = FilterTargets(zedPosition, targets);
		int max = Mathf.Min(targets.Count, 3);
		return targets[Random.Range(0, max)];
	}

	private List<PathTarget> FilterTargets(Vector3 zedPosition, List<PathTarget> targets)
	{
		List<SortTarget> list = new List<SortTarget>();
		float num = 0f;
		foreach (PathTarget target in targets)
		{
			num = Vector3.Distance(zedPosition, target.Position);
			if (num >= 1f || list.Count == 0)
			{
				list.Add(new SortTarget(target, num));
			}
		}
		list.Sort(Comparison);
		targets.Clear();
		foreach (SortTarget item in list)
		{
			targets.Add(item.target);
		}
		return targets;
	}

	private GameObject CreateSphere(Vector3 position, float scale)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		gameObject.transform.position = position;
		gameObject.transform.localScale = new Vector3(scale, scale, scale);
		gameObject.GetComponent<Collider>().enabled = false;
		Object.Destroy(gameObject, 3f);
		return gameObject;
	}
}
