using UnityEngine;

namespace Calamity.Input
{
	public class DoubleTapInputWrapper : InputWrapper
	{
		private string gameObjectName;

		private bool thisFrame;

		private bool clicked;

		private bool initialized;

		private DoubleTapInterval doubleTap;

		public DoubleTapInputWrapper(string gameObjectName)
		{
			this.gameObjectName = gameObjectName;
			requiresUpdate = true;
		}

		private void OnDoubleTap(DoubleTapInterval doubleTap)
		{
			clicked = true;
			thisFrame = true;
			DeviceSwitchController.SubscribeToSwitch(this, OnDeviceSwitch);
		}

		public override void InitializeIngame()
		{
			GameObject gameObject = GameObject.Find(gameObjectName);
			if (gameObject != null)
			{
				doubleTap = gameObject.GetComponent<DoubleTapInterval>();
				if (doubleTap != null)
				{
					doubleTap.OnDoubleTap = OnDoubleTap;
					initialized = true;
					return;
				}
				DoubleTapInterval[] componentsInChildren = gameObject.GetComponentsInChildren<DoubleTapInterval>(true);
				if (componentsInChildren != null && componentsInChildren.Length > 0)
				{
					doubleTap = componentsInChildren[0];
					doubleTap.OnDoubleTap = OnDoubleTap;
					initialized = true;
				}
				else
				{
					Debug.LogError("DoubleTapInputWrapper found the game object, but it contains no DoubleTapInterval component!");
				}
			}
			else
			{
				Debug.LogError("DoubleTapInputWrapper could not find game object: " + gameObjectName);
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
			if (doubleTap != null)
			{
				doubleTap.OnDoubleTap = null;
			}
			DeviceSwitchController.UnsubscribeToSwitch(this);
		}

		public override void Update()
		{
			if (thisFrame)
			{
				thisFrame = false;
			}
			else
			{
				clicked = false;
			}
		}

		public override bool GetBool()
		{
			return clicked;
		}

		public override int GetInt()
		{
			return GetBool() ? 1 : 0;
		}
	}
}
