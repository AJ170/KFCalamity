namespace AIActionTransitions
{
	public class Transition
	{
		public TransitionType Type;

		public AIAction NextAction;

		public Transition(TransitionType type)
		{
			Type = type;
		}

		public Transition(TransitionType type, AIAction nextAction)
		{
			Type = type;
			NextAction = nextAction;
		}
	}
}
