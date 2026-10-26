using UnityEngine;

namespace Calamity.Camera
{
	public abstract class CameraState
	{
		protected Transform cameraTransform;

		protected CameraStateType type;

		public CameraStateType Type
		{
			get
			{
				return type;
			}
		}

		public abstract Vector3 Position { get; }

		public abstract Quaternion Rotation { get; }

		public virtual void Start(Transform transform)
		{
			cameraTransform = transform;
		}

		public virtual void Update()
		{
		}

		public virtual void LateUpdate()
		{
		}

		public virtual void OnActivated()
		{
		}

		public virtual void OnDeactivated()
		{
		}

		public virtual void Shutdown()
		{
		}
	}
}
