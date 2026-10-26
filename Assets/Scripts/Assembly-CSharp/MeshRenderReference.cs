using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshRenderReference : MonoBehaviour
{
	[Serializable]
	public class MeshAndMaterial
	{
		public SkinnedMeshRenderer Renderer;

		public Material DefaultMaterial;
	}

	public List<MeshAndMaterial> References;

	private void OnDestroy()
	{
		if (References == null)
		{
			return;
		}
		for (int i = 0; i < References.Count; i++)
		{
			if (References[i] != null && References[i].DefaultMaterial != null && References[i].Renderer != null)
			{
				References[i].Renderer.material = References[i].DefaultMaterial;
			}
		}
	}
}
