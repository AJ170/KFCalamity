using UnityEngine;

public class PlayerPrediction
{
	public Vector3 Position;

	public Vector3 Direction;

	public bool Moving;

	public PlayerPrediction(Vector3 position, Vector3 direction, bool moving)
	{
		Position = position;
		Direction = direction;
		Moving = moving;
	}
}
