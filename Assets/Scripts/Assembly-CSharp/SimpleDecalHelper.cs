using UnityEngine;

public class SimpleDecalHelper
{
	public static RaycastHit FindAcceptedLocation(Vector3 point, LayerMask layerMask, float startHeight = 0.1f, float rayLength = 0.5f)
	{
		Vector3 origin = point + Vector3.up * startHeight;
		RaycastHit hitInfo;
		Physics.Raycast(origin, Vector3.down, out hitInfo, startHeight + rayLength, layerMask);
		return hitInfo;
	}
}
