using System.Collections.Generic;
using UnityEngine;

public class MaterialPulse : MonoBehaviour
{
	public Color TintColor = Color.red;

	public string TintShader = string.Empty;

	public float PulseSpeed = 5f;

	public int MaxPulses = 3;

	public bool StripTextures;

	protected string defaultTintShaderName = "Self-Illumin/Diffuse";

	protected PulseColorTint pulseColor;

	protected Color baseColor = Color.white;

	protected Shader tintShader;

	protected List<MaterialData> materials = new List<MaterialData>();

	protected bool Pulsing
	{
		get
		{
			return pulseColor.Pulsing;
		}
	}

	protected virtual void Awake()
	{
		pulseColor = new PulseColorTint(baseColor, TintColor);
		tintShader = Shader.Find(defaultTintShaderName);
		if (TintShader != defaultTintShaderName && !string.IsNullOrEmpty(TintShader))
		{
			Shader shader = Shader.Find(TintShader);
			if (shader != null)
			{
				tintShader = shader;
			}
			else
			{
				Debug.LogWarning(string.Format("MaterialPulse:Awake() Shader '{0}' not found ({1}). Using Diffuse", TintShader, base.transform.name));
			}
		}
	}

	protected virtual void Start()
	{
		FindMaterials(base.transform.root);
	}

	protected virtual void Update()
	{
		pulseColor.Update();
		if (pulseColor.Pulsing)
		{
			SetMaterialColor(pulseColor.CurrentColor);
		}
	}

	public bool Pulse()
	{
		if (pulseColor.Pulse(MaxPulses, PulseSpeed, OnPulseDone))
		{
			if (StripTextures)
			{
				ClearTextures();
			}
			SetTintShader();
			return true;
		}
		return false;
	}

	public void StopPulse(bool triggerCallback = false)
	{
		pulseColor.Stop(triggerCallback);
	}

	protected virtual void OnPulseDone()
	{
		if (StripTextures)
		{
			SetDefaultTextures();
		}
		SetMaterialColor(baseColor);
		SetDefaultShader();
	}

	protected void FindMaterials(Transform transform)
	{
		materials = GlobalHelpers.FindMaterials<Renderer>(transform);
	}

	protected void FindMaterials<T>(Transform transform) where T : Renderer
	{
		materials = GlobalHelpers.FindMaterials<T>(transform);
	}

	private void SetMaterialColor(Color color)
	{
		foreach (MaterialData material in materials)
		{
			material.Material.color = color;
		}
	}

	private void SetTintShader()
	{
		foreach (MaterialData material in materials)
		{
			material.Material.shader = tintShader;
		}
	}

	private void SetDefaultShader()
	{
		foreach (MaterialData material in materials)
		{
			material.Material.shader = material.DefaultShader;
		}
	}

	private void ClearTextures()
	{
		foreach (MaterialData material in materials)
		{
			material.Material.mainTexture = null;
		}
	}

	private void SetDefaultTextures()
	{
		foreach (MaterialData material in materials)
		{
			material.Material.mainTexture = material.DefaultTexture;
		}
	}

	private void DebugPrintMaterials(Transform transform)
	{
		MonoBehaviour.print("Found materials in " + transform.name);
		foreach (MaterialData material in materials)
		{
			MonoBehaviour.print(material.Material.name);
		}
		MonoBehaviour.print("----");
	}
}
