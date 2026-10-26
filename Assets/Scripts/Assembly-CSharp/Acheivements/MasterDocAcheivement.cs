using UnityEngine;

namespace Acheivements
{
	public class MasterDocAcheivement : AcheivementBase
	{
		public IntGoalAscending HealingGoal;

		private int numPlayers = 1;

		protected override void Awake()
		{
			id = AchievementId.MasterDoc;
			eventId = EventId.Healing;
			int numberOfPlayers = PlayerManager.Instance.NumberOfPlayers;
			if (numberOfPlayers > numPlayers)
			{
				numPlayers = numberOfPlayers;
			}
			base.Awake();
		}

		protected override void OnProcess(EventBase e)
		{
			HealingEvent healingEvent = e as HealingEvent;
			int num = healingEvent.Healing;
			if (numPlayers > 1)
			{
				num = Mathf.Max(num / numPlayers, 1);
			}
			status = HealingGoal.Process(num);
		}
	}
}
