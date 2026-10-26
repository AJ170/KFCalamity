using System.Collections.Generic;
using UnityEngine;

public class FollowMultipleCamera : MonoBehaviour
{
	public string TargetTag = "Soldier";

	public Vector3 Offset = Vector3.zero;

	public bool UseCustomBoundaries;

	public Vector3 BoundRestrictions = Vector3.zero;

	public float TeleportThreshold = 1f;

	public float ZoomSpeed = 2f;

	public float ZoomExtentMultiplier = 0.5f;

	public float DistanceBeforeZoom = 3f;

	public float DistanceStopZoom = 6f;

	private LerpVector3Step cameraZoomLerp = new LerpVector3Step();

	private GameObject[] targets;

	private float updateFreq = 0.25f;

	private float updateTimer;

	private Vector3 cameraCenter = Vector3.zero;

	private Vector3 cameraZoom = Vector3.zero;

	private Vector3 averageTargetPosition = Vector3.zero;

	private int lastScreenWidth;

	private int lastScreenHeight;

	public Vector3 CameraCenter
	{
		get
		{
			return cameraCenter;
		}
	}

	public Vector3 AvarageTargetPosition
	{
		get
		{
			return averageTargetPosition;
		}
	}

	private void Start()
	{
		if (UseCustomBoundaries && BoundRestrictions == Vector3.zero)
		{
			BoundRestrictions = new Vector3(5f, 0f, 5f);
		}
	}

	private void Update()
	{
		if (TimeToUpdate())
		{
			UpdateBounds();
			UpdateTargets();
		}
		UpdateZoomLerp();
	}

	private void LateUpdate()
	{
		if (ValidateTargets())
		{
			UpdateAverageTargetPosition();
			UpdateZoom();
			UpdateCamera();
		}
	}

	private bool TimeToUpdate()
	{
		updateTimer = Mathf.Max(updateTimer - Time.deltaTime, 0f);
		if (updateTimer == 0f)
		{
			updateTimer = updateFreq;
			return true;
		}
		return false;
	}

	private void UpdateCamera()
	{
		Transform transform = base.transform;
		float num = Vector3.Magnitude(Offset);
		Vector3 vector = averageTargetPosition + Offset;
		Vector3 position = vector + cameraZoom;
		Vector3 center = vector + transform.forward * num;
		if (AllTargetsInsideBounds(center))
		{
			cameraCenter = center;
			transform.position = position;
		}
		else if (AllTargetsInsideBounds(cameraCenter))
		{
			transform.position = position;
		}
	}

	private void UpdateBounds()
	{
		if (UseCustomBoundaries)
		{
			return;
		}
		Camera camera = base.GetComponent<Camera>();
		if (camera != null)
		{
			int width = Screen.width;
			int height = Screen.height;
			if (lastScreenWidth != width || lastScreenHeight != height)
			{
				float z = Vector3.Magnitude(Offset);
				Vector3 a = camera.ViewportToWorldPoint(new Vector3(0f, 0f, z));
				Vector3 b = camera.ViewportToWorldPoint(new Vector3(1f, 0f, z));
				Vector3 b2 = camera.ViewportToWorldPoint(new Vector3(0f, 1f, z));
				float num = Vector3.Distance(a, b);
				float num2 = Vector3.Distance(a, b2);
				BoundRestrictions.x = num * 0.7f;
				BoundRestrictions.z = num2 * 0.9f;
				lastScreenWidth = width;
				lastScreenHeight = height;
			}
		}
	}

	private void DebugDrawBounds()
	{
		Vector3 vector = cameraCenter;
		float num = BoundRestrictions.x * 0.5f;
		float num2 = BoundRestrictions.z * 0.5f;
		float y = vector.y + 1f;
		Vector3 vector2 = new Vector3(vector.x - num, y, vector.z - num2);
		Vector3 vector3 = new Vector3(vector.x + num, y, vector.z - num2);
		Vector3 vector4 = new Vector3(vector.x - num, y, vector.z + num2);
		Vector3 vector5 = new Vector3(vector.x + num, y, vector.z + num2);
		float duration = 0.1f;
		Debug.DrawLine(vector2, vector3, Color.red, duration);
		Debug.DrawLine(vector3, vector5, Color.red, duration);
		Debug.DrawLine(vector5, vector4, Color.red, duration);
		Debug.DrawLine(vector4, vector2, Color.red, duration);
		Debug.DrawLine(vector + Vector3.forward, vector - Vector3.forward, Color.red, duration);
		Debug.DrawLine(vector + Vector3.right, vector - Vector3.right, Color.red, duration);
		Debug.DrawRay(vector, Vector3.up, Color.red, duration);
	}

	private void UpdateZoomLerp()
	{
		float num = CalculateLongestTargetDistance(targets);
		if (num > DistanceBeforeZoom)
		{
			float num2 = Mathf.Min(num, DistanceStopZoom) - DistanceBeforeZoom;
			Vector3 vector = -(base.transform.forward * num2) * ZoomExtentMultiplier;
			if (vector != cameraZoom)
			{
				cameraZoomLerp.Lerp(cameraZoom, vector, ZoomSpeed, null, true);
			}
		}
		else if (cameraZoom != Vector3.zero)
		{
			cameraZoomLerp.Lerp(cameraZoom, Vector3.zero, ZoomSpeed, null, true);
		}
	}

