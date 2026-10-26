using System;
using UnityEngine;

[Serializable]
public class DamageTextSetting
{
	public Color WeakColor = new Color(0.7f, 0.698f, 0.56f);

	public Color NormalColor = new Color(0.847f, 0.823f, 0.215f);

	public Color EffectiveColor = new Color(0.98f, 0.717f, 0.086f);

	public Color HealColor = new Color(0.325f, 0.8549f, 0.2549f);

	public float WeakDamageThreshold = 0.9f;

	public float EffectiveDamageThreshold = 1.1f;

	public bool IgnoreLineOfSight;
}
