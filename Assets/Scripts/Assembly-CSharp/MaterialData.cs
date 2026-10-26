using UnityEngine;

public class MaterialData
{
	private Material material;

	private Shader defaultShader;

	private Texture defaultTexture;

	private Color defaultColor = Color.white;

	public Material Material
	{
		get
		{
			return material;
		}
	}

	public Shader DefaultShader
	{
		get
		{
			return defaultShader;
		}
	}

	public Texture DefaultTexture
	{
		get
		{
			return defaultTexture;
		}
	}

	public Color DefaultColor
	{
		get
		{
			return defaultColor;
		}
		set
		{
			defaultColor = value;
		}
	}

	public Color color
	{
		get
		{
			return material.color;
		}
		set
		{
			material.color = value;
		}
	}

	public MaterialData(Material material)
	{
		this.material = material;
		defaultShader = material.shader;
		defaultTexture = material.mainTexture;
		if (material.HasProperty("_Color"))
		{
			defaultColor = material.color;
		}
	}
}
