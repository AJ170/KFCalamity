using UnityEngine;

namespace Calamity.Camera
{
	public abstract class CameraEffect
	{
		protected Transform cameraTransform;

		protected CameraEffectArchetype archetype;

		protected CameraEffectType type;

		protected GameObject source;

		protected bool isStatic;

		public CameraEffectArchetype Archetype
		{
			get
			{
				return archetype;
			}
		}

		public CameraEffectType Type
		{
			get
			{
				return type;
			}
		}

		public virtual bool IsDone
		{
			get
			{
				return false;
			}
		}

		public GameObject Source
		{
			get
			{
				return source;
			}
		}

		public bool IsStatic
		{
			get
			{
				return isStatic;
			}
		}

		public virtual Vector3 Position
		{
			get
			{
				return Vector3.zero;
			}
		}

		public virtual Quaternion Rotation
		{
			get
			{
				return Quaternion.identity;
			}
		}

		public virtual void Start(Transform transform)
		{
			cameraTransform = transform;
		}

		public virtual void Update()
		{
		}

		public virtual void Shutdown()
		{
		}

		public virtual bool OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			return false;
		}
	}
}
