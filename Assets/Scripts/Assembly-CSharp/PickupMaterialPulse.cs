using UnityEngine;

public class PickupMaterialPulse : MaterialPulse
{
	protected override void Start()
	{
		FindMaterials<MeshRenderer>(base.transform);
	}
}
