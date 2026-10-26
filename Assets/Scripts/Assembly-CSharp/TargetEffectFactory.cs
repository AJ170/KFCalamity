using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetEffectFactory : MonoBehaviour
{
	private static TargetEffectFactory instance;

	private Dictionary<string, Type> effects = new Dictionary<string, Type>();

	private void Awake()
	{
		instance = this;
		RegisterEffect(typeof(ContinuousSampleEffect));
		RegisterEffect(typeof(InfiniteSampleEffect));
		RegisterEffect(typeof(InstantSampleEffect));
		RegisterEffect(typeof(StackingContinuousSampleEffect));
		RegisterEffect(typeof(BurningEffect));
		RegisterEffect(typeof(ClotGrabEffect));
		RegisterEffect(typeof(DamageOverTimeEffect));
		RegisterEffect(typeof(FleshpoundEnrageEffect));
		RegisterEffect(typeof(FocusedFireEffect));
		RegisterEffect(typeof(FuriousRoarEffect));
		RegisterEffect(typeof(GorefastChargeEffect));
		RegisterEffect(typeof(HealOverTimeEffect));
		RegisterEffect(typeof(HealthRegenEffect));
		RegisterEffect(typeof(HurryEffect));
		RegisterEffect(typeof(HuskBurningEffect));
		RegisterEffect(typeof(KnockbackEffect));
		RegisterEffect(typeof(LastStandEffect));
		RegisterEffect(typeof(Mac10BurningEffect));
		RegisterEffect(typeof(MigraneEffect));
		RegisterEffect(typeof(PatriarchChargeEffect));
		RegisterEffect(typeof(PatriarchHealDamageReductionEffect));
		RegisterEffect(typeof(PatriarchHealEffect));
		RegisterEffect(typeof(PatriarchImpaleEffect));
		RegisterEffect(typeof(PatriarchStealthChargeEffect));
		RegisterEffect(typeof(PukeEffect));
		RegisterEffect(typeof(ScrakeChargeEffect));
		RegisterEffect(typeof(ShotgunSlowEffect));
		RegisterEffect(typeof(SyringeSelfHealEffect));
		RegisterEffect(typeof(SyringeTargetHealEffect));
		RegisterEffect(typeof(WeaponKnockbackEffect));
		RegisterEffect(typeof(ZedTimeSoldierEffect));
		RegisterEffect(typeof(TauntEffect));
		RegisterEffect(typeof(BerserkerRamageEffect));
		RegisterEffect(typeof(RampageKnockbackEffect));
		RegisterEffect(typeof(ImpaleTargetEffect));
		RegisterEffect(typeof(StompKnockbackEffect));
		RegisterEffect(typeof(StunEffect));
		RegisterEffect(typeof(DestructableBurnEffect));
		RegisterEffect(typeof(PatriarchLastStandShieldEffect));
		RegisterEffect(typeof(PatriarchLastStandBoostEffect));
		RegisterEffect(typeof(SuperDamageBonusEffect));
		RegisterEffect(typeof(SuperResistanceBonusEffect));
		RegisterEffect(typeof(SuperSpeedBonusEffect));
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void RegisterEffect(Type type)
	{
		if (type != null)
		{
			effects.Add(type.Name, type);
		}
	}

	public static EffectBaseTarget CreateEffect(string effectName)
	{
		if (instance != null)
		{
			Type value = null;
			if (instance.effects.TryGetValue(effectName, out value))
			{
				EffectBaseTarget effectBaseTarget = Activator.CreateInstance(value) as EffectBaseTarget;
				if (effectBaseTarget != null)
				{
					return effectBaseTarget;
				}
				Debug.LogError("TargetEffectFactory:Create() Failed to create effect " + effectName);
			}
			else
			{
				Debug.LogError("TargetEffectFactory:Create() Didn't find effect " + effectName);
			}
		}
		return null;
	}
}
