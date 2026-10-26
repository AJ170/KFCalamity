namespace Calamity.Input
{
	public class AxisDownInputWrapper : AxisInputWrapper
	{
		protected bool isReset = true;

		public AxisDownInputWrapper(string axis, bool inverted = false)
			: base(axis, inverted)
		{
			requiresUpdate = true;
		}

		public AxisDownInputWrapper(string axis, float threshold)
			: base(axis, threshold)
		{
			requiresUpdate = true;
		}

		public AxisDownInputWrapper(string axis, bool inverted, float threshold)
			: base(axis, inverted, threshold)
		{
			requiresUpdate = true;
		}

		public override void Update()
		{
			if (!isReset && !base.GetBool())
			{
				isReset = true;
			}
		}

		public override bool GetBool()
		{
			bool flag = false;
			if (isReset)
			{
				flag = base.GetBool();
				if (flag)
				{
					isReset = false;
				}
			}
			return flag;
		}

		public override int GetInt()
		{
			int num = 0;
			if (isReset)
			{
				num = base.GetInt();
				if (num != 0)
				{
					isReset = false;
				}
			}
			return num;
		}

		public override float GetFloat()
		{
			float num = 0f;
			if (isReset)
			{
				num = base.GetFloat();
				if (num != 0f)
				{
					isReset = false;
				}
			}
			return num;
		}
	}
}
