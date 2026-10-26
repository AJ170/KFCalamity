using EffectHelpers;
using UnityEngine;

public class InfiniteSampleEffect : EffectBaseTarget
{
	private int CustomRepeatCounter;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Infinite;
		repeatInterval = 1f;
	}

	protected override void OnUpdate()
	{
		Debug.Log("InfiniteSampleEffect:OnUpdate()");
		CustomRepeatCounter++;
		if (CustomRepeatCounter == 10)
		{
			deleted = true;
		}
	}
}
