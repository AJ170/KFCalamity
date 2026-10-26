using UnityEngine;

namespace Calamity.Input
{
	public class MouseRotationWrapper : InputWrapper
	{
		private float planeHeight = -1.4f;

		private Plane plane;

		private GameObject gameObject;

		private Transform transform;

		private PlayerWeaponController weaponController;

		private UnityEngine.Camera camera;

		private Weapon weapon;

		private Transform muzzle;

		public MouseRotationWrapper()
		{
			requiresUpdate = true;
			plane = new Plane(Vector3.up, Vector3.zero + Vector3.up * planeHeight);
		}

		public override void Update()
		{
			if (gameObject != null)
			{
				if (weaponController != null && weapon != weaponController.ActiveWeapon)
				{
					muzzle = weaponController.ActiveWeapon.PrimaryMuzzleBone;
					weapon = weaponController.ActiveWeapon;
				}
				if (muzzle != null)
				{
					planeHeight = 0f - (muzzle.position.y - transform.position.y);
				}
				plane.normal = transform.up;
				plane.distance = 0f - transform.position.y + planeHeight;
			}
		}

		private Vector3 GetValidMousePos(Vector3 mousePos)
		{
			Vector3 position = transform.position;
			float magnitude = (muzzle.position - position).magnitude;
			Vector3 result = mousePos;
			float num = (mousePos - position).sqrMagnitude - (muzzle.position - position).sqrMagnitude;
			if (num < 0.5f)
			{
				Vector3 vector = mousePos - position;
				vector.y = 0f;
				result += vector.normalized * magnitude;
			}
			return result;
		}

		public override Vector3 GetVector()
		{
			if (gameObject != null && camera != null)
			{
				Vector3 mousePosition = UnityEngine.Input.mousePosition;
				Vector3 vector = ScreenPointToWorldPointOnPlane(mousePosition);
				Vector3 zero = Vector3.zero;
				zero = ((!(muzzle != null)) ? (vector - transform.position) : (GetValidMousePos(vector) - muzzle.position));
				zero.y = 0f;
				return zero;
			}
			return Vector3.zero;
		}

		public override void OnGameObjectUpdated(GameObject gameObject)
		{
			this.gameObject = gameObject;
			if (gameObject != null)
			{
				transform = gameObject.transform;
				weaponController = gameObject.GetComponent<PlayerWeaponController>();
				OnLevelLoaded();
			}
			else
			{
				transform = null;
			}
		}

		public override void OnLevelLoaded()
		{
			camera = UnityEngine.Camera.main;
		}

		private Vector3 ScreenPointToWorldPointOnPlane(Vector3 screenPosition)
		{
			Ray ray = camera.ScreenPointToRay(screenPosition);
			float enter;
			plane.Raycast(ray, out enter);
			return ray.GetPoint(enter);
		}
	}
}
