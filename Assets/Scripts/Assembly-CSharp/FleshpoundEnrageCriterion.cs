using EffectEvents;
using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class FleshpoundEnrageCriterion : MonoBehaviour, IAbilityCriterion, IEffectTriggerListener
{
	public int DamageThreshold = 200;

	public float ResetTime = 2f;

	public float InactivityTimer = 10f;

	private float resetCounter;

	private float inactivityTimer;

	private int damageCounter;

	private void Start()
	{
		GlobalHelpers.RegisterAbilityCritertion(base.gameObject, this);
		GlobalHelpers.RegisterEffectEventListener(base.gameObject, this);
	}

	private void Update()
	{
		if (resetCounter > 0f)
		{
			resetCounter -= Time.deltaTime;
			if (resetCounter <= 0f)
			{
				damageCounter = 0;
			}
		}
		inactivityTimer += Time.deltaTime;
	}

	public bool Evaluate()
	{
		return damageCounter >= DamageThreshold || inactivityTimer >= InactivityTimer;
	}

	public void OnEffectTrigger(TriggerBase e)
	{
		if (e.Id == TriggerId.DamageReceived)
		{
			DamageReceived damageReceived = e as DamageReceived;
			OnDamageReceived(damageReceived.Amount);
		}
		else if (e.Id == TriggerId.AIAttack)
		{
			ResetInactivityTimer();
		}
	}

	private void OnDamageReceived(int damage)
	{
		damageCounter += damage;
		resetCounter = ResetTime;
		ResetInactivityTimer();
	}

	private void ResetInactivityTimer()
	{
		inactivityTimer = 0f;
	}
}
