using EventHelpers;
using UnityEngine;

public class ForcedTargetSearchEffect : EffectBaseListener
{
	private AITargetHandler targetHandler;

	private void OnMounted(GameObject parent)
	{
		targetHandler = parent.GetComponent<AITargetHandler>();
	}

	protected override void Execute(TriggerBase e)
	{
		if ((NoCustomEvents() || MatchCustomEvent(e.CustomName)) && targetHandler != null)
		{
			targetHandler.SearchForTarget();
		}
	}
}
