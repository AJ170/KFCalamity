using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class REDDOT_OldMovie_PostProcess : MonoBehaviour
{
	public enum Detail
	{
		Simple = 0,
		Medium = 1,
		Full = 2
	}

	public Detail mode = Detail.Full;

	public float blackAndWhiteIntensity = 1f;

	public Color colorTint = Color.white;

	public float timeScale = 1f;

	public float grainIntensity = 1f;

	public float filmDirtSize = 1f;

	public Shader shader;

	private Material rbMaterial;

	private Material GetMaterial()
	{
		if (rbMaterial == null)
		{
			rbMaterial = new Material(shader);
			rbMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		return rbMaterial;
	}

	private void Start()
	{
		if (shader == null)
		{
			Debug.LogError("No shader assigned!", this);
			base.enabled = false;
		}
	}

	private void Update()
	{
		switch (mode)
		{
		case Detail.Simple:
			Shader.EnableKeyword("MODE1");
			Shader.DisableKeyword("MODE2");
			Shader.DisableKeyword("MODE3");
			break;
		case Detail.Medium:
			Shader.DisableKeyword("MODE1");
			Shader.EnableKeyword("MODE2");
			Shader.DisableKeyword("MODE3");
			break;
		case Detail.Full:
			Shader.DisableKeyword("MODE1");
			Shader.DisableKeyword("MODE2");
			Shader.EnableKeyword("MODE3");
			break;
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		timeScale = Mathf.Clamp(timeScale, 0f, 100f);
		GetMaterial().SetFloat("_timeScale", timeScale);
		filmDirtSize = Mathf.Clamp(filmDirtSize, 0f, 100f);
		GetMaterial().SetFloat("_filmDirtSize", filmDirtSize);
		blackAndWhiteIntensity = Mathf.Clamp(blackAndWhiteIntensity, 0f, 1f);
		GetMaterial().SetFloat("_bawIntensity", blackAndWhiteIntensity);
		GetMaterial().SetColor("_colorTint", colorTint);
		grainIntensity = Mathf.Clamp(grainIntensity, 0f, 3f);
		GetMaterial().SetFloat("_grainIntensity", grainIntensity);
		Graphics.Blit(source, dest, GetMaterial());
	}
}
