using System.Collections;
using EffectEvents;
using EventHelpers;
using UnityEngine;

public class AimedRaycastProjectileEffect : AdvancedRaycastProjectileEffect
{
	public float TargetHeightOffset = 1.2f;

	public bool OnlyAimHeight;

	public bool OnlyAimInFrontOfMuzzle;

	private AITargetHandler targetHandler;

	private Transform parentTransform;

	protected override void OnMounted(GameObject parent)
	{
		base.OnMounted(parent);
		parentTransform = parent.transform;
		targetHandler = parent.GetComponent<AITargetHandler>();
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		if (BoneOffset == null)
		{
			FindMuzzle();
		}
		for (int i = 0; i < NumberOfProjectiles; i++)
		{
			Vector3 vector;
			if (e.Id == TriggerId.FriendlyAim)
			{
				FriendlyAim friendlyAim = (FriendlyAim)e;
				GameObject target = friendlyAim.Target;
				vector = Vector3.Normalize(target.transform.position - parentTransform.position);
			}
			else if (targetHandler != null && targetHandler.Target != null)
			{
				Transform transform = targetHandler.Target.transform;
				Vector3 vector2 = transform.position + Vector3.up * TargetHeightOffset;
				Vector3 position = BoneOffset.position;
				if (!OnlyAimInFrontOfMuzzle || IsInFrontOfMuzzle(vector2))
				{
					if (OnlyAimHeight)
					{
						Vector3 vector3 = vector2 - position;
						Vector3 value = new Vector3(0f, vector3.y, Pyth(vector3.x, vector3.z));
						Vector3 forward = BoneOffset.forward;
						forward.y = 0f;
						Quaternion quaternion = Quaternion.LookRotation(forward);
						Quaternion quaternion2 = Quaternion.LookRotation(Vector3.Normalize(value));
						vector = quaternion * quaternion2 * Vector3.forward;
					}
					else
					{
						vector = Vector3.Normalize(vector2 - position);
					}
				}
				else
				{
					vector = BoneOffset.forward;
				}
				if (HorizontalSpread > 0f || VerticalSpread > 0f)
				{
					Quaternion quaternion3 = Quaternion.LookRotation(vector);
					Quaternion quaternion4 = Quaternion.Euler(GetSpread(VerticalSpread), GetSpread(HorizontalSpread), 0f);
					vector = quaternion3 * quaternion4 * Vector3.forward;
				}
			}
			else
			{
				vector = BoneOffset.forward;
				if (HorizontalSpread > 0f || VerticalSpread > 0f)
				{
					Quaternion quaternion5 = Quaternion.LookRotation(vector);
					Quaternion quaternion6 = Quaternion.Euler(GetSpread(VerticalSpread), GetSpread(HorizontalSpread), 0f);
					vector = quaternion5 * quaternion6 * Vector3.forward;
				}
			}
			if (Delay > 0f)
			{
				StartCoroutine(WaitForDelayBeforeFire(vector));
			}
			else
			{
				PerformRaycast(vector);
			}
		}
	}

	private float Pyth(float a, float b)
	{
		return Mathf.Sqrt(a * a + b * b);
	}

	private IEnumerator WaitForDelayBeforeFire(Vector3 direction)
	{
		yield return new WaitForSeconds(Delay);
		PerformRaycast(direction);
	}

	private void PerformRaycast(Vector3 direction)
	{
		Vector3 position = BoneOffset.position + direction * RaycastOffset;
		if (UsePiercing)
		{
			FirePiercingRaycast(position, direction, null);
		}
		else
		{
			FireRaycast(position, direction, null);
		}
	}

	private bool IsInFrontOfMuzzle(Vector3 position)
	{
		return (double)BoneOffset.InverseTransformPoint(position).z > 0.0;
	}
}
