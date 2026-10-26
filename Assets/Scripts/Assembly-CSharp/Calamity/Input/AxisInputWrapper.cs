using UnityEngine;

namespace Calamity.Input
{
	public class AxisInputWrapper : InputWrapper
	{
		protected string axis;

		protected bool inverted;

		private float threshold = 0.1f;

		public AxisInputWrapper(string axis, bool inverted = false)
		{
			this.axis = axis;
			this.inverted = inverted;
		}

		public AxisInputWrapper(string axis, float threshold)
		{
			this.axis = axis;
			this.threshold = Mathf.Abs(threshold);
		}

		public AxisInputWrapper(string axis, bool inverted, float threshold)
		{
			this.axis = axis;
			this.inverted = inverted;
			this.threshold = Mathf.Abs(threshold);
		}

		public override bool GetBool()
		{
			float num = UnityEngine.Input.GetAxis(axis);
			if (inverted)
			{
				num = 0f - num;
			}
			return num > threshold;
		}

		public override int GetInt()
		{
			return GetBool() ? 1 : 0;
		}

		public override float GetFloat()
		{
			float num = UnityEngine.Input.GetAxis(axis);
			if (inverted)
			{
				num = 0f - num;
			}
			if (num > threshold)
			{
				return num;
			}
			return 0f;
		}
	}
}
