using UnityEngine;

public class TargetClearPathCriterion : MonoBehaviour, IAbilityCriterion
{
	public float HeightOffset;

	public LayerMask Layer;

	private AITargetHandler targetHandler;

	private GameObject parentGO;

	private void Start()
	{
		GlobalHelpers.RegisterAbilityCritertion(base.gameObject, this);
	}

	private void OnMounted(GameObject parent)
	{
		targetHandler = parent.GetComponent<AITargetHandler>();
		parentGO = parent;
	}

	public bool Evaluate()
	{
		bool result = false;
		if (targetHandler != null && targetHandler.HasTarget)
		{
			result = !Physics.Linecast(parentGO.transform.position + Vector3.up * HeightOffset, targetHandler.Target.transform.position + Vector3.up * HeightOffset, Layer.value);
		}
		return result;
	}
}
