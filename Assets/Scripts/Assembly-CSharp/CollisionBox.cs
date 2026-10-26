using UnityEngine;

public class CollisionBox : MonoBehaviour
{
	private void Awake()
	{
		Object.Destroy(GetComponent<MeshFilter>());
		Object.Destroy(GetComponent<MeshRenderer>());
		Object.Destroy(this);
	}
}
