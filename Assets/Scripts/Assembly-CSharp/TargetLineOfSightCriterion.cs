using UnityEngine;

public class TargetLineOfSightCriterion : MonoBehaviour, IAbilityCriterion
{
	public bool ShouldBeInLineOfSight = true;

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
		if (targetHandler != null && targetHandler.HasTarget)
		{
			result = targetHandler.TargetInLineOfSight == ShouldBeInLineOfSight;
		}
		return result;
	}
}
