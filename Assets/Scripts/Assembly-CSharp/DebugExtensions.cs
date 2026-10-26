using UnityEngine;

public static class DebugExtensions
{
	public static void CreateSphere(Vector3 position, float duration)
	{
		CreateSphere(position, new Vector3(1f, 1f, 1f), duration);
	}

	public static void CreateSphere(Vector3 position, Vector3 scale, float duration)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Transform transform = gameObject.transform;
		transform.position = position;
		transform.localScale = scale;
		Object.Destroy(gameObject, duration);
	}
}
