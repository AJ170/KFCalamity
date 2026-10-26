using EffectEvents;
using EventHelpers;
using UnityEngine;

public class AimedPhysicalProjectileEffect : SpawnPhysicalProjectileEffect
{
	private AITargetHandler targetHandler;

	public float TargetHeightOffset = 1.5f;

	public bool OnlyAimHeight;

	protected override void OnMounted(GameObject parent)
	{
		base.OnMounted(parent);
		targetHandler = parent.GetComponent<AITargetHandler>();
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && Projectile != null && targetHandler != null)
		{
			if (BoneOffset == null)
			{
				LocateMuzzle();
			}
			Vector3 aimedRotation = GetAimedRotation();
			Vector3 position = BoneOffset.position + aimedRotation * SpawnOffset;
			if (CheckMuzzleClearance)
			{
				position = PerformMuzzleClearanceCheck(position, aimedRotation);
			}
			if (e.Id == TriggerId.Attack)
			{
				Attack attackEvent = (Attack)e;
				FireProjectile(position, aimedRotation, attackEvent);
			}
			else
			{
				FireProjectile(position, aimedRotation);
			}
		}
	}

	private Vector3 GetAimedRotation()
	{
		if (targetHandler.HasTarget)
		{
			Vector3 vector = targetHandler.Target.transform.position + Vector3.up * TargetHeightOffset;
			Vector3 position = BoneOffset.position;
			if (OnlyAimHeight)
			{
				Vector3 vector2 = vector - position;
				Vector3 value = new Vector3(0f, vector2.y, Pyth(vector2.x, vector2.z));
				Vector3 forward = BoneOffset.forward;
				forward.y = 0f;
				Quaternion quaternion = Quaternion.LookRotation(forward);
				Quaternion quaternion2 = Quaternion.LookRotation(Vector3.Normalize(value));
				return quaternion * quaternion2 * Vector3.forward;
			}
			return Vector3.Normalize(vector - position);
		}
		return BoneOffset.forward;
	}

	private float Pyth(float a, float b)
	{
		return Mathf.Sqrt(a * a + b * b);
	}
}
