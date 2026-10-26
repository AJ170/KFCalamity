using System;
using System.Collections.Generic;
using Edelweiss.DecalSystem;
using UnityEngine;

[Serializable]
public class BulletExampleDynamicJS : MonoBehaviour
{
	public GameObject decalsPrefab;

	private DS_Decals m_Decals;

	private Matrix4x4 m_WorldToDecalsMatrix;

	private List<DecalProjector> m_DecalProjectors;

	private DecalsMesh m_DecalsMesh;

	private DecalsMeshCutter m_DecalsMeshCutter;

	public float decalProjectorOffset;

	public Vector3 decalScale;

	public float cullingAngle;

	public float meshOffset;

	private int m_UVRectangleIndex;

	public BulletExampleDynamicJS()
	{
		m_DecalProjectors = new List<DecalProjector>();
		decalProjectorOffset = 0.5f;
		decalScale = new Vector3(0.2f, 2f, 0.2f);
		cullingAngle = 90f;
		meshOffset = 0.001f;
	}

	private void NextUVRectangleIndex()
	{
		m_UVRectangleIndex++;
		if (m_UVRectangleIndex >= m_Decals.uvRectangles.Length)
		{
			m_UVRectangleIndex = 0;
		}
	}

	public virtual void Start()
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(decalsPrefab);
		m_Decals = gameObject.GetComponentInChildren<DS_Decals>();
		if (m_Decals == null)
		{
			Debug.LogError("The 'decalsPrefab' does not contain a 'DS_Decals' instance!");
			return;
		}
		m_DecalsMesh = new DecalsMesh(m_Decals);
		m_DecalsMeshCutter = new DecalsMeshCutter();
		m_WorldToDecalsMatrix = m_Decals.CachedTransform.worldToLocalMatrix;
	}

	public virtual void Update()
	{
		if (!Input.GetButtonDown("Fire1"))
		{
			return;
		}
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit hitInfo = default(RaycastHit);
		if (!Physics.Raycast(ray, out hitInfo, float.PositiveInfinity))
		{
			return;
		}
		if (hitInfo.rigidbody == null)
		{
			if (m_DecalProjectors.Count >= 50)
			{
				DecalProjector a_Projector = m_DecalProjectors[0];
				m_DecalProjectors.RemoveAt(0);
				m_DecalsMesh.RemoveProjector(a_Projector);
			}
			Vector3 a_Position = hitInfo.point - decalProjectorOffset * ray.direction.normalized;
			Vector3 up = Camera.main.transform.up;
			Vector3 upwards = -Camera.main.transform.forward;
			Quaternion a_Rotation = Quaternion.LookRotation(up, upwards);
			Quaternion quaternion = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
			a_Rotation *= quaternion;
			TerrainCollider terrainCollider = hitInfo.collider as TerrainCollider;
			if (terrainCollider != null)
			{
				Terrain component = terrainCollider.GetComponent<Terrain>();
				if (component != null)
				{
					DecalProjector decalProjector = new DecalProjector(a_Position, a_Rotation, decalScale, cullingAngle, meshOffset, m_UVRectangleIndex, m_UVRectangleIndex);
					m_DecalProjectors.Add(decalProjector);
					m_DecalsMesh.AddProjector(decalProjector);
					Matrix4x4 a_TerrainToDecalsMatrix = Matrix4x4.TRS(component.transform.position, Quaternion.identity, Vector3.one) * m_WorldToDecalsMatrix;
					m_DecalsMesh.Add(component, a_TerrainToDecalsMatrix);
					m_DecalsMeshCutter.CutDecalsPlanes(m_DecalsMesh);
					m_DecalsMesh.OffsetActiveProjectorVertices();
					m_Decals.UpdateDecalsMeshes(m_DecalsMesh);
					NextUVRectangleIndex();
				}
				else
				{
					Debug.Log("Terrain is null!");
				}
				return;
			}
			MeshCollider component2 = hitInfo.collider.GetComponent<MeshCollider>();
			MeshFilter component3 = hitInfo.collider.GetComponent<MeshFilter>();
			if (component2 != null || component3 != null)
			{
				Mesh mesh = null;
				if (component2 != null)
				{
					mesh = component2.sharedMesh;
				}
				else if (component3 != null)
				{
					mesh = component3.sharedMesh;
				}
				if (mesh != null)
				{
					DecalProjector decalProjector2 = new DecalProjector(a_Position, a_Rotation, decalScale, cullingAngle, meshOffset, m_UVRectangleIndex, m_UVRectangleIndex);
					m_DecalProjectors.Add(decalProjector2);
					m_DecalsMesh.AddProjector(decalProjector2);
					Matrix4x4 worldToLocalMatrix = hitInfo.collider.GetComponent<Renderer>().transform.worldToLocalMatrix;
					Matrix4x4 localToWorldMatrix = hitInfo.collider.GetComponent<Renderer>().transform.localToWorldMatrix;
					m_DecalsMesh.Add(mesh, worldToLocalMatrix, localToWorldMatrix);
					m_DecalsMeshCutter.CutDecalsPlanes(m_DecalsMesh);
					m_DecalsMesh.OffsetActiveProjectorVertices();
					m_Decals.UpdateDecalsMeshes(m_DecalsMesh);
					NextUVRectangleIndex();
				}
			}
		}
		else
		{
			BulletExampleDynamicObjectJS component4 = hitInfo.collider.GetComponent<BulletExampleDynamicObjectJS>();
			component4.AddDecalProjector(ray, hitInfo);
		}
	}
}
