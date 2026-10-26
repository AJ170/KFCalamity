using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
	public float Delay = 3.5f;

	public float DissolveDuration = 1.5f;

	public Material DissolveMaterial;

	public List<Renderer> MeshRenderers;

	private bool usingDissolveMaterial;

	private bool dissolving = true;

	private List<Material> startMaterials = new List<Material>();

	private float dissolveAmount;

	private float counter;

	private void Start()
	{
		counter = Delay + DissolveDuration;
		if (MeshRenderers != null)
		{
			if (MeshRenderers != null && DissolveMaterial != null)
			{
				for (int i = 0; i < MeshRenderers.Count; i++)
				{
					if (MeshRenderers[i].material != null)
					{
						startMaterials.Add(MeshRenderers[i].sharedMaterial);
					}
				}
			}
			else
			{
				base.enabled = false;
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	private void OnDissolveFinished()
	{
		dissolveAmount = 0f;
		for (int i = 0; i < MeshRenderers.Count; i++)
		{
			MeshRenderers[i].enabled = false;
			MeshRenderers[i].material.SetFloat("_Cutoff", dissolveAmount);
			if (startMaterials.Count > i)
			{
				MeshRenderers[i].material = startMaterials[i];
			}
		}
		usingDissolveMaterial = false;
		counter = Delay + DissolveDuration;
		dissolving = false;
	}

	public void Reset()
	{
		if (!base.enabled)
		{
			return;
		}
		dissolving = true;
		dissolveAmount = 0f;
		for (int i = 0; i < MeshRenderers.Count; i++)
		{
			MeshRenderers[i].enabled = true;
			if (startMaterials.Count > i)
			{
				MeshRenderers[i].material = startMaterials[i];
			}
		}
		usingDissolveMaterial = false;
		counter = Delay + DissolveDuration;
	}

	private void Update()
	{
		if (!dissolving)
		{
			return;
		}
		if (counter <= DissolveDuration)
		{
			if (!usingDissolveMaterial)
			{
				for (int i = 0; i < MeshRenderers.Count; i++)
				{
					Texture mainTexture = MeshRenderers[i].material.mainTexture;
					MeshRenderers[i].material = DissolveMaterial;
					MeshRenderers[i].material.mainTexture = mainTexture;
				}
				usingDissolveMaterial = true;
			}
			dissolveAmount = 1f - counter / DissolveDuration;
			for (int j = 0; j < MeshRenderers.Count; j++)
			{
				MeshRenderers[j].material.SetFloat("_Cutoff", dissolveAmount);
			}
		}
		if (counter > 0f)
		{
			counter -= Time.deltaTime;
			if (counter <= 0f)
			{
				OnDissolveFinished();
			}
		}
	}
}
