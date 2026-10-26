using UnityEngine;

public class CanStealthCriterion : MonoBehaviour, IAbilityCriterion
{
	private StealthController stealthController;

	private void Start()
	{
		GlobalHelpers.RegisterAbilityCritertion(base.gameObject, this);
	}

	private void OnMounted(GameObject parent)
	{
		stealthController = parent.GetComponent<StealthController>();
	}

	public bool Evaluate()
	{
		bool result = false;
		if (stealthController != null)
		{
			result = stealthController.CanStealth;
		}
		return result;
	}
}
