using UnityEngine;

public class TargetHeightDifferenceCriterion : MonoBehaviour, IAbilityCriterion
{
	public float MaxHeightOffset = 1f;

	private AITargetHandler targetHandler;

	private GameObject parent;

	private void Start()
	{
		GlobalHelpers.RegisterAbilityCritertion(base.gameObject, this);
	}

	private void OnMounted(GameObject parent)
	{
		this.parent = parent;
		targetHandler = parent.GetComponent<AITargetHandler>();
	}

	public bool Evaluate()
	{
		bool result = false;
		if (parent != null && targetHandler != null && targetHandler.HasTarget)
		{
			Vector3 position = targetHandler.Target.transform.position;
			Vector3 position2 = parent.transform.root.position;
			float num = Mathf.Abs(position.y - position2.y);
			result = num < MaxHeightOffset;
		}
		return result;
	}
}
