using UnityEngine;

namespace Calamity.Input
{
	public class VirtualStickThresholdWrapper : VirtualStickWrapper
	{
		private float minThreshold;

		private float maxThreshold;

		private float timer;

		private float time;

		private bool sticky;

		private string zoneName;

		private bool trigger = true;

		public VirtualStickThresholdWrapper(string virtualStickObjectName, string zoneName, float time = 0f, bool sticky = false)
			: this(virtualStickObjectName, 0f, 0f, 0f)
		{
			this.zoneName = zoneName;
		}

		public VirtualStickThresholdWrapper(string virtualStickObjectName, float minThreshold, float maxThreshold, float time = 0f, bool sticky = false)
			: base(virtualStickObjectName)
		{
			this.minThreshold = minThreshold;
			this.maxThreshold = maxThreshold;
			requiresUpdate = true;
			this.time = time;
			this.sticky = sticky;
			if (time > 0f)
			{
				trigger = false;
			}
		}

		public override void InitializeIngame()
		{
			base.InitializeIngame();
			if (!string.IsNullOrEmpty(zoneName) && joystick != null)
			{
				joystick.GetZoneThreshold(zoneName, ref minThreshold, ref maxThreshold);
			}
		}

		public override void Update()
		{
			if (!base.Initialized || !(time > 0f))
			{
				return;
			}
			int num = IsInThreshold();
			if (num == 0)
			{
				timer += Time.deltaTime;
				if (timer >= time)
				{
					trigger = true;
				}
			}
			else if (!sticky || (sticky && num == -1))
			{
				trigger = false;
				timer = 0f;
			}
		}

		private int IsInThreshold()
		{
			float @float = GetFloat();
			return (@float < minThreshold) ? (-1) : ((@float > maxThreshold) ? 1 : 0);
		}

		public override bool GetBool()
		{
			if (!trigger || !base.Initialized)
			{
				return false;
			}
			int num = IsInThreshold();
			return num == 0 || (sticky && num == 1);
		}
	}
}
