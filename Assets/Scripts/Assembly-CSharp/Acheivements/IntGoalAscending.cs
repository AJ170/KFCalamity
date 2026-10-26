using System;

namespace Acheivements
{
	[Serializable]
	public class IntGoalAscending : IntGoalBase
	{
		protected override AcheivementStatus Status(int value)
		{
			if (value >= Bronze && value < Silver)
			{
				return AcheivementStatus.Bronze;
			}
			if (value >= Silver && value < Gold)
			{
				return AcheivementStatus.Silver;
			}
			if (value >= Gold)
			{
				return AcheivementStatus.Gold;
			}
			return AcheivementStatus.Uncompleted;
		}
	}
}
