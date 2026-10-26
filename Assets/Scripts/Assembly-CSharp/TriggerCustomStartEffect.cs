using EffectEvents;

public class TriggerCustomStartEffect : EffectBaseListener
{
	private bool firstUpdate = true;

	private void Update()
	{
		if (firstUpdate)
		{
			TriggerCustomStart();
			firstUpdate = false;
		}
	}

	private void TriggerCustomStart()
	{
		effectSystem.TriggerInternal(new CustomStart());
	}
}
