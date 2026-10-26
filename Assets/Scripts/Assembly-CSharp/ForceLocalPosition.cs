using UnityEngine;

public class ForceLocalPosition : MonoBehaviour
{
	public Vector3 Position = Vector3.zero;

	private Transform myTransform;

	private void Start()
	{
		myTransform = base.transform;
	}

	private void Update()
	{
		myTransform.localPosition = Position;
	}
}
