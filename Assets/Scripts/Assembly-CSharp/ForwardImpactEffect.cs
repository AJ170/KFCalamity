using EffectEvents;
using EventHelpers;
using UnityEngine;

public class ForwardImpactEffect : EffectBaseListener
{
	public string Name;

	public float Range = 3f;

	public Vector3 Offset;

	public string GoalLayer;

	public LayerMask Layer;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		Vector3 vector = base.transform.position + Offset;
		Vector3 vector2 = vector + base.transform.forward * Range;
		Vector3 vector3 = vector2 + -base.transform.up * Range;
		Vector3 vector4 = Vector3.Normalize(vector3 - vector);
		RaycastHit hitInfo;
		Physics.Raycast(vector, vector4, out hitInfo, Range, Layer);
		if (CheckIfGoalLayer(hitInfo))
		{
			TriggerImpactEvent(hitInfo.point);
			return;
		}
		float num = CalculateSearchRange(vector, hitInfo);
		float num2 = 0.5f;
		Vector3 direction = -base.transform.up;
		while (num > 0f)
		{
			num = Mathf.Max(num - num2, 0f);
			Vector3 origin = vector + vector4 * num;
			Physics.Raycast(origin, direction, out hitInfo, Range - num, Layer);
			if (CheckIfGoalLayer(hitInfo))
			{
				TriggerImpactEvent(hitInfo.point);
				break;
			}
		}
	}

	private float CalculateSearchRange(Vector3 startPosition, RaycastHit hit)
	{
		if (hit.transform != null)
		{
			return Vector3.Distance(startPosition, hit.point);
		}
		return Range;
	}

	private bool CheckIfGoalLayer(RaycastHit hit)
	{
		if (hit.transform != null)
		{
			if (string.IsNullOrEmpty(GoalLayer))
			{
				return true;
			}
			return hit.transform.gameObject.layer == LayerMask.NameToLayer(GoalLayer);
		}
		return false;
	}

	private void TriggerImpactEvent(Vector3 impactPosition)
	{
		effectSystem.TriggerInternal(new Impact(Name, impactPosition));
	}
}
