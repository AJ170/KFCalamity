using System.Collections.Generic;
using UnityEngine;

public class FS_ShadowManager : MonoBehaviour
{
	private static FS_ShadowManager _manager;

	private Dictionary<Material, FS_ShadowManagerMesh> shadowMeshes = new Dictionary<Material, FS_ShadowManagerMesh>();

	private Dictionary<Material, FS_ShadowManagerMesh> shadowMeshesStatic = new Dictionary<Material, FS_ShadowManagerMesh>();

	private int frameCalcedFustrum;

	private Plane[] fustrumPlanes;

	private void Start()
	{
		FS_ShadowManager[] array = (FS_ShadowManager[])Object.FindObjectsOfType(typeof(FS_ShadowManager));
		if (array.Length > 1)
		{
			Debug.LogWarning("There should only be one FS_ShadowManger in the scene. Found " + array.Length);
		}
	}

	private void OnApplicationQuit()
	{
		shadowMeshes.Clear();
		shadowMeshesStatic.Clear();
	}

	public static FS_ShadowManager Manager()
	{
		if (_manager == null)
		{
			FS_ShadowManager fS_ShadowManager = (FS_ShadowManager)Object.FindObjectOfType(typeof(FS_ShadowManager));
			if (fS_ShadowManager == null)
			{
				GameObject gameObject = new GameObject("FS_ShadowManager");
				_manager = gameObject.AddComponent<FS_ShadowManager>();
			}
			else
			{
				_manager = fS_ShadowManager;
			}
		}
		return _manager;
	}

	public void RecalculateStaticGeometry(FS_ShadowSimple removeShadow)
	{
		FS_MeshKey meshKey = new FS_MeshKey(removeShadow.shadowMaterial, true);
		RecalculateStaticGeometry(removeShadow, meshKey);
	}

	public void RecalculateStaticGeometry(FS_ShadowSimple removeShadow, FS_MeshKey meshKey)
	{
		if (shadowMeshesStatic.ContainsKey(meshKey.mat))
		{
			FS_ShadowManagerMesh fS_ShadowManagerMesh = shadowMeshesStatic[meshKey.mat];
			if (removeShadow != null)
			{
				fS_ShadowManagerMesh.removeShadow(removeShadow);
			}
			fS_ShadowManagerMesh.recreateStaticGeometry();
		}
	}

	public void registerGeometry(FS_ShadowSimple s, FS_MeshKey meshKey)
	{
		FS_ShadowManagerMesh fS_ShadowManagerMesh;
		if (meshKey.isStatic)
		{
			if (!shadowMeshesStatic.ContainsKey(meshKey.mat))
			{
				GameObject gameObject = new GameObject("ShadowMeshStatic_" + meshKey.mat.name);
				gameObject.transform.parent = base.transform;
				fS_ShadowManagerMesh = gameObject.AddComponent<FS_ShadowManagerMesh>();
				fS_ShadowManagerMesh.shadowMaterial = s.shadowMaterial;
				fS_ShadowManagerMesh.isStatic = true;
				shadowMeshesStatic.Add(meshKey.mat, fS_ShadowManagerMesh);
			}
			else
			{
				fS_ShadowManagerMesh = shadowMeshesStatic[meshKey.mat];
			}
		}
		else if (!shadowMeshes.ContainsKey(meshKey.mat))
		{
			GameObject gameObject2 = new GameObject("ShadowMesh_" + meshKey.mat.name);
			gameObject2.transform.parent = base.transform;
			fS_ShadowManagerMesh = gameObject2.AddComponent<FS_ShadowManagerMesh>();
			fS_ShadowManagerMesh.shadowMaterial = s.shadowMaterial;
			fS_ShadowManagerMesh.isStatic = false;
			shadowMeshes.Add(meshKey.mat, fS_ShadowManagerMesh);
		}
		else
		{
			fS_ShadowManagerMesh = shadowMeshes[meshKey.mat];
		}
		fS_ShadowManagerMesh.registerGeometry(s);
	}

	public Plane[] getCameraFustrumPlanes()
	{
		if (Time.frameCount != frameCalcedFustrum || fustrumPlanes == null)
		{
			Camera main = Camera.main;
			if (main == null)
			{
				Debug.LogWarning("No main camera could be found for visibility culling.");
				fustrumPlanes = null;
			}
			else
			{
				fustrumPlanes = GeometryUtility.CalculateFrustumPlanes(main);
				frameCalcedFustrum = Time.frameCount;
			}
		}
		return fustrumPlanes;
	}
}
