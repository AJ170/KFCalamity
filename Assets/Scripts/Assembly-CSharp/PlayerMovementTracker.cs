using UnityEngine.AI;
using System.Collections.Generic;
using Calamity.Camera;
using UnityEngine;

public class PlayerMovementTracker : MonoBehaviour
{
	public float updateFrequency = 0.25f;

	public int PositionsToKeep = 10;

	public float DistanceThreshold = 5f;

	public float DistanceMultiplier = 1.5f;

	public float MaxAdjustDistance = 3f;

	public LayerMask Mask;

	public bool DebugMode;

	private Camera mainCamera;

	private FollowCamera followCamera;

	private Queue<Vector3> recentCameraPositions;

	private float updateCounter;

	private void Start()
	{
		recentCameraPositions = new Queue<Vector3>(PositionsToKeep);
		UpdateCamera();
	}

	private void Update()
	{
		updateCounter -= Time.deltaTime;
		if (updateCounter <= 0f)
		{
			UpdatePosition();
			updateCounter = updateFrequency;
		}
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

	private void UpdatePosition()
	{
		if (!(mainCamera != null) || followCamera == null)
		{
			return;
		}
		Vector3 cameraCenter = followCamera.CameraCenter;
		if (cameraCenter != Vector3.zero)
		{
			if (recentCameraPositions.Count == PositionsToKeep)
			{
				recentCameraPositions.Dequeue();
			}
			recentCameraPositions.Enqueue(followCamera.CameraCenter);
		}
	}

	public PlayerPrediction GetPredictedPosition()
	{
		int count = recentCameraPositions.Count;
		if (count > 1)
		{
			Vector3 cameraCenter = followCamera.CameraCenter;
			Vector3 predictedPosition;
			Vector3 direction;
			float distance;
			GetPositionFromMovement(cameraCenter, out predictedPosition, out direction, out distance);
			bool flag = distance > DistanceThreshold;
			if (flag)
			{
				predictedPosition += direction * distance * DistanceMultiplier;
				AdjustToSpawnBlockers(cameraCenter, ref predictedPosition, direction, distance);
				if (NavMeshSamplePosition(ref predictedPosition))
				{
					Vector3 position = cameraCenter;
					if (NavMeshSamplePosition(ref position))
					{
						AdaptToPath(position, ref predictedPosition, distance);
					}
				}
				else
				{
					predictedPosition = cameraCenter;
					NavMeshSamplePosition(ref predictedPosition);
				}
			}
			else
			{
				predictedPosition = cameraCenter;
			}
			return new PlayerPrediction(predictedPosition, direction, flag);
		}
		if (count == 1)
		{
			return new PlayerPrediction(recentCameraPositions.Peek(), Vector3.zero, false);
		}
		if (mainCamera != null && followCamera != null)
		{
			return new PlayerPrediction(followCamera.CameraCenter, Vector3.zero, false);
		}
		return null;
	}

	private void GetPositionFromMovement(Vector3 center, out Vector3 predictedPosition, out Vector3 direction, out float distance)
	{
		Vector3 vector = recentCameraPositions.Peek();
		Vector3 vector2 = vector;
		direction = Vector3.zero;
		foreach (Vector3 recentCameraPosition in recentCameraPositions)
		{
			direction += recentCameraPosition - vector2;
			vector2 = recentCameraPosition;
		}
		distance = Vector3.Distance(vector, vector2);
		direction.Normalize();
		predictedPosition = vector2;
	}

	private void AdjustToSpawnBlockers(Vector3 center, ref Vector3 position, Vector3 direction, float distance)
	{
		RaycastHit hitInfo;
		if (Physics.Linecast(center, position, out hitInfo, Mask.value))
		{
			position = hitInfo.point;
		}
	}

	private bool NavMeshSamplePosition(ref Vector3 position)
	{
		NavMeshHit hit;
		if (NavMesh.SamplePosition(position, out hit, MaxAdjustDistance, -1))
		{
			position = hit.position;
			return true;
		}
		return false;
	}

	private void AdaptToPath(Vector3 center, ref Vector3 position, float distance)
	{
		NavMeshPath navMeshPath = new NavMeshPath();
		if (!NavMesh.CalculatePath(center, position, 1, navMeshPath))
		{
			return;
		}
		float num = 0f;
		float num2 = distance * distance;
		Vector3 vector = center;
		Vector3[] corners = navMeshPath.corners;
		foreach (Vector3 vector2 in corners)
		{
			Vector3 vector3 = vector2 - vector;
			distance = vector3.sqrMagnitude;
			if (num + distance >= num2)
			{
				float num3 = Mathf.Sqrt(num2 - num);
				position = vector + vector3.normalized * num3;
				break;
			}
			num += distance;
			vector = vector2;
		}
	}

	public Vector3? GetCameraCenter()
	{
		if (mainCamera != null && followCamera != null)
		{
			return followCamera.CameraCenter;
		}
		return null;
	}

	public bool WithinCameraBounds(Vector3 position)
	{
		if (mainCamera != null)
		{
			Vector3 vector = mainCamera.WorldToScreenPoint(position);
			return !(vector.x < 0f) && !(vector.y < 0f) && !(vector.x > (float)Screen.width) && !(vector.y > (float)Screen.height);
		}
		return true;
	}
}
