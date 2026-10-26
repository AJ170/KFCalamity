using System;
using UnityEngine;

namespace Calamity.Camera
{
	[Serializable]
	public class FocusCamera : CameraState
	{
		public GameObject Roof;

		public Vector3 Offset = Vector3.zero;

		public Vector3 TargetLookOffset = Vector3.zero;

		private GameObject target;

		private Transform targetTransform;

		private Vector3 position = Vector3.zero;

		private Quaternion rotation = Quaternion.identity;

		public override Vector3 Position
		{
			get
			{
				return position;
			}
		}

		public override Quaternion Rotation
		{
			get
			{
				return rotation;
			}
		}

		public FocusCamera()
		{
			type = CameraStateType.Focus;
		}

		public override void Update()
		{
			UpdatePosition();
		}

		public override void OnActivated()
		{
			if (SoldierManager.Instance != null)
			{
				SoldierManager.Instance.SetLockedInput(true, this);
			}
			if (GuiInGameManager.Instance != null)
			{
				GuiInGameManager.Instance.SetUnitBarVisibility(false);
			}
			if (Roof != null)
			{
				Roof.SetActive(true);
			}
			UpdatePosition();
		}

		public override void OnDeactivated()
		{
			if (SoldierManager.Instance != null)
			{
				SoldierManager.Instance.SetLockedInput(false, this);
			}
			if (GuiInGameManager.Instance != null)
			{
				GuiInGameManager.Instance.SetUnitBarVisibility(true);
			}
			if (Roof != null)
			{
				Roof.SetActive(false);
			}
			target = null;
			targetTransform = null;
		}

		public void SetTarget(GameObject target)
		{
			this.target = target;
			if (target != null)
			{
				targetTransform = target.transform;
			}
			else
			{
				targetTransform = null;
			}
		}

		private void UpdatePosition()
		{
			if (target != null)
			{
				position = targetTransform.TransformPoint(Offset);
				rotation = Quaternion.LookRotation(Vector3.Normalize(targetTransform.position + TargetLookOffset - cameraTransform.position));
			}
		}
	}
}
