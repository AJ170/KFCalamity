using System;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
[RequireComponent(typeof(MeshFilter))]
public class TronTrail : MonoBehaviour
{
	public float height;

	public float time;

	public bool alwaysUp;

	public float minDistance;

	public Color startColor;

	public Color endColor;

	private UnityScript.Lang.Array sections;

	public TronTrail()
	{
		height = 2f;
		time = 2f;
		minDistance = 0.1f;
		startColor = Color.white;
		endColor = new Color(1f, 1f, 1f, 0f);
		sections = new UnityScript.Lang.Array();
	}

	public virtual void LateUpdate()
	{
		Vector3 position = transform.position;
		float num = Time.time;
		if (sections.length > 0)
		{
			object obj = sections[sections.length - 1];
			if (!(obj is TronTrailSection))
			{
				obj = RuntimeServices.Coerce(obj, typeof(TronTrailSection));
			}
			TronTrailSection tronTrailSection = (TronTrailSection)obj;
			while (sections.length > 0 && num > tronTrailSection.time + time)
			{
				sections.Pop();
			}
		}
		TronTrailSection tronTrailSection2 = null;
		if (sections.length != 0)
		{
			object obj2 = sections[0];
			if (!(obj2 is TronTrailSection))
			{
				obj2 = RuntimeServices.Coerce(obj2, typeof(TronTrailSection));
			}
			tronTrailSection2 = (TronTrailSection)obj2;
		}
		if (sections.length == 0 || !((tronTrailSection2.point - position).sqrMagnitude <= minDistance * minDistance))
		{
			TronTrailSection tronTrailSection3 = new TronTrailSection();
			tronTrailSection3.point = position;
			if (alwaysUp)
			{
				tronTrailSection3.upDir = Vector3.up;
			}
			else
			{
				tronTrailSection3.upDir = transform.TransformDirection(Vector3.up);
			}
			tronTrailSection3.time = num;
			sections.Unshift(tronTrailSection3);
		}
		Mesh mesh = ((MeshFilter)GetComponent(typeof(MeshFilter))).mesh;
		mesh.Clear();
		if (sections.length < 2)
		{
			return;
		}
		Vector3[] array = new Vector3[sections.length * 2];
		Color[] array2 = new Color[sections.length * 2];
		Vector2[] array3 = new Vector2[sections.length * 2];
		object obj3 = sections[0];
		if (!(obj3 is TronTrailSection))
		{
			obj3 = RuntimeServices.Coerce(obj3, typeof(TronTrailSection));
		}
		TronTrailSection tronTrailSection4 = (TronTrailSection)obj3;
		object obj4 = sections[0];
		if (!(obj4 is TronTrailSection))
		{
			obj4 = RuntimeServices.Coerce(obj4, typeof(TronTrailSection));
		}
		TronTrailSection tronTrailSection5 = (TronTrailSection)obj4;
		Matrix4x4 worldToLocalMatrix = transform.worldToLocalMatrix;
		for (int i = 0; i < sections.length; i++)
		{
			tronTrailSection4 = tronTrailSection5;
			object obj5 = sections[i];
			if (!(obj5 is TronTrailSection))
			{
				obj5 = RuntimeServices.Coerce(obj5, typeof(TronTrailSection));
			}
			tronTrailSection5 = (TronTrailSection)obj5;
			float num2 = 0f;
			if (i != 0)
			{
				num2 = Mathf.Clamp01((Time.time - tronTrailSection5.time) / time);
			}
			Vector3 upDir = tronTrailSection5.upDir;
			array[i * 2 + 1] = worldToLocalMatrix.MultiplyPoint(tronTrailSection5.point);
			array[i * 2 + 0] = worldToLocalMatrix.MultiplyPoint(tronTrailSection5.point + upDir * height);
			array3[i * 2 + 0] = new Vector2(num2, 0f);
			array3[i * 2 + 1] = new Vector2(num2, 1f);
			Color color = Color.Lerp(startColor, endColor, num2);
			array2[i * 2 + 0] = color;
			array2[i * 2 + 1] = color;
		}
		int[] array4 = new int[(sections.length - 1) * 2 * 3];
		for (int i = 0; i < Extensions.get_length((System.Array)array4) / 6; i++)
		{
			array4[i * 6 + 0] = i * 2;
			array4[i * 6 + 1] = i * 2 + 1;
			array4[i * 6 + 2] = i * 2 + 2;
			array4[i * 6 + 3] = i * 2 + 2;
			array4[i * 6 + 4] = i * 2 + 1;
			array4[i * 6 + 5] = i * 2 + 3;
		}
		mesh.vertices = array;
		mesh.colors = array2;
		mesh.uv = array3;
		mesh.triangles = array4;
	}

	public virtual void Main()
	{
	}
}
