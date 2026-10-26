using AIActionTransitions;

internal class WaitForMotionAIAction : AIAction
{
	public void Stop()
	{
		status = AIActionStatus.Complete;
	}

	public override Transition OnAIEvent(AIEvent triggeredEvent)
	{
		if (triggeredEvent.ID == AIEventID.UnitDeath)
		{
			return new Transition(TransitionType.Done);
		}
		return null;
	}
}
