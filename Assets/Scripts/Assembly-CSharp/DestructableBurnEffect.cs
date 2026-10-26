using EffectEvents;
using EffectHelpers;
using EventHelpers;

public class DestructableBurnEffect : DamageOverTimeEffect
{
	protected override void Awake()
	{
		base.Awake();
		receiveTriggers = true;
		buffClass = BuffClass.Fire;
		repeatInterval = 0.5f;
		maxRepeats = 200;
		damagePerTick = 25;
		damageType = DamageType.Fire;
	}

	public override void OnReceivedTrigger(TriggerBase e)
	{
		if (e.Id == TriggerId.DamageReceived)
		{
			DamageReceived damageReceived = e as DamageReceived;
			if (damageReceived != null && base.Source != damageReceived.Attacker)
			{
				base.Source = damageReceived.Attacker;
			}
		}
	}
}
