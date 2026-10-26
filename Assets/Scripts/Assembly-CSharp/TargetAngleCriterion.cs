using System;
using UnityEngine;

public class TargetAngleCriterion : MonoBehaviour, IAbilityCriterion
{
	public float Angle = 15f;

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
			float num = Mathf.Cos(Angle * ((float)Math.PI / 180f));
			Vector3 rhs = Vector3.Normalize(position - parentTransform.position);
			float num2 = Vector3.Dot(parentTransform.forward, rhs);
			result = num2 > num;
		}
		return result;
	}
}
