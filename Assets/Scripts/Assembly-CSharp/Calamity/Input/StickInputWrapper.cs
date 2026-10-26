using UnityEngine;

namespace Calamity.Input
{
	public class StickInputWrapper : InputWrapper
	{
		private string horizontalAxis;

		private string verticalAxis;

		private bool invertHorizontal;

		private bool invertVertical;

		private Vector3 screenMovementRight;

		private Vector3 screenMovementForward;

		public StickInputWrapper(string horizontalAxis, string verticalAxis, bool invertHorizontal, bool invertVertical)
		{
			this.horizontalAxis = horizontalAxis;
			this.verticalAxis = verticalAxis;
			this.invertHorizontal = invertHorizontal;
			this.invertVertical = invertVertical;
			UpdateScreenMovement();
		}

		public override Vector3 GetVector()
		{
			Vector3 result = UnityEngine.Input.GetAxis(horizontalAxis) * screenMovementRight * ((!invertHorizontal) ? 1 : (-1)) + UnityEngine.Input.GetAxis(verticalAxis) * screenMovementForward * ((!invertVertical) ? 1 : (-1));
			if (result.sqrMagnitude > 0f)
			{
				result.Normalize();
			}
			return result;
		}

		public override bool GetBool()
		{
			return GetVector() != Vector3.zero;
		}

		public override int GetInt()
		{
			return GetBool() ? 1 : 0;
		}

		public override float GetFloat()
		{
			return GetVector().magnitude;
		}

		public override void OnLevelLoaded()
		{
			UpdateScreenMovement();
		}

		public override void OnGameObjectUpdated(GameObject gameObject)
		{
			if (gameObject != null)
			{
				UpdateScreenMovement();
			}
		}

		private void UpdateScreenMovement()
		{
			UnityEngine.Camera main = UnityEngine.Camera.main;
			if (main != null)
			{
				Quaternion quaternion = Quaternion.Euler(0f, main.transform.eulerAngles.y, 0f);
				screenMovementForward = quaternion * Vector3.forward;
				screenMovementRight = quaternion * Vector3.right;
			}
			else
			{
				screenMovementForward = Vector3.forward;
				screenMovementRight = Vector3.right;
			}
		}
	}
}
