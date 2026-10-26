using EventHelpers;
using UnityEngine;

public class ClearTargetEffect : EffectBaseListener
{
	private AITargetHandler targetHandler;

	private void OnMounted(GameObject parent)
	{
		targetHandler = parent.GetComponent<AITargetHandler>();
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && targetHandler != null)
		{
			targetHandler.ClearTarget();
		}
	}
}
