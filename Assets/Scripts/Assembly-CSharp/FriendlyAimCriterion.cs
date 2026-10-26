using UnityEngine;

public class FriendlyAimCriterion : MonoBehaviour, IAbilityCriterion
{
	public float SearchAngle = 45f;

	public float SearchRange = 3f;

	public FriendlyAimTargetEvaluator TargetEvaluator;

	private PlayerActionController actionController;

	private bool validTarget;

	private void Start()
	{
		GlobalHelpers.RegisterAbilityCritertion(base.gameObject, this);
	}

	private void OnMounted(GameObject parent)
	{
		actionController = parent.GetComponent<PlayerActionController>();
		if (actionController == null)
		{
			Debug.LogWarning(string.Format("[FriendlyAimAbilityEvaluator:OnMounted] Failed to locate actionController in parent {0}", parent.name));
		}
	}

	public bool Evaluate()
	{
		if (actionController != null)
		{
			FriendlyAimAction action = new FriendlyAimAction(SearchAngle, SearchRange, 10f, false, FriendlyAimType.Instant, TargetEvaluator, TargetCallback);
			actionController.AddAction(action);
			return validTarget;
		}
		return false;
	}

	private void TargetCallback(GameObject target)
	{
		if (target != null)
		{
			validTarget = true;
		}
		else
		{
			validTarget = false;
		}
	}
}
