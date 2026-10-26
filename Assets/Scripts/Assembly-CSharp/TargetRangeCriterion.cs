using UnityEngine;

public class TargetRangeCriterion : MonoBehaviour, IAbilityCriterion
{
	public float Range = 10f;

	public bool MaxRange = true;

	private Transform parentTransform;

	private AITargetHandler targetHandler;

	private void Start()
	{
		GlobalHelpers.RegisterAbilityCritertion(base.gameObject, this);
	}

	private void OnMounted(GameObject parent)
	{
		parentTransform = parent.transform;
		targetHandler = parent.GetComponent<AITargetHandler>();
	}

	public bool Evaluate()
	{
		bool result = false;
		if (targetHandler != null && targetHandler.HasTarget)
		{
			GameObject target = targetHandler.Target;
			Vector3 position = target.transform.position;
			float num = Vector3.Distance(parentTransform.position, position);
			result = ((!MaxRange) ? (num >= Range) : (num <= Range));
		}
		return result;
	}
}
