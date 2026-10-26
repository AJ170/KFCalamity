using Calamity.Camera;
using UnityEngine;

public class BrightnessComponent : MonoBehaviour
{
	public float BrightnessAmount = 1.5f;

	public float saturationAmount = 1f;

	public float contrastAmount = 1f;

	private BrightnessCameraEffect effect;

	private OptionManager optionManager;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
	}
}
