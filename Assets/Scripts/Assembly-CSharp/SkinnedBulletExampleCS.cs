using System.Collections.Generic;
using Edelweiss.DecalSystem;
using UnityEngine;

public class SkinnedBulletExampleCS : MonoBehaviour
{
	public GameObject decalsPrefab;

	private DS_SkinnedDecals m_Decals;

	private List<SkinnedDecalProjector> m_DecalProjectors = new List<SkinnedDecalProjector>();

	private SkinnedDecalsMesh m_DecalsMesh;

	private SkinnedDecalsMeshCutter m_DecalsMeshCutter;

	public float decalProjectorOffset = 0.5f;

	public Vector3 decalProjectorScale = new Vector3(0.2f, 2f, 0.2f);

	public float cullingAngle = 90f;

	public float meshOffset = 0.001f;

	private int m_UVRectangleIndex;

	private void NextUVRectangleIndex()
	{
		m_UVRectangleIndex++;
		if (m_UVRectangleIndex >= m_Decals.uvRectangles.Length)
		{
			m_UVRectangleIndex = 0;
		}
	}

	private void Start()
	{
		GameObject gameObject = Object.Instantiate(decalsPrefab) as GameObject;
		m_Decals = gameObject.GetComponentInChildren<DS_SkinnedDecals>();
		if (m_Decals == null)
		{
			Debug.LogError("The 'decalsPrefab' does not contain a 'DS_SkinnedDecals' instance!");
			return;
		}
		m_DecalsMesh = new SkinnedDecalsMesh(m_Decals);
		m_DecalsMeshCutter = new SkinnedDecalsMeshCutter();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			while (m_DecalProjectors.Count > 0)
			{
				SkinnedDecalProjector a_Projector = m_DecalProjectors[m_DecalProjectors.Count - 1];
				m_DecalsMesh.RemoveProjector(a_Projector);
				m_DecalProjectors.RemoveAt(m_DecalProjectors.Count - 1);
			}
			m_Decals.UpdateSkinnedDecalsMeshes(m_DecalsMesh);
		}
		if (!Input.GetButtonDown("Fire1"))
		{
			return;
		}
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, float.PositiveInfinity))
		{
			return;
		}
		if (m_DecalProjectors.Count >= 50)
		{
			SkinnedDecalProjector a_Projector2 = m_DecalProjectors[0];
			m_DecalProjectors.RemoveAt(0);
			m_DecalsMesh.RemoveProjector(a_Projector2);
		}
		Vector3 a_Position = hitInfo.point - decalProjectorOffset * ray.direction.normalized;
		Vector3 up = Camera.main.transform.up;
		Vector3 upwards = -Camera.main.transform.forward;
		Quaternion a_Rotation = Quaternion.LookRotation(up, upwards);
		Quaternion quaternion = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
		a_Rotation *= quaternion;
		SkinnedMeshRenderer component = hitInfo.collider.GetComponent<SkinnedMeshRenderer>();
		if (component != null)
		{
			Mesh mesh = null;
			mesh = component.sharedMesh;
			if (mesh != null)
			{
				SkinnedDecalProjector skinnedDecalProjector = new SkinnedDecalProjector(a_Position, a_Rotation, decalProjectorScale, cullingAngle, meshOffset, m_UVRectangleIndex, m_UVRectangleIndex);
				m_DecalProjectors.Add(skinnedDecalProjector);
				m_DecalsMesh.AddProjector(skinnedDecalProjector);
				Matrix4x4 worldToLocalMatrix = hitInfo.collider.GetComponent<Renderer>().transform.worldToLocalMatrix;
				Matrix4x4 localToWorldMatrix = hitInfo.collider.GetComponent<Renderer>().transform.localToWorldMatrix;
				m_DecalsMesh.Add(mesh, component.bones, component.quality, worldToLocalMatrix, localToWorldMatrix);
				m_DecalsMeshCutter.CutDecalsPlanes(m_DecalsMesh);
				m_DecalsMesh.OffsetActiveProjectorVertices();
				m_Decals.UpdateSkinnedDecalsMeshes(m_DecalsMesh);
				NextUVRectangleIndex();
			}
		}
	}
}
