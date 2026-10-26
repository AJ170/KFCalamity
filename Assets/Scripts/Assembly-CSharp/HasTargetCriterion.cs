using UnityEngine;

public class HasTargetCriterion : MonoBehaviour, IAbilityCriterion
{
	public bool ShouldHaveTarget = true;

	private AITargetHandler targetHandler;

	private void Start()
	{
		GlobalHelpers.RegisterAbilityCritertion(base.gameObject, this);
	}

	private void OnMounted(GameObject parent)
	{
		targetHandler = parent.GetComponent<AITargetHandler>();
	}

	public bool Evaluate()
	{
		bool result = false;
		if (targetHandler != null)
		{
			result = targetHandler.HasTarget == ShouldHaveTarget;
		}
		return result;
	}
}
