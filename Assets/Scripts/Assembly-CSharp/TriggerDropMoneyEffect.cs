using System.Collections.Generic;
using EventHelpers;
using UnityEngine;

public class TriggerDropMoneyEffect : EffectBaseListener
{
	public int Money = 10;

	public bool RandomFixedMoneyAmount = true;

	public List<int> FixedMoneyAmounts;

	protected override void Execute(TriggerBase e)
	{
		if (!NoCustomEvents() && !MatchCustomEvent(e.CustomName))
		{
			return;
		}
		PickupManager instance = PickupManager.Instance;
		if (instance != null)
		{
			if (instance.CanCreateMoneyPickup())
			{
				MoneyPickup moneyPickup = instance.CreateMoneyPickup(base.transform.position);
				int wealth = Money;
				if (RandomFixedMoneyAmount && FixedMoneyAmounts.Count > 0)
				{
					wealth = FixedMoneyAmounts[Random.Range(0, FixedMoneyAmounts.Count)];
				}
				moneyPickup.Wealth = wealth;
				moneyPickup.Rotate(base.transform.rotation);
			}
		}
		else
		{
			Debug.LogError("TriggerDropMoneyEffect:Execute() -> Missing PickupManager");
		}
	}
}
