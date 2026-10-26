using EventHelpers;
using UnityEngine;

public class ToggleTransformEffect : EffectBaseListener
{
	public Transform[] Targets;

	public bool Active;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		Transform[] targets = Targets;
		foreach (Transform transform in targets)
		{
			if (transform != null)
			{
				transform.gameObject.SetActive(Active);
			}
		}
	}
}
