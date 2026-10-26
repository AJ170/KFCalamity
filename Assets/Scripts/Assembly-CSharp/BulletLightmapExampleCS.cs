using System.Collections.Generic;
using Edelweiss.DecalSystem;
using UnityEngine;

public class BulletLightmapExampleCS : MonoBehaviour
{
	public GameObject decalsPrefab;

	private List<GameObject> m_DecalsInstances = new List<GameObject>();

	private DecalsMesh m_DecalsMesh;

	private DecalsMeshCutter m_DecalsMeshCutter;

	public float decalProjectorOffset = 0.5f;

	public Vector3 decalProjectorScale = new Vector3(0.2f, 2f, 0.2f);

	public float cullingAngle = 90f;

	public float meshOffset = 0.001f;

	private int m_UVRectangleIndex = 7;

	private void NextUVRectangleIndex(DS_Decals a_Decals)
	{
		m_UVRectangleIndex++;
		if (m_UVRectangleIndex >= a_Decals.uvRectangles.Length)
		{
			m_UVRectangleIndex = 7;
		}
	}

	private void Start()
	{
		m_DecalsMesh = new DecalsMesh();
		m_DecalsMeshCutter = new DecalsMeshCutter();
	}

	private void Update()
	{
		if (!Input.GetButtonDown("Fire1"))
		{
			return;
		}
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, float.PositiveInfinity) || !(hitInfo.collider as TerrainCollider == null))
		{
			return;
		}
		if (m_DecalsInstances.Count >= 50)
		{
			m_DecalsInstances.RemoveAt(0);
		}
		GameObject gameObject = Object.Instantiate(decalsPrefab) as GameObject;
		DS_Decals componentInChildren = gameObject.GetComponentInChildren<DS_Decals>();
		m_DecalsMesh.Initialize(componentInChildren);
		Vector3 a_Position = hitInfo.point - decalProjectorOffset * ray.direction.normalized;
		Vector3 up = Camera.main.transform.up;
		Vector3 upwards = -Camera.main.transform.forward;
		Quaternion a_Rotation = Quaternion.LookRotation(up, upwards);
		Quaternion quaternion = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
		a_Rotation *= quaternion;
		MeshCollider component = hitInfo.collider.GetComponent<MeshCollider>();
		MeshFilter component2 = hitInfo.collider.GetComponent<MeshFilter>();
		MeshRenderer component3 = hitInfo.collider.GetComponent<MeshRenderer>();
		if (component != null || component2 != null)
		{
			Mesh mesh = null;
			if (component != null)
			{
				mesh = component.sharedMesh;
			}
			else if (component2 != null)
			{
				mesh = component2.sharedMesh;
			}
			if (mesh != null)
			{
				DecalProjector a_Projector = new DecalProjector(a_Position, a_Rotation, decalProjectorScale, cullingAngle, meshOffset, m_UVRectangleIndex, m_UVRectangleIndex);
				m_DecalsMesh.AddProjector(a_Projector);
				Matrix4x4 worldToLocalMatrix = hitInfo.collider.GetComponent<Renderer>().transform.worldToLocalMatrix;
				Matrix4x4 localToWorldMatrix = hitInfo.collider.GetComponent<Renderer>().transform.localToWorldMatrix;
				m_DecalsMesh.Add(mesh, worldToLocalMatrix, localToWorldMatrix);
				m_DecalsMeshCutter.CutDecalsPlanes(m_DecalsMesh);
				m_DecalsMesh.OffsetActiveProjectorVertices();
				componentInChildren.UpdateDecalsMeshes(m_DecalsMesh);
				componentInChildren.DecalsMeshRenderers[0].MeshRenderer.lightmapIndex = component3.lightmapIndex;
				componentInChildren.DecalsMeshRenderers[0].MeshRenderer.lightmapScaleOffset = component3.lightmapScaleOffset;
				NextUVRectangleIndex(componentInChildren);
			}
		}
	}
}
