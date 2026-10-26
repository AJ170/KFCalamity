using UnityEngine;

public class HealthThresholdCriterion : MonoBehaviour, IAbilityCriterion
{
	public enum CompareMetods
	{
		Above = 0,
		Below = 1,
		Equal = 2,
		NotEqual = 3
	}

	public float Threshold = 0.5f;

	public CompareMetods CompareMetod;

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
		if (health != null)
		{
			float num = (float)health.CurrentHealth / (float)health.MaxHealth;
			if (CompareMetod == CompareMetods.Above)
			{
				result = num > Threshold;
			}
			else if (CompareMetod == CompareMetods.Below)
			{
				result = num < Threshold;
			}
			else if (CompareMetod == CompareMetods.Equal)
			{
				result = num == Threshold;
			}
			else if (CompareMetod == CompareMetods.NotEqual)
			{
				result = num != Threshold;
			}
		}
		return result;
	}
}
