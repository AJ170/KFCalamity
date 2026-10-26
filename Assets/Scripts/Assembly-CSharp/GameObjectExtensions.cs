using UnityEngine;

public static class GameObjectExtensions
{
	public static T GetComponentInParents<T>(this GameObject gameObject) where T : Component
	{
		T val = (T)null;
		Transform transform = gameObject.transform;
		while (transform != null && (Object)val == (Object)null)
		{
			val = transform.gameObject.GetComponent<T>();
			transform = transform.transform.parent;
		}
		return val;
	}

	public static T GetComponentInRoot<T>(this GameObject gameObject) where T : Component
	{
		return gameObject.transform.root.GetComponent<T>();
	}
}
