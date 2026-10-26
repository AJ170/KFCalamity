using System.Collections.Generic;
using Edelweiss.DecalSystem;
using UnityEngine;

public class ColoredBulletExampleCS : MonoBehaviour
{
	public GameObject decalsPrefab;

	private DS_Decals m_Decals;

	private Matrix4x4 m_WorldToDecalsMatrix;

	private List<DecalProjector> m_DecalProjectors = new List<DecalProjector>();

	private DecalsMesh m_DecalsMesh;

	private DecalsMeshCutter m_DecalsMeshCutter;

	private List<Color> m_VertexColors = new List<Color>();

	public float decalProjectorOffset = 0.5f;

	public Vector3 decalProjectorScale = new Vector3(0.2f, 2f, 0.2f);

	public float cullingAngle = 90f;

	public float meshOffset = 0.001f;

	private int m_UVRectangleIndex;

	private int m_ColorIndex;

	private Color CurrentColor
	{
		get
		{
			if (m_ColorIndex == 0)
			{
				return Color.red;
			}
			if (m_ColorIndex == 1)
			{
				return Color.green;
			}
			return Color.blue;
		}
	}

	private void NextUVRectangleIndex()
	{
		m_UVRectangleIndex++;
		if (m_UVRectangleIndex >= m_Decals.uvRectangles.Length)
		{
			m_UVRectangleIndex = 0;
		}
	}

	private void NextColorIndex()
	{
		m_ColorIndex++;
		if (m_ColorIndex > 2)
		{
			m_ColorIndex = 0;
		}
	}

	private void Start()
	{
		GameObject gameObject = Object.Instantiate(decalsPrefab) as GameObject;
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

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			while (m_DecalProjectors.Count > 0)
			{
				DecalProjector a_Projector = m_DecalProjectors[m_DecalProjectors.Count - 1];
				m_DecalsMesh.RemoveProjector(a_Projector);
				m_DecalProjectors.RemoveAt(m_DecalProjectors.Count - 1);
			}
			m_Decals.UpdateDecalsMeshes(m_DecalsMesh);
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
			DecalProjector decalProjector = m_DecalProjectors[0];
			m_VertexColors.RemoveRange(0, decalProjector.DecalsMeshUpperVertexIndex + 1);
			m_DecalProjectors.RemoveAt(0);
			m_DecalsMesh.RemoveProjector(decalProjector);
		}
		Vector3 a_Position = hitInfo.point - decalProjectorOffset * ray.direction.normalized;
		Vector3 up = Camera.main.transform.up;
		Vector3 upwards = -Camera.main.transform.forward;
		Quaternion a_Rotation = Quaternion.LookRotation(up, upwards);
		Quaternion quaternion = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
		a_Rotation *= quaternion;
		TerrainCollider terrainCollider = hitInfo.collider as TerrainCollider;
		if (terrainCollider != null)
		{
			Terrain component = terrainCollider.GetComponent<Terrain>();
			if (component != null)
			{
				DecalProjector decalProjector2 = new DecalProjector(a_Position, a_Rotation, decalProjectorScale, cullingAngle, meshOffset, m_UVRectangleIndex, m_UVRectangleIndex);
				m_DecalProjectors.Add(decalProjector2);
				m_DecalsMesh.AddProjector(decalProjector2);
				Matrix4x4 a_TerrainToDecalsMatrix = Matrix4x4.TRS(component.transform.position, Quaternion.identity, Vector3.one) * m_WorldToDecalsMatrix;
				m_DecalsMesh.Add(component, a_TerrainToDecalsMatrix);
				m_DecalsMeshCutter.CutDecalsPlanes(m_DecalsMesh);
				m_DecalsMesh.OffsetActiveProjectorVertices();
				m_Decals.UpdateDecalsMeshes(m_DecalsMesh);
				Color currentColor = CurrentColor;
				int num = decalProjector2.DecalsMeshUpperVertexIndex - decalProjector2.DecalsMeshLowerVertexIndex + 1;
				for (int i = 0; i < num; i++)
				{
					m_VertexColors.Add(currentColor);
				}
				m_Decals.DecalsMeshRenderers[0].MeshFilter.mesh.colors = m_VertexColors.ToArray();
				NextUVRectangleIndex();
				NextColorIndex();
			}
			else
			{
				Debug.Log("Terrain is null!");
			}
			return;
		}
		MeshCollider component2 = hitInfo.collider.GetComponent<MeshCollider>();
		MeshFilter component3 = hitInfo.collider.GetComponent<MeshFilter>();
		if (!(component2 != null) && !(component3 != null))
		{
			return;
		}
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
			DecalProjector decalProjector3 = new DecalProjector(a_Position, a_Rotation, decalProjectorScale, cullingAngle, meshOffset, m_UVRectangleIndex, m_UVRectangleIndex);
			m_DecalProjectors.Add(decalProjector3);
			m_DecalsMesh.AddProjector(decalProjector3);
			Matrix4x4 worldToLocalMatrix = hitInfo.collider.GetComponent<Renderer>().transform.worldToLocalMatrix;
			Matrix4x4 localToWorldMatrix = hitInfo.collider.GetComponent<Renderer>().transform.localToWorldMatrix;
			m_DecalsMesh.Add(mesh, worldToLocalMatrix, localToWorldMatrix);
			m_DecalsMeshCutter.CutDecalsPlanes(m_DecalsMesh);
			m_DecalsMesh.OffsetActiveProjectorVertices();
			m_Decals.UpdateDecalsMeshes(m_DecalsMesh);
			Color currentColor2 = CurrentColor;
			int num2 = decalProjector3.DecalsMeshUpperVertexIndex - decalProjector3.DecalsMeshLowerVertexIndex + 1;
			for (int j = 0; j < num2; j++)
			{
				m_VertexColors.Add(currentColor2);
			}
			m_Decals.DecalsMeshRenderers[0].MeshFilter.mesh.colors = m_VertexColors.ToArray();
			NextUVRectangleIndex();
			NextColorIndex();
		}
	}
}
