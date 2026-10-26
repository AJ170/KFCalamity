using UnityEngine;

namespace Calamity.Input
{
	public class TouchButtonWrapper : InputWrapper
	{
		private string buttonName;

		private bool clicked;

		private bool toggle;

		private bool canHoldDown;

		private float delay;

		private float startTime;

		private bool thisFrame;

		private ButtonActions buttonActions;

		public TouchButtonWrapper(string buttonName, bool toggle = false, float delay = 0f, bool canHoldDown = false)
		{
			this.buttonName = buttonName;
			this.toggle = toggle;
			requiresUpdate = true;
			this.delay = delay;
			this.canHoldDown = canHoldDown;
		}

		public TouchButtonWrapper(GameObject buttonObject, bool toggle = false, float delay = 0f, bool canHoldDown = false)
			: this(string.Empty, toggle, delay, canHoldDown)
		{
			InitializeIngame(buttonObject);
		}

		public void InitializeIngame(GameObject buttonObject)
		{
			buttonActions = buttonObject.GetComponent<ButtonActions>();
			if (buttonActions == null)
			{
				ButtonActions[] componentsInChildren = buttonObject.GetComponentsInChildren<ButtonActions>(true);
				buttonActions = ((componentsInChildren == null || componentsInChildren.Length <= 0) ? null : componentsInChildren[0]);
			}
			if (buttonActions != null)
			{
				buttonActions.SetOnClickCallback(OnButtonClicked);
				buttonActions.SetOnReleaseCallback(OnButtonReleased);
			}
			else
			{
				Debug.LogWarning("TouchButtonWrapper: Could not find a ButtonActions component on the object: " + buttonName);
			}
		}

		public override void InitializeIngame()
		{
			GameObject gameObject = GameObject.Find(buttonName);
			if (gameObject == null)
			{
				Debug.LogError("TouchButtonWrapper tried to find button: " + buttonName + " but no button could be found!");
			}
			else
			{
				InitializeIngame(gameObject);
			}
		}

		public override void Shutdown()
		{
			buttonActions.RemoveOnClickCallback();
			buttonActions.RemoveOnReleaseCallback();
		}

		public override void OnGameObjectUpdated(GameObject gameObject)
		{
		}

		public override void Update()
		{
			if (!toggle && !canHoldDown)
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
			if (startTime > 0f && Time.unscaledTime - startTime > delay)
			{
				clicked = true;
				thisFrame = true;
				startTime = -1f;
			}
		}

		private void OnButtonClicked(ButtonActions sender)
		{
			if (!toggle)
			{
				if (delay > 0f)
				{
					startTime = Time.unscaledTime;
					return;
				}
				clicked = true;
				thisFrame = true;
			}
			else
			{
				clicked = !clicked;
			}
		}

		private void OnButtonReleased(ButtonActions sender)
		{
			if (!toggle || canHoldDown)
			{
				clicked = false;
				thisFrame = false;
				startTime = -1f;
			}
		}

		public override bool GetBool()
		{
			return clicked;
		}
	}
}
