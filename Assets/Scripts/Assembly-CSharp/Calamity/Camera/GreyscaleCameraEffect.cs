using UnityEngine;

namespace Calamity.Camera
{
	public class GreyscaleCameraEffect : CameraEffect
	{
		private const string shaderName = "Hidden/Grayscale Effect";

		private Shader shader;

		private Material material;

		private float offset;

		public GreyscaleCameraEffect(float offset = 0f)
		{
			this.offset = offset;
			archetype = CameraEffectArchetype.Visual;
			type = CameraEffectType.Greyscale;
			isStatic = true;
		}

		public override void Start(Transform transform)
		{
			base.Start(transform);
			if (SystemInfo.supportsImageEffects)
			{
				shader = Shader.Find("Hidden/Grayscale Effect");
				if (shader != null && shader.isSupported)
				{
					material = new Material(shader);
					material.hideFlags = HideFlags.HideAndDontSave;
					material.SetFloat("_RampOffset", offset);
				}
			}
		}

		public override bool OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (material != null)
			{
				Graphics.Blit(source, destination, material);
				return true;
			}
			return false;
		}

		public override void Shutdown()
		{
			if (material != null)
			{
				Object.DestroyImmediate(material);
			}
		}
	}
}
