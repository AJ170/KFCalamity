using System;
using System.Collections;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
[AddComponentMenu("Simple Mesh Combine")]
public class SimpleMeshCombine : MonoBehaviour
{
	public GameObject[] combinedGameOjects;

	public GameObject combined;

	public string meshName;

	public bool _savedPrefab;

	public bool _canGenerateLightmapUV;

	public int vCount;

	public SimpleMeshCombine()
	{
		meshName = "Combined_Meshes";
	}

	public virtual void EnableRenderers(bool e)
	{
		for (int i = 0; i < Extensions.get_length((System.Array)combinedGameOjects); i++)
		{
			combinedGameOjects[i].GetComponent<Renderer>().enabled = e;
		}
	}

	public virtual MeshFilter[] FindEnabledMeshes()
	{
		MeshFilter[] array = null;
		int num = default(int);
		array = transform.GetComponentsInChildren<MeshFilter>();
		for (int i = 0; i < Extensions.get_length((System.Array)array); i++)
		{
			if ((bool)(MeshRenderer)array[i].GetComponent(typeof(MeshRenderer)) && ((MeshRenderer)array[i].GetComponent(typeof(MeshRenderer))).enabled)
			{
				num++;
			}
		}
		MeshFilter[] array2 = new MeshFilter[num];
		num = 0;
		for (int j = 0; j < Extensions.get_length((System.Array)array); j++)
		{
			if ((bool)(MeshRenderer)array[j].GetComponent(typeof(MeshRenderer)) && ((MeshRenderer)array[j].GetComponent(typeof(MeshRenderer))).enabled)
			{
				array2[num] = array[j];
				num++;
			}
		}
		return array2;
	}

	public virtual void CombineMeshes()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "_Combined Mesh [" + name + "]";
		gameObject.gameObject.AddComponent(typeof(MeshFilter));
		gameObject.gameObject.AddComponent(typeof(MeshRenderer));
		MeshFilter[] array = null;
		array = FindEnabledMeshes();
		ArrayList arrayList = new ArrayList();
		ArrayList arrayList2 = new ArrayList();
		combinedGameOjects = new GameObject[Extensions.get_length((System.Array)array)];
		for (int i = 0; i < Extensions.get_length((System.Array)array); i++)
		{
			MeshFilter[] componentsInChildren = array[i].GetComponentsInChildren<MeshFilter>();
			combinedGameOjects[i] = array[i].gameObject;
			int j = 0;
			MeshFilter[] array2 = componentsInChildren;
			for (int length = array2.Length; j < length; j++)
			{
				MeshRenderer meshRenderer = (MeshRenderer)array2[j].GetComponent(typeof(MeshRenderer));
				((Renderer)array[i].transform.gameObject.GetComponent(typeof(Renderer))).enabled = false;
				for (int k = 0; k < array2[j].sharedMesh.subMeshCount; k++)
				{
					if (k < meshRenderer.sharedMaterials.Length && k < array2[j].sharedMesh.subMeshCount)
					{
						int num = Contains(arrayList, meshRenderer.sharedMaterials[k]);
						if (num == -1)
						{
							arrayList.Add(meshRenderer.sharedMaterials[k]);
							num = arrayList.Count - 1;
						}
						arrayList2.Add(new ArrayList());
						CombineInstance combineInstance = default(CombineInstance);
						combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
						combineInstance.subMeshIndex = k;
						combineInstance.mesh = array2[j].sharedMesh;
						(arrayList2[num] as ArrayList).Add(combineInstance);
					}
				}
			}
		}
		Mesh[] array3 = new Mesh[arrayList.Count];
		CombineInstance[] array4 = new CombineInstance[arrayList.Count];
		for (int l = 0; l < arrayList.Count; l++)
		{
			CombineInstance[] combine = (arrayList2[l] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
			array3[l] = new Mesh();
			array3[l].CombineMeshes(combine, true, true);
			array4[l] = default(CombineInstance);
			array4[l].mesh = array3[l];
			array4[l].subMeshIndex = 0;
		}
		((MeshFilter)gameObject.GetComponent(typeof(MeshFilter))).sharedMesh = new Mesh();
		((MeshFilter)gameObject.GetComponent(typeof(MeshFilter))).sharedMesh.CombineMeshes(array4, false, false);
		int m = 0;
		Mesh[] array5 = array3;
		for (int length2 = array5.Length; m < length2; m++)
		{
			array5[m].Clear();
			UnityEngine.Object.DestroyImmediate(array5[m]);
		}
		MeshRenderer meshRenderer2 = (MeshRenderer)((MeshFilter)gameObject.GetComponent(typeof(MeshFilter))).GetComponent(typeof(MeshRenderer));
		if (!meshRenderer2)
		{
			meshRenderer2 = (MeshRenderer)this.gameObject.AddComponent(typeof(MeshRenderer));
		}
		Material[] materials = arrayList.ToArray(typeof(Material)) as Material[];
		meshRenderer2.materials = materials;
		combined = gameObject.gameObject;
		EnableRenderers(false);
		gameObject.transform.parent = transform;
		((MeshFilter)gameObject.GetComponent(typeof(MeshFilter))).sharedMesh.uv2 = null;
		vCount = ((MeshFilter)gameObject.GetComponent(typeof(MeshFilter))).sharedMesh.vertexCount;
		if (vCount > 65536)
		{
			Debug.LogWarning("Vertex Count: " + vCount + "- Vertex Count too high, please divide mesh combine into more groups. Max 65536 for each mesh");
			_canGenerateLightmapUV = false;
		}
		else
		{
			_canGenerateLightmapUV = true;
		}
	}

	public virtual void GenerateLightmapUV()
	{
	}

	public virtual int Contains(ArrayList l, Material n)
	{
		int num = 0;
		int result;
		while (true)
		{
			if (num < l.Count)
			{
				if (l[num] as Material == n)
				{
					result = num;
					break;
				}
				num++;
				continue;
			}
			result = -1;
			break;
		}
		return result;
	}

	public virtual void Main()
	{
	}
}
