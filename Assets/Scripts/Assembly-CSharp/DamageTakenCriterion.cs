using UnityEngine;

public class DamageTakenCriterion : MonoBehaviour, IAbilityCriterion
{
	public int Threshold;

	public bool AboveThreshold = true;

	public float timer;

	private Health health;

	private void Start()
	{
		GlobalHelpers.RegisterAbilityCritertion(base.gameObject, this);
	}

	private void OnMounted(GameObject parent)
	{
		health = parent.GetComponent<Health>();
	}

	public bool Evaluate()
	{
		bool result = false;
		if (health != null && timer == 0f)
		{
			float num = health.MaxHealth - health.CurrentHealth;
			result = num <= (float)Threshold;
		}
		return result;
	}
}
