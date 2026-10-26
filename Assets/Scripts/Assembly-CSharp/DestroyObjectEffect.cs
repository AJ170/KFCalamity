using EventHelpers;
using UnityEngine;

public class DestroyObjectEffect : EffectBaseListener
{
	public float Delay;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			Object.Destroy(base.transform.root.gameObject, Delay);
		}
	}
}
