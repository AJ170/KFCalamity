using UnityEngine;

namespace Calamity.Camera
{
	public class BlurCameraEffect : CameraEffect
	{
		private const string shaderName = "Hidden/BlurEffectConeTap";

		private int iterations = 3;

		private float spread = 0.6f;

		private Shader shader;

		private Material material;

		public BlurCameraEffect()
		{
			archetype = CameraEffectArchetype.Visual;
			type = CameraEffectType.Blur;
			isStatic = true;
		}

		public BlurCameraEffect(int iterations, float spread)
			: this()
		{
			this.iterations = iterations;
			this.spread = spread;
		}

		public override void Start(Transform transform)
		{
			base.Start(transform);
			if (SystemInfo.supportsImageEffects)
			{
				shader = Shader.Find("Hidden/BlurEffectConeTap");
				if (shader != null && shader.isSupported)
				{
					material = new Material(shader);
					material.hideFlags = HideFlags.HideAndDontSave;
				}
			}
		}

		public override void Shutdown()
		{
			if (material != null)
			{
				Object.DestroyImmediate(material);
			}
		}

		public override bool OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (material != null)
			{
				RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
				RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
				DownSample4x(source, temporary);
				bool flag = true;
				for (int i = 0; i < iterations; i++)
				{
					if (flag)
					{
						FourTapCone(temporary, temporary2, i);
					}
					else
					{
						FourTapCone(temporary2, temporary, i);
					}
					flag = !flag;
				}
				if (flag)
				{
					Graphics.Blit(temporary, destination);
				}
				else
				{
					Graphics.Blit(temporary2, destination);
				}
				RenderTexture.ReleaseTemporary(temporary);
				RenderTexture.ReleaseTemporary(temporary2);
				return true;
			}
			return false;
		}

		public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
		{
			float num = 0.5f + (float)iteration * spread;
			Graphics.BlitMultiTap(source, dest, material, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
		}

		private void DownSample4x(RenderTexture source, RenderTexture dest)
		{
			float num = 1f;
			Graphics.BlitMultiTap(source, dest, material, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
		}
	}
}
