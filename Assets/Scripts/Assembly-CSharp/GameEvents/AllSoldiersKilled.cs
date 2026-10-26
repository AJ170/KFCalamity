using EventHelpers;

namespace GameEvents
{
	public class AllSoldiersKilled : EventBase
	{
		public AllSoldiersKilled()
			: base(EventId.AllSoldiersKilled)
		{
		}
	}
}
