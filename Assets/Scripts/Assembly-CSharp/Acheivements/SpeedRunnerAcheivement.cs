namespace Acheivements
{
	public class SpeedRunnerAcheivement : AcheivementBase
	{
		public IntGoalDescending TimeGoal;

		protected override void Awake()
		{
			id = AchievementId.SpeedRunner;
			eventId = EventId.Time;
			base.Awake();
		}

		protected override void OnProcess(EventBase e)
		{
			TimeEvent timeEvent = e as TimeEvent;
			status = TimeGoal.Process(timeEvent.Time);
		}
	}
}
