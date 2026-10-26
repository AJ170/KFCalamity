namespace Acheivements
{
	public abstract class IntGoalBase
	{
		public int Bronze;

		public int Silver;

		public int Gold;

		protected int current;

		public virtual AcheivementStatus Process(int value)
		{
			current += value;
			return Status(current);
		}

		protected virtual AcheivementStatus Status(int value)
		{
			return AcheivementStatus.Uncompleted;
		}
	}
}
