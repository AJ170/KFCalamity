using EffectHelpers;
using UnityEngine;

public class InstantSampleEffect : EffectBaseTarget
{
	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Instant;
	}

	protected override void Execute()
	{
		Debug.Log("InstantSampleEffect:Execute()");
	}
}
