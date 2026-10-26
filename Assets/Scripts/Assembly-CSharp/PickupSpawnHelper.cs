using UnityEngine;

public class PickupSpawnHelper
{
	public static void Arc(GameObject go, float height = 4f, float force = 2f)
	{
		if (go != null && go.GetComponent<Rigidbody>() != null)
		{
			Transform transform = go.transform;
			transform.position += new Vector3(0f, 1.5f, 0f);
			transform.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f, height, 0f) + transform.forward * force, ForceMode.Impulse);
		}
	}
}
