using UnityEngine;

public class TouchData
{
	public int fingerId;

	public Vector2 position;

	public Vector2 deltaPosition;

	public float deltaTime;

	public TouchPhase phase;

	public int tapCount;

	public TouchData()
	{
		phase = TouchPhase.Began;
		tapCount = 1;
		fingerId = int.MinValue;
	}

	public TouchData(int id, Vector2 pos)
	{
		fingerId = id;
		position = pos;
		phase = TouchPhase.Began;
		tapCount = 1;
	}
}
