using EventHelpers;
using UnityEngine;

public class Mac10FireEffect : AddComponentEffect
{
	public Perk RequiredPerk = Perk.Firebug;

	private Soldier soldier;

	private void OnMounted(GameObject parent)
	{
		soldier = parent.GetComponent<Soldier>();
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && soldier != null && soldier.Perk == RequiredPerk)
		{
			ApplyComponent(e);
		}
	}
}
