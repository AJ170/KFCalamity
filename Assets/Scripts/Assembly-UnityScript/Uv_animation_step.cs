using System;
using UnityEngine;

[Serializable]
public class Uv_animation_step : MonoBehaviour
{
	public float scrollSpeedx;

	public int Yoffset;

	public float stepx;

	public float stepy;

	public float stepTimeX;

	public Material materials;

	private float timex;

	private float offsetx;

	private float offsety;

	private int offset;

	public Uv_animation_step()
	{
		scrollSpeedx = 0.1f;
		Yoffset = 2;
		stepx = 0.5f;
		stepy = 0.5f;
		stepTimeX = 20f;
	}

	public virtual void Update()
	{
		if (!(stepTimeX >= timex))
		{
			offsetx += stepx;
			if (Yoffset == offset)
			{
				offsety += stepy;
				offset = 0;
			}
			materials.SetTextureOffset("_Illum", new Vector2(offsetx, offsety));
			materials.SetTextureOffset("_MainTex", new Vector2(offsetx, offsety));
			timex = 0f;
			offset++;
		}
		if (!(stepTimeX <= timex))
		{
			timex += scrollSpeedx;
		}
		if (!(offsetx <= 100000f))
		{
			offsetx = 0f;
			offsety = 0f;
		}
	}

	public virtual void Main()
	{
	}
}
