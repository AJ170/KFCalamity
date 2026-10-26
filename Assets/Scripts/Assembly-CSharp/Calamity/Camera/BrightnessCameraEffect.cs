using UnityEngine;

namespace Calamity.Camera
{
	public class BrightnessCameraEffect : CameraEffect
	{
		private const string shaderName = "Hidden/Brightness";

		private Material material;

		private Shader shader;

		public float brightnessAmount = 1.5f;

		public float saturationAmount = 1f;

		public float contrastAmount = 1f;

		public BrightnessCameraEffect()
		{
			archetype = CameraEffectArchetype.Visual;
			type = CameraEffectType.Brightness;
			isStatic = true;
		}

		public override void Start(Transform transform)
		{
			base.Start(transform);
			if (SystemInfo.supportsImageEffects)
			{
				shader = Shader.Find("Hidden/Brightness");
				if (shader != null && shader.isSupported)
				{
					material = new Material(shader);
					material.hideFlags = HideFlags.HideAndDontSave;
				}
				else
				{
					Debug.LogError("System does not support the brightness shader");
				}
			}
			else
			{
				Debug.LogError("System does not support image effects!");
			}
		}

		public override bool OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (shader != null)
			{
				material.SetFloat("_BrightnessAmount", brightnessAmount);
				material.SetFloat("_SaturationAmount", saturationAmount);
				material.SetFloat("_ContrastAmount", contrastAmount);
				Graphics.Blit(source, destination, material);
				return true;
			}
			Graphics.Blit(source, destination);
			return true;
		}

		public override void Update()
		{
			brightnessAmount = Mathf.Clamp(brightnessAmount, 0f, 1.5f);
			saturationAmount = Mathf.Clamp(saturationAmount, 0f, 2f);
			contrastAmount = Mathf.Clamp(contrastAmount, 0f, 3f);
		}
	}
}
