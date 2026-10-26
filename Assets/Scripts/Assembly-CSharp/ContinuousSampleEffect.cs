using EffectHelpers;
using UnityEngine;

public class ContinuousSampleEffect : EffectBaseTarget
{
	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		repeatInterval = 1f;
		maxRepeats = 3;
	}

	protected override void OnUpdate()
	{
		Debug.Log("ContinuousSampleEffect:OnUpdate()");
	}
}
