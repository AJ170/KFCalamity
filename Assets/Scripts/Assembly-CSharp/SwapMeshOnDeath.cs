using UnityEngine;

public class SwapMeshOnDeath : MonoBehaviour
{
	public GameObject PrimaryMesh;

	public GameObject SecondaryMesh;

	private void OnDeath(Health health)
	{
		if (PrimaryMesh != null)
		{
			PrimaryMesh.SetActive(false);
		}
		if (SecondaryMesh != null)
		{
			SecondaryMesh.SetActive(true);
		}
	}
}
