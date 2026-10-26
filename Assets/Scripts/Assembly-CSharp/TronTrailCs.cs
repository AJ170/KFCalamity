using System.Collections.Generic;
using UnityEngine;

public class TronTrailCs : MonoBehaviour
{
	public float Height = 2f;

	public float TrailTime = 2f;

	public bool AlwaysUp;

	public float MinDistance = 0.1f;

	public float Delay = 0.1f;

	public int DebugBreakSections = 2;

	public Color StartColor = Color.white;

	public Color EndColor = Color.white;

	private List<TronTrailSectionCS> sections = new List<TronTrailSectionCS>();

	private Transform cachedTransform;

	private MeshFilter cachedMeshFiler;

	private float timer;

	private void Start()
	{
		cachedTransform = base.transform;
		cachedMeshFiler = GetComponent<MeshFilter>();
		timer = -1f;
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void LateUpdate()
	{
		Vector3 vector = ((!AlwaysUp) ? cachedTransform.TransformDirection(Vector3.up) : Vector3.up);
		Vector3 position = cachedTransform.position;
		Vector3 pos = cachedTransform.position + vector * Height;
		float time = Time.time;
		if (sections.Count > 0)
		{
			while (sections.Count > 0 && sections[sections.Count - 1].Time + TrailTime <= time)
			{
				sections.RemoveAt(sections.Count - 1);
			}
		}
		TronTrailSectionCS tronTrailSectionCS = null;
		if (sections.Count > 0)
		{
			tronTrailSectionCS = sections[0];
		}
		if (sections.Count == 0 || (tronTrailSectionCS.Pos - position).sqrMagnitude > MinDistance * MinDistance)
		{
			TronTrailSectionCS item = new TronTrailSectionCS(position, pos, vector, time);
			sections.Insert(0, item);
		}
		Mesh mesh = cachedMeshFiler.mesh;
		mesh.Clear();
		if (sections.Count < 2)
		{
			return;
		}
		int num = sections.Count * 2;
		Vector3[] array = new Vector3[num];
		Color[] array2 = new Color[num];
		Vector2[] array3 = new Vector2[num];
		TronTrailSectionCS tronTrailSectionCS2 = sections[0];
		TronTrailSectionCS tronTrailSectionCS3 = sections[0];
		Matrix4x4 worldToLocalMatrix = cachedTransform.worldToLocalMatrix;
		for (int i = 0; i < sections.Count; i++)
		{
			tronTrailSectionCS3 = tronTrailSectionCS2;
			tronTrailSectionCS2 = sections[i];
			float num2 = 0f;
			if (i != 0)
			{
				num2 = Mathf.Clamp01((Time.time - tronTrailSectionCS2.Time) / TrailTime);
			}
			Vector3 upDir = tronTrailSectionCS2.UpDir;
			array[i * 2 + 1] = worldToLocalMatrix.MultiplyPoint(tronTrailSectionCS2.Pos);
			array[i * 2] = worldToLocalMatrix.MultiplyPoint(tronTrailSectionCS2.Pos2);
			array3[i * 2] = new Vector2(num2, 0f);
			array3[i * 2 + 1] = new Vector2(num2, 1f);
			Color color = Color.Lerp(StartColor, EndColor, num2);
			array2[i * 2] = color;
			array2[i * 2 + 1] = color;
		}
		int[] array4 = new int[(sections.Count - 1) * 2 * 3];
		for (int j = 0; j < array4.Length / 6; j++)
		{
			array4[j * 6] = j * 2;
			array4[j * 6 + 1] = j * 2 + 1;
			array4[j * 6 + 2] = j * 2 + 2;
			array4[j * 6 + 3] = j * 2 + 2;
			array4[j * 6 + 4] = j * 2 + 1;
			array4[j * 6 + 5] = j * 2 + 3;
		}
		mesh.vertices = array;
		mesh.colors = array2;
		mesh.uv = array3;
		mesh.triangles = array4;
	}
}
