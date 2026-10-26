namespace Acheivements
{
	public class ZedSlayerAcheivement : AcheivementBase
	{
		public IntGoalAscending KillsGoal;

		protected override void Awake()
		{
			id = AchievementId.ZedSlayer;
			eventId = EventId.Kills;
			base.Awake();
		}

		protected override void OnProcess(EventBase e)
		{
			KillsEvent killsEvent = e as KillsEvent;
			status = KillsGoal.Process(killsEvent.Kills);
		}
	}
}
