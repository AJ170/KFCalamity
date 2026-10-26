using UnityEngine;

public class InternalTouchData
{
	public bool _wasUpdated = true;

	public int _internalID = -1;

	public TouchData _data;

	public InternalTouchData(int internalID, int fingerId, Vector2 pos)
	{
		_internalID = internalID;
		_data = new TouchData(fingerId, pos);
	}
}
