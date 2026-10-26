using UnityEngine;

public class ThirdPersonCamera : CameraScript
{
	public Vector3 offset = Vector3.zero;

	private void Update()
	{
		if (base.transform.parent == null && Target != null)
		{
			base.transform.parent = Target;
		}
		if (base.transform.parent != null)
		{
			base.transform.localPosition = offset;
		}
	}
}
