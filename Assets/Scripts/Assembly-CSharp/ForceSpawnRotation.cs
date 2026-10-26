using UnityEngine;

public class ForceSpawnRotation : MonoBehaviour
{
	private Quaternion rotation;

	private Transform myTransform;

	private void Start()
	{
		myTransform = base.transform;
		rotation = myTransform.rotation;
	}

	private void Update()
	{
		myTransform.rotation = rotation;
	}
}
