using System;
using System.Collections.Generic;
using Edelweiss.DecalSystem;
using UnityEngine;

[Serializable]
public class BulletExampleDynamicObjectJS : MonoBehaviour
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

	public BulletExampleDynamicObjectJS()
	{
		m_DecalProjectors = new List<DecalProjector>();
		decalProjectorOffset = 0.5f;
		decalScale = new Vector3(0.2f, 2f, 0.2f);
		cullingAngle = 180f;
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
		m_Decals.transform.parent = transform;
		m_Decals.transform.localPosition = Vector3.zero;
		m_Decals.transform.localScale = Vector3.one;
		if (m_Decals == null)
		{
			Debug.LogError("The 'decalsPrefab' does not contain a 'DS_Decals' instance!");
			return;
		}
		m_DecalsMesh = new DecalsMesh(m_Decals);
		m_DecalsMeshCutter = new DecalsMeshCutter();
		m_WorldToDecalsMatrix = m_Decals.CachedTransform.worldToLocalMatrix;
	}

	public virtual void AddDecalProjector(Ray a_Ray, RaycastHit a_RaycastHit)
	{
		if (m_DecalProjectors.Count >= 50)
		{
			DecalProjector a_Projector = m_DecalProjectors[0];
			m_DecalProjectors.RemoveAt(0);
			m_DecalsMesh.RemoveProjector(a_Projector);
		}
		Vector3 a_Position = a_RaycastHit.point - decalProjectorOffset * a_Ray.direction.normalized;
		Vector3 up = Camera.main.transform.up;
		Vector3 upwards = -Camera.main.transform.forward;
		Quaternion a_Rotation = Quaternion.LookRotation(up, upwards);
		Quaternion quaternion = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
		a_Rotation *= quaternion;
		MeshCollider component = a_RaycastHit.collider.GetComponent<MeshCollider>();
		MeshFilter component2 = a_RaycastHit.collider.GetComponent<MeshFilter>();
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
				DecalProjector decalProjector = new DecalProjector(a_Position, a_Rotation, decalScale, cullingAngle, meshOffset, m_UVRectangleIndex, m_UVRectangleIndex);
				m_DecalProjectors.Add(decalProjector);
				m_DecalsMesh.AddProjector(decalProjector);
				Matrix4x4 worldToLocalMatrix = a_RaycastHit.collider.GetComponent<Renderer>().transform.worldToLocalMatrix;
				Matrix4x4 localToWorldMatrix = a_RaycastHit.collider.GetComponent<Renderer>().transform.localToWorldMatrix;
				m_DecalsMesh.Add(mesh, worldToLocalMatrix, localToWorldMatrix);
				m_DecalsMeshCutter.CutDecalsPlanes(m_DecalsMesh);
				m_DecalsMesh.OffsetActiveProjectorVertices();
				m_Decals.UpdateDecalsMeshes(m_DecalsMesh);
				NextUVRectangleIndex();
			}
		}
	}
}
