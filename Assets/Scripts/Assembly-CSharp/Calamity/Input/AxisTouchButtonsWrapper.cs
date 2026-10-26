using UnityEngine;

namespace Calamity.Input
{
	public class AxisTouchButtonsWrapper : InputWrapper
	{
		private TouchButtonWrapper minButton;

		private TouchButtonWrapper maxButton;

		public AxisTouchButtonsWrapper(string minButton, string maxButton)
		{
			this.minButton = new TouchButtonWrapper(minButton);
			this.maxButton = new TouchButtonWrapper(maxButton);
			requiresUpdate = true;
		}

		public AxisTouchButtonsWrapper(GameObject minButton, GameObject maxButton)
		{
			this.minButton = new TouchButtonWrapper(minButton);
			this.maxButton = new TouchButtonWrapper(maxButton);
			requiresUpdate = true;
		}

		public override void InitializeIngame()
		{
			minButton.InitializeIngame();
			maxButton.InitializeIngame();
		}

		public override void Update()
		{
			minButton.Update();
			maxButton.Update();
		}

		public override int GetInt()
		{
			if (minButton.GetBool())
			{
				return -1;
			}
			if (maxButton.GetBool())
			{
				return 1;
			}
			return 0;
		}

		public override float GetFloat()
		{
			return GetInt();
		}

		public override bool GetBool()
		{
			return minButton.GetBool() || maxButton.GetBool();
		}

		public override void Shutdown()
		{
			minButton.Shutdown();
			maxButton.Shutdown();
		}
	}
}
