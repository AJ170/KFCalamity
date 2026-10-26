using UnityEngine;

public class FindObjectsWithLayer : MonoBehaviour
{
	public LayerMask SearchLayer;

	private void OnGUI()
	{
		if (GUI.Button(new Rect(20f, 40f, 80f, 20f), "Find layer"))
		{
			FindLayer();
		}
		if (GUI.Button(new Rect(20f, 80f, 130f, 20f), "Find mesh colliders"))
		{
			FindMeshColliders();
		}
	}

	private void FindLayer()
	{
		Collider[] array = Object.FindObjectsOfType(typeof(Collider)) as Collider[];
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (SearchLayerMatch(collider.gameObject.layer))
			{
				Debug.Log(string.Format("Found object with layer {0}: {1}", collider.gameObject.layer, collider.gameObject.name), collider.gameObject);
			}
		}
	}

	private void FindMeshColliders()
	{
		MeshCollider[] array = Object.FindObjectsOfType(typeof(MeshCollider)) as MeshCollider[];
		MeshCollider[] array2 = array;
		foreach (Collider collider in array2)
		{
			Debug.Log(string.Format("Found object with mesh collider: {0}", collider.gameObject.name), collider.gameObject);
		}
	}

	protected bool SearchLayerMatch(int layer)
	{
		return (SearchLayer.value & (1 << layer)) != 0;
	}
}
