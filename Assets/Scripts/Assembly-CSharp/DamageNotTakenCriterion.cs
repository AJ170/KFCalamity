using UnityEngine;

public class DamageNotTakenCriterion : MonoBehaviour, IAbilityCriterion
{
	public float timer = 10f;

	public float disabledFor = 10f;

	private float timeSinceLastHit;

	private float lol;

	private float lastHealth;

	private Health health;

	private void Start()
	{
		GlobalHelpers.RegisterAbilityCritertion(base.gameObject, this);
	}

	private void OnMounted(GameObject parent)
	{
		health = parent.GetComponent<Health>();
		lastHealth = health.CurrentHealth;
		lol = Time.time;
	}

	public bool Evaluate()
	{
		bool result = false;
		if ((float)health.CurrentHealth < lastHealth)
		{
			timeSinceLastHit = 0f;
			lol = Time.time;
			lastHealth = health.CurrentHealth;
		}
		timeSinceLastHit = Time.time - lol;
		if (health != null && timeSinceLastHit >= timer)
		{
			MonoBehaviour.print(timeSinceLastHit);
			result = true;
		}
		return result;
	}
}
