using UnityEngine;

namespace Calamity.Camera
{
	public class ShakeCameraEffect : CameraEffect
	{
		private Vector3 relativePosition = Vector3.zero;

		private float intensity = 1f;

		private CameraShakeDecayType decayType;

		private float decay = 1f;

		private bool useDuration;

		private float duration = 1f;

		private float startIntensity = 1f;

		private float counter;

		public override Vector3 Position
		{
			get
			{
				return relativePosition;
			}
		}

		public override Quaternion Rotation
		{
			get
			{
				return Quaternion.identity;
			}
		}

		public override bool IsDone
		{
			get
			{
				return intensity <= 0f || (useDuration && counter <= 0f);
			}
		}

		public ShakeCameraEffect(float intensity, CameraShakeDecayType decayType, float decay, bool useDuration, float duration, GameObject source = null)
		{
			this.intensity = intensity;
			this.decayType = decayType;
			this.decay = decay;
			this.useDuration = useDuration;
			this.duration = duration;
			base.source = source;
			archetype = CameraEffectArchetype.Transform;
			type = CameraEffectType.Shake;
		}

		public override void Start(Transform transform)
		{
			base.Start(transform);
			startIntensity = intensity;
			if (useDuration)
			{
				counter = duration;
			}
			else if (decayType == CameraShakeDecayType.OverDuration)
			{
				decayType = CameraShakeDecayType.PerSecond;
			}
		}

		public override void Update()
		{
			UpdateShake();
			UpdateIntensity();
			UpdateDuration();
		}

		private void UpdateShake()
		{
			relativePosition = Random.insideUnitSphere * intensity;
		}

		private void UpdateIntensity()
		{
			if (decayType == CameraShakeDecayType.OverDuration)
			{
				intensity = startIntensity - decay * (1f - counter / duration);
			}
			else if (decayType == CameraShakeDecayType.PerSecond)
			{
				intensity -= decay * Time.deltaTime;
			}
			else if (decayType == CameraShakeDecayType.PerFrame)
			{
				intensity -= decay;
			}
		}

		private void UpdateDuration()
		{
			if (useDuration)
			{
				counter -= Time.deltaTime;
			}
		}
	}
}
