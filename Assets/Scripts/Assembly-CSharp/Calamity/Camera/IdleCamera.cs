using UnityEngine;

namespace Calamity.Camera
{
	public class IdleCamera : CameraState
	{
		public override Vector3 Position
		{
			get
			{
				return cameraTransform.position;
			}
		}

		public override Quaternion Rotation
		{
			get
			{
				return cameraTransform.rotation;
			}
		}

		public IdleCamera()
		{
			type = CameraStateType.Idle;
		}
	}
}
