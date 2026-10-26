using UnityEngine;

public class RemoveVisualsOnLoad : MonoBehaviour
{
	public bool Enabled = true;

	private void Awake()
	{
		if (Enabled)
		{
			DestroyComponent<Renderer>();
			DestroyComponent<MeshFilter>();
		}
		Object.Destroy(this);
	}

	private void DestroyComponent<T>() where T : Component
	{
		T component = GetComponent<T>();
		if ((Object)component != (Object)null)
		{
			Object.Destroy(component);
		}
	}
}
