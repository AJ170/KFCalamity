using EffectEvents;
using EffectHelpers;
using EventHelpers;
using UnityEngine;

public class DestructableBurnTrigger : MonoBehaviour, IEffectTriggerListener
{
	public float HealthThreshold = 0.5f;

	public string EffectName;

	public GameObject Particle;

	private EffectController effectController;

	private Health health;

	private bool triggered;

	private void Start()
	{
		GlobalHelpers.RegisterEffectEventListener(base.gameObject, this);
		effectController = GetComponent<EffectController>();
		health = GetComponent<Health>();
	}

	public void OnEffectTrigger(TriggerBase e)
	{
		if (e.Id == TriggerId.DamageReceived)
		{
			OnDamageReceived(e as DamageReceived);
		}
	}

	private void OnDamageReceived(DamageReceived e)
	{
		if (!triggered && (float)health.CurrentHealth / (float)health.MaxHealth < HealthThreshold)
		{
			CreateEffect(e.Attacker);
			EnableParticles();
			triggered = true;
		}
	}

	private void CreateEffect(GameObject source)
	{
		if (effectController != null && !string.IsNullOrEmpty(EffectName))
		{
			effectController.CreateEffect(EffectName, source, null);
		}
	}

	private void EnableParticles()
	{
		if (Particle != null)
		{
			Particle.SetActive(true);
		}
	}
}
