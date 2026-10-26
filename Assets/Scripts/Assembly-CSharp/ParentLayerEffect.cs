using EventHelpers;
using UnityEngine;

public class ParentLayerEffect : EffectBaseListener
{
	public LayerMask Layer;

	private GameObject parent;

	private void OnMounted(GameObject parent)
	{
		this.parent = parent;
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && parent != null)
		{
			base.gameObject.layer = Layer.value;
		}
	}
}
