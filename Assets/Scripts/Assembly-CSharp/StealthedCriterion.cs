using UnityEngine;

public class StealthedCriterion : MonoBehaviour, IAbilityCriterion
{
	public bool ShouldBeStealthed = true;

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
		bool flag = false;
		if (stealthController != null)
		{
			return stealthController.IsStealthed == ShouldBeStealthed;
		}
		return !ShouldBeStealthed;
	}
}
