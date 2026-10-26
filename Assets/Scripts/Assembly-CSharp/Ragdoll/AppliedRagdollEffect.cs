using UnityEngine;

namespace Ragdoll
{
	public class AppliedRagdollEffect
	{
		public float Force;

		public Vector3 Source;

		public RagdollEffectType Type;

		public float Lifetime = 0.1f;

		public AppliedRagdollEffect(float force, Vector3 source, RagdollEffectType type)
		{
			Force = force;
			Source = source;
			Type = type;
		}

		public AppliedRagdollEffect(float force, Vector3 source, RagdollEffectType type, float lifetime)
		{
			Force = force;
			Source = source;
			Type = type;
			Lifetime = lifetime;
		}

		public AppliedRagdollEffect(RagdollEffectType type)
		{
			Force = 0f;
			Source = Vector3.zero;
			Type = type;
		}

		public AppliedRagdollEffect(RagdollEffectType type, float lifetime)
			: this(type)
		{
			Lifetime = lifetime;
		}
	}
}
