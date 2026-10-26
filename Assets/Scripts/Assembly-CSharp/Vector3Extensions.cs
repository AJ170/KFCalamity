using UnityEngine;

public static class Vector3Extensions
{
	public static Vector3 RotateX(this Vector3 v, float angleAsRad)
	{
		float num = Mathf.Sin(angleAsRad);
		float num2 = Mathf.Cos(angleAsRad);
		float y = v.y;
		float z = v.z;
		v.y = num2 * y - num * z;
		v.z = num2 * z + num * y;
		return v;
	}

	public static Vector3 RotateY(this Vector3 v, float angleAsRad)
	{
		float num = Mathf.Sin(angleAsRad);
		float num2 = Mathf.Cos(angleAsRad);
		float x = v.x;
		float z = v.z;
		v.x = num2 * x + num * z;
		v.z = num2 * z - num * x;
		return v;
	}

	public static Vector3 RotateZ(this Vector3 v, float angleAsRad)
	{
		float num = Mathf.Sin(angleAsRad);
		float num2 = Mathf.Cos(angleAsRad);
		float x = v.x;
		float y = v.y;
		v.x = num2 * x - num * y;
		v.y = num2 * y + num * x;
		return v;
	}
}
