using UnityEngine;

public class FalloffApplyDamageEffect : ApplyDamageEffect
{
	public float StartFalloffAfter = 2f;

	public float FalloffExtent = 1f;

	public float FalloffMultiplierPerExtent = 0.1f;

	protected override float GetDamageMultiplier(GameObject attacker, GameObject victim)
	{
		float num = base.GetDamageMultiplier(attacker, victim);
		if (attacker != null && victim != null)
		{
			float num2 = Vector3.Distance(attacker.transform.position, victim.transform.position);
			if (num2 > StartFalloffAfter)
			{
				int num3 = (int)((num2 - StartFalloffAfter) / FalloffExtent);
				num *= Mathf.Max(1f - (float)num3 * FalloffMultiplierPerExtent, 0.1f);
			}
		}
		return num;
	}
}
