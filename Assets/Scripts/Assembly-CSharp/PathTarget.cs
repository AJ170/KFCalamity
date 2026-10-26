using UnityEngine;

public struct PathTarget
{
	private Vector3 position;

	private float distanceOffset;

	public Vector3 Position
	{
		get
		{
			return position;
		}
	}

	public float DistanceOffset
	{
		get
		{
			return distanceOffset;
		}
	}

	public PathTarget(Vector3 position, float distanceOffset)
	{
		this.position = position;
		this.distanceOffset = distanceOffset;
	}
}
