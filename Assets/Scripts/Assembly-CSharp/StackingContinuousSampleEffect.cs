using EffectHelpers;
using UnityEngine;

public class StackingContinuousSampleEffect : EffectBaseTarget
{
	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		repeatInterval = 1f;
		maxRepeats = 3;
		stackable = true;
		maxStacks = 3;
	}

	protected override void OnUpdate()
	{
		Debug.Log("StackingContinuousSampleEffect:OnUpdate()");
	}

	protected override void OnAddStack()
	{
		Debug.Log("StackingContinuousSampleEffect:OnAddStack() Num stacks " + base.NumStacks);
	}
}
