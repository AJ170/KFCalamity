using EventHelpers;
using UnityEngine;

public class SetTargetSearchSettingsEffect : EffectBaseListener
{
	public bool RequireLineOfSight;

	public bool RequireClearPath;

	public float CurrentTargetSearchModifier = 0.65f;

	public bool SearchInArc;

	public float Arc = 135f;

	private AITargetHandler targetHandler;

	private void OnMounted(GameObject parent)
	{
		targetHandler = parent.GetComponent<AITargetHandler>();
	}

	protected override void Execute(TriggerBase e)
	{
		if ((NoCustomEvents() || MatchCustomEvent(e.CustomName)) && targetHandler != null)
		{
			targetHandler.RequireLineOfSight = RequireLineOfSight;
			targetHandler.RequireClearPath = RequireClearPath;
			targetHandler.CurrentTargetSearchModifier = CurrentTargetSearchModifier;
			targetHandler.SearchInArc = SearchInArc;
			targetHandler.SetArc(Arc);
		}
	}
}
