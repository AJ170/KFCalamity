using UnityEngine;

namespace Calamity.Input
{
	public class DualAxisInputWrapper : InputWrapper
	{
		protected string axis;

		protected bool inverted;

		private float threshold = 0.1f;

		public DualAxisInputWrapper(string axis, bool inverted = false)
		{
			this.axis = axis;
			this.inverted = inverted;
		}

		public DualAxisInputWrapper(string axis, float threshold)
		{
			this.axis = axis;
			this.threshold = Mathf.Abs(threshold);
		}

		public DualAxisInputWrapper(string axis, bool inverted, float threshold)
		{
			this.axis = axis;
			this.inverted = inverted;
			this.threshold = Mathf.Abs(threshold);
		}

		public override bool GetBool()
		{
			float f = UnityEngine.Input.GetAxis(axis);
			return Mathf.Abs(f) > threshold;
		}

		public override int GetInt()
		{
			float num = UnityEngine.Input.GetAxis(axis);
			if (Mathf.Abs(num) > threshold)
			{
				if (inverted)
				{
					num = 0f - num;
				}
				return (num > 0f) ? 1 : (-1);
			}
			return 0;
		}

		public override float GetFloat()
		{
			float num = UnityEngine.Input.GetAxis(axis);
			if (Mathf.Abs(num) > threshold)
			{
				if (inverted)
				{
					num = 0f - num;
				}
				return num;
			}
			return 0f;
		}
	}
}
