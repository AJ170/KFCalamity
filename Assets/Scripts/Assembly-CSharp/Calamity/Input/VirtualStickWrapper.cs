using UnityEngine;

namespace Calamity.Input
{
	public class VirtualStickWrapper : InputWrapper
	{
		private string virtualStickName;

		private Vector3 screenMovementForward;

		private Vector3 screenMovementRight;

		private bool invertHorizontal;

		private bool invertVertical;

		private bool initialized;

		protected VirtualJoystick joystick;

		protected bool Initialized
		{
			get
			{
				return initialized;
			}
		}

		public VirtualStickWrapper(string virtualStickObjectName)
		{
			virtualStickName = virtualStickObjectName;
			invertHorizontal = false;
			invertVertical = false;
			UpdateScreenMovement();
		}

		public override void InitializeIngame()
		{
			initialized = false;
			GameObject gameObject = GameObject.Find(virtualStickName);
			if ((bool)gameObject)
			{
				VirtualJoystick component = gameObject.GetComponent<VirtualJoystick>();
				if (component == null)
				{
					VirtualJoystick[] componentsInChildren = gameObject.GetComponentsInChildren<VirtualJoystick>(true);
					if (componentsInChildren.Length > 0)
					{
						joystick = componentsInChildren[0];
					}
				}
				else
				{
					joystick = component;
				}
				initialized = true;
			}
			else
			{
				DeviceSwitchController.SubscribeToSwitch(this, OnDeviceSwitch);
				initialized = false;
			}
		}

		private void OnDeviceSwitch(DeviceSwitchController.SwitchArgs args)
		{
			if (!initialized)
			{
				InitializeIngame();
			}
		}

		public override void Shutdown()
		{
			DeviceSwitchController.UnsubscribeToSwitch(this);
		}

		public override void OnLevelLoaded()
		{
			UpdateScreenMovement();
		}

		public override float GetFloat()
		{
			return GetVector().magnitude;
		}

		public override bool GetBool()
		{
			return GetVector() != Vector3.zero;
		}

		public override Vector3 GetVector()
		{
			if (joystick == null)
			{
				return Vector3.zero;
			}
			return joystick.Direction.x * screenMovementRight * ((!invertHorizontal) ? 1 : (-1)) + joystick.Direction.y * screenMovementForward * ((!invertVertical) ? 1 : (-1));
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
