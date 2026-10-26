using UnityEngine;

public class UITransformValues : MonoBehaviour
{
	public Vector3 Position;

	public Vector3 Scale;

	private void Start()
	{
		base.transform.position = Position;
		base.transform.localScale = Scale;
	}
}
