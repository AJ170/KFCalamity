using UnityEngine;

public class MaterialSelector : MonoBehaviour
{
	public Material[] Materials;

	public Renderer Target;

	private void Awake()
	{
		if (Target == null)
		{
			Target = GetComponent<Renderer>();
		}
	}

	public void SetMaterial(int id)
	{
		if (Target != null && 0 <= id && id < Materials.Length)
		{
			Material material = Materials[id];
			Target.material = material;
		}
	}
}