	private void UpdateZoom()
	{
		if (cameraZoomLerp.Lerping)
		{
			cameraZoomLerp.Update();
			cameraZoom = cameraZoomLerp.Current;
		}
	}

	private bool ValidateTargets()
	{
		if (targets != null && targets.Length > 0)
		{
			GameObject[] array = targets;
			foreach (GameObject gameObject in array)
			{
				if (gameObject == null)
				{
					updateTimer = 0f;
					return false;
				}
			}
			return true;
		}
		return false;
	}

	private void UpdateTargets()
	{
		targets = FindTargets();
	}

	private void UpdateAverageTargetPosition()
	{
		averageTargetPosition = Vector3.zero;
		GameObject[] array = targets;
		foreach (GameObject gameObject in array)
		{
			averageTargetPosition += gameObject.transform.position;
		}
		averageTargetPosition /= (float)targets.Length;
	}

	private GameObject[] FindTargets()
	{
		return GameObject.FindGameObjectsWithTag(TargetTag);
	}

	public Vector3 PushPositionToBounds(Vector3 source, out bool pushed)
	{
		if (BoundRestrictions == Vector3.zero || cameraCenter == Vector3.zero)
		{
			pushed = false;
			return source;
		}
		if (IsPositionInsideBounds(source))
		{
			pushed = false;
			return source;
		}
		if (IsPositionInsideTeleportBounds(source))
		{
			Vector3 vector = cameraCenter;
			float num = BoundRestrictions.x * 0.5f;
			float num2 = BoundRestrictions.z * 0.5f;
			float num3 = 0.01f;
			if (source.x < vector.x - num)
			{
				source.x = vector.x + num3 - num;
			}
			else if (source.x > vector.x + num)
			{
				source.x = vector.x - num3 + num;
			}
			if (source.z < vector.z - num2)
			{
				source.z = vector.z + num3 - num2;
			}
			else if (source.z > vector.z + num2)
			{
				source.z = vector.z - num3 + num2;
			}
		}
		else
		{
			source = GetValidTeleportPosition();
		}
		pushed = true;
		return source;
	}

	public Vector3 PushPositionToBounds(Vector3 source)
	{
		bool pushed = false;
		return PushPositionToBounds(source, out pushed);
	}

	public bool IsPositionInsideBounds(Vector3 center, Vector3 source)
	{
		if (BoundRestrictions == Vector3.zero || cameraCenter == Vector3.zero)
		{
			return true;
		}
		float num = BoundRestrictions.x * 0.5f;
		float num2 = BoundRestrictions.z * 0.5f;
		if (source.x < center.x - num || source.x > center.x + num || source.z < center.z - num2 || source.z > center.z + num2)
		{
			return false;
		}
		return true;
	}

	public bool IsPositionInsideBounds(Vector3 source)
	{
		return IsPositionInsideBounds(cameraCenter, source);
	}

	public bool IsPositionInsideTeleportBounds(Vector3 source)
	{
		Vector3 vector = cameraCenter;
		float num = BoundRestrictions.x * 0.5f + TeleportThreshold;
		float num2 = BoundRestrictions.z * 0.5f + TeleportThreshold;
		if (source.x < vector.x - num || source.x > vector.x + num || source.z < vector.z - num2 || source.z > vector.z + num2)
		{
			return false;
		}
		return true;
	}

	private Vector3 GetValidTeleportPosition()
	{
		GameObject[] array = targets;
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject != null))
			{
				continue;
			}
			Vector3 position = gameObject.transform.position;
			if (!IsPositionInsideBounds(position))
			{
				continue;
			}
			List<Vector3> list = GlobalHelpers.CircleVectors(position, 10, 2f);
			foreach (Vector3 item in list)
			{
				if (IsPositionInsideBounds(item) && GlobalHelpers.ValidNavMeshPosition(item))
				{
					return item;
				}
			}
		}
		return cameraCenter;
	}

	private bool AllTargetsInsideBounds(Vector3 center)
	{
		bool result = true;
		GameObject[] array = targets;
		foreach (GameObject gameObject in array)
		{
			if (!IsPositionInsideBounds(center, gameObject.transform.position))
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private float CalculateLongestTargetDistance(GameObject[] targets)
	{
		if (targets != null)
		{
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			float num = 0f;
			float num2 = 0f;
			foreach (GameObject gameObject3 in targets)
			{
				foreach (GameObject gameObject4 in targets)
				{
					if (!(gameObject3 == gameObject4))
					{
						num2 = (gameObject3.transform.position - gameObject4.transform.position).sqrMagnitude;
						if (num2 > num)
						{
							num = num2;
							gameObject = gameObject3;
							gameObject2 = gameObject4;
						}
					}
				}
			}
			if (num > 0f)
			{
				return Vector3.Distance(gameObject.transform.position, gameObject2.transform.position);
			}
		}
		return 0f;
	}
}
