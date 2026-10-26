using UnityEngine.AI;
using System.Collections.Generic;
using Calamity.Camera;
using Spawning;
using UnityEngine;

public class RelocateManager : MonoBehaviour
{
	public float BaseRelocateRange = 30f;

	public float MinAliveTime = 5f;

	private SpawnPointManager spawnPointManager;

	private SpawnedUnitTracker spawnedUnitTracker;

	private Camera mainCamera;

	private FollowCamera followCamera;

	private QueuedRelocation queuedRelocation;

	private void Start()
	{
		spawnPointManager = GetComponent<SpawnPointManager>();
		spawnedUnitTracker = GetComponent<SpawnedUnitTracker>();
		UpdateCamera();
	}

	private void OnLevelWasLoaded(int level)
	{
		UpdateCamera();
	}

	private void UpdateCamera()
	{
		mainCamera = Camera.main;
		if (mainCamera != null)
		{
			CameraController component = mainCamera.GetComponent<CameraController>();
			if (component != null)
			{
				followCamera = component.FollowCamera;
			}
			else
			{
				followCamera = null;
			}
		}
		else
		{
			followCamera = null;
		}
	}

	public bool AnyUnitNeedsRelocating()
	{
		if (mainCamera != null && followCamera != null && spawnedUnitTracker != null && spawnPointManager != null)
		{
			Vector3 cameraCenter = followCamera.CameraCenter;
			float num = BaseRelocateRange * BaseRelocateRange;
			SpawnedUnit spawnedUnit = null;
			float num2 = num;
			List<SpawnedUnit> trackedUnits = spawnedUnitTracker.GetTrackedUnits();
			foreach (SpawnedUnit item in trackedUnits)
			{
				if (CanBeRelocated(item))
				{
					float sqrMagnitude = (item.GameObject.transform.position - cameraCenter).sqrMagnitude;
					if (sqrMagnitude > num2)
					{
						spawnedUnit = item;
						num2 = sqrMagnitude;
					}
				}
			}
			if (spawnedUnit != null)
			{
				queuedRelocation = new QueuedRelocation(spawnedUnit, num2);
				return true;
			}
			return false;
		}
		return false;
	}

	public void PrepareRelocation()
	{
		if (spawnPointManager != null && queuedRelocation != null)
		{
			List<ZedType> list = new List<ZedType>(1);
			list.Add(queuedRelocation.SpawnedObject.Type);
			List<ZedType> squad = list;
			RatingData ratingData = new RatingData(squad, SpawnRatingCallback);
			spawnPointManager.StartRating(ratingData);
		}
	}

	private void SpawnRatingCallback(SpawnPoint spawnPoint, Vector3 direction)
	{
		if (spawnPoint != null && queuedRelocation != null && queuedRelocation.SpawnedObject != null && queuedRelocation.SpawnedObject.GameObject != null)
		{
			GameObject gameObject = queuedRelocation.SpawnedObject.GameObject;
			Transform transform = gameObject.transform;
			NavMeshAgent component = gameObject.GetComponent<NavMeshAgent>();
			if (component != null)
			{
				component.Warp(spawnPoint.Position);
				transform.rotation = Quaternion.LookRotation(direction);
			}
			else
			{
				transform.position = spawnPoint.Position;
				transform.rotation = Quaternion.LookRotation(direction);
			}
			AIController component2 = gameObject.GetComponent<AIController>();
			if (component2 != null)
			{
				component2.TriggerAIEvent(new RelocatedAIEvent());
			}
		}
		queuedRelocation = null;
	}

	private bool CanBeRelocated(SpawnedUnit unit)
	{
		return unit.Type != ZedType.Patriarch && unit.GameObject != null && unit.TimeAlive > MinAliveTime;
	}
}
